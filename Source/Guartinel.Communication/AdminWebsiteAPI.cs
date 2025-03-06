using Newtonsoft.Json ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters ;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.Communication {
   public static class AdminWebsiteAPI {
      public const string URL = "api" ;

      public class AllURLSerializationSummary {
         [JsonProperty]
         public const string MANAGEMENT_SERVER_ADD = ManagementServer.Add.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_GET_EXISTING = ManagementServer.GetExisting.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_UPDATE = ManagementServer.Update.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_REMOVE = ManagementServer.Remove.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_DATABASE_GET_STATUS = ManagementServer.Database.GetStatus.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_DATABASE_UPDATE = ManagementServer.Database.Update.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_STATUS_GET_STATUS = ManagementServer.Status.GetStatus.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_STATUS_GET_EVENTS = ManagementServer.Status.GetEvents.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_STATUS_GET_LOG = ManagementServer.Status.GetLog.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_STATUS_GET_INVALID_REQUESTS = ManagementServer.Status.GetInvalidRequests.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_WATCHER_SERVER_GET_EXISTING = ManagementServer.WatcherServer.GetExisting.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_WATCHER_SERVER_GET_EVENTS = ManagementServer.WatcherServer.GetEvents.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_WATCHER_SERVER_GET_STATUS = ManagementServer.WatcherServer.GetStatus.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_WATCHER_SERVER_REGISTER = ManagementServer.WatcherServer.Register.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_WATCHER_SERVER_REMOVE = ManagementServer.WatcherServer.Remove.URL ;

         [JsonProperty]
         public const string MANAGEMENT_SERVER_WATCHER_SERVER_UPDATE = ManagementServer.WatcherServer.Update.URL ;

         [JsonProperty]
         public const string WEB_SITE_STATUS_GET_STATUS = WebSiteStatus.GetStatus.URL ;

         [JsonProperty]
         public const string ADMINISTRATOR_LOGIN = Administrator.Login.URL ;

         [JsonProperty]
         public const string ADMINISTRATOR_GET_STATUS = Administrator.GetStatus.URL ;

         [JsonProperty]
         public const string ADMINISTRATOR_UPDATE = Administrator.Update.URL ;

         [JsonProperty]
         public const string ADMINISTRATOR_LOGOUT = Administrator.Logout.URL ;

         [JsonProperty]
         public const string USER_WEB_SERVER_REGISTER = UserWebServer.Register.URL ;

         [JsonProperty]
         public const string USER_WEB_SERVER_GET_STATUS = UserWebServer.GetStatus.URL ;

         [JsonProperty]
         public const string USER_WEB_SERVER_UPDATE = UserWebServer.Update.URL ;

         [JsonProperty]
         public const string USER_WEB_SERVER_REMOVE = UserWebServer.Remove.URL ;

         [JsonProperty]
         public const string USER_WEB_SERVER_GET_AVAILABLE = UserWebServer.GetAvailable.URL ;
      }

      #region USER_WEB_SERVER
      public static class UserWebServer {
         public const string URL_PART = "UserWebServer" ;
         public const string URL = AdminWebsiteAPI.URL + "/" + URL_PART ;

         public static class Register {
            public const string URL_PART = "Register" ;
            public const string URL = UserWebServer.URL + "/" + URL_PART ;

            public static class Request {
               public const string USER_WEB_SERVER_ADDRESS = AllParameters.USER_WEB_SERVER_ADDRESS ;
               public const string PASSWORD = AllParameters.PASSWORD ;
               public const string USER_NAME = AllParameters.USER_NAME ;
               public const string NAME = AllParameters.NAME ;
            }
         }

         public static class Remove {
            public const string URL_PART = "Remove" ;
            public const string URL = UserWebServer.URL + "/" + URL_PART ;

            public static class Request {}
         }

         public static class GetStatus {
            public const string URL_PART = "GetStatus" ;
            public const string URL = UserWebServer.URL + "/" + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }
         }

         public static class Update {
            public const string URL_PART = "Update" ;
            public const string URL = UserWebServer.URL + "/" + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string ADDRESS = AllParameters.ADDRESS ;
               public const string USER_WEB_SERVER_ADDRESS = AllParameters.USER_WEB_SERVER_ADDRESS ;
            }
         }

         public static class GetAvailable {
            public const string URL_PART = "GetAvailable" ;
            public const string URL = UserWebServer.URL + "/" + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string ADDRESS = AllParameters.ADDRESS ;
               public const string NAME = AllParameters.NAME ;
            }
         }
      }
      #endregion USER_WEB_SERVER

      #region MANAGEMENT 
      public static class ManagementServer {
         public const string URL_PART = "ManagementServer" ;
         public const string URL = AdminWebsiteAPI.URL + "/" + URL_PART ;

         public static class Add {
            public const string URL_PART = "Add" ;
            public const string URL = ManagementServer.URL + "/" + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string NAME = AllParameters.NAME ;
               public const string USER_NAME = AllParameters.USER_NAME ;
               public const string PASSWORD = AllParameters.PASSWORD ;
               public const string ADDRESS = AllParameters.ADDRESS ;
               public const string PORT = AllParameters.PORT ;
               public const string DESCRIPTION = AllParameters.DESCRIPTION ;
               public const string EMAIL_PROVIDER = AllParameters.EMAIL_PROVIDER ;
               public const string EMAIL_PASSWORD = AllParameters.EMAIL_PASSWORD ;
               public const string EMAIL_USER_NAME = AllParameters.EMAIL_USER_NAME ;
            }

            public static class Response {
               public const string SUCCESS = AllParameters.SUCCESS ;

               public static class SuccessValues {
                  public const string SUCCESS = AllSuccessValues.SUCCESS ;
               }

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class GetExisting {
            public const string URL_PART = "GetExisting" ;
            public const string URL = ManagementServer.URL + "/" + URL_PART ;

            public static class Response {
               public const string MANAGEMENT_SERVER = AllParameters.MANAGEMENT_SERVER ;
               public const string SUCCESS = AllParameters.SUCCESS ;
               public const string ID = AllParameters.ID ;
               public const string NAME = AllParameters.NAME ;
               public const string DESCRIPTION = AllParameters.DESCRIPTION ;
               public const string ADDRESS = AllParameters.ADDRESS ;
               public const string PORT = AllParameters.PORT ;
               public const string USER_NAME = AllParameters.USER_NAME ;

               public static class SuccessValues {
                  public const string SUCCESS = AllSuccessValues.SUCCESS ;
               }

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class Update {
            public const string URL_PART = "Update" ;
            public const string URL = ManagementServer.URL + "/" + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string NAME = AllParameters.NAME ;
               public const string DESCRIPTION = AllParameters.DESCRIPTION ;
               public const string ADDRESS = AllParameters.ADDRESS ;
               public const string PORT = AllParameters.PORT ;
               public const string USER_NAME = AllParameters.USER_NAME ;
               public const string PASSWORD = AllParameters.PASSWORD ;
            }

            public static class Response {
               public const string CONFIGURED = AllParameters.CONFIGURED ;
               public const string FIRST_NAME = AllParameters.FIRST_NAME ;
               public const string LAST_NAME = AllParameters.LAST_NAME ;
               public const string SUCCESS = AllParameters.SUCCESS ;

               public static class SuccessValues {
                  public const string SUCCESS = AllSuccessValues.SUCCESS ;
               }

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class Remove {
            public const string URL_PART = "Remove" ;
            public const string URL = ManagementServer.URL + "/" + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string ID = AllParameters.ID ;
            }
         }

         public static class Database {
            public const string URL_PART = "Database" ;
            public const string URL = ManagementServer.URL + "/" + URL_PART ;

            public static class GetStatus {
               public const string URL_PART = "GetStatus" ;
               public const string URL = Database.URL + "/" + URL_PART ;
            }

            public static class Remove {
               public const string URL_PART = "Remove" ;
               public const string URL = Database.URL + "/" + URL_PART ;
            }

            public static class Update {
               public const string URL_PART = "Update" ;
               public const string URL = Database.URL + "/" + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string DATA_BASE_URL = AllParameters.DATABASE_URL ;
                  public const string USER_NAME = AllParameters.USER_NAME ;
                  public const string PASSWORD = AllParameters.PASSWORD ;
               }
            }
         }

         #region STATUS
         public static class Status {
            public const string URL_PART = "Status" ;
            public const string URL = ManagementServer.URL + "/" + URL_PART ;

            public static class GetStatus {
               public const string URL_PART = "GetStatus" ;
               public const string URL = Status.URL + "/" + URL_PART ;
            }

            public static class GetEvents {
               public const string URL_PART = "GetEvents" ;
               public const string URL = Status.URL + "/" + URL_PART ;
            }

            public static class GetLog {
               public const string URL_PART = "GetLog" ;
               public const string URL = Status.URL + "/" + URL_PART ;
            }

            public static class GetInvalidRequests {
               public const string URL_PART = "GetInvalidRequests" ;
               public const string URL = Status.URL + "/" + URL_PART ;
            }
         }
         #endregion STATUS

         #region WATCHER_SERVER
         public static class WatcherServer {
            public const string URL_PART = "WatcherServer" ;
            public const string URL = ManagementServer.URL + "/" + URL_PART ;

            public static class GetExisting {
               public const string URL_PART = "GetExisting" ;
               public const string URL = WatcherServer.URL + "/" + URL_PART ;

               public static class Request {
                  public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;
               }
            }

            public static class GetEvents {
               public const string URL_PART = "GetEvents" ;
               public const string URL = WatcherServer.URL + "/" + URL_PART ;

               public static class Request {
                  public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;
               }
            }

            public static class GetStatus {
               public const string URL_PART = "GetStatus" ;
               public const string URL = WatcherServer.URL + "/" + URL_PART ;

               public static class Request {
                  public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;
               }
            }

            public static class Register {
               public const string URL_PART = "Register" ;
               public const string URL = WatcherServer.URL + "/" + URL_PART ;

               public static class Request {
                  public const string NAME = AllParameters.NAME ;
                  public const string ADDRESS = AllParameters.ADDRESS ;
                  public const string PORT = AllParameters.PORT ;
                  public const string USER_NAME = AllParameters.USER_NAME ;
                  public const string PASSWORD = AllParameters.PASSWORD ;
                  public const string NEW_USER_NAME = AllParameters.NEW_USER_NAME ;
                  public const string NEW_PASSWORD = AllParameters.NEW_PASSWORD ;
                  public const string CATEGORIES = AllParameters.CATEGORIES ;
               }
            }

            public static class Remove {
               public const string URL_PART = "Remove" ;
               public const string URL = WatcherServer.URL + "/" + URL_PART ;
            }

            public static class Update {
               public const string URL_PART = "Update" ;
               public const string URL = WatcherServer.URL + "/" + URL_PART ;

               public static class Request {
                  public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;
                  public const string NAME = AllParameters.NAME ;
                  public const string ADDRESS = AllParameters.ADDRESS ;
                  public const string PORT = AllParameters.PORT ;
                  public const string USER_NAME = AllParameters.USER_NAME ;
                  public const string PASSWORD = AllParameters.PASSWORD ;
               }
            }
         }
         #endregion WATCHER_SERVER
      }
      #endregion MANAGEMENT

      #region WEBSERVER_STATUS
      public static class WebSiteStatus {
         public const string URL_PART = "WebsiteStatus" ;
         public const string URL = AdminWebsiteAPI.URL + "/" + URL_PART ;

         public static class GetStatus {
            public const string URL_PART = "GetStatus" ;
            public const string URL = WebSiteStatus.URL + "/" + URL_PART ;
         }
      }
      #endregion WEBSERVER_STATUS

      #region ADMIN
      public static class Administrator {
         public const string URL_PART = "Administrator" ;
         public const string URL = AdminWebsiteAPI.URL + "/" + URL_PART ;

         public static class Login {
            public const string URL_PART = "Login" ;
            public const string URL = Administrator.URL + "/" + URL_PART ;

            public static class Request {
               public const string USER_NAME = AllParameters.USER_NAME ;
               public const string PASSWORD = AllParameters.PASSWORD ;
            }
         }

         public static class GetStatus {
            public const string URL_PART = "GetStatus" ;
            public const string URL = Administrator.URL + "/" + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string CONFIGURED = AllParameters.CONFIGURED ;
               public const string FIRST_NAME = AllParameters.FIRST_NAME ;
               public const string LAST_NAME = AllParameters.LAST_NAME ;
               public const string SUCCESS = AllParameters.SUCCESS ;

               public static class SuccessValues {
                  public const string SUCCESS = AllSuccessValues.SUCCESS ;
               }

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class Update {
            public const string URL_PART = "Update" ;
            public const string URL = Administrator.URL + "/" + URL_PART ;

            public static class Request {
               public const string USER_NAME = AllParameters.USER_NAME ;
               public const string PASSWORD = AllParameters.PASSWORD ;
               public const string NEW_PASSWORD = AllParameters.NEW_PASSWORD ;
               public const string TOKEN = AllParameters.TOKEN ;
            }
         }

         public static class Logout {
            public const string URL_PART = "Logout" ;
            public const string URL = Administrator.URL + "/" + URL_PART ;
         }
      }
      #endregion ADMIN
   }
}
