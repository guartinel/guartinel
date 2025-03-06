using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Threading.Tasks ;
using Guartinel.Kernel.Utility ;
using Microsoft.AspNetCore.Http ;
using Microsoft.AspNetCore.Mvc ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.Configuration.Controllers {
   public class SetMasterKeyModel {
      [JsonProperty ("old_key")]
      public string OldKey {get ; set ;}

      [JsonProperty ("new_key")]
      public string NewKey {get ; set ;}
   }

   public class ResetRequestModel {
      [JsonProperty ("master_key")]
      public string MasterKey {get ; set ;}
   }

   public class ManageStatusRequestModel {

   }

   public class ManageStatusResponseModel {
      [JsonProperty("data_hash")]
      public string DataHash { get; set; }

      [JsonProperty("version")]
      public string Version { get; set; }
    

      public new JObject AsJObject () {
         JObject jObject = new JObject();
         if (DataHash != null) {
            jObject[nameof(DataHash).NameToJSONName()] = DataHash;
         }

         if (Version != null) {
            jObject[nameof(Version).NameToJSONName()] = Version;
         }
         return jObject;
      }
   }

   [Route ("manage")]
   [ApiController]
   public class ManageController : Controller {
      private RequestManager _requestManager ;

      public ManageController (RequestManager requestManager) {
         _requestManager = requestManager ;
      }

      [HttpPost ("setMasterKey")]
      public IActionResult SetMasterKey (SetMasterKeyModel model) {
         return _requestManager.SetMasterKey (model.OldKey, model.NewKey) ;
      }

      [HttpPost ("reset")]
      public IActionResult SetMasterKey (ResetRequestModel model) {
         return _requestManager.Reset (model.MasterKey) ;
      }

      [HttpPost("status")]
      public IActionResult SetMasterKey (ManageStatusRequestModel model) {
         return _requestManager.ManageStatus();
      }

   }
}
