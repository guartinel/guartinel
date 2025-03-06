using System ;
using System.ComponentModel.DataAnnotations ;
using System.Linq ;
using Guartinel.Website.User.Misc ;
using Guartinel.Website.User.Models.Account ;
using Newtonsoft.Json ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.User.Models.Package {
   public class PackageDeleteModel : AuthenticationModel {
      [JsonProperty (PropertyName = AllParameters.PACKAGE_ID)]
      [Required (ErrorMessage = ErrorMessages.PACKAGE_ID_REQUIRED)]
      public string PackageID {get ; set ;}
   }
}
