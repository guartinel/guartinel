using System;
using System.Linq;
using System.Text;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.WebsiteChecker {
   public class SiteDownloadRequest {
      public SiteDownloadRequest (Website website,
                                  string downloadType,
                                  int? timeoutSeconds,
                                  int? tryCount,
                                  int? retryWaitingSeconds,
                                  string searchInPage) {
         Website = website ;
         DownloadType = downloadType ;
         TimeoutSeconds = timeoutSeconds ;
         TryCount = tryCount ;
         RetryWaitingSeconds = retryWaitingSeconds ;
         SearchInPage = searchInPage ;
      }

      public Website Website {get ;}
      public string DownloadType {get ;}
      public int? TimeoutSeconds {get ;}
      public int? TryCount {get ;}
      public int? RetryWaitingSeconds {get ;}
      public string SearchInPage {get ;}

      public JObject AsJObject() {
         JObject result = new JObject() ;

         result [nameof(Website.Address).NameToJSONName()] = Website.Address ;
         result [nameof(Website.Caption).NameToJSONName()] = Website.Caption ;
         result [nameof(DownloadType).NameToJSONName()] = DownloadType ;
         result [nameof(TimeoutSeconds).NameToJSONName()] = TimeoutSeconds ;
         result [nameof(TryCount).NameToJSONName()] = TryCount ;
         result [nameof(RetryWaitingSeconds).NameToJSONName()] = RetryWaitingSeconds ;
         result [nameof(SearchInPage).NameToJSONName()] = SearchInPage ;

         return result ;
      }

      public static SiteDownloadRequest FromJObject (JObject jobject) {
         var website = new Website (jobject.GetStringValue (nameof(WebsiteChecker.Website.Address).NameToJSONName()),
                                    jobject.GetStringValue (nameof(WebsiteChecker.Website.Caption).NameToJSONName())) ;
         var downloadType = jobject.GetStringValue (nameof(DownloadType).NameToJSONName()) ;
         var timeoutSeconds = jobject.GetIntegerValue (nameof(TimeoutSeconds).NameToJSONName(), 0) ;
         var tryCount = jobject.GetIntegerValue (nameof(TryCount).NameToJSONName(), 0) ;
         var retryWaitingSeconds = jobject.GetIntegerValue (nameof(RetryWaitingSeconds).NameToJSONName(), 0) ;
         var searchInPage = jobject.GetStringValue (nameof(SearchInPage).NameToJSONName()) ;

         var result = new SiteDownloadRequest (website, downloadType, timeoutSeconds, tryCount, retryWaitingSeconds, searchInPage) ;

         return result ;
      }
   }
}