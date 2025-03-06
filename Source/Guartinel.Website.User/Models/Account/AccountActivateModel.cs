using System ;
using System.Linq ;
using Newtonsoft.Json ;

namespace Guartinel.Website.User.Models.Account {
   public class AccountActivateModel {
      [JsonProperty (PropertyName = Guartinel.Communication.UserWebsiteAPI.Account.Activate.Request.ACTIVATION_CODE)]
      public string ActivationCode {get ; set ;}

      [JsonProperty (PropertyName = Guartinel.Communication.UserWebsiteAPI.Account.Activate.Request.EMAIL)]
      public string Email {get ; set ;}
   }
}
