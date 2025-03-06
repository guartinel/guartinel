using System ;
using System.ComponentModel.DataAnnotations ;
using System.Linq ;
using Guartinel.Communication ;
using Guartinel.Website.User.Misc ;
using Newtonsoft.Json ;

namespace Guartinel.Website.User.Models.Account {
   public class AccountUpdateModel : AuthenticationModel {
      [JsonProperty (PropertyName = UserWebsiteAPI.Account.Update.Request.ID)]
      [Required (ErrorMessage = "Account ID is required!")]
      public string Id {get ; set ;}

      // These fields can be updated:

      [JsonProperty (PropertyName = UserWebsiteAPI.Account.Update.Request.EMAIL)]
      [EmailAddress (ErrorMessage = ErrorMessages.EMAIL_INVALID)]
      public string Email {get ; set ;}

      [JsonProperty (PropertyName = UserWebsiteAPI.Account.Update.Request.PASSWORD)]
      [MinLength (ValidatorConstants.MINIMUM_USER_PASSWORD_LENGTH, ErrorMessage = ErrorMessages.USER_PASSWORD_TOO_SHORT)]
      public string Password {get ; set ;}

      [JsonProperty (PropertyName = UserWebsiteAPI.Account.Update.Request.NEW_PASSWORD)]
      [MinLength (ValidatorConstants.MINIMUM_USER_PASSWORD_LENGTH, ErrorMessage = ErrorMessages.USER_PASSWORD_TOO_SHORT)]
      public string NewPassword {get ; set ;}

      [JsonProperty (PropertyName = UserWebsiteAPI.Account.Update.Request.FIRST_NAME)]
      public string FirstName {get ; set ;}

      [JsonProperty (PropertyName = UserWebsiteAPI.Account.Update.Request.LAST_NAME)]
      public string LastName {get ; set ;}
   }
}
