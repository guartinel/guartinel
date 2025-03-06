using System ;
using Newtonsoft.Json;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;
using AllDeviceTypeValues = Guartinel.Communication.Strings.Strings.AllDeviceTypeValues;

namespace Guartinel.Communication {
   public static class ManagementServerAPI {
      public class AllURLSerializationSummary {

         [JsonProperty]
         public const string GET_VERSION = GetVersion.FULL_URL ;

         [JsonProperty]
         public const string API_GET_TOKEN = API.GetToken.FULL_URL ;

         [JsonProperty]
         public const string API_PACKAGE_DELETE = API.Package.Delete.FULL_URL ;

         [JsonProperty]
         public const string API_PACKAGE_GET_ALL = API.Package.GetAll.FULL_URL ;

         [JsonProperty]
         public const string API_PACKAGE_GET_PACKAGE = API.Package.GetPackage.FULL_URL ;

         [JsonProperty]
         public const string API_PACKAGE_GET_VERSION = API.Package.GetVersion.FULL_URL ;

         [JsonProperty]
         public const string API_PACKAGE_SAVE = API.Package.Save.FULL_URL ;

         [Obsolete]
         [JsonProperty]
         public const string HARDWARE_SUPERVISOR_REGISTER_MEASUREMENT = Supervisors.HardwareSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.FULL_URL ;

         [JsonProperty]
         public const string HARDWARE_SUPERVISOR_REGISTER_MEASURED_DATA = Supervisors.HardwareSupervisor.Strings.ManagementServerRoutes.RegisterMeasuredData.FULL_URL ;

         [JsonProperty]
         public const string HARDWARE_SUPERVISOR_REGISTER_HARDWARE = Supervisors.HardwareSupervisor.Strings.ManagementServerRoutes.RegisterHardware.FULL_URL ;

         [JsonProperty]
         public const string HARDWARE_SUPERVISOR_REGISTER = Supervisors.HardwareSupervisor.Strings.ManagementServerRoutes.Register.FULL_URL ;


         [JsonProperty]
         public const string HARDWARE_SUPERVISOR_CHECK_FOR_UPDATE = Supervisors.HardwareSupervisor.Strings.ManagementServerRoutes.CheckForUpdate.FULL_URL ;

         [JsonProperty]
         public const string HARDWARE_SUPERVISOR_REMOTE_LOG = Supervisors.HardwareSupervisor.Strings.ManagementServerRoutes.RemoteLog.FULL_URL ;


         [JsonProperty]
         public const string HARDWARE_SUPERVISOR_VALIDATE_HARDWARE = Supervisors.HardwareSupervisor.Strings.ManagementServerRoutes.ValidateHardware.FULL_URL ;

         [JsonProperty]
         public const string APPLICATION_SUPERVISOR_REGISTER_MEASUREMENT = Supervisors.ApplicationSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.FULL_URL ;

         [JsonProperty]
         public const string APPLICATION_SUPERVISOR_GET_APPLICATION_INSTANCE_IDS = Supervisors.ApplicationSupervisor.Strings.ManagementServerRoutes.GetApplicationInstanceIDs.FULL_URL ;

         [JsonProperty]
         public const string ALERT_SEND_EMAIL = Alert.SendEmail.FULL_URL ;

         [JsonProperty]
         public const string ALERT_CONFIRM_DEVICE_ALERT = Alert.ConfirmDeviceAlert.FULL_URL ;

         [JsonProperty]
         public const string ALERT_UNSUBSCRIBE_FROM_PACKAGE_EMAIL = Alert.UnSubscribeFromPackageEmail.FULL_URL ;

         [JsonProperty]
         public const string ALERT_UNSUBSCRIBE_ALL_EMAIL = Alert.UnsubscribeAllEmail.FULL_URL ;

         [JsonProperty]
         public const string ALERT_SEND_DEVICE_ALERT = Alert.SendDeviceAlert.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_LOGIN = Account.Login.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_CREATE = Account.Create.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_FREEZE = Account.Freeze.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_LOGOUT = Account.Logout.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_DELETE = Account.Delete.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_GET_STATUS = Account.GetStatus.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_UPDATE = Account.Update.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_RESEND_ACTIVATION_CODE = Account.ResendActivationCode.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_SEND_NEW_PASSWORD = Account.SendNewPassword.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_VERIFY_SEND_NEW_PASSWORD = Account.VerifySendNewPassword.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_VALIDATE_TOKEN = Account.ValidateToken.FULL_URL ;

         [JsonProperty]
         public const string ACCOUNT_ACTIVATE_ACCOUNT = Account.Activate.FULL_URL ;

         [JsonProperty]
         public const string DEVICE_GET_AVAILABLE = Device.GetAvailable.FULL_URL ;

         [JsonProperty]
         public const string DEVICE_DELETE = Device.Delete.FULL_URL ;

         [JsonProperty]
         public const string DEVICE_REGISTER = Device.Register.FULL_URL ;

         [JsonProperty]
         public const string DEVICE_EDIT = Device.Edit.FULL_URL ;

         [JsonProperty]
         public const string DEVICE_DISCONNECT = Device.Disconnect.FULL_URL ;

         [JsonProperty]
         public const string DEVICE_LOGIN = Device.Login.FULL_URL ;

         [JsonProperty]
         public const string DEVICE_ANDROID_TEST = Device.Android.Test.FULL_URL ;

         [JsonProperty]
         public const string DEVICE_ANDROID_LOGIN = Device.Android.Login.FULL_URL ;

         [JsonProperty]
         public const string DEVICE_ANDROID_REGISTER = Device.Android.Register.FULL_URL ;


         [JsonProperty]
         public const string EMAIL_SUPERVISOR_REGISTER_MEASUREMENT = Supervisors.EmailSupervisor.Strings.ManagementServerRoutes.RegisterMeasurement.FULL_URL ;

         [JsonProperty]
         public const string LICENSE_GET_AVAILABLE = License.GetAvailable.FULL_URL ;

         [JsonProperty]
         public const string LICENSE_GET_LICENSE_ORDER = License.GetLicenseOrder.FULL_URL ;

         [JsonProperty]
         public const string LICENSE_SAVE_LICENSE_ORDER = License.SaveLicenseOrder.FULL_URL ;

         [JsonProperty]
         public const string LICENSE_ADD_TO_ACCOUNT = License.AddToAccount.FULL_URL ;

         [JsonProperty]
         public const string LICENSE_ACTIVATE_LICENSE = License.ActivateLicense.FULL_URL ;

         [JsonProperty]
         public const string PACKAGE_SAVE = Package.Save.FULL_URL ;

         [JsonProperty]
         public const string PACKAGE_DELETE = Package.Delete.FULL_URL ;

         [JsonProperty]
         public const string PACKAGE_GET_AVAILABLE = Package.GetAvailable.FULL_URL ;

         [JsonProperty]
         public const string PACKAGE_GET_STATISTICS = Package.GetStatistics.FULL_URL ;

         [JsonProperty]
         public const string PACKAGE_TEST_EMAIL = Package.SendTestEmail.FULL_URL ;

         [JsonProperty]
         public const string PACKAGE_REMOVE_ACCESS = Package.RemoveAccess.FULL_URL ;


         [JsonProperty]
         public const string PACKAGE_STORE_MEASUREMENT = Package.StoreMeasurement.FULL_URL ;

         [JsonProperty]
         public const string PACKAGE_STORE_STATE = Package.StoreState.FULL_URL ;

         [JsonProperty]
         public const string WATCHER_SERVER_LOGIN = WatcherServer.Login.FULL_URL ;

         [JsonProperty]
         public const string WATCHER_SERVER_REGISTER = WatcherServer.Register.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_SEND_MAINTENANCE_EMAIL = Admin.SendMaintenanceEmail.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_UPDATE = Admin.Update.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_LOGIN = Admin.Login.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_LOGOUT = Admin.Logout.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_DATABASE_GET_STATUS = Admin.Database.GetStatus.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_DATABASE_UPDATE = Admin.Database.Update.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_GET_SUPERVISOR_STATUS = Admin.GetSupervisorStatus.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_RUN_JOB = Admin.RunJob.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_SET_WEBSITE_ADDRESS = Admin.SetWebSiteAddress.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_STATUS_GET_STATUS = Admin.Status.GetStatus.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_STATUS_GET_EVENTS = Admin.Status.GetEvents.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_STATUS_GET_LOG = Admin.Status.GetLog.FULL_URL ;


         [JsonProperty]
         public const string ADMIN_STATUS_GET_INVALID_REQUESTS = Admin.Status.GetInvalidRequests.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_WATCHER_SERVER_GET_AVAILABLE = Admin.WatcherServer.GetAvailable.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_WATCHER_SERVER_GET_EVENTS = Admin.WatcherServer.GetEvents.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_WATCHER_SERVER_GET_STATUS = Admin.WatcherServer.GetStatus.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_WATCHER_SERVER_REGISTER = Admin.WatcherServer.Register.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_WATCHER_SERVER_REMOVE = Admin.WatcherServer.Remove.FULL_URL ;

         [JsonProperty]
         public const string ADMIN_WATCHER_SERVER_UPDATE = Admin.WatcherServer.Update.FULL_URL ;

         [JsonProperty]
         public const string WATCHER_SERVER_REQUEST_SYNCHRONIZATION = WatcherServer.RequestSynchronization.FULL_URL ;
      }

      /// <summary>
      /// General response.
      /// </summary>
      public static class GeneralResponse {
         public static class Names {
            public const string SUCCESS = AllParameters.SUCCESS ;
            public const string ERROR = AllParameters.ERROR ;
            public const string ERROR_UUID = AllParameters.ERROR_UUID ;
         }

         public static class SuccessValues {
            public const string SUCCESS = AllSuccessValues.SUCCESS ;
            public const string ERROR = AllSuccessValues.ERROR ;
         }

         public static class ErrorValues {
            public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
            public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
            public const string TOKEN_EXPIRED = AllErrorValues.TOKEN_EXPIRED ;
         }
      }

      public static class GetVersion {
         public const string FULL_URL = "/getVersion" ;
      }

      #region Watcher Server
      public static class WatcherServer {
         public const string URL_BASE = "/watcherServer/" ;

         public static class Login {
            private const string URL_PART = "login" ;
            public const string FULL_URL = WatcherServer.URL_BASE + URL_PART ;

            public static class Request {
               public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;
               public const string PASSWORD = AllParameters.PASSWORD ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string ACCOUNT_EXPIRED = AllErrorValues.ACCOUNT_EXPIRED ;
               }
            }
         }

         public static class Register {
            private const string URL_PART = "register" ;
            public const string FULL_URL = WatcherServer.URL_BASE + URL_PART ;

            public static class Request {
               public const string UID = AllParameters.UID ;
               public const string PASSWORD = AllParameters.PASSWORD ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string ACCOUNT_EXPIRED = AllErrorValues.ACCOUNT_EXPIRED ;
               }
            }
         }

         public static class RequestSynchronization {
            private const string URL_PART = "requestSynchronization" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string SUCCESS = AllParameters.SUCCESS ;

               public static class ErrorValues {
                  public const string SUCCESS = AllParameters.SUCCESS ;

                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }
      }
      #endregion

      #region Account
      public static class Account {
         public const string URL_BASE = "/account/" ;

         public static class Login {
            private const string URL_PART = "login" ;
            public const string FULL_URL = Account.URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;
               public const string PASSWORD = AllParameters.PASSWORD ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string ACCOUNT_EXPIRED = AllErrorValues.ACCOUNT_EXPIRED ;
               }
            }
         }

         public static class Freeze {
            private const string URL_PART = "freeze" ;
            public const string FULL_URL = Account.URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;
               public const string PASSWORD = AllParameters.PASSWORD ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string ACCOUNT_EXPIRED = AllErrorValues.ACCOUNT_EXPIRED ;
               }
            }
         }

         public static class Logout {
            private const string URL_PART = "logout" ;
            public const string FULL_URL = Account.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class ValidateToken {
            private const string URL_PART = "validateToken" ;
            public const string FULL_URL = Account.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class GetStatus {
            private const string URL_PART = "getStatus" ;
            public const string FULL_URL = Account.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string ACCOUNT = AllParameters.ACCOUNT ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class Create {
            private const string URL_PART = "create" ;
            public const string FULL_URL = Account.URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;
               public const string PASSWORD = AllParameters.PASSWORD ;

               public const string FIRST_NAME_OPT = AllParameters.FIRST_NAME ;
               public const string LAST_NAME_OPT = AllParameters.LAST_NAME ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string EMAIL_ALREADY_REGISTERED = AllErrorValues.EMAIL_ALREADY_REGISTERED ;
               }
            }
         }

         public static class Update {
            private const string URL_PART = "update" ;
            public const string FULL_URL = Account.URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;
               public const string PASSWORD = AllParameters.PASSWORD ;
               public const string TOKEN = AllParameters.TOKEN ;
               public const string ID = AllParameters.ID ;

               public const string NEW_EMAIL_NEW_OPT = AllParameters.NEW_EMAIL ;
               public const string NEW_PASSWORD_NEW_OPT = AllParameters.NEW_PASSWORD ;

               public const string FIRST_NAME_OPT = AllParameters.FIRST_NAME ;
               public const string LAST_NAME_OPT = AllParameters.LAST_NAME ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string EMAIL_ALREADY_REGISTERED = AllErrorValues.EMAIL_ALREADY_REGISTERED ;
                  public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string ACCOUNT_EXPIRED = AllErrorValues.ACCOUNT_EXPIRED ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class Activate {
            private const string URL_PART = "activateAccount" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string ACTIVATION_CODE = AllParameters.ACTIVATION_CODE ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_ACTIVATION_CODE = AllErrorValues.INVALID_ACTIVATION_CODE ;
               }
            }
         }

         public static class ResendActivationCode {
            private const string URL_PART = "resendActivationCode" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;

               public static class Response {
                  public const string ERROR = AllParameters.ERROR ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  }
               }
            }
         }

         public static class SendNewPassword {
            private const string URL_PART = "sendNewPassword" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;
               public const string ADDRESS = AllParameters.ADDRESS ;

               public static class Response {
                  public const string ERROR = AllParameters.ERROR ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  }
               }
            }
         }

         public static class VerifySendNewPassword {
            private const string URL_PART = "verifySendNewPassword" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;
               public const string VERIFICATION_CODE = AllParameters.VERIFICATION_CODE ;

               public static class Response {
                  public const string ERROR = AllParameters.ERROR ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  }
               }
            }
         }

         public static class Delete {
            private const string URL_PART = "delete" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;
               public const string PASSWORD = AllParameters.PASSWORD ;

               public static class Response {
                  public const string ERROR = AllParameters.ERROR ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  }
               }
            }
         }
      }
      #endregion

      #region Alert
      public static class Alert {
         private const string URL_BASE = "/alert/" ;

         public static class SendDeviceAlert {
            private const string URL_PART = "sendDeviceAlert" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string INSTANCE_ID = AllParameters.INSTANCE_ID ;
               public const string ALERT_ID = AllParameters.ALERT_ID ;
               public const string ALERT_DEVICE_ID = AllParameters.ALERT_DEVICE_ID ;
               public const string ALERT_MESSAGE = AllParameters.ALERT_MESSAGE ;
               public const string ALERT_DETAILS = AllParameters.ALERT_DETAILS ;
               public const string IS_RECOVERY = AllParameters.IS_RECOVERY ;
               public const string FORCED_DEVICE_ALERT = AllParameters.FORCED_DEVICE_ALERT ;
               public const string PACKAGE_STATE = AllParameters.PACKAGE_STATE ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }


         public static class UnsubscribeAllEmail {
            private const string URL_PART = "unsubscribeAllEmail" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;
               public const string BLACK_LIST_TOKEN = AllParameters.BLACK_LIST_TOKEN ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }

         public static class UnSubscribeFromPackageEmail {
            private const string URL_PART = "unsubscribeFromPackageEmail" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string BLACK_LIST_TOKEN = AllParameters.BLACK_LIST_TOKEN ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }

         public static class ConfirmDeviceAlert {
            private const string URL_PART = "confirmDeviceAlert" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string ALERT_ID = AllParameters.ALERT_ID ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }

         public static class SendEmail {
            public const string URL_PART = "sendEmail" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string INSTANCE_ID = AllParameters.INSTANCE_ID ;
               public const string ALERT_ID = AllParameters.ALERT_ID ;
               public const string ALERT_EMAIL = AllParameters.ALERT_EMAIL ;
               public const string ALERT_MESSAGE = AllParameters.ALERT_MESSAGE ;
               public const string ALERT_DETAILS = AllParameters.ALERT_DETAILS ;
               public const string FROM_EMAIL = AllParameters.FROM_EMAIL ;
               public const string IS_RECOVERY = AllParameters.IS_RECOVERY ;
               public const string PACKAGE_STATE = AllParameters.PACKAGE_STATE ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }
      }
      #endregion

      #region License
      public static class License {
         public const string URL_BASE = "/license/" ;

         public static class GetAvailable {
            public const string URL_PART = "getAvailable" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string LICENSES = AllParameters.LICENSES ;
            }
         }

         public static class SaveLicenseOrder {
            public const string URL_PART = "saveLicenseOrder" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string LICENSE_ORDER = AllParameters.LICENSE_ORDER ;
            }

            public static class Response {
               public const string ID = AllParameters.ID ;
            }
         }

         public static class GetLicenseOrder {
            public const string URL_PART = "getLicenseOrder" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string ID = AllParameters.ID ;
            }

            public static class Response {
               public const string LICENSE_ORDER = AllParameters.LICENSE_ORDER ;
            }
         }

         public static class ActivateLicense {
            public const string URL_PART = "activateLicense" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string LICENSE_ID = AllParameters.LICENSE_ID ;
            }

            public static class Response { }
         }

         public static class AddToAccount {
            public const string URL_PART = "addToAccount" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string LICENSE_ID = AllParameters.LICENSE_ID ;
               public const string START_DATE = AllParameters.START_DATE ;
               public const string EXPIRY_DATE = AllParameters.EXPIRY_DATE ;
               public const string PAYMENT = AllParameters.PAYMENT ;
            }
         }
      }
      #endregion

      #region Package
      public static class Package {
         public const string URL_BASE = "/package/" ;

         public static class Save {
            private const string URL_PART = "save" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ; // needed for update an existing package
               public const string PACKAGE_NAME = AllParameters.PACKAGE_NAME ;
               public const string CONFIGURATION = AllParameters.CONFIGURATION ;
               public const string PACKAGE_TYPE = AllParameters.PACKAGE_TYPE ;
               public const string IS_ENABLED = AllParameters.IS_ENABLED ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class StoreMeasurement {
            private const string URL_PART = "storeMeasurement" ;
            public const string FULL_URL = Package.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string MEASUREMENT = AllParameters.MEASUREMENT ;
               public const string MEASUREMENT_TYPE = AllParameters.MEASUREMENT_TYPE ;
               public const string MEASUREMENT_TIMESTAMP = AllParameters.MEASUREMENT_TIMESTAMP ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class StoreState {
            private const string URL_PART = "storeState" ;
            public const string FULL_URL = Package.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
               public const string STATE = AllParameters.STATE ;

               public static class State {
                  public const string NAME = AllParameters.STATE_NAME ;
                  public const string MESSAGE = AllParameters.STATE_MESSAGE ;
                  public const string DETAILS = AllParameters.STATE_DETAILS ;

                  public static class StateNames {
                     public const string OK = AllParameters.PACKAGE_STATE_OK ;
                     public const string ALERTING = AllParameters.PACKAGE_STATE_ALERTING ;
                     public const string UNKNOWN = AllParameters.PACKAGE_STATE_UNKNOWN ;
                  }

                  public const string STATES = AllParameters.PACKAGE_PART_STATES ;

                  public static class States {
                     public const string IDENTIFIER = AllParameters.PACKAGE_PART_IDENTIFIER ;
                     public const string STATE_NAME = AllParameters.PACKAGE_PART_STATE_NAME ;
                     public const string STATE_MESSAGE = AllParameters.PACKAGE_PART_STATE_MESSAGE ;
                     public const string STATE_DETAILS = AllParameters.PACKAGE_PART_STATE_DETAILS ;
                     public const string STATE_EXTRACT = AllParameters.PACKAGE_PART_STATE_EXTRACT ;
                  }
               }
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class GetStatistics {
            private const string URL_PART = "getStatistics" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
            }

            public static class Response {
               public const string STATISTICS = AllParameters.STATISTICS ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class GetAvailable {
            private const string URL_PART = "getAvailable" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string PACKAGES = AllParameters.PACKAGES ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class RemoveAccess {
            private const string URL_PART = "removeAccess" ;
            public const string FULL_URL = Package.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class Delete {
            private const string URL_PART = "delete" ;
            public const string FULL_URL = Package.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string PACKAGE_ID = AllParameters.PACKAGE_ID ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class SendTestEmail {
            private const string URL_PART = "sendTestEmail" ;
            public const string FULL_URL = Package.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string EMAIL = AllParameters.EMAIL ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }
      }
      #endregion

      #region Device
      public static class Device {
         public const string URL_BASE = "/device/" ;

         public static class Android {
            private const string URL_BASE2 = URL_BASE + "android/" ;

            public static class Test {
               private const string URL_PART = "test" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string DEVICE_UUID = AllParameters.DEVICE_UUID ;
               }

               public static class Response {
                  public static class ErrorValues {
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  }
               }
            }

            public static class Register {
               private const string URL_PART = "register" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string DEVICE_UUID = AllParameters.DEVICE_UUID ;
                  public const string DEVICE_NAME = AllParameters.DEVICE_NAME ;
                  public const string GCM_ID = AllParameters.GCM_ID ;
                  public const string PROPERTIES = AllParameters.PROPERTIES ;
                  public const string EMAIL = AllParameters.EMAIL ;
                  public const string PASSWORD = AllParameters.PASSWORD ;

               }

               public static class Response {
                  public const string TOKEN = AllParameters.TOKEN ;

                  public static class ErrorValues {
                     public const string DEVICE_ALREADY_REGISTERED = AllErrorValues.DEVICE_ALREADY_REGISTERED ;
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                     public const string ACCOUNT_EXPIRED = AllErrorValues.ACCOUNT_EXPIRED ;
                  }
               }
            }

            public static class Login {
               private const string URL_PART = "login" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string DEVICE_UUID = AllParameters.DEVICE_UUID ;
                  public const string GCM_ID = AllParameters.GCM_ID ;
                  public const string EMAIL = AllParameters.EMAIL ;
                  public const string PASSWORD = AllParameters.PASSWORD ;
               }

               public static class Response {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string DEVICE_NAME = AllParameters.DEVICE_NAME ;

                  public static class ErrorValues {
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  }
               }
            }

         }

         public static class Login {
            private const string URL_PART = "login" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string PASSWORD = AllParameters.PASSWORD ;
               public const string DEVICE_UUID = AllParameters.DEVICE_UUID ;
               public const string GCM_ID_OPT = AllParameters.GCM_ID ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string DEVICE_NAME = AllParameters.DEVICE_NAME ;
               public const string CATEGORY = AllParameters.CATEGORY ;

               public static class ErrorValues {
                  public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string ACCOUNT_EXPIRED = AllErrorValues.ACCOUNT_EXPIRED ;
                  public const string INVALID_DEVICE_UUID = AllErrorValues.INVALID_DEVICE_UUID ;
               }
            }
         }

         public static class Register {
            private const string URL_PART = "register" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string UID = AllParameters.UID ;
               public const string EMAIL = AllParameters.EMAIL ;
               public const string PASSWORD = AllParameters.PASSWORD ;
               public const string CATEGORY = AllParameters.CATEGORY ;
               public const string GCM_ID_OPT = AllParameters.GCM_ID ;
               public const string DEVICE_TYPE = AllParameters.DEVICE_TYPE ;
               public const string DEVICE_NAME = AllParameters.DEVICE_NAME ;
               public const string OVER_WRITE = AllParameters.OVER_WRITE ;
               public const string PROPERTIES = AllParameters.PROPERTIES ;

               public static class DeviceTypeValues {
                  public const string WINDOWS_AGENT_DEVICE = AllDeviceTypeValues.WINDOWS_AGENT_DEVICE ;
                  public const string LINUX_AGENT_DEVICE = AllDeviceTypeValues.LINUX_AGENT_DEVICE ;
               }
            }

            public static class Response {
               public const string DEVICE_UUID = AllParameters.DEVICE_UUID ;
               public const string TOKEN = AllParameters.TOKEN ;

               public static class ErrorValues {
                  public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string ACCOUNT_EXPIRED = AllErrorValues.ACCOUNT_EXPIRED ;
                  public const string DEVICE_NAME_TAKEN = AllErrorValues.DEVICE_NAME_TAKEN ;
                  public const string MAXIMUM_DEVICE_COUNT_REACHED = AllErrorValues.MAXIMUM_DEVICE_COUNT_REACHED ;
               }
            }
         }

         public static class Delete {
            private const string URL_PART = "delete" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string DEVICE_UUID = AllParameters.DEVICE_UUID ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }

         public static class Disconnect {
            private const string URL_PART = "disconnect" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string DEVICE_UUID = AllParameters.DEVICE_UUID ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }

         public static class GetAvailable {
            private const string URL_PART = "getAvailable" ;
            public const string FULL_URL = Device.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public const string DEVICES = AllParameters.DEVICES ;

               public static class ErrorValues {
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }

         public static class Edit {
            private const string URL_PART = "edit" ;
            public const string FULL_URL = Device.URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string DEVICE_UUID = AllParameters.DEVICE_UUID ;
               public const string DEVICE_NAME = AllParameters.DEVICE_NAME ;
               public const string CATEGORIES = AllParameters.CATEGORIES ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
               }
            }
         }
      }
      #endregion

      #region Admin
      public static class Admin {
         public const string URL_BASE = "/admin/" ;

         public static class GetSupervisorStatus {
            private const string URL_PART = "getSupervisorStatus" ;
            public const string FULL_URL = URL_BASE + URL_PART ;
         }

         public static class SetWebSiteAddress {
            private const string URL_PART = "setWebsiteAddress" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string WEBSITE_ADDRESS = AllParameters.WEBSITE_ADDRESS ;
            }
         }

         public static class RunJob {
            private const string URL_PART = "runJob" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string JOB_NAME = AllParameters.JOB_NAME ;
               public const string START_DATE = AllParameters.START_DATE ;
            }
         }

         public static class Login {
            private const string URL_PART = "login" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string USER_NAME = AllParameters.USER_NAME ;
               public const string PASSWORD = AllParameters.PASSWORD ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
               }
            }
         }

         public static class SendMaintenanceEmail {
            private const string URL_PART = "sendMaintenanceEMail" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
               public const string MESSAGE = AllParameters.MESSAGE ;

            }

            public static class Response {

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class Update {
            private const string URL_PART = "update" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string USER_NAME = AllParameters.USER_NAME ;
               public const string PASSWORD = AllParameters.PASSWORD ;
               public const string WEB_PAGE_URL = AllParameters.WEB_PAGE_URL ;
               public const string TOKEN = AllParameters.TOKEN ;
               public const string EMAIL_USER_NAME = AllParameters.EMAIL_USER_NAME ;
               public const string EMAIL_PROVIDER = AllParameters.EMAIL_PROVIDER ;
               public const string EMAIL_PASSWORD = AllParameters.EMAIL_PASSWORD ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         public static class Logout {
            private const string URL_PART = "logout" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string TOKEN = AllParameters.TOKEN ;
            }

            public static class Response {
               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
               }
            }
         }

         #region Admin.Watcher Server
         public static class WatcherServer {
            public const string URL_BASE2 = URL_BASE + "watcherServer/" ;

            public static class GetAvailable {
               private const string URL_PART = "getAvailable" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
               }

               public static class Response {
                  public const string SUCCESS = AllParameters.SUCCESS ;
                  public const string SERVERS = AllParameters.SERVERS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class GetEvents {
               private const string URL_PART = "getEvents" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;
               }

               public static class Response {
                  public const string SUCCESS = AllParameters.SUCCESS ;
                  public const string EVENTS = AllParameters.EVENTS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class GetStatus {
               private const string URL_PART = "getStatus" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;
               }

               public static class Response {
                  public const string STATUS = AllParameters.STATUS ;
                  public const string SUCCESS = AllParameters.SUCCESS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class Register {
               private const string URL_PART = "register" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string NAME = AllParameters.NAME ;
                  public const string ADDRESS = AllParameters.ADDRESS ;
                  public const string PORT = AllParameters.PORT ;
                  public const string USER_NAME = AllParameters.USER_NAME ;
                  public const string PASSWORD = AllParameters.PASSWORD ;
                  public const string NEW_USER_NAME = AllParameters.NEW_USER_NAME ;
                  public const string NEW_PASSWORD = AllParameters.NEW_PASSWORD ;
                  public const string CATEGORIES = AllParameters.CATEGORIES ;
               }

               public static class Response {
                  public const string SUCCESS = AllParameters.SUCCESS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class Update {
               private const string URL_PART = "update" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string NAME = AllParameters.NAME ;
                  public const string ADDRESS = AllParameters.ADDRESS ;
                  public const string PORT = AllParameters.PORT ;
                  public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;
               }

               public static class Response {
                  public const string SUCCESS = AllParameters.SUCCESS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class Remove {
               private const string URL_PART = "remove" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string WATCHER_SERVER_ID = AllParameters.WATCHER_SERVER_ID ;
               }

               public static class Response {
                  public const string SUCCESS = AllParameters.SUCCESS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }
         }
         #endregion

         #region Admin.Database
         public static class Database {
            public const string URL_BASE2 = Admin.URL_BASE + "database/" ;

            public static class GetStatus {
               private const string URL_PART = "getStatus" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
               }

               public static class Response {
                  public const string SUCCESS = AllParameters.SUCCESS ;
                  public const string STATUS = AllParameters.STATUS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class Remove {
               private const string URL_PART = "remove" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
               }

               public static class Response {
                  public const string SUCCESS = AllParameters.SUCCESS ;
                  public const string STATUS = AllParameters.STATUS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class Update {
               private const string URL_PART = "update" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string DATABASE_URL = AllParameters.DATABASE_URL ;
                  public const string USER_NAME = AllParameters.USER_NAME ;
                  public const string PASSWORD = AllParameters.PASSWORD ;
               }

               public static class Response {
                  public const string SUCCESS = AllParameters.SUCCESS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }
         }
         #endregion

         #region Admin.Status
         public static class Status {
            public const string URL_BASE2 = Admin.URL_BASE + "status/" ;

            public static class GetEvents {
               private const string URL_PART = "getEvents" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
               }

               public static class Response {
                  public const string EVENTS = AllParameters.EVENTS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class GetStatus {
               private const string URL_PART = "getStatus" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
               }

               public static class Response {
                  public const string STATUS = AllParameters.STATUS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class GetInvalidRequests {
               private const string URL_PART = "getInvalidRequests" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
               }

               public static class Response {
                  public const string INVALID_REQUESTS = AllParameters.INVALID_REQUESTS ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class GetLog {
               private const string URL_PART = "getLog" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
               }

               public static class Response {
                  public const string LOG = AllParameters.LOG ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }
         }
         #endregion
      }
      #endregion

      #region API
      public static class API {
         public const string URL_BASE = "/api/" ;

         public static class GetToken {
            private const string URL_PART = "getToken" ;
            public const string FULL_URL = URL_BASE + URL_PART ;

            public static class Request {
               public const string EMAIL = AllParameters.EMAIL ;
               public const string PASSWORD = AllParameters.PASSWORD ;
            }

            public static class Response {
               public const string TOKEN = AllParameters.TOKEN ;

               public static class ErrorValues {
                  public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                  public const string INVALID_TOKEN = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD ;
               }
            }
         }

         #region API.Package
         public static class Package {
            public const string URL_BASE2 = API.URL_BASE + "package/" ;

            public static class GetAll {
               private const string URL_PART = "getAll" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
               }

               public static class Response {
                  public const string PACKAGES = AllParameters.PACKAGES ;

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class Save {
               private const string URL_PART = "save" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string PACKAGE = AllParameters.PACKAGE ;
               }

               public static class Response {

                  public static class ErrorValues {
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class Delete {
               private const string URL_PART = "delete" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string PACKAGE_NAME = AllParameters.PACKAGE_NAME ;
               }

               public static class Response {

                  public static class ErrorValues {
                     public const string INVALID_PACKAGE_NAME = AllErrorValues.INVALID_PACKAGE_NAME ;
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class GetVersion {
               private const string URL_PART = "getVersion" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string PACKAGE_NAME = AllParameters.PACKAGE_NAME ;
               }

               public static class Response {
                  public const string VERSION = AllParameters.VERSION ;

                  public static class ErrorValues {
                     public const string INVALID_PACKAGE_NAME = AllErrorValues.INVALID_PACKAGE_NAME ;
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }

            public static class GetPackage {
               private const string URL_PART = "getPackage" ;
               public const string FULL_URL = URL_BASE2 + URL_PART ;

               public static class Request {
                  public const string TOKEN = AllParameters.TOKEN ;
                  public const string PACKAGE_NAME = AllParameters.PACKAGE_NAME ;
               }

               public static class Response {
                  public const string PACKAGE = AllParameters.PACKAGE ;

                  public static class ErrorValues {
                     public const string INVALID_PACKAGE_NAME = AllErrorValues.INVALID_PACKAGE_NAME ;
                     public const string INTERNAL_SYSTEM_ERROR = AllErrorValues.INTERNAL_SYSTEM_ERROR ;
                     public const string INVALID_TOKEN = AllErrorValues.INVALID_TOKEN ;
                  }
               }
            }
         }

         #endregion

      }
      #endregion
   }
}