using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.Website.Common.Error {
   public static class ErrorMessages {
      public const string MISSING_USER_WEB_SITE = AllErrorValues.MISSING_USER_WEB_SITE ;
      public const string MISSING_MANAGEMENT_SERVER = AllErrorValues.MISSING_MANAGEMENT_SERVER ;
      public const string CANNOT_CONNECT_TO_REMOTE_HOST = AllErrorValues.CANNOT_CONNECT_TO_REMOTE_HOST ;
      public const string INVALID_USER_NAME_OR_PASSWORD = "User Name or Password is incorrect." ;

      /// <summary>
      /// DEPRECATED
      /// </summary>
      public const string MANAGEMENT_ID_REQUIRED = "Management server ID is required!" ;
      public const string NAME_REQUIRED = "Name is required!" ;
      public const string USERNAME_REQUIRED = "Username is required!" ;
      public const string TOKEN_REQUIRED = "Token is required!" ;
      public const string PASSWORD_REQUIRED = "Password is required!" ;
      public const string ADMIN_PASSWORD_TOO_SHORT = "Password is too short! Minimum: 8" ;
      public const string URL_REQUIRED = "Url required!" ;
      public const string WATCHER_SERVER_ID_REQUIRED = "Watcher server id required!" ;
      public const string ADDRESS_REQUIRED = "Address required!" ;
      public const string PORT_REQUIRED = "Port required!" ;
      public const string INVALID_REQUEST = "Invalid request!" ;
      public const string MANAGEMENT_SERVER_ALREADY_ADDED = "Management server already added" ;
      public const string MANAGEMENT_INVALID_REQUEST = "Invalid data provided" ;
   }
}
