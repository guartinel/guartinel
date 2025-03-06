using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.Service.WebsiteChecker ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Packages ;
using Newtonsoft.Json.Linq ;
using MeasurementConstants = Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement;
using Strings = Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings;
using SaveRequestConstants = Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.WatcherServerRoutes.Save.Request ;

namespace Guartinel.WatcherServer.Supervisors.WebsiteSupervisor {
   // public class WebsiteSupervisorPackage : InstanceDataListsPackage {
   public class WebsiteSupervisorPackage : Package, IDisposable {
      public new static class Constants {
         public const string CAPTION = "Website Supervisor Package" ;
         public static readonly List<string> CREATOR_IDENTIFIERS = new List<string> {Strings.Use.PackageType} ;

         public const int CACHE_EXPIRY_SECONDS = 45 ;

         // public const int DO_NOT_USE_CACHE = 0 ;
         public const string SERVICE_NAME = "WebsiteChecker" ;
         public const string SERVICE_VERSION = "1.0" ;

         public const int DEFAULT_TIMEOUT_SECONDS = 60 ;
      }

      // protected override int? GetDefaultTimeout => ApplicationSettings.Use.DefaultWebsiteTimeoutSeconds ;

      //public new static Creator GetCreator() {
      //   return new Creator<Package, WebsiteSupervisorPackage> (() => new WebsiteSupervisorPackage(), Constants.CAPTION, Constants.CREATOR_IDENTIFIERS) ;
      //}

      public WebsiteSupervisorPackage() {
         _logger.Info ("Create website check client.") ;

         //_websiteCheckSender = IoC.Use.Single.GetInstance<IWebsiteCheckSender>() ;
         //_websiteCheckSender.Configure ($"{Constants.SERVICE_NAME}.{Constants.SERVICE_VERSION}", _logger.Tags) ;
      }

      protected override void Dispose1() {
         //_websiteCheckSender?.Dispose() ;
      }

      protected List<Website> _websites = new List<Website>() ;
      protected string _checkTextPattern ;
      protected int? _checkCertificateDays ;
      protected int? _checkLoadTimeSeconds ;
      protected int _tryCount = WebsiteChecker.Defaults.TRY_COUNT ;
      protected int _retryWaitTimeSeconds = WebsiteChecker.Defaults.RETRY_WAIT_TIME_SECONDS ;

      protected IWebsiteCheckSender CreateWebsiteCheckSender() {
         var result = IoC.Use.Single.GetInstance<IWebsiteCheckSender>() ;
         return result ;
      }

      // Check results: key: website address, values: results
      protected readonly Dictionary<string, List<SiteDownloadResult>> _downloadResults = new Dictionary<string, List<SiteDownloadResult>>() ;

      protected List<SiteDownloadResult> EnsureWebsiteResults (string address) {
         lock (_downloadResults) {
            if (!_downloadResults.ContainsKey (address)) {
               _downloadResults.Add (address, new List<SiteDownloadResult>()) ;
            }

            return _downloadResults [address] ;
         }
      }

      protected override void SpecificConfigure (ConfigurationData configuration) {
         var detailedWebsiteJObjects = configuration.AsArray (SaveRequestConstants.DETAILED_WEBSITES).Select (x => x.AsJObject) ;
         // Remove the empty ones
         detailedWebsiteJObjects = detailedWebsiteJObjects.Where (x => !string.IsNullOrWhiteSpace (x.GetStringValue (SaveRequestConstants.DETAILED_WEBSITE_ADDRESS, string.Empty))) ;
         // Convert to website objects
         _websites = detailedWebsiteJObjects.Select (x => new Website (x.GetStringValue (SaveRequestConstants.DETAILED_WEBSITE_ADDRESS, string.Empty),
                                                                       x.GetStringValue (SaveRequestConstants.DETAILED_WEBSITE_CAPTION, string.Empty))).ToList() ;

         // SetInstances (new List<string> {_websites}) ;
         SetInstances (_websites.Select (x => x.Address).ToList()) ;

         _checkTextPattern = configuration [SaveRequestConstants.CHECK_TEXT_PATTERN] ;
         _checkCertificateDays = configuration.AsIntegerNull (SaveRequestConstants.CHECK_CERTIFICATE_DAYS) ;
         _checkLoadTimeSeconds = configuration.AsIntegerNull (SaveRequestConstants.CHECK_LOAD_TIME_SECONDS) ;
         _tryCount = configuration.AsInteger (SaveRequestConstants.TRY_COUNT,
                                              WebsiteChecker.Defaults.TRY_COUNT) ;
         _retryWaitTimeSeconds = configuration.AsInteger (SaveRequestConstants.RETRY_WAIT_TIME_SECONDS,
                                                          WebsiteChecker.Defaults.RETRY_WAIT_TIME_SECONDS) ;
      }

      protected void RegisterWebsiteResult (Website website,
                                            SiteDownloadResult result,
                                            string[] tags) {
         var logger = new TagLogger (_logger.Tags, tags) ;

         logger.Info ($"Register website result. Success: {result.Success}. Load time (ms): {result.LoadTimeMilliseconds}") ;
         var downloadResults = EnsureWebsiteResults (website.Address) ;

         lock (_downloadResults) {
            downloadResults.Add (result) ;
         }

         StoreMeasuredData (website.Address, result.Success == SiteDownloadResultSuccess.Success,
                            result.LoadTimeMilliseconds,
                            result.CertificateExpiryDate,
                            new XSimpleString (result.Message),
                            new XSimpleString (result.Details)) ;
      }

      protected void StoreMeasuredData (string url,
                                        bool success,
                                        long? loadingTimeMilliSeconds,
                                        DateTime? certificateExpiryDate,
                                        XString message,
                                        XString details) {
         JObject measuredData = new JObject() ;
         measuredData.Add (MeasurementConstants.SUCCESS, success) ;
         measuredData.Add (MeasurementConstants.WEBSITE, url) ;
         if (loadingTimeMilliSeconds != null) {
            measuredData.Add (MeasurementConstants.LOAD_TIME_SECONDS, (loadingTimeMilliSeconds.Value / 1000.0).NormalizeValue()) ;
         }

         measuredData.Add (MeasurementConstants.CERTIFICATE_EXPIRY, certificateExpiryDate) ;
         measuredData.Add (MeasurementConstants.MESSAGE, message.EmptyIfNull().AsJObject()) ;
         measuredData.Add (MeasurementConstants.DETAILS, details.EmptyIfNull().AsJObject()) ;

         measuredData.Add (MeasurementConstants.CHECK_TEXT_PATTERN, _checkTextPattern) ;

         IoC.Use.Single.GetInstance<IMeasuredDataStore>()?.StoreMeasuredData (ID,
                                                                              MeasurementConstants.TYPE_VALUE,
                                                                              DateTime.UtcNow,
                                                                              measuredData) ;
      }

      protected Task CreateRequestTask (SiteDownloadRequest request,
                                        int tryCount,
                                        int? retryWaitSeconds,
                                        string[] tags) {
         int taskID = 0 ;

         var task = new Task (() => {
            var logger = new TagLogger (tags, TagLogger.CreateTag (nameof(taskID), taskID.ToString())) ;

            logger.Debug ($"Website check task started.") ;
            if (retryWaitSeconds != null) {
               logger.Debug ($"Waiting before webcheck retry for {retryWaitSeconds} seconds...");
               new TimeoutSeconds (retryWaitSeconds.Value).Wait() ;
            }

            // _websiteCheckSender.SendRequest (request, (result,
            var websiteCheckSender = CreateWebsiteCheckSender() ;

            logger.Debug ("Sending website check request...") ;

            websiteCheckSender.SendRequest ($"{Constants.SERVICE_NAME}.{Constants.SERVICE_VERSION}",
                                            request, (result,
                                                      cancellation) => {

                                                         // logger.Info ($"Website check for '{website.Address}' returned. Success: {siteDownloadResult.Success}. Load time (ms): {siteDownloadResult.LoadTimeMilliseconds}") ;

                                               logger.Debug ($"Website check result arrived. Message: {result.Message}") ;

                                               RegisterWebsiteResult (request.Website, result, logger.Tags) ;

                                               // Repeat request if not successful
                                               if (result.Success != SiteDownloadResultSuccess.Success) {
                                                  // Check if error is a service error, if it is, then retry right now
                                                  if (result.Message != null &&
                                                      result.Message.Contains ("unknown error: session deleted because of page crash from")) {

                                                     CreateRequestTask (request, tryCount, 0, tags).Start() ;
                                                  } else {
                                                     logger.Debug ($"Website check next try, tries left: {tryCount}.") ;

                                                     tryCount-- ;

                                                     if (tryCount > 0) {
                                                        // Send another request, use package retry time
                                                        CreateRequestTask (request, tryCount, _retryWaitTimeSeconds, tags).Start() ;
                                                     }
                                                  }
                                               }

                                               // Register in cache
                                               if (result.Success == SiteDownloadResultSuccess.Success) {
                                                  WebsiteCheckCache.Register (request.Website.Address,
                                                                              result.LoadTimeMilliseconds ?? 0,
                                                                              result.Content,
                                                                              result.CertificateExpiryDate) ;
                                               }

                                               logger.Debug ("Website check task finished.") ;
                                            }, _checkCancellationSource.Token,
                                            TimeSpan.FromSeconds (request.TimeoutSeconds ?? Constants.DEFAULT_TIMEOUT_SECONDS),
                                            logger.Tags) ;
         }) ;

         taskID = task.Id ;

         return task ;
      }

      protected void SendCheckRequest (Website website,
                                       string[] tags) {
         // Get download kind from configuration
         var kind = ApplicationSettings.Use.WebsiteChecker ;

         SiteDownloadRequest request = new SiteDownloadRequest (website,
                                                                kind,
                                                                // _checkLoadTimeSeconds == null ? null : (int?) Math.Round (_checkLoadTimeSeconds.Value * 2 * retryFactor, 0),
                                                                _checkLoadTimeSeconds * 2,
                                                                _tryCount,
                                                                _retryWaitTimeSeconds,
                                                                _checkTextPattern) ;
         // siteDownloadResult = SiteDownloadResult.FromJObject (client.Call (request.AsJObject())) ;
         // logger.InfoWithDebug ($"Calling website check for '{website.Address}'.", $"Result: {request.AsJObject().ConvertToLog()}") ;

         // Send request multiple times         
         //for (int index = 0; index < ApplicationSettings.Use.WebsiteCheckRequestCount; index++) {
         //var logger = new TagLogger (tags, website.Address, $"request{index}") ;
         var logger = new TagLogger (tags, $"website:{website.Address}") ;
         logger.Debug ("Creating website check request task...") ;
         // close send channel asap
         // add timeout to request
         var task = CreateRequestTask (request, _tryCount, null, logger.Tags) ;
         logger.Debug ($"Website check task ID is {task.Id}.") ;
         task.Start() ;
         //}
      }

      protected override List<Checker> CreateCheckers1() {
         var result = new List<Checker>() ;

         // Expiry: package interval
         // var cacheExpirySeconds = CheckIntervalSeconds ;         

         foreach (Website website in _websites) {
            var logger = new TagLogger (_logger.Tags, TagLogger.CreateTag (nameof (website), website.Address)) ;

            var checker = new WebsiteChecker() ;
            // Use and drop current results
            checker.Configure (Name, ID, website, _checkLoadTimeSeconds,
                               _checkTextPattern, _checkCertificateDays,
                               EnsureWebsiteResults (website.Address).ToArray().ToList()) ;
            EnsureWebsiteResults (website.Address).Clear() ;
            result.Add (checker) ;

            // Check if the check result is in the cache
            var cachedWebSite = WebsiteCheckCache.GetCached (website.Address, TimeSpan.FromSeconds (Constants.CACHE_EXPIRY_SECONDS)) ;

            if (cachedWebSite != null) {
               logger.Info ("Website check found a hit in cache.") ;
               SiteDownloadResult siteDownloadResult = new SiteDownloadResult (website,
                                                                               cachedWebSite.RoundtripMilliseconds,
                                                                               cachedWebSite.Content,
                                                                               cachedWebSite.CertificateExpiryDate) ;
               RegisterWebsiteResult (website, siteDownloadResult, logger.Tags) ;
            } else {
               logger.Info ("Website check did not find a hit in cache.") ;
               SendCheckRequest (website, _logger.Tags) ;
            }
         }

         return result ;
      }
   }
}