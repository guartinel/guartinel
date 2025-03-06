using System ;
using System.Linq ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using LogLevel = Guartinel.Kernel.Logging.LogLevel ;

namespace Guartinel.Service.WebsiteChecker {
   public abstract class SiteDownloader {
      public static class Constants {
         public const int MAX_TIMEOUT_SECONDS = 60 ;

         public const string HTTP_PREFIX = "http" ;
         public const string HTTPS_PREFIX = "https" ;

         public const int MAX_REDIRECTIONS = 50 ;

         public const string DEFAULT_USER_AGENT = "Mozilla / 5.0(compatible; Guartinel / 1.0) (+https://www.guartinel.com/about-us)" ;
      }

      //private SiteDownloadResult DownloadSiteChrome (Website website,
      //                                                  int timeoutSeconds = Constants.DEFAULT_TIMEOUT_SECONDS,
      //                                                  string userAgent = "",
      //                                                  string searchInPage = "") {
      //   try {
      //      return ChromeDownloader.DownloadPage (website, timeoutSeconds, userAgent, searchInPage) ;
      //   } catch (Exception e) {
      //      Logger.Info ($"Error accessing website '{website.DisplayText}'. Message: {e.GetAllMessages()}") ;

      //      // return new SiteDownloadResult.Fail (website, e.Message) ;
      //      return new SiteDownloadResult (e.Message) ;
      //   }
      //}

      //private SiteDownloadResult DownloadSiteHttpWebRequest (Website website,
      //                                                          int timeoutSeconds,
      //                                                          string userAgent) {

      //   try {
      //      return HttpRequest.DownloadPage (website, timeoutSeconds, userAgent) ;
      //   } catch (Exception e) {
      //      Logger.Log ($"Error accessing website '{website.DisplayText}'. Message: {e.GetAllMessages()}") ;

      //      // return new SiteDownloadResult.Fail (website, e.Message) ;
      //      return new SiteDownloadResult (e.Message) ;
      //   }
      //}

      protected static bool IsValidWebsiteUri (string website) {
         return Uri.TryCreate (website, UriKind.Absolute, out var uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) ;
      }

      protected static string BuildUri (string website,
                                        string scheme) {
         try {
            var builder = new UriBuilder (website) ;
            builder.Scheme = scheme ;
            return builder.Uri.ToString() ;
         } catch (UriFormatException) {
            throw new Exception ($"The address format of '{website}' is invalid. Use a valid website or IP address (like https://www.guartinel.com).") ;
         }
      }

      protected static string CorrectWebsiteSyntax (string website) {
         if (IsValidWebsiteUri (website)) return website ;

         // Try to add http
         var correctedWebsite = BuildUri (website, Constants.HTTP_PREFIX) ;
         if (IsValidWebsiteUri (correctedWebsite)) return correctedWebsite ;

         // Try to add https
         correctedWebsite = BuildUri (website, Constants.HTTPS_PREFIX) ;
         if (IsValidWebsiteUri (correctedWebsite)) return correctedWebsite ;

         return website ;
      }

      public SiteDownloadResult DownloadSite (Website website,
                                              int? timeoutSeconds = null,
                                              string searchInPage = "") {
         try {
            Logger.Log (LogLevel.Info, $"Site checker called for {website.Address}. Kind: {GetType().Name}. Timeout (sec): {timeoutSeconds}.") ;

            string userAgent = Constants.DEFAULT_USER_AGENT ;
            var correctedWebsite = new Website (CorrectWebsiteSyntax (website.Address), website.Caption) ;
            Logger.Debug ($"Corrected website address is '{correctedWebsite.Address}'.") ;

            var siteProperties = new HttpRequest().GetSiteProperties (correctedWebsite, timeoutSeconds, userAgent) ;
            Logger.Debug ("Site properties returned.") ;
            SiteDownloadResult siteDownloadResult = DownloadSite1 (correctedWebsite, timeoutSeconds, userAgent, searchInPage) ;

            if (siteDownloadResult.Success != SiteDownloadResultSuccess.Success) {
               Logger.Error ($"Site checker error for {website.Address}. Message: {siteDownloadResult.Message}") ;

               return new SiteDownloadResult (correctedWebsite,
                                              siteDownloadResult.Message,
                                              siteDownloadResult.Details) ;
            }

            Logger.Info ($"Site checker success for {website.Address}. Load time: {siteDownloadResult.LoadTimeMilliseconds} ms.") ;
            return new SiteDownloadResult (correctedWebsite,
                                           siteDownloadResult.LoadTimeMilliseconds,
                                           siteDownloadResult.Content,
                                           siteProperties.CertificateExpiryDate) ;
         } catch (Exception e) {
            if (e.Message.Contains ("Name or service not known") ||
                e.Message.Contains ("The operation was canceled.") ||
                e.Message.Contains ("Error forwarding the new session") ||
                e.Message.Contains ("session deleted because of page crash")) {
               Logger.ErrorWithDetails ($"Site checker internal error {website.Address}. Message: {e.Message}", e.GetAllMessages (false)) ;
               return new SiteDownloadResult (website, SiteDownloadResultSuccess.InternalError, e.Message, e.GetAllMessages (false)) ;
            }

            Logger.ErrorWithDetails ($"Site checker exception {website.Address}. Message: {e.Message}", e.GetAllMessages (false)) ;
            return new SiteDownloadResult (website, e.Message, e.GetAllMessages (false)) ;
         }
      }

      protected abstract SiteDownloadResult DownloadSite1 (Website website,
                                                           int? timeoutSeconds,
                                                           string userAgent,
                                                           string searchInPage) ;
   }
}
