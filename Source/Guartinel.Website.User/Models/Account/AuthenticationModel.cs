using System ;
using System.ComponentModel.DataAnnotations ;
using System.Linq ;
using Guartinel.Communication ;
using Guartinel.Website.User.Misc ;
using Newtonsoft.Json ;

namespace Guartinel.Website.User.Models.Account {
   public class AuthenticationModel {
#warning should be different model to use this property
      [JsonProperty (PropertyName = ManagementServerAPI.Account.GetStatus.Request.TOKEN)]
      [Required (ErrorMessage = ErrorMessages.TOKEN_REQUIRED)]
      public string Token {get ; set ;}
   }
}
