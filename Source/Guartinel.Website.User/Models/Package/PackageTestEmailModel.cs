using System ;
using System.ComponentModel.DataAnnotations ;
using Guartinel.Website.User.Misc ;
using Guartinel.Website.User.Models.Account ;
using Newtonsoft.Json ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.User.Models.Package {
   public class PackageTestEmailModel : AuthenticationModel {
      [JsonProperty (PropertyName = AllParameters.EMAIL)]
      [Required (ErrorMessage = ErrorMessages.EMAIL_REQUIRED)]
      [EmailAddress (ErrorMessage = ErrorMessages.EMAIL_INVALID)]
      public string Email {get ; set ;}
   }
}
