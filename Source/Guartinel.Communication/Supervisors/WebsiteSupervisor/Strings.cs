using System;
using System.Collections.Generic;
using Guartinel.Communication.Strings;
using Newtonsoft.Json;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Communication.Supervisors.WebsiteSupervisor {
   public class Strings : StringsBase {
      public override string Prefix => PackageType ;
      public string PackageType => "WEBSITE_SUPERVISOR" ;

      private Strings() {
         _languages.Add (new Languages.English()) ;
      }

      public static Strings Use {get ;} = new Strings() ;

      public interface IMessages {
         // Messages
         string WebsiteIsNotAvailable {get ;}
         string CheckTextPatternFailed {get ;}
         string WebsiteLoadTimeTooMuch {get ;}
         string CheckCertificateMinDaysFailed {get ;}
         string CertificateExpired {get ;}
         string WebsiteIsOKMessage {get ;}
         string WebsiteIsOKDetails { get; }
         string WebsiteIsOKDetailsExpireDate { get; }
         string WebsiteIsOKExtract { get; }
         string WebsiteIsOKExtractExpireDate { get; }
         string WebsiteCheckErrorMessage {get ;}
         string WebsiteCheckErrorDetails {get ;}
         string WebsiteCheckErrorExtract { get; }
         string WebsiteCheckErrorsMessage { get; }
         string WebsiteCheckErrorsDetails { get; }
         string WebsiteCheckErrorsExtract { get; }
      }

      public class Messages : IMessages {
         public static Messages Use {get ;} = new Messages() ;

         public string WebsiteIsNotAvailable => nameof(WebsiteIsNotAvailable) ;
         public string CheckTextPatternFailed => nameof(CheckTextPatternFailed) ;
         public string WebsiteLoadTimeTooMuch => nameof(WebsiteLoadTimeTooMuch) ;
         public string CheckCertificateMinDaysFailed => nameof(CheckCertificateMinDaysFailed) ;
         public string CertificateExpired => nameof(CertificateExpired) ;
         public string WebsiteIsOKMessage => nameof(WebsiteIsOKMessage) ;
         public string WebsiteIsOKDetails => nameof(WebsiteIsOKDetails) ;
         public string WebsiteIsOKDetailsExpireDate => nameof(WebsiteIsOKDetailsExpireDate) ;
         public string WebsiteIsOKExtract => nameof(WebsiteIsOKExtract) ;
         public string WebsiteIsOKExtractExpireDate => nameof(WebsiteIsOKExtractExpireDate) ;
         public string WebsiteCheckErrorMessage => nameof(WebsiteCheckErrorMessage) ;
         public string WebsiteCheckErrorDetails => nameof(WebsiteCheckErrorDetails) ;
         public string WebsiteCheckErrorExtract => nameof(WebsiteCheckErrorExtract) ;
         public string WebsiteCheckErrorsMessage => nameof(WebsiteCheckErrorsMessage) ;
         public string WebsiteCheckErrorsDetails => nameof(WebsiteCheckErrorsDetails) ;
         public string WebsiteCheckErrorsExtract => nameof(WebsiteCheckErrorsExtract) ;
      }

      public static class Parameters {

         // Parameters         
         public static string Website => nameof(Website) ;

         public static string ErrorMessage => nameof(ErrorMessage) ;
         public static string WebSiteLoadTimeSeconds => nameof(WebSiteLoadTimeSeconds) ;
         public static string CheckTextPattern => nameof(CheckTextPattern) ;
         public static string CheckCertificateMinimumDays => nameof(CheckCertificateMinimumDays) ;
         public static string CertificateExpiryDate => nameof(CertificateExpiryDate) ;
         public static string CertificateExpiryDays => nameof(CertificateExpiryDays) ;
      }

      public override Dictionary<string, string> GetProperties() {
         return Helper.ObjectToDictionary (new Properties()) ;
      }

      public class Properties {
         [JsonProperty]
         public const string DETAILED_WEBSITES = AllParameters.DETAILED_WEBSITES ;

         [JsonProperty]
         public const string WEBSITES = AllParameters.WEBSITES ;

         [JsonProperty]
         public const string CHECK_TEXT_PATTERN = "check_text_pattern" ;

         [JsonProperty]
         public const string CHECK_CERTIFICATE_MINIMUM_DAYS = "check_certificate_minimum_days" ;

         [JsonProperty]
         public const string CHECK_LOAD_TIME_SECONDS = "check_load_time_seconds" ;

         [JsonProperty]
         public const string RETRY_COUNT = "retry_count" ;

         [JsonProperty]
         public const string RETRY_WAIT_TIME_SECONDS = "retry_wait_time_seconds" ;
      }

      public static class WatcherServerRoutes {
         public const string URL_BASE = "websiteSupervisor/" ;

         public static class Save {
            public static class Request {
               // public const string WEBSITE_ADDRESS = Common.AllParameters.WEBSITE_ADDRESS ;
               public const string DETAILED_WEBSITES = AllParameters.DETAILED_WEBSITES ;
               public const string DETAILED_WEBSITE_ADDRESS = AllParameters.DETAILED_WEBSITE_ADDRESS ;
               public const string DETAILED_WEBSITE_CAPTION = AllParameters.DETAILED_WEBSITE_CAPTION ;

               // If not defined, null or empty, then there is no check
               public const string CHECK_TEXT_PATTERN = Properties.CHECK_TEXT_PATTERN ;

               // If not defined, null, empty or zero, then there is no check
               public const string CHECK_CERTIFICATE_DAYS = Properties.CHECK_CERTIFICATE_MINIMUM_DAYS ;

               // If not defined, null, empty or zero, then there is no check
               public const string CHECK_LOAD_TIME_SECONDS = Properties.CHECK_LOAD_TIME_SECONDS ;

               // Retry count if no success
               public const string TRY_COUNT = Properties.RETRY_COUNT ;

               // Retry wait time
               public const string RETRY_WAIT_TIME_SECONDS = Properties.RETRY_WAIT_TIME_SECONDS ;
            }
         }

         public static class CheckWebsite {
            private const string URL_PART = "checkWebsite" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;

               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string WEBSITE_ADDRESS = AllParameters.WEBSITE_ADDRESS ;

               // If not defined, null, empty or zero, then there is no check
               public const string CHECK_LOAD_TIME_SECONDS = Properties.CHECK_LOAD_TIME_SECONDS ;

               // If not defined, null or empty, then there is no check
               public const string CHECK_TEXT_PATTERN = Properties.CHECK_TEXT_PATTERN ;

               // If not defined, null, empty or zero, then there is no check
               public const string CHECK_CERTIFICATE_DAYS = Properties.CHECK_CERTIFICATE_MINIMUM_DAYS ;

               // Retry count if no success
               public const string TRY_COUNT = Properties.RETRY_COUNT ;

               // Retry wait time
               public const string RETRY_WAIT_TIME_SECONDS = Properties.RETRY_WAIT_TIME_SECONDS ;

               // Reply URL
               public const string REPLY_URL = "REPLY_URL" ;
            }
         }

         public static class RegisterResult {
            private const string URL_PART = "registerResult" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;

               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string WEBSITE_ADDRESS = AllParameters.WEBSITE_ADDRESS ;

               // If not defined, null, empty or zero, then there is no check
               public const string CHECK_LOAD_TIME_SECONDS = Properties.CHECK_LOAD_TIME_SECONDS ;

               // If not defined, null or empty, then there is no check
               public const string CHECK_TEXT_PATTERN = Properties.CHECK_TEXT_PATTERN ;

               // If not defined, null, empty or zero, then there is no check
               public const string CHECK_CERTIFICATE_DAYS = Properties.CHECK_CERTIFICATE_MINIMUM_DAYS ;

               // Retry count if no success
               public const string TRY_COUNT = Properties.RETRY_COUNT ;

               // Retry wait time
               public const string RETRY_WAIT_TIME_SECONDS = Properties.RETRY_WAIT_TIME_SECONDS ;

               // Reply URL
               public const string REPLY_URL = "REPLY_URL" ;
            }
         }
      }

      public static class ManagementServerRoutes {
         public static class RegisterMeasurement {
            public const string WEBSITE = "website" ;
            public const string TYPE_VALUE = "WebsiteChecker" ;

            public const string SUCCESS = AllParameters.SUCCESS ;

            public const string LOAD_TIME_SECONDS = "load_time" ;
            public const string CERTIFICATE_EXPIRY = "certificate_expiry" ;
            public const string MESSAGE = AllParameters.MESSAGE ;
            public const string DETAILS = AllParameters.DETAILS ;

            public const string CHECK_TEXT_PATTERN = Properties.CHECK_TEXT_PATTERN ;
         }
      }
   }
}