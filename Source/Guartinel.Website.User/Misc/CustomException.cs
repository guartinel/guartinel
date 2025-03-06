using System ;

namespace Guartinel.Website.User.Misc {
   public class CustomException : Exception {
      public string ErrorMessage {get ; set ;}
      /*
        public class ManagementServerNotFoundException : CustomException {
            public ManagementServerNotFoundException() {
                ErrorMessage = ErrorMessages.MANAGEMENT_SERVER_NOT_FOUND;
                }
            }



        /*    public class InvalidTokenException : CustomException {
                public InvalidTokenException() {
                    ErrorMessage = Communication.Common.AllErrorValues.INVALID_TOKEN;
                    }
                }*/

      /* public class InvalidRequestException : CustomException {
             public InvalidRequestException() {
                 ErrorMessage = ErrorMessages.INVALID_REQUEST;
                 }
             }*/

      /*  public class InvalidAdminUserNameOrPasswordException : CustomException {
              public InvalidAdminUserNameOrPasswordException() {
                  ErrorMessage = ErrorMessages.INVALID_ADMIN_USER_NAME_OR_PASSWORD;
                  }
              }*/

      /* public class ManagementServerAlreadyAddedException : CustomException {
             public ManagementServerAlreadyAddedException() {
                 ErrorMessage = ErrorMessages.MANAGEMENT_SERVER_ALREADY_ADDED;
                 }
             } 

         public class ManagementServerInvalidTokenException : CustomException {
             public ManagementServerInvalidTokenException() {
                 // this exception is handled differently than the others
                 }
             }

      /*   public class ManagementServerInvalidRequestException : CustomException {
             public ManagementServerInvalidRequestException() {
                 ErrorMessage = ErrorMessages.MANAGEMENT_INVALID_REQUEST;
                 }
             }*/
   }
}
