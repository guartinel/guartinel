using System ;
using System.Collections.Generic ;
using System.Linq ;
using Guartinel.Kernel.Configuration;
using Guartinel.WatcherServer.InstanceData ;

namespace Guartinel.WatcherServer.Supervisors.ApplicationSupervisor {
   public static class ApplicationInstanceData {
      //public class Constants {
      //   public const string IS_HEARTBEAT = "is_heartbeat" ;
      //}

      //public static bool IsHeartbeat (InstanceData.InstanceData instanceData) {
      //   if (instanceData == null) return false ;

      //   return instanceData.Result.AsBoolean (Constants.IS_HEARTBEAT) ;
      //}

      //public static void SetHeartbeat (InstanceData.InstanceData instanceData,
      //                                 bool value) {
      //   if (instanceData == null) return ;
      //   instanceData.Result.AsJObject [Constants.IS_HEARTBEAT] = value ;
      //}

      //public static void RemoveHeartbeat (InstanceData.InstanceData instanceData) {
      //   instanceData?.Result.AsJObject [Constants.IS_HEARTBEAT].Remove() ;
      //}
   }

   public class ApplicationInstanceDataLists {
      public ApplicationInstanceDataLists (InstanceDataLists instanceDataLists) {
         _instanceDataLists = instanceDataLists ;
      }

      private readonly InstanceDataLists _instanceDataLists ;

      public List<InstanceData.InstanceData> Get (string id) {
         return _instanceDataLists.Get (id) ;
      }

      //public bool IsHeartbeat (string id) {
      //   if (!_instanceDataLists.Contains (id)) return false ;
         
      //   return ApplicationInstanceData.IsHeartbeat (_instanceDataLists.Get (id).LastOrDefault()) ;

      //   // return _heartbeats.Contains (id) ;
      //}

      // public bool HeartbeatExists => _instanceDataLists.HeartbeatExists ;

      //public void SetHeartbeat (string id,
      //                          bool value) {
      //   if (_instanceDataLists.Contains(id)) {
      //      ApplicationInstanceData.SetHeartbeat(_instanceDataLists.Get(id).LastOrDefault(), value);
      //   } else {
      //      var instanceData = new InstanceData.InstanceData(id, id, new ConfigurationData());
      //      ApplicationInstanceData.SetHeartbeat(instanceData, value);
      //      lock (_instanceDataLists) {
      //         _instanceDataLists.Add(id, instanceData);
      //      }
      //   }
      //}

      //public void RemoveHeartbeat (string id) {
      //   SetHeartbeat (id, false) ;
      //}
   }
}