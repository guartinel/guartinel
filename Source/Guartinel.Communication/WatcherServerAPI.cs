using Newtonsoft.Json ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.Communication {
   public static class WatcherServerAPI {
      #region General
      public class AllURLSerializationSummary {
         [JsonProperty]
         public const string PACKAGE_GET_ALL_WITH_TIME_STAMP = WatcherServerAPI.Packages.GetAllWithTimeStamp.FULL_URL ;

         [JsonProperty]
         public const string PACKAGE_SAVE = Packages.Save.FULL_URL ;

         [JsonProperty]
         public const string PACKAGE_DELETE = Packages.Delete.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_LOGIN = Admin.Login.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_CONFIRM_DEVICE_ALERT = Admin.ConfirmDeviceAlert.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_REGISTER = Admin.RegisterServer.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_GET_EVENTS = Admin.GetEvents.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_GET_VERSION = Admin.GetVersion.FULL_URL;


         [JsonProperty]
         public const string ADMIN_GET_STATUS = Admin.GetStatus.FULL_URL ;

         [JsonProperty]
         public const string APPLICATION_SUPERVISOR_CHECK_RESULTS = Supervisors.ApplicationSupervisor.Strings.WatcherServerRoutes.RegisterResult.FULL_URL ;
         [JsonProperty]
         public const string HARDWARE_SUPERVISOR_REGISTER_MEASUREMENTS = Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.RegisterMeasurement.FULL_URL;

      }

      public static class GeneralRequest {
         public const string TOKEN = AllParameters.TOKEN ;
      }

      /// <summary>
      /// General response.
      /// </summary>
      public static class GeneralResponse {
         public static class Names {
            public const string SUCCESS = AllParameters.SUCCESS ;
            public const string ERROR = AllParameters.ERROR ;
            public const string ERROR_UUID = AllParameters.ERROR_UUID ;
            public const string ERROR_PARAMETERS = AllParameters.ERROR_PARAMETERS ;
         }

         public static class SuccessValues {
            public const string SUCCESS = AllSuccessValues.SUCCESS ;
         }
      }

      public static class Default {
         public const string URL = "hello" ;

         public static class Request {}

         public static class Response {
            public const string CONTENT = "content" ;
         }
      }
      #endregion

      #region Alert
      public static class Alert {
         public const string URL_BASE = "alert/" ;

         public static class ConfirmDeviceAlert {
            private const string URL_PART = "confirmDeviceAlert" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string ALERT_ID = AllParameters.ALERT_ID ;
            }

            public static class Response {}
         }
      }
      #endregion

      #region Package
      public static class Packages {
         public const string URL_BASE = "packages/" ;

         public static class Save {
            private const string URL_PART = "save" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_TYPE = AllParameters.PACKAGE_TYPE ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string PACKAGE_NAME = AllParameters.PACKAGE_NAME ;
               public const string CATEGORIES = AllParameters.CATEGORIES ;
               public const string LICENSE = AllParameters.LICENSE ;
               public const string LAST_MODIFICATION_TIMESTAMP = AllParameters.LAST_MODIFICATION_TIMESTAMP ;

               // public const string ALERT_TEXT = Common.AllParameters.ALERT_TEXT ;
               public const string CHECK_INTERVAL_SECONDS = AllParameters.CHECK_INTERVAL_SECONDS ;
               public const string TIMEOUT_INTERVAL_SECONDS = AllParameters.TIMEOUT_INTERVAL_SECONDS;
               public const string STARTUP_DELAY_SECONDS = AllParameters.STARTUP_DELAY_SECONDS ;
               public const string FORCED_DEVICE_ALERT = AllParameters.FORCED_DEVICE_ALERT ;

               public const string ALERT_EMAILS = AllParameters.ALERT_EMAILS ;
               public const string ALERT_DEVICE_IDS = AllParameters.ALERT_DEVICE_IDS ;

               public const string CONFIGURATION = AllParameters.CONFIGURATION ;
               public const string STATE = AllParameters.STATE ;
               public const string DISABLE_ALERTS = AllParameters.DISABLE_ALERTS ;

               public static class State {
                  public const string STATES = AllParameters.PACKAGE_PART_STATES ;

                  public static class States {
                     public const string IDENTIFIER = AllParameters.PACKAGE_PART_IDENTIFIER ;
                     public const string STATE_NAME = AllParameters.PACKAGE_PART_STATE_NAME ;
                     public const string STATE_MESSAGE = AllParameters.PACKAGE_PART_STATE_MESSAGE ;
                     public const string STATE_DETAILS = AllParameters.PACKAGE_PART_STATE_DETAILS ;
                     public const string STATE_EXTRACT = AllParameters.PACKAGE_PART_STATE_EXTRACT ;
                  }                  
               }

               // public static class Configuration {}
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }

         public static class Delete {
            private const string URL_PART = "delete" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
            }

            public static class Response {}
         }

         public static class GetAllWithTimeStamp {
            private const string URL_PART = "getAllWithTimeStamp" ;

            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string TIMESTAMPS = AllParameters.TIMESTAMPS ;

               public static class Timestamp {
                  public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
                  public const string MODIFICATION_TIMESTAMP = AllParameters.TIMESTAMP ;
               }
            }
         }
      }
      #endregion PACKAGE

      #region Admin
      public static class Admin {
         public const string URL_BASE = "admin/" ;

         public static class GetVersion {
            private const string URL_PART = "getVersion";
            public const string FULL_URL = URL_BASE + URL_PART;

            public static class Request {               
            }

            public static class Response {
               public const string VERSION = "version" ;

               public static class Version {
                  public const string WATCHER_SERVER_VERSION = "watcher_server";
               }
            }
         }

         public static class Login {
            private const string URL_PART = "login" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string PASSWORD = AllParameters.PASSWORD ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;
            }
         }

         public static class RegisterServer {
            private const string URL_PART = "register" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string USER_NAME = AllParameters.USER_NAME ;
               public const string PASSWORD = AllParameters.PASSWORD ;
               public const string NEW_USER_NAME = AllParameters.NEW_USER_NAME ;
               public const string NEW_PASSWORD = AllParameters.NEW_PASSWORD ;
               public const string MANAGEMENT_SERVER_ADDRESS = AllParameters.MANAGEMENT_SERVER_ADDRESS ;
               public const string ONE_TIME_REGISTRATION_TOKEN = AllParameters.ONE_TIME_REGISTRATION_TOKEN ;
               public const string UID = AllParameters.UID ;

               public const string CATEGORIES = AllParameters.CATEGORIES ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string MANAGEMENT_SERVER_ID = AllParameters.MANAGEMENT_SERVER_ID ;
            }
         }

         public static class GetEvents {
            private const string URL_PART = "getEvents" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string EVENTS = AllParameters.EVENTS ;

               public static class Event {
                  public const string TIME_STAMP = "time_stamp" ;
                  public const string EVENT = "event" ;
               }
            }
         }

         public static class GetStatus {
            private const string URL_PART = "getStatus" ;

            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string STATUS = AllParameters.STATUS ;

               public static class Status {
                  public const string CPU = "cpu" ;
                  public const string STRESS_LEVEL = "stress_level" ;
                  public const string MEMORY = "memory" ;
                  public const string MEMORY_PERCENT = "memory_percent" ;
                  public const string HDD_FREE_GB = "hdd_free_gb" ;
                  public const string PACKAGE_COUNT = "package_count" ;
                  //public const string AVERAGE_LATENCY = "average_latency" ;
               }
            }
         }

         public static class ConfirmDeviceAlert {
            private const string URL_PART = "confirmDeviceAlert" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string ALERT_ID = AllParameters.ALERT_ID ;
               public const string DEVICE_ID = AllParameters.ALERT_DEVICE_ID ;
               // This is the agent ID or the application ID
               public const string INSTANCE_ID = AllParameters.INSTANCE_ID ;
            }

            public static class Response {}
         }
      }
      #endregion
   }
}
