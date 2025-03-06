using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication.Supervisors.ApplicationSupervisor ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.InstanceData ;
using CommonStrings = Guartinel.Communication.Strings.Strings ;

namespace Guartinel.WatcherServer.Supervisors.ApplicationSupervisor {
   public class ApplicationSupervisorPackage : InstanceDataListsPackage {
      // private readonly ApplicationInstanceDataLists _applicationDataLists ;

      public new static class Constants {
         public const string CAPTION = "Application Supervisor Package" ;
         public static readonly List<string> CREATOR_IDENTIFIERS = new List<string> {Strings.Use.PackageType} ;
      }

      //public ApplicationSupervisorPackage() {
         // _applicationDataLists = new ApplicationInstanceDataLists (_instanceDataLists) ;
      //}

      //public new static Creator GetCreator() {
      //   return new Creator<Package, ApplicationSupervisorPackage> (() => new ApplicationSupervisorPackage(), Constants.CAPTION, Constants.CREATOR_IDENTIFIERS) ;
      //}
      protected List<string> _heartbeats = new List<string>();

      protected override List<Checker> CreateCheckers2() {

         // Get instance IDs from server
         IList<string> instanceIds = IoC.Use.Single.GetInstance<IManagementServerPackages>()?.GetApplicationInstanceIDs (ID) ?? new List<string>() ;

         //// Check if there is a heartbeat
         //var extraHeartbeatNeeded = (!_instanceDataLists.IsEmpty) &&
         //                           (!instanceIds.Any (x => (x != CommonStrings.HEARTBEAT_INSTANCE_ID) &&
         //                                                   (_applicationDataLists.IsHeartbeat (x)))) ;

         //if (extraHeartbeatNeeded) {
         //   if (!instanceIds.Contains (CommonStrings.HEARTBEAT_INSTANCE_ID)) {
         //      Logger.Log ($"Create extra heartbeat checker to package '{ID}'.") ;
         //      instanceIds.Add (CommonStrings.HEARTBEAT_INSTANCE_ID) ;
         //      _applicationDataLists.SetHeartbeat (CommonStrings.HEARTBEAT_INSTANCE_ID, true) ;
         //   }
         //} else {
         //   if (instanceIds.Contains (CommonStrings.HEARTBEAT_INSTANCE_ID)) {
         //      Logger.Log ($"Remove extra heartbeat checker from package '{ID}'.") ;
         //      instanceIds.Remove (CommonStrings.HEARTBEAT_INSTANCE_ID) ;
         //      _applicationDataLists.RemoveHeartbeat (CommonStrings.HEARTBEAT_INSTANCE_ID) ;
         //   }
         //}

         // Check if extra heartbeat is needed
         if (!_heartbeats.Any()) {
            if (!instanceIds.Contains (CommonStrings.HEARTBEAT_INSTANCE_ID)) {
               instanceIds.Add (CommonStrings.HEARTBEAT_INSTANCE_ID) ;
            }
         } else {
            instanceIds.Remove (CommonStrings.HEARTBEAT_INSTANCE_ID) ;
         }

         SetInstances (instanceIds) ;

         List<Checker> result = instanceIds.Select (instanceID => {
            var checker = new ApplicationInstanceDataChecker() ;
            // Handle heartbeat
            // bool isHeartbeat = instanceID == CommonStrings.HEARTBEAT_INSTANCE_ID || _applicationDataLists.IsHeartbeat (instanceID) ;
            bool isHeartbeat = instanceID == CommonStrings.HEARTBEAT_INSTANCE_ID || _heartbeats.Contains (instanceID) ;
            var instanceDataList = instanceID == CommonStrings.HEARTBEAT_INSTANCE_ID ? null : _instanceDataLists.Get (instanceID) ;
            // string instanceName = instanceDataList?.LastOrDefault()?.Name ?? string.Empty ;
            string instanceName = _instanceNamesByIDs.ContainsKey (instanceID) ? _instanceNamesByIDs [instanceID] : instanceDataList?.LastOrDefault()?.Name ?? string.Empty ;

            // Configure checker
            return checker.Configure (Name, ID, instanceID,
                                      instanceName,
                                      _timeouts.Ensure (instanceID),
                                      instanceDataList,
                                      isHeartbeat,
                                      InstanceDataListCheckKind.FailIfLastFails) as Checker ;
         }).ToList() ;

         return result ;
      }

      protected override void RegisterInstanceData1 (InstanceDataMessage dataMessage) {
         // Reset package heartbeat
         _timeouts.Ensure (CommonStrings.HEARTBEAT_INSTANCE_ID).Reset() ;

         if (dataMessage.Data.AsBoolean (CommonStrings.IS_HEARTBEAT_PROPERTY_NAME)) {
            if (!_heartbeats.Contains (ID)) {
               _heartbeats.Add (dataMessage.ID) ;
            }
         } else {
            if (_heartbeats.Contains (ID)) {
               _heartbeats.Remove (dataMessage.ID) ;
            }
         }

         //_applicationDataLists.SetHeartbeat (dataMessage.ID,
         //                                    dataMessage.Data.AsBoolean (CommonStrings.IS_HEARTBEAT_PROPERTY_NAME)) ;

         //if (dataMessage.ID != CommonStrings.HEARTBEAT_INSTANCE_ID) {
         //   var extraHeartbeatNeeded = _instanceDataLists.Keys.Count (x => (x != CommonStrings.HEARTBEAT_INSTANCE_ID) &&
         //                                                                (_applicationDataLists.IsHeartbeat (x))) == 0 ;

         //   if (extraHeartbeatNeeded) {
         //      var heartbeatState = new ConfigurationData() ;
         //      heartbeatState [CheckResultConstants.SUCCESS] = CheckResultConstants.SuccessValue.SUCCESS_VALUE ;
         //      heartbeatState [CheckResultConstants.MESSAGE] = string.Empty ;
         //      heartbeatState.AsJObject [CommonStrings.IS_HEARTBEAT_PROPERTY_NAME] = true ;

         //      InstanceDataMessage heartbeatDataMessage = new InstanceDataMessage (CommonStrings.HEARTBEAT_INSTANCE_ID,
         //                                                                      CommonStrings.HEARTBEAT_INSTANCE_NAME,
         //                                                                      heartbeatState) ;

         //      RegisterInstanceData (heartbeatDataMessage) ;
         //   }
         //}
      }
   }
}