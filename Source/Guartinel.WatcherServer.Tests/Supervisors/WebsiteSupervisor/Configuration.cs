using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Service.WebsiteChecker ;
using Guartinel.WatcherServer.Supervisors.WebsiteSupervisor ;
using Newtonsoft.Json.Linq ;
using RequestStrings = Guartinel.Communication.Supervisors.WebsiteSupervisor.Strings.WatcherServerRoutes.Save.Request ;

namespace Guartinel.WatcherServer.Tests.Supervisors.WebsiteSupervisor {
   public static class Configuration {
      public static void ConfigureChecker (WebsiteChecker checker,
                                           string packageID,
                                           Website website,
                                           List<SiteDownloadResult> siteDownloadResults,
                                           int? checkLoadTimeSeconds = null,
                                           string checkTextPattern = null,
                                           int? checkCertificateDays = null) {
         checker.Configure ("checker1", packageID, website, checkLoadTimeSeconds, checkTextPattern, checkCertificateDays, siteDownloadResults) ;
      }

      public static void CreatePackageConfiguration (JObject configuration,
                                                     List<Website> websites,
                                                     int? checkLoadTimeSeconds = null,
                                                     string checkTextPattern = null,
                                                     int tryCount = WebsiteChecker.Defaults.TRY_COUNT,
                                                     int retryWaitTimeSeconds = WebsiteChecker.Defaults.RETRY_WAIT_TIME_SECONDS,
                                                     int? checkCertificateDays = null) {
         if (configuration == null) return ;

         JArray websitesArray = new JArray() ;
         foreach (var website in websites) {
            JObject websiteJObject = new JObject();

            websiteJObject [RequestStrings.DETAILED_WEBSITE_ADDRESS] = website.Address ;
            websiteJObject [RequestStrings.DETAILED_WEBSITE_CAPTION] = website.Caption ;
            
            websitesArray.Add (websiteJObject) ;
         }

         configuration [RequestStrings.DETAILED_WEBSITES] = websitesArray ;
         configuration [RequestStrings.CHECK_TEXT_PATTERN] = checkTextPattern ;
         configuration [RequestStrings.CHECK_CERTIFICATE_DAYS] = checkCertificateDays ;
         configuration [RequestStrings.CHECK_LOAD_TIME_SECONDS] = checkLoadTimeSeconds ;
         configuration [RequestStrings.TRY_COUNT] = tryCount ;
         configuration[RequestStrings.RETRY_WAIT_TIME_SECONDS] = retryWaitTimeSeconds ;
      }
   }
}