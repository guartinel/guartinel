using System ;
using System.ComponentModel.DataAnnotations ;
using System.Linq ;
using Guartinel.Website.User.Misc ;
using Newtonsoft.Json;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.User.Models.Account {
   public class AccountResendActivationCodeModel {
      [JsonProperty (PropertyName = AllParameters.EMAIL)]
      [Required (ErrorMessage = ErrorMessages.EMAIL_REQUIRED)]
      [EmailAddress (ErrorMessage = ErrorMessages.EMAIL_INVALID)]
      public string Email {get ; set ;}
   }
}