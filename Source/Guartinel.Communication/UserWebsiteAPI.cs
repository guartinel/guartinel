using Newtonsoft.Json;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.Communication
{
    public static class UserWebsiteAPI
    {
        public const string URL = "api";

        public class AllURLSerializationSummary
        {
            [JsonProperty]
            public const string GET_VERSION = GetVersion.URL;

            [JsonProperty]
            public const string UNSUBSCRIBE_ALL_EMAIL = Account.UnsubscribeAllEmail.URL;

            [JsonProperty]
            public const string UNSUBSCRIBE_FROM_PACKAGE_EMAIL = Account.UnsubscribeFromPackageEmail.URL;

            [JsonProperty]
            public const string ACCOUNT_LOGIN = Account.Login.URL;

            [JsonProperty]
            public const string ACCOUNT_CREATE = Account.Create.URL;

            [JsonProperty]
            public const string ACCOUNT_DELETE = Account.Delete.URL;

            [JsonProperty]
            public const string ACCOUNT_LOGOUT = Account.Logout.URL;

            [JsonProperty]
            public const string ACCOUNT_VALIDATE_TOKEN = Account.ValidateToken.URL;


            [JsonProperty]
            public const string ACCOUNT_GET_ACCOUNT_INFO = Account.GetAccountInfo.URL;

            [JsonProperty]
            public const string ACCOUNT_UPDATE = Account.Update.URL;

            [JsonProperty]
            public const string ACCOUNT_FREEZE = Account.Freeze.URL;

            [JsonProperty]
            public const string ACCOUNT_RESEND_ACTIVATION_CODE = Account.ResendActivationCode.URL;

            [JsonProperty]
            public const string ACCOUNT_SEND_NEW_PASSWORD = Account.SendNewPassword.URL;

            [JsonProperty]
            public const string ACCOUNT_VERIFY_SEND_NEW_PASSWORD = Account.VerifySendNewPassword.URL;

            [JsonProperty]
            public const string ACCOUNT_ACTIVATE = Account.Activate.URL;

            [JsonProperty]
            public const string DEVICE_GET_EXISTING = Device.GetExisting.URL;

            [JsonProperty]
            public const string DEVICE_DELETE = Device.Delete.URL;

            [JsonProperty]
            public const string DEVICE_TEST = Device.Test.URL;
            [JsonProperty]
            public const string DEVICE_DISCONNECT = Device.Disconnect.URL;

            [JsonProperty]
            public const string DEVICE_EDIT = Device.Edit.URL;

            [JsonProperty]
            public const string LICENSE_START_BUYING_LICENSE = License.StartBuyingLicense.URL;

            [JsonProperty]
            public const string LICENSE_FINALIZE_BUYING_LICENSE = License.FinalizeBuyingLicense.URL;

            [JsonProperty]
            public const string LICENSE_GET_AVAILABLE = License.GetAvailable.URL;

            [JsonProperty]
            public const string LICENSE_ACTIVATE_LICENSE = License.ActivateLicense.URL;

            [JsonProperty]
            public const string PACKAGE_SAVE = Package.Save.URL;
            [JsonProperty]
            public const string PACKAGE_REMOVE_ACCESS = Package.RemoveAccess.URL;

            [JsonProperty]
            public const string PACKAGE_GET_STATISTICS = Package.GetStatistics.URL;

            [JsonProperty]
            public const string PACKAGE_DELETE = Package.Delete.URL;

            [JsonProperty]
            public const string PACKAGE_GET_AVAILABLE = Package.GetAvailable.URL;

            [JsonProperty]
            public const string PACKAGE_TEST_EMAIL = Package.TestEmail.URL;
        }

        public static class Index
        {
            public const string URL_PART = "Index";
            public const string URL = UserWebsiteAPI.URL + "/" + URL_PART;
        }

        public static class GetVersion
        {
            public const string URL_PART = "GetVersion";
            public const string URL = UserWebsiteAPI.URL + "/" + URL_PART;
        }


        public static class License
        {
            public const string URL_PART = "License";
            public const string URL = UserWebsiteAPI.URL + "/" + URL_PART;

            public static class GetAvailable
            {
                public const string URL_PART = "GetAvailable";
                public const string URL = License.URL + "/" + URL_PART;
            }

            public static class ActivateLicense
            {
                public const string URL_PART = "ActivateLicense";
                public const string URL = License.URL + "/" + URL_PART;
            }

            public static class StartBuyingLicense
            {
                public const string URL_PART = "StartBuyingLicense";
                public const string URL = License.URL + "/" + URL_PART;

                public static class Request
                {
                    public const string LICENSE = AllParameters.LICENSE;
                    public const string ACCOUNT = AllParameters.ACCOUNT;
                }
            }

            public static class FinalizeBuyingLicense
            {
                public const string URL_PART = "FinalizeBuyingLicense";
                public const string URL = License.URL + "/" + URL_PART;

                public static class Request
                {
                    public const string PAYER_ID = AllParameters.PAYER_ID;
                    public const string PAYPAL_TOKEN = AllParameters.PAYPAL_TOKEN;
                }
            }
        }

        #region ACCOUNT
        public static class Account
        {
            public const string URL_PART = "Account";
            public const string URL = UserWebsiteAPI.URL + "/" + URL_PART;

            public static class UnsubscribeAllEmail
            {
                public const string URL_PART = "UnsubscribeAllEmail";
                public const string URL = Account.URL + "/" + URL_PART;
                public static class Request
                {
                    public const string BLACK_LIST_TOKEN = AllParameters.BLACK_LIST_TOKEN;
                }
            }
            public static class UnsubscribeFromPackageEmail
            {
                public const string URL_PART = "UnsubscribeFromPackageEmail";
                public const string URL = Account.URL + "/" + URL_PART;
                public static class Request
                {
                    public const string BLACK_LIST_TOKEN = AllParameters.BLACK_LIST_TOKEN;
                    public const string PACKAGE_ID = AllParameters.PACKAGE_ID;

                }
            }

            public static class SendNewPassword
            {
                public const string URL_PART = "SendNewPassword";
                public const string URL = Account.URL + "/" + URL_PART;
            }

            public static class VerifySendNewPassword
            {
                public const string URL_PART = "VerifySendNewPassword";
                public const string URL = Account.URL + "/" + URL_PART;
            }

            public static class Login
            {
                public const string URL_PART = "Login";
                public const string URL = Account.URL + "/" + URL_PART;
            }

            public static class Create
            {
                public const string URL_PART = "Create";
                public const string URL = Account.URL + "/" + URL_PART;
            }

            public static class Delete
            {
                public const string URL_PART = "Delete";
                public const string URL = Account.URL + "/" + URL_PART;
            }

            public static class Logout
            {
                public const string URL_PART = "Logout";
                public const string URL = Account.URL + "/" + URL_PART;
            }

            public static class ValidateToken
            {
                public const string URL_PART = "ValidateToken";
                public const string URL = Account.URL + "/" + URL_PART;
            }
            public static class GetAccountInfo
            {
                public const string URL_PART = "GetAccountInfo";
                public const string URL = Account.URL + "/" + URL_PART;
            }

            public static class Update
            {
                public const string URL_PART = "Update";
                public const string URL = Account.URL + "/" + URL_PART;

                public static class Request
                {
                    public const string TOKEN = AllParameters.TOKEN;
                    public const string ID = AllParameters.ID;
                    public const string EMAIL = AllParameters.EMAIL;
                    public const string PASSWORD = AllParameters.PASSWORD;
                    public const string NEW_PASSWORD = AllParameters.NEW_PASSWORD;
                    public const string FIRST_NAME = AllParameters.FIRST_NAME;
                    public const string LAST_NAME = AllParameters.LAST_NAME;
                }
            }

            public static class Freeze
            {
                public const string URL_PART = "Freeze";
                public const string URL = Account.URL + "/" + URL_PART;

                public static class Request
                {
                    public const string EMAIL = AllParameters.EMAIL;
                    public const string PASSWORD = AllParameters.PASSWORD;
                }
            }

            public static class Activate
            {
                public const string URL_PART = "Activate";
                public const string URL = Account.URL + "/" + URL_PART;

                public static class Request
                {
                    public const string ACTIVATION_CODE = AllParameters.ACTIVATION_CODE;
                    public const string EMAIL = AllParameters.EMAIL;
                }
            }

            public static class ResendActivationCode
            {
                public const string URL_PART = "ResendActivationCode";
                public const string URL = Account.URL + "/" + URL_PART;
            }
        }
        #endregion ACCOUNT

        #region DEVICE
        public static class Device
        {
            public const string URL_PART = "Device";
            public const string URL = UserWebsiteAPI.URL + "/" + URL_PART;

            public static class GetExisting
            {
                public const string URL_PART = "GetExisting";
                public const string URL = Device.URL + "/" + URL_PART;
            }

            public static class Test
            {
                public const string URL_PART = "Test";
                public const string URL = Device.URL + "/" + URL_PART;
            }
            public static class Disconnect
            {
                public const string URL_PART = "Disconnect";
                public const string URL = Device.URL + "/" + URL_PART;
            }

            public static class Delete
            {
                public const string URL_PART = "Delete";
                public const string URL = Device.URL + "/" + URL_PART;
            }

            public static class Edit
            {
                public const string URL_PART = "Edit";
                public const string URL = Device.URL + "/" + URL_PART;
            }
        }
        #endregion DEVICE

        #region FILE
        public static class File
        {
            public const string URL_PART = "File";
            public const string URL = UserWebsiteAPI.URL + "/" + URL_PART;

            public static class WindowsAgent
            {
                public const string URL_PART = "WindowsAgent";
                public const string URL = File.URL + "/" + URL_PART;
            }

            public static class LinuxAgent
            {
                public const string URL_PART = "LinuxAgent";
                public const string URL = File.URL + "/" + URL_PART;
            }
        }
        #endregion FILE

        #region PACKAGE
        public static class Package
        {
            public const string URL_PART = "Package";
            public const string URL = UserWebsiteAPI.URL + "/" + URL_PART;

            public static class Save
            {
                public const string URL_PART = "Save";
                public const string URL = Package.URL + "/" + URL_PART;
            }

            public static class GetStatistics
            {
                public const string URL_PART = "GetStatistics";
                public const string URL = Package.URL + "/" + URL_PART;
            }
            public static class RemoveAccess
            {
                public const string URL_PART = "RemoveAccess";
                public const string URL = Package.URL + "/" + URL_PART;
            }

            public static class GetAvailable
            {
                public const string URL_PART = "GetExisting";
                public const string URL = Package.URL + "/" + URL_PART;
            }

            public static class TestEmail
            {
                public const string URL_PART = "TestEmail";
                public const string URL = Package.URL + "/" + URL_PART;
            }

            public static class Delete
            {
                public const string URL_PART = "Delete";
                public const string URL = Package.URL + "/" + URL_PART;
            }
        }
        #endregion PACKAGE

        public static class Administrator
        {
            public const string URL_PART = "Administrator";
            public const string URL = UserWebsiteAPI.URL + "/" + URL_PART;

            public static class Register
            {
                public const string URL_PART = "Register";
                public const string URL = Administrator.URL + "/" + URL_PART;

                public static class Request
                {
                    public const string PASSWORD = AllParameters.PASSWORD;
                    public const string USER_NAME = AllParameters.USER_NAME;
                    public const string NEW_USER_NAME = AllParameters.NEW_USER_NAME;
                    public const string NEW_PASSWORD = AllParameters.NEW_PASSWORD;
                    public const string ACTIVATION_CODE = AllParameters.ACTIVATION_CODE;
                    public const string MANAGEMENT_SERVER_ADDRESS = AllParameters.MANAGEMENT_SERVER_ADDRESS;
                    public const string MANAGEMENT_SERVER_PORT = AllParameters.MANAGEMENT_SERVER_PORT;
                }

                public static class Response
                {
                    public const string INVALID_USER_NAME_OR_PASSWORD = AllErrorValues.INVALID_USER_NAME_OR_PASSWORD;
                }
            }

            public static class GetStatus
            {
                public const string URL_PART = "GetStatus";
                public const string URL = Administrator.URL + "/" + URL_PART;

                public static class Request
                {
                    public const string PASSWORD = AllParameters.PASSWORD;
                    public const string USER_NAME = AllParameters.USER_NAME;
                }
            }
        }
    }
}
