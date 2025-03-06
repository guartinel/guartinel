using System ;
using System.Linq ;

namespace Guartinel.Website.User.Misc {
   public static class ErrorMessages {
      public const string MODEL_ERROR = "INVALID_MODEL_STATE" ;
      public const string ERROR_FIELD = "error" ;

      public const string MANAGEMENT_ID_REQUIRED = "Management server ID is required!" ;

      public const string NAME_REQUIRED = "Name is required!" ;

      public const string USERNAME_REQUIRED = "Username is required!" ;
      public const string USERNAME_TOO_SHORT = "Username is too short!" ;

      public const string ADMIN_NOT_FOUND = "No admin account is registered with this username!" ;
      public const string INVALID_ADMIN_USER_NAME_OR_PASSWORD = "Invalid administrator user name or password!" ;

      public const string TOKEN_REQUIRED = "Token is required!" ;

      public const string EMAIL_REQUIRED = "Email addres is required!" ;
      public const string EMAIL_INVALID = "Invalid email address!" ;

      public const string PASSWORD_REQUIRED = "Password is required!" ;

      // todo: fix these !!!!!!
      public const string ADMIN_PASSWORD_TOO_SHORT = "Password is too short! Minimum: 8" ;

      public const string USER_PASSWORD_TOO_SHORT = "Password is too short! Minimum: 6" ;
      //Constants.MINIMUM_USER_PASSWORD_LENGTH + " characters" ;

      public const string DEVICE_ID_REQUIRED = "Device id is required!" ;

      public const string PACKAGE_NAME_REQUIRED = "Package name is required!" ;
      public const string PACKAGE_ID_REQUIRED = "Package id is required!" ;
  
      public const string ALERT_DEVICE_ID_REQUIRED = "Alert device id is required!" ;
      public const string ALERT_EMAIL_REQUIRED = "Alert email is required!" ;

      public const string URL_REQUIRED = "Url required!" ;
      public const string WATCHER_SERVER_ID_REQUIRED = "Watcher server id required!" ;
      public const string ADDRESS_REQUIRED = "Address required!" ;
      public const string PORT_REQUIRED = "Port required!" ;
      public const string WEBPAGE_URL_REQUIRED = "Webpage url required!" ;
      public const string MANAGEMENT_SERVER_NOT_FOUND = "Management server not found!" ;
      public const string MANAGEMENT_SERVER_CONNECTION_ERROR = "Can't send request to management server!" ;

      public const string INVALID_REQUEST = "Invalid request!" ;

      public const string MANAGEMENT_SERVER_ALREADY_ADDED = "Management server already added" ;
      public const string MANAGEMENT_INVALID_REQUEST = "Invalid data provided" ;

      public const string WEBSITE_SYSTEM_ERROR = "Website system error" ;

      public const string ACCOUNT_ACTIVATION_CODE_REQUIRED = "Account activation code required!" ;
   }
}
