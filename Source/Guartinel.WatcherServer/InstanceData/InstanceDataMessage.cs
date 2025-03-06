using System ;
using System.Linq ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;

namespace Guartinel.WatcherServer.InstanceData {
   public class InstanceDataMessage : MessageBus.Message {
      public InstanceDataMessage (string id,
                                  string name,
                                  ConfigurationData data) {
         ID = id ;
         Name = name ;
         Data = data.Duplicate() ;
      }

      public string ID {get ;}
      public string Name { get; }
      public ConfigurationData Data {get ;}
   }
}