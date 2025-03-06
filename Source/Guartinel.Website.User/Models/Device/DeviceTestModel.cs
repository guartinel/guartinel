using System ;
using System.ComponentModel.DataAnnotations ;
using System.Linq ;
using Guartinel.Website.User.Misc ;
using Guartinel.Website.User.Models.Account ;
using Newtonsoft.Json ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.User.Models.Device {
   public class DeviceTestModel : AuthenticationModel {
      [JsonProperty (PropertyName = AllParameters.DEVICE_UUID)]
      [Required (ErrorMessage = ErrorMessages.DEVICE_ID_REQUIRED)]
      public string DeviceID {get ; set ;}
   }
}
