using System;
using System.Collections.Generic;
using Guartinel.Communication.Strings;
using Newtonsoft.Json;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Communication.Supervisors.HostSupervisor {
   public class Strings : StringsBase {
      public override string Prefix => PackageType ;
      public string PackageType => "HOST_SUPERVISOR" ;

      protected Strings() {
         _languages.Add (new Languages.English()) ;
      }

      public static Strings Use {get ;} = new Strings() ;

      public interface IMessages {
         string HostIsNotAvailableAlertMessage {get ;}
         string HostIsNotAvailableAlertDetails { get; }
         string HostIsNotAvailableAlertExtract { get; }
         string HostIsOKMessage {get ;}
         string HostIsOKDetails { get; }
         string HostIsOKExtract { get; }
      }

      public class Messages : IMessages {
         public static Messages Use {get ;} = new Messages() ;

         public string HostIsNotAvailableAlertMessage => nameof(HostIsNotAvailableAlertMessage) ;
         public string HostIsNotAvailableAlertDetails => nameof(HostIsNotAvailableAlertDetails) ;
         public string HostIsNotAvailableAlertExtract => nameof(HostIsNotAvailableAlertExtract) ;
         public string HostIsOKMessage => nameof(HostIsOKMessage) ;
         public string HostIsOKDetails => nameof(HostIsOKDetails) ;
         public string HostIsOKExtract => nameof(HostIsOKExtract) ;
      }

      public static class Parameters {
         public static string Host => nameof(Host) ;
         public static string ErrorMessage => nameof(ErrorMessage);
         public static string LatencyInMilliseconds => nameof(LatencyInMilliseconds) ;
      }

      public class Properties {
         [JsonProperty]
         public const string DETAILED_HOSTS = "detailed_hosts" ;

         [JsonProperty]
         public const string DETAILED_HOST_ADDRESS = "address" ;

         [JsonProperty]
         public const string DETAILED_HOST_CAPTION = "caption" ;

         [JsonProperty]
         public const string HOSTS = "hosts" ;

         [JsonProperty]
         public const string RETRY_COUNT = "retry_count" ;

         [JsonProperty]
         public const string WAIT_TIME_SECONDS = "wait_time_seconds" ;

         [JsonProperty]
         public const string PING_TIME_MILLISECONDS = "ping_time" ;
      }

      public override Dictionary<string, string> GetProperties() {
         return Helper.ObjectToDictionary (new Properties()) ;
      }

      public static class ManagementServerRoutes {
         public static class RegisterMeasurement {
            public const string TYPE_VALUE = "HostStatusCheck" ;

            public static class Request {
               public const string TOKEN = Communication.Strings.Strings.AllParameters.TOKEN ;
               public const string MEASUREMENT = Communication.Strings.Strings.AllParameters.MEASUREMENT ;

               // public const string TYPE_VALUE = "HostCheck";

               public const string SUCCESS = AllParameters.SUCCESS ;
               public const string MESSAGE = AllParameters.MESSAGE ;
               public const string DETAILS = AllParameters.DETAILS ;
               public const string PING_TIME_MILLISECONDS = Properties.PING_TIME_MILLISECONDS ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = Communication.Strings.Strings.AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string ACCOUNT_EXPIRED = Communication.Strings.Strings.AllErrorValues.ACCOUNT_EXPIRED ;
                  public const string INVALID_TOKEN = Communication.Strings.Strings.AllErrorValues.INVALID_TOKEN ;
                  public const string NOT_USED_IN_PACKAGE = Communication.Strings.Strings.AllErrorValues.NOT_USED_IN_PACKAGE ;
               }
            }
         }
      }

      public static class WatcherServerRoutes {
         public static class Save {
            public static class Request {
               public const string HOSTS = Properties.HOSTS ;
               public const string DETAILED_HOSTS = Properties.DETAILED_HOSTS ;
               public const string DETAILED_HOST_ADDRESS = Properties.DETAILED_HOST_ADDRESS ;
               public const string DETAILED_HOST_CAPTION = Properties.DETAILED_HOST_CAPTION ;
               public const string RETRY_COUNT = Properties.RETRY_COUNT ;
               public const string WAIT_TIME_SECONDS = Properties.WAIT_TIME_SECONDS ;
            }
         }
      }
   }
}