

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Utility;

namespace Guartinel.Service.Configuration.Controllers {
   public class AddReplicaModel {
      [JsonProperty ("url")]
      public string URL {get ; set ;}

      [JsonProperty ("master_key")]
      public string MasterKey {get ; set ;}

      [JsonProperty ("replica_master_key")]
      public string ReplicaMasterKey {get ; set ;}

      [JsonProperty("overwrite")]
      public bool Overwrite { get; set; }
   }

   public class DeleteReplicaModel {
      [JsonProperty ("master_key")]
      public string MasterKey {get ; set ;}
      [JsonProperty("url")]
      public string URL { get; set; }

   }
   public class StatusRequestModel {
      [JsonProperty("master_key")]
      public string MasterKey { get; set; }
   }

   public class StatusResponseModel {
      [JsonProperty("data_hash")]
      public string DataHash { get; set; }

      [JsonProperty("version")]
      public string Version { get; set; }

      [JsonProperty("replicas")]
      public JArray Replicas { get; set; }


      public new JObject AsJObject () {
         JObject jObject = new JObject();
         if (DataHash != null) {
            jObject[nameof(DataHash).NameToJSONName()] = DataHash;
         }

         if (Version != null) {
            jObject[nameof(Version).NameToJSONName()] = Version;
         }

         if (Replicas != null) {
            jObject[nameof(Replicas).NameToJSONName()] = Replicas;
         }

         return jObject;
      }
   }

   [Microsoft.AspNetCore.Mvc.Route("replica")]
   [ApiController]
   public class ReplicaController : Controller {
      private RequestManager _requestManager ;

      public ReplicaController (RequestManager requestManager) {
         _requestManager = requestManager ;
      }

      [HttpPost ("add")]
      public IActionResult Add (AddReplicaModel model) {
         return _requestManager.AddReplica (model.MasterKey, model.URL, model.ReplicaMasterKey,model.Overwrite) ;
      }

      [HttpPost ("delete")]
      public IActionResult Delete (DeleteReplicaModel model) {
         return _requestManager.DeleteReplica (model.MasterKey,model.URL) ;
      }

      [HttpPost("sync")]
      public IActionResult Sync (DeleteReplicaModel model) {
         return _requestManager.SyncReplicas(model.MasterKey);
      }

      [HttpPost("status")]
      public IActionResult Status (StatusRequestModel requestModel) {
         return _requestManager.ReplicateStatus(requestModel.MasterKey);
      }
   }
}
