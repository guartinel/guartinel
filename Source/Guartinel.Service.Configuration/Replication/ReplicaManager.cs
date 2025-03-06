using System ;
using Guartinel.Service.Configuration.Controllers ;
using System.Collections.Generic ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Service.Configuration.Replication {
   public class ReplicaManager {
      public PersistanceManager _persistanceManager ;

      public ReplicaManager (PersistanceManager persistanceManager) {
         _persistanceManager = persistanceManager ;
      }

      private List<Replica> _replicas = new List<Replica>() ;

      public void DeleteReplica (string url) {
         Replica replicaToDelete = null ;
         foreach (Replica replica in _replicas) {
            if (replica.URL.Equals (url)) {
               replicaToDelete = replica ;
               break ;
            }
         }

         if (replicaToDelete == null) {
            throw new Exception ("Could not found the replica.") ;
         }

         _replicas.Remove (replicaToDelete) ;
      }

      public void Sync() {
         foreach (Replica replica in _replicas) {
            Logger.Info ($"Started to sync replica {replica.URL}") ;
            replica.Reset() ;
            Logger.Info ($"Replica reset done.") ;

            _persistanceManager.IterateOverAllData (null, replica, SetValue) ;
            Logger.Info ($"Replica synced") ;
         }
      }

      public void AddReplica (string url,
                              string replicaMasterKey,
                              bool overWrite) {
         Replica replica = null ;
         foreach (Replica _replica in _replicas) {
            if (!_replica.URL.Equals (new Uri (url))) {
               continue ;
            }

            if (!overWrite) {
               Logger.Error ($"Tried to overwrite replica ({_replica.URL} without overwrite flag.") ;
               throw new System.Exception ("Replicate already added. Specify overwrite flag to proceed.") ;
            }

            replica = _replica ;
            Logger.Info ("Replica will be overwritten") ;
         }

         if (replica == null) {
            Logger.Info ("New replica created.") ;
            replica = new Replica (new Uri (url), replicaMasterKey) ;
         }

         StatusResponseModel response = replica.GetStatus() ;

         bool isReplicaHasTheSameFiles = response.DataHash.Equals (_persistanceManager.GetDataContentHash()) ;
         if (isReplicaHasTheSameFiles) {
            Logger.Info ($"Replica: {replica.URL} has the same files content like us. Simply adding it as replica") ;
            AddIfNotContains(replica) ;
            return ;
         }

         Logger.Info ($"Replica: {replica.URL} has different files content than us. Resetting it first, then send it again") ;

         ServiceResult resetResponse = replica.Reset() ;
         if (!resetResponse.Success) {
            throw new Exception ("Could not reset the replica to start from scratch.") ;
         }

         Logger.Info ("Replica reset was successful.  Copying current content to there.") ;
         _persistanceManager.IterateOverAllData (null, replica, SetValue) ;
         AddIfNotContains(replica);

      }

      private void AddIfNotContains (Replica replica) {
         if (_replicas.Contains (replica)) {
            return ;
         }
         _replicas.Add (replica);
   }

      private void SetValue (Replica replica,
                             string token,
                             string key,
                             JObject value) {
         replica.Set (token, key, value) ;
      }

      public void Set (string userToken,
                       string key,
                       JObject value) {
         foreach (Replica replica in _replicas) {
            try {
               replica.Set (userToken, key, value) ;
            } catch (Exception e) {
               Logger.Info ($"Could not set key {key} on replica {replica.URL}");
            }
         }
      }

      public JArray GetReplicasStatus() {
         JArray replicas = new JArray() ;
         foreach (Replica replica in _replicas) {
            StatusResponseModel response = null ;
            try {
               response = replica.GetStatus() ;
            } catch (Exception e) {
               Logger.Error ($"Could not get status from replica {replica.URL}, E: {e.GetAllMessages()}") ;
            }

            JObject replicaStatus = response.AsJObject() ;
            replicaStatus ["url"] = replica.URL ;
            replicas.Add (replicaStatus) ;
         }

         return replicas ;
      }
   }
}
