using System ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Network ;
using Guartinel.Service.Configuration.Controllers ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.Configuration.Replication {
   public class Replica {
      public string MasterKey {get ; set ;}
      public Uri URL {get ; set ;}

      private readonly PostSender _postSender ;

      public Replica (Uri url,
                      string masterkey) {
         MasterKey = masterkey ;
         URL = url ;
         _postSender = new PostSender() ;
      }

      public StatusResponseModel GetStatus() {
         Logger.Info ($"Replica {URL} calling manage/status") ;
         string address = new Uri (URL, "manage/status").ToString() ;
         StatusRequestModel model = new StatusRequestModel() {
                  MasterKey = MasterKey
         } ;
        
         JObject response = _postSender.Post (address, JObject.FromObject(model)) ;
         ServiceResult result = new ServiceResult(response);
         if (!result.Success) {
            throw new Exception (result.Message) ;
         }
         return result.Result.ToObject<StatusResponseModel>() ;
      }

      public ServiceResult Reset () {
         Logger.Info ($"Replica {URL} calling manage/reset") ;
         string address = new Uri (URL, "manage/reset").ToString() ;
         ResetRequestModel model = new ResetRequestModel() {
                  MasterKey = MasterKey
         };
         JObject response = _postSender.Post (address, JObject.FromObject(model)) ;
         ServiceResult result = new ServiceResult(response);
         if (!result.Success) {
            throw new Exception (result.Message) ;
         }

         return result;
      }

      public ServiceResult Set (string userToken,
                                   string key,
                                   JObject value) {
         Logger.Info ($"Replica {URL} calling set/value") ;

         string address = new Uri (URL, "set/value").ToString() ;
         SetRequestModel model =   (new SetRequestModel() {
                  MasterKey = MasterKey,
                  Key = key,
                  Token = userToken,
                  Value = value
         }) ;
         JObject response = _postSender.Post (address, JObject.FromObject(model)) ;
         ServiceResult result = new ServiceResult(response);
         if (!result.Success) {
            throw new Exception(result.Message);
         }

         return result;
      }
   }
}
