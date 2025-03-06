using System ;
using System.Collections.Generic ;
using System.Linq ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.Packages ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.InstanceData {
   public abstract class InstanceDataListsPackage : Package {
      protected readonly Timeouts _timeouts = new Timeouts() ;
      protected readonly InstanceDataLists _instanceDataLists = new InstanceDataLists() ;
      protected readonly Dictionary<string, string> _instanceNamesByIDs = new Dictionary<string, string>();

      protected sealed override void SpecificConfigure (ConfigurationData configuration) {
         _timeouts.Configure (CheckIntervalSeconds, TimeoutIntervalSeconds) ;

         SpecificConfigure1 (configuration) ;
      }

      protected virtual void SpecificConfigure1 (ConfigurationData configuration) { }

      protected override void RegisterMessages1() {         
         MessageBus.Use.Register<InstanceDataMessage> (ID, message => ExceptionEx.ExecuteWithLog (() => RegisterInstanceData (message),
                                                                                                  messageString => Logger.Error (messageString, "RegisterMessages1"))) ;
      }

      protected override void UnregisterMessages1() {
         MessageBus.Use.Unregister<InstanceDataMessage> (ID) ;
      }

      protected sealed override List<Checker> CreateCheckers1 () {
         var result = CreateCheckers2() ;

         // Drop current data
         _instanceDataLists.Clear() ;

         return result ;
      }

      protected abstract List<Checker> CreateCheckers2() ;

      protected virtual void RegisterInstanceData (InstanceDataMessage dataMessage) {
         if (dataMessage == null) return ;
         if (string.IsNullOrEmpty (dataMessage.ID)) return ;

         _logger.Debug ($"Register instance data. Package ID: {ID} Instance ID: {dataMessage.ID}. Data: {dataMessage.Data.AsJObject.ConvertToLog()}") ;
         var instanceData = new InstanceData (dataMessage.ID,
                                              dataMessage.Name,
                                              dataMessage.Data.Duplicate().CastTo<ConfigurationData>()) ;

         // Add result
         _instanceDataLists.Add (dataMessage.ID, instanceData) ;
         _instanceNamesByIDs [dataMessage.ID] = dataMessage.Name ;
 
         _timeouts.Ensure (dataMessage.ID).Reset() ;

         RegisterInstanceData1 (dataMessage) ;
      }

      protected virtual void RegisterInstanceData1 (InstanceDataMessage dataMessage) {}
   }
}