using System ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.Service.Configuration.Controllers ;
using Guartinel.Service.Configuration.Replication ;
using Microsoft.AspNetCore.Mvc ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.Configuration {
   public class RequestManager {
      private PersistanceManager _persistanceManager ;
      private ReplicaManager _replicaManager ;

      public RequestManager (PersistanceManager persistanceManager) {
         this._persistanceManager = persistanceManager ;
         _replicaManager = new ReplicaManager (_persistanceManager) ;
      }

      protected JsonResult CreateMasterKeyError() {
         return new JsonResult (ServiceResult.CreateError ("INVALID_MASTER_KEY", $"The provided master key is invalid.").AsJObject()) ;
      }

      protected JsonResult CreateTokenError() {
         return new JsonResult(ServiceResult.CreateError("INVALID_TOKEN", $"The provided token is invalid.").AsJObject());
      }

      protected JsonResult CreateKeyError (string key) {
         return new JsonResult(ServiceResult.CreateError("INVALID_KEY", $"The provided key '{key}' is invalid.").AsJObject());
      }

      public JsonResult Set (string key,
                             string masterKey,
                             string userToken,
                             JObject value) {
         ServiceResult result = null ;
         if (masterKey != Constants.MASTER_KEY) {
            return CreateMasterKeyError() ;
         }

         _persistanceManager.SetToken (key, userToken) ;
         _persistanceManager.SetData (key, value.ToString (Formatting.Indented)) ;

         _replicaManager.Set (userToken, key, value) ;
         result = ServiceResult.CreateSuccess ("Key set!") ;
         return new JsonResult (result.AsJObject()) ;
      }

      public JsonResult Get (string key,
                             string userToken) {
         ServiceResult result = null ;
         string token = null ;
         try {
            token = _persistanceManager.GetToken (key) ;
         } catch (Exception e) {
            return CreateKeyError(key) ;
         }

         if (token != userToken) {
            return CreateTokenError() ;
         }

         string stringData = _persistanceManager.GetData (key) ;
         try {
            result = ServiceResult.CreateSuccess (JObject.Parse (stringData)) ;
         } catch (Exception e) {
            //  Logger.Log ("Could not parse json") ;
            result = ServiceResult.CreateError ("Cannot parse config file.", e.Message) ;
         }

         return new JsonResult (result.AsJObject()) ;
      }

      public JsonResult GetHash (string key,
                                 string userToken) {
         ServiceResult result = null ;
         string token = null ;
         try {
            token = _persistanceManager.GetToken (key) ;
         } catch (Exception e) {
            return CreateKeyError(key);
         }

         if (token != userToken) {
            return CreateTokenError();
         }

         string stringData = _persistanceManager.GetData (key) ;

         result = ServiceResult.CreateSuccess (Hashing.GenerateHash (stringData, key)) ;

         return new JsonResult (result.AsJObject()) ;
      }

      public JsonResult Reset (string masterKey) {
         if (masterKey != Constants.MASTER_KEY) {
            return CreateMasterKeyError() ;
         }

         try {
            _persistanceManager.Reset() ;
         } catch (Exception e) {
            return new JsonResult (ServiceResult.CreateError ("Cannot parse config file.", e.Message).AsJObject()) ;
         }

         return new JsonResult (ServiceResult.CreateSuccess ("Reset done!").AsJObject()) ;
      }

      public JsonResult SetMasterKey (string oldKey,
                                      string newKey) {
         if (Constants.MASTER_KEY != null && Constants.MASTER_KEY != oldKey) {
            return CreateMasterKeyError();
         }

         Constants.MASTER_KEY = newKey ;
         return new JsonResult (ServiceResult.CreateSuccess ("MasterKey set!").AsJObject()) ;
      }

      public JsonResult AddReplica (string masterkey,
                                    string url,
                                    string replicaMasterKey,
                                    bool overWrite = false) {
         if (Constants.MASTER_KEY != masterkey) {
            return CreateMasterKeyError() ;
         }

         try {
            _replicaManager.AddReplica (url, replicaMasterKey, overWrite) ;
         } catch (Exception e) {
            return new JsonResult (ServiceResult.CreateError ("Cannot add the replica.", e.Message).AsJObject()) ;
         }

         return new JsonResult (ServiceResult.CreateSuccess ("Replica added!").AsJObject()) ;
      }

      public JsonResult SyncReplicas (string masterKey) {
         if (Constants.MASTER_KEY != masterKey) {
            return CreateMasterKeyError() ;
         }

         try {
            _replicaManager.Sync() ;
         } catch (Exception e) {
            return new JsonResult (ServiceResult.CreateError ("Cannot sync replicas.", e.Message).AsJObject()) ;
         }

         return new JsonResult (ServiceResult.CreateSuccess ("Replicas synced!").AsJObject()) ;
      }

      public JsonResult DeleteReplica (string masterkey,
                                       string url) {
         if (Constants.MASTER_KEY != masterkey) {
            return CreateMasterKeyError() ;
         }

         try {
            _replicaManager.DeleteReplica (url) ;
         } catch (Exception e) {
            return new JsonResult (ServiceResult.CreateError ("Cannot delete the replica.", e.Message).AsJObject()) ;
         }

         return new JsonResult (ServiceResult.CreateSuccess ("Replica deleted!").AsJObject()) ;
      }

      public JsonResult ReplicateStatus (string masterKey) {
         if (Constants.MASTER_KEY != masterKey) {
            return CreateMasterKeyError() ;
         }

         string dataHash = "" ;

         try {
            dataHash = _persistanceManager.GetDataContentHash() ;
         } catch (Exception e) {
            Logger.Error ($"Cloud not get data hash: {e.GetAllMessages()}") ;
         }

         StatusResponseModel result = new StatusResponseModel() {
                  DataHash = dataHash,
                  Version = Constants.VERSION,
                  Replicas = _replicaManager.GetReplicasStatus()
         } ;
         return new JsonResult (ServiceResult.CreateSuccess (result.AsJObject()).AsJObject()) ;
      }

      public JsonResult ManageStatus() {
         string dataHash = "" ;

         try {
            dataHash = _persistanceManager.GetDataContentHash() ;
         } catch (Exception e) {
            Logger.Error ($"Cloud not get data hash: {e.GetAllMessages()}") ;
         }

         ManageStatusResponseModel result = new ManageStatusResponseModel() {
                  DataHash = dataHash,
                  Version = Constants.VERSION
         } ;
         return new JsonResult (ServiceResult.CreateSuccess (result.AsJObject()).AsJObject()) ;
      }
   }
}
