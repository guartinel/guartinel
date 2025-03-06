using System;
using System.Linq;
using System.Text;

namespace Guartinel.WatcherServer.InstanceData {
   public class InstanceRegisterDataMessage {
      public string InstanceID { get; }
      public InstanceData InstanceData { get; }

      public InstanceRegisterDataMessage (string instanceID,
                                            InstanceData instanceData) {
         InstanceID = instanceID;
         InstanceData = instanceData;
      }
   }
}
