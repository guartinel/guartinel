using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Text.RegularExpressions ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Service.WebsiteChecker ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.CheckResults ;
// using MeasurementConstants = Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement ;
using Strings = Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings ;

namespace Guartinel.WatcherServer.Supervisors.WebsiteSupervisor {
   public class WebsiteChecker : Checker {
      public static class Constants {
         public const string CAPTION = "Website Availability Checker" ;

         // public const int DO_NOT_CHECK_LOAD_TIME = 0 ;
         // public const string DO_NOT_CHECK_TEXT_PATTERN = "" ;
         // public const int DO_NOT_CHECK_CERTIFICATE_EXPIRY_DAYS = -1 ;

         public const int DO_NOT_USE_CACHE = 0 ;
      }

      public static class Defaults {
         // public const string CHECK_TEXT_PATTERN = Constants.DO_NOT_CHECK_TEXT_PATTERN ;
         // public const int CHECK_CERTIFICATE = Constants.DO_NOT_CHECK_CERTIFICATE_EXPIRY_DAYS ;
         // public const int CHECK_LOAD_TIME_SECONDS = 10 ;
         // public const int CACHE_EXPIRY_SECONDS = 300 ;
         public const int TRY_COUNT = 3 ;
         public const int RETRY_WAIT_TIME_SECONDS = 20 ;
      }

      // Message = new XConstantString(MessageStrings.Get (MessageStrings.WEBSITE_IS_OK)) ;

      #region Construction

      public WebsiteChecker() : base() { }

      #endregion

      #region Configuration
      public WebsiteChecker Configure (string name,
                                       string packageID,
                                       Website website,
                                       int? checkLoadTimeSeconds,
                                       string checkTextPattern,
                                       int? checkCertificateDays,
                                       List<SiteDownloadResult> siteDownloadResults) {
         base.Configure (name, packageID, website.Address) ;

         Website = website ;
         CheckLoadTimeSeconds = checkLoadTimeSeconds ;
         CheckTextPattern = checkTextPattern ;
         CheckCertificateExpiryDays = checkCertificateDays ;
         SiteDownloadResults = siteDownloadResults ;

         //CacheExpirySeconds = cacheExpirySeconds ;

         //TryCount = tryCount ;
         //RetryWaitTimeSeconds = retryWaitTimeSeconds ;

         return this ;
      }

      //protected override void Duplicate1 (Entity target) {
      //   (target.CastTo<WebsiteChecker>()).Configure (Name, PackageID, Website, CheckLoadTimeSeconds, CheckTextPattern, CheckCertificateExpiryDays,
      //                                                // CheckExpirySeconds, CheckLoadTimeSeconds, RetryCount, RetryWaitTimeSeconds) ;
      //                                                CacheExpirySeconds) ;
      //}

      //protected override Entity Create() {
      //   return new WebsiteChecker (_measuredDataStore) ;
      //}
      #endregion

      // private List<InstanceData.InstanceData> _instanceDataList = new List<InstanceData.InstanceData>();

      #region Properties
      public Website Website {get ; private set ;}
      public int? CheckLoadTimeSeconds {get ; private set ;}
      public string CheckTextPattern {get ; private set ;}
      public int? CheckCertificateExpiryDays {get ; private set ;}
      public List<SiteDownloadResult> SiteDownloadResults {get ; private set ;}

      // public int TryCount {get ; private set ;}
      // public int RetryWaitTimeSeconds {get ; private set ;}
      #endregion

      //      if (response.StatusCode != HttpStatusCode.OK) {
      //         return new XConstantString(MessageStrings.Get (MessageStrings.CANNOT_READ_FROM_WEBSITE),
      //                                     new XConstantString.Parameter (MessageStrings.WEB_SITE_ADDRESS, url), CreatePackageNameParameter(PackageID)) ;
      //      }

      //      using (Stream receiveStream = response.GetResponseStream()) {
      //         if (receiveStream == null) {
      //            return new XConstantString(MessageStrings.Get (MessageStrings.CANNOT_READ_FROM_WEBSITE),
      //                                        new XConstantString.Parameter (MessageStrings.WEB_SITE_ADDRESS, url), CreatePackageNameParameter(PackageID)) ;
      //         }

      //         using (StreamReader readStream = response.CharacterSet == null ? new StreamReader(receiveStream) :
      //                  new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet))) {

      //            // Debug.WriteLine ($"Website {website} character set is {response.CharacterSet}.") ;
      //            var bytesRead = readStream.ReadBlock(testBytes, 0, TEST_BYTES);
      //            // Debug.WriteLine ($"Bytes read {bytesRead} from website {website}: {new string (testBytes)}") ;
      //            // if (readStream.Read (testBytes, 0, TEST_BYTES) != TEST_BYTES) {

      //            if (bytesRead != TEST_BYTES) {
      //               return new XConstantString(MessageStrings.Get (MessageStrings.CANNOT_READ_FROM_WEBSITE),
      //                                           new XConstantString.Parameter (MessageStrings.WEB_SITE_ADDRESS, url), CreatePackageNameParameter(PackageID)) ;
      //            }
      //         }
      //      }
      //   }
      //   return null ;
      //} catch (Exception e) {
      //   Debug.WriteLine ($"Error accessing website '{url}'. Message: {e.GetAllMessages()}") ;
      //   Logger.Error ($"Error accessing website '{url}'. Message: {e.GetAllMessages()}") ;
      //   return new XConstantString(MessageStrings.Get (MessageStrings.ERROR_ACCESSING_WEBSITE),
      //                               new XConstantString.Parameter (MessageStrings.WEB_SITE_ADDRESS, url), CreatePackageNameParameter(PackageID)) ;
      //   // new XConstantString.Parameter (MessageStrings.ERROR_MESSAGE, e.Message),CreatePackageNameParameter (PackageID)) ; //DTAP the e.Message is:  Could not find file 'C:\Windows\system32\asdasdasd' this is not desirable here i think.
      //}

      //protected void StoreMeasuredData (string url,
      //                                  bool success,
      //                                  long? loadingTimeMilliSeconds,
      //                                  DateTime? certificateExpiryDate,
      //                                  XString message,
      //                                  XString details) {
      //   if (_measuredDataStore == null) return ;


      //   JObject measuredData = new JObject() ;
      //   measuredData.Add (MeasurementConstants.SUCCESS, success) ;
      //   measuredData.Add (MeasurementConstants.WEBSITE, url) ;
      //   measuredData.Add (MeasurementConstants.LOAD_TIME_SECONDS, (loadingTimeMilliSeconds / 1000.0)?.NormalizeValue()) ;
      //   measuredData.Add (MeasurementConstants.CERTIFICATE_EXPIRY, certificateExpiryDate) ;
      //   measuredData.Add (MeasurementConstants.MESSAGE, message.EmptyIfNull().AsJObject()) ;
      //   measuredData.Add (MeasurementConstants.DETAILS, details.EmptyIfNull().AsJObject()) ;

      //   measuredData.Add (MeasurementConstants.CHECK_TEXT_PATTERN, CheckTextPattern) ;

      //   _measuredDataStore?.StoreMeasuredData (PackageID,
      //                                          MeasurementConstants.TYPE_VALUE,
      //                                          DateTime.UtcNow,
      //                                          measuredData) ;
      //}

      protected override IList<CheckResult> Check1 (string[] tags) {
         var logger = new TagLogger (tags) ;

         // Check if this is the first check
         if (!SiteDownloadResults.Any()) {
            logger.Debug ($"No website check result found.") ;

            return new List<CheckResult> {
                     CheckResult.CreateUndefined (Name)
            } ;
         }

         CheckResult checkResult = null ;

         //int tryIndex = 0;
         //double retryFactor = 1.0;
         //while (true) {         
         //RetryCount,
         //RetryWaitTimeSeconds

         if (SiteDownloadResults.All (result => result.Success != SiteDownloadResultSuccess.Success)) {
            var siteDownloadResult = SiteDownloadResults.Last() ;
            logger.InfoWithDebug ($"Error when downloading website {Website.Address}. Message: {siteDownloadResult.Message}", $"Details: {siteDownloadResult.Details}") ;

            var message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorMessage),
                                               new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText)) ;
            var details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorDetails),
                                               new XConstantString.Parameter (Strings.Parameters.ErrorMessage, siteDownloadResult.Message)) ;
            var extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorExtract)) ;

            checkResult = new CheckResult().Configure (Name, CheckResultKind.Fail, message, details, extract) ;
         } else { 
            XStrings errorDetails = new XStrings() ;
            var siteDownloadResult = SiteDownloadResults.Last (result => result.Success == SiteDownloadResultSuccess.Success) ;

            // Check content
            if (!string.IsNullOrEmpty (CheckTextPattern)) {
               if (!Regex.IsMatch (siteDownloadResult.Content.ToLowerInvariant(), CheckTextPattern.ToLowerInvariant())) {
                  logger.Info ($"Site '{Website.DisplayText}' is available but does not contain '{CheckTextPattern}'.") ;

                  errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.CheckTextPatternFailed),
                                                         new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText), CreatePackageNameParameter (PackageID),
                                                         new XConstantString.Parameter (Strings.Parameters.CheckTextPattern, CheckTextPattern))) ;

                  // Log website contents
                  // string fileName = System.IO.Path.GetTempFileName() ;
                  // System.IO.File.WriteAllText (fileName, successResult.Content) ;
                  // Logger.Log (LogLevel.Info, $"Site '{Website.DisplayText}' is logged into '{fileName}'.") ;
               } else {
                  logger.Info ($"Site '{Website.DisplayText}' is available and contains '{CheckTextPattern}'.") ;
               }
            }

            // Check load time
            if (CheckLoadTimeSeconds != null && siteDownloadResult.LoadTimeMilliseconds != null) {
               if (TimeSpan.FromMilliseconds (siteDownloadResult.LoadTimeMilliseconds.Value) > TimeSpan.FromSeconds (CheckLoadTimeSeconds.Value)) {
                  logger.Info ($"Site '{Website.DisplayText}' is available but loads in more than {CheckLoadTimeSeconds} seconds.") ;

                  errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteLoadTimeTooMuch),
                                                         new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText), CreatePackageNameParameter (PackageID),
                                                         new XConstantString.Parameter (Strings.Parameters.WebSiteLoadTimeSeconds,
                                                                                        (int) TimeSpan.FromMilliseconds (siteDownloadResult.LoadTimeMilliseconds ?? 0).TotalSeconds))) ;
               }
            }

            // Check certificate expiry date
            if ((CheckCertificateExpiryDays != null) &&
                (siteDownloadResult.CertificateExpiryDate != null)) {

               int certificateExpiryDays = siteDownloadResult.CertificateExpiryDate.Value.Subtract (DateTime.Today).Days ;
               if (siteDownloadResult.CertificateExpiryDate < DateTime.Today) {
                  // Already expired
                  logger.Log (LogLevel.Info, $"Site '{Website.DisplayText}' certification expired on {siteDownloadResult.CertificateExpiryDate}.") ;

                  errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.CertificateExpired),
                                                         new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText),
                                                         new XConstantString.Parameter (Strings.Parameters.CertificateExpiryDate, siteDownloadResult.CertificateExpiryDate))) ;
               } else if (certificateExpiryDays < CheckCertificateExpiryDays) {
                  logger.Log (LogLevel.Info, $"Site '{Website.DisplayText}' certification expires on {siteDownloadResult.CertificateExpiryDate}.") ;

                  // Expires witin the given days
                  errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.CheckCertificateMinDaysFailed),
                                                         new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText),
                                                         new XConstantString.Parameter (Strings.Parameters.CertificateExpiryDays, certificateExpiryDays))) ;
               }
            }

            // Everything is OK?
            if (errorDetails.IsEmpty) {
               // Setup success result
               XString message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteIsOKMessage),
                                                      new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText)) ;

               XString details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteIsOKDetails),
                                                      new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText),
                                                      new XConstantString.Parameter (Strings.Parameters.WebSiteLoadTimeSeconds, (int) (siteDownloadResult.LoadTimeMilliseconds / 1000))) ;
               if (siteDownloadResult.CertificateExpiryDate != null) {
                  details = new XStrings (details,
                                          new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteIsOKDetailsExpireDate),
                                                               new XConstantString.Parameter (Strings.Parameters.CertificateExpiryDate, siteDownloadResult.CertificateExpiryDate))) ;
               }

               XString extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteIsOKExtract),
                                                      new XConstantString.Parameter (Strings.Parameters.WebSiteLoadTimeSeconds, (int) (siteDownloadResult.LoadTimeMilliseconds / 1000))) ;

               if (siteDownloadResult.CertificateExpiryDate != null) {
                  extract = new XStrings (extract,
                                          new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteIsOKExtractExpireDate),
                                                               new XConstantString.Parameter (Strings.Parameters.CertificateExpiryDate, siteDownloadResult.CertificateExpiryDate))) ;
               }

               checkResult = new CheckResult().Configure (Name, CheckResultKind.Success, message, details, extract) ;
            } else {
               WebsiteCheckCache.Delete (Website.Address) ;

               XString message ;
               XString details ;
               XString extract ;

               if (errorDetails.Values.Count == 1) {
                  message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorMessage),
                                                 new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText),
                                                 CreatePackageNameParameter (PackageID)) ;
                  details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorDetails),
                                                 new XConstantString.Parameter (Strings.Parameters.ErrorMessage, errorDetails.Values.First())) ;
                  extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorExtract)) ;
               } else {
                  // Multiple messages
                  message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorsMessage),
                                                 new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText),
                                                 CreatePackageNameParameter (PackageID)) ;
                  details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorsDetails),
                                                 new XConstantString.Parameter (Strings.Parameters.ErrorMessage, errorDetails)) ;
                  extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorsExtract)) ;
               }

               checkResult = new CheckResult().Configure (Name, CheckResultKind.Fail, message, details, extract) ;
            }
         }

         var successString = checkResult != null && checkResult.CheckResultKind == CheckResultKind.Success ? "successful" : "unsuccessful" ;
         logger.Info ($"Check of website '{Website.DisplayText}' was {successString}.") ;

         return new List<CheckResult> {
                  checkResult ?? new CheckResult().Configure (Name, CheckResultKind.Fail,
                                                              new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorMessage),
                                                                                   new XConstantString.Parameter (Strings.Parameters.Website, Website.DisplayText)),
                                                              new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorDetails),
                                                                                   new XConstantString.Parameter (Strings.Parameters.ErrorMessage, "Unknown error.")),
                                                              new XConstantString (Strings.Use.Get (Strings.Messages.Use.WebsiteCheckErrorExtract)))
         } ;
      }
   }
}