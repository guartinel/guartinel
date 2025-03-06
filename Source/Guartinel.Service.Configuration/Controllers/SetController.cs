using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Threading.Tasks ;
using Guartinel.Kernel.Logging ;
using Microsoft.AspNetCore.Http ;
using Microsoft.AspNetCore.Mvc ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.Configuration.Controllers {
   public class SetRequestModel {
      public string Key {get ; set ;}
      public JObject Value {get ; set ;}
      public string Token {get ; set ;}

      [JsonProperty ("master_key")]
      public string MasterKey {get ; set ;}
   }

 

   [Route ("set")]
   [ApiController]
   public class SetController : Controller {
      private RequestManager _requestManager ;

      public SetController (RequestManager requestManager) {
         _requestManager = requestManager ;
      }

      [HttpPost ("value")]
      public IActionResult Set (SetRequestModel model) {
         Logger.Debug ($"Incoming set request. key: {model.Key}, masterkey: {model.MasterKey}, token: {model.Token}, value: {model.Value}");
         return _requestManager.Set (model.Key, model.MasterKey, model.Token, model.Value) ;
      }
   }
}
