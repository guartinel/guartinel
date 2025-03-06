using System ;
using System.ComponentModel.DataAnnotations ;
using System.Linq ;
using Guartinel.Communication ;
using Guartinel.Website.User.Misc ;
using Newtonsoft.Json ;

namespace Guartinel.Website.User.Models.Account {
   public class AccountLoginModel {
      [JsonProperty (PropertyName = ManagementServerAPI.Account.Login.Request.EMAIL)]
      [Required (ErrorMessage = ErrorMessages.EMAIL_REQUIRED)]
      [EmailAddress (ErrorMessage = ErrorMessages.EMAIL_INVALID)]
      public string Email {get ; set ;}

      [JsonProperty (PropertyName = ManagementServerAPI.Account.Login.Request.PASSWORD)]
      [Required (ErrorMessage = ErrorMessages.PASSWORD_REQUIRED)]
      [MinLength (ValidatorConstants.MINIMUM_USER_PASSWORD_LENGTH, ErrorMessage = ErrorMessages.USER_PASSWORD_TOO_SHORT)]
      public string Password {get ; set ;}
   }
}
