using System ;
using System.Collections.Generic ;
using Guartinel.Communication.Languages ;
using Newtonsoft.Json ;

namespace Guartinel.Communication.Plugins.WebsiteSupervisor {
   public class Strings : PluginStrings {
      public override string PackageType => "WEBSITE_SUPERVISOR";

      public Strings() {
         _languages.Add (new Languages.English()) ;
      }

      // Messages
      public const string WEBSITE_IS_NOT_AVAILABLE = "WEBSITE_IS_NOT_AVAILABLE" ;
      public const string ERROR_ACCESSING_WEBSITE = "ERROR_ACCESSING_WEBSITE" ;
      public const string CHECK_TEXT_PATTERN_FAILED = "CHECK_TEXT_PATTERN_FAILED" ;
      public const string WEBSITE_LOAD_TIME_TOO_MUCH = "WEBSITE_LOAD_TIME_TOO_MUCH" ;
      public const string CHECK_CERTIFICATE_MIN_DAYS_FAILED = "CHECK_CERTIFICATE_MIN_DAYS_FAILED" ;
      public const string CERTIFICATE_EXPIRED = "CERTIFICATE_EXPIRED" ;
      public const string WEBSITE_IS_OK = "WEBSITE_IS_OK" ;

      // Parameters         
      public const string WEB_SITE_ADDRESS = "WEB_SITE_ADDRESS" ;
      public const string ERROR_MESSAGE = "ERROR_MESSAGE" ;
      public const string WEB_SITE_LOAD_TIME_MILLISECONDS = "WEB_SITE_LOAD_TIME_MILLISECONDS" ;
      public const string CHECK_TEXT_PATTERN = "CHECK_TEXT_PATTERN" ;
      public const string CHECK_CERTIFICATE_MINIMUM_DAYS = "CHECK_CERTIFICATE_MINIMUM_DAYS" ;
      public const string CERTIFICATE_EXPIRY_DATE = "CERTIFICATE_EXPIRY_DATE" ;
      public const string CERTIFICATE_EXPIRY_DAYS = "CERTIFICATE_EXPIRY_DAYS" ;

      public override Dictionary<string, string> GetProperties() {
         return new Dictionary<string, string>() ;
      }

      public static class Save {
         public static class Request {
            // public const string WEBSITE_ADDRESS = Common.AllParameters.WEBSITE_ADDRESS ;
            public const string WEBSITES = Common.AllParameters.WEBSITES ;
            // If not defined, null or empty, then there is no check
            public const string CHECK_TEXT_PATTERN = "check_text_pattern" ;
            // If not defined, null, empty or zero, then there is no check
            public const string CHECK_CERTIFICATE_DAYS = "check_certificate_minimum_days" ;
            // If not defined, null, empty or zero, then there is no check
            public const string CHECK_LOAD_TIME_SECONDS = "check_load_time_seconds" ;
            //public const string RETRY_COUNT = "retry_count" ;
            //public const string RETRY_WAIT_TIME_SECONDS = "retry_wait_time_seconds" ;
         }
      }

      public static class Measurement {
         public const string WEBSITE = "website" ;
         public const string TYPE_VALUE = "WebsiteChecker" ;

         public const string SUCCESS = Common.AllParameters.SUCCESS ;
         // public const string LOAD_TIME_MILLISECONDS = Parameters.LOAD_TIME_MILLISECONDS ;
         public const string LOAD_TIME_MILLISECONDS = "load_time_milliseconds" ;
         public const string CHECK_TEXT_PATTERN = "check_text_pattern" ;
         public const string CHECK_CERTIFICATE_DAYS = "check_certificate_days" ;
         public const string MESSAGE = Common.AllParameters.MESSAGE ;
      }
   }
}