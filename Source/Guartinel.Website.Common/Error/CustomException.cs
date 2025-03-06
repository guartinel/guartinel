using System ;
using System.Linq ;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.Website.Common.Error {
   public class CustomException : Exception {
      public string ErrorMessage {get ; set ;}

      public class InvalidUserNameOrPasswordException : CustomException {
         public InvalidUserNameOrPasswordException() {
            ErrorMessage = ErrorMessages.INVALID_USER_NAME_OR_PASSWORD ;
         }
      }

      public class InvalidTokenException : CustomException {
         public InvalidTokenException() {
            ErrorMessage = AllErrorValues.INVALID_TOKEN ;
         }
      }

      public class MissingManagementServerException : CustomException {
         public MissingManagementServerException() {
            ErrorMessage = ErrorMessages.MISSING_MANAGEMENT_SERVER ;
         }
      }

      public class MissingUserWebSite : CustomException {
         public MissingUserWebSite() {
            ErrorMessage = ErrorMessages.MISSING_USER_WEB_SITE ;
         }
      }

      public class ManagementServerInvalidTokenException : CustomException {
         public ManagementServerInvalidTokenException() {
            // this exception is handled differently than the others so dont need ErrorMessage
         }
      }

      public class ManagementServerInvalidRequestException : CustomException {
         public ManagementServerInvalidRequestException() {
            ErrorMessage = ErrorMessages.MANAGEMENT_INVALID_REQUEST ;
         }
      }
   }
}
