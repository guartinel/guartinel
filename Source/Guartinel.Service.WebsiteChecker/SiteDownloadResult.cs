using System;
using System.Linq;
using System.Text;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.WebsiteChecker {
   public enum SiteDownloadResultSuccess {
      Success,
      Error,
      InternalError
   }

   public class SiteDownloadResult {
      public SiteDownloadResult (Website website,
                                 SiteDownloadResultSuccess success,
                                 long? loadTimeMilliseconds,
                                 string content,
                                 string message,
                                 string details,
                                 DateTime? certificateExpiryDate) {
         Website = website ;
         Success = success ;
         LoadTimeMilliseconds = loadTimeMilliseconds ;
         Content = content ;
         Message = message ;
         Details = details ;
         CertificateExpiryDate = certificateExpiryDate ;
      }

      public SiteDownloadResult (Website website,
                                 long? loadTimeMilliseconds,
                                 string content,
                                 DateTime? certificateExpiryDate) : this (website, SiteDownloadResultSuccess.Success, loadTimeMilliseconds, content, null, null, certificateExpiryDate) { }

      public SiteDownloadResult (Website website,
                                 string message,
                                 string details) : this (website, SiteDownloadResultSuccess.Error, null, null, message, details, null) { }

      public SiteDownloadResult (Website website,
                                 SiteDownloadResultSuccess success,
                                 string message,
                                 string details) : this(website, success, null, null, message, details, null) { }

      public Website Website {get ;}
      public SiteDownloadResultSuccess Success {get ;}
      public long? LoadTimeMilliseconds {get ;}
      public string Content {get ;}
      public string Message {get ;}
      public string Details { get; }
      public DateTime? CertificateExpiryDate { get; }

      public JObject AsJObject() {
         JObject result = new JObject() ;

         result [nameof(Website.Address).NameToJSONName()] = Website.Address ;
         result [nameof(Website.Caption).NameToJSONName()] = Website.Caption ;

         result [nameof(Success).NameToJSONName()] = Success == SiteDownloadResultSuccess.Success ;

         if (LoadTimeMilliseconds > 0) {
            result [nameof(LoadTimeMilliseconds).NameToJSONName()] = LoadTimeMilliseconds ;
         }

         if (!string.IsNullOrEmpty (Content)) {
            result [nameof(Content).NameToJSONName()] = Content ;
         }

         if (!string.IsNullOrEmpty (Message)) {
            result [nameof(Message).NameToJSONName()] = Message ;
         }

         if (!string.IsNullOrEmpty (Details)) {
            result [nameof(Details).NameToJSONName()] = Details ;
         }

         if (CertificateExpiryDate != null) {
            result[nameof(CertificateExpiryDate).NameToJSONName()] = CertificateExpiryDate ;
         }

         return result ;
      }

      public static SiteDownloadResult FromJObject (JObject jobject) {

         var success = jobject.GetBooleanValue (nameof(Success).NameToJSONName(), false) ;
         var website = new Website (jobject.GetStringValue (nameof(WebsiteChecker.Website.Address).NameToJSONName()),
                                    jobject.GetStringValue (nameof(WebsiteChecker.Website.Caption).NameToJSONName())) ;

         var loadTimeMilliseconds = jobject.GetIntegerValueNull (nameof(LoadTimeMilliseconds).NameToJSONName()) ;
         var content = jobject.GetStringValue (nameof(Content).NameToJSONName()) ;
         var message = jobject.GetStringValue (nameof(Message).NameToJSONName()) ;
         var messageDetails = jobject.GetStringValue (nameof(Details).NameToJSONName()) ;
         DateTime? certificateExpiryDate = jobject.GetDateTimeValue(nameof(CertificateExpiryDate).NameToJSONName(), DateTime.MinValue) ;
         certificateExpiryDate = certificateExpiryDate == DateTime.MinValue ? null : certificateExpiryDate ;

         var result = new SiteDownloadResult (website, success ? SiteDownloadResultSuccess.Success : SiteDownloadResultSuccess.Error, loadTimeMilliseconds, content, message, messageDetails, certificateExpiryDate) ;

         return result ;
      }
   }
}