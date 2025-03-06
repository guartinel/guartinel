using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication.Supervisors.HardwareSupervisor ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.InstanceData ;
using SaveConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.Save.Request ;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor {
   public class TimeoutChecker : Checker {
      public Checker Configure (string name,
                                string packageID,
                                string instanceID,
                                string instanceName) {
         base.Configure(name, packageID, instanceID);
         
         _instanceName = instanceName ;

         // _lastResult = new CheckResult().Configure (Name, CheckResultSuccess.Undefined) ;

         return this;
      }

      protected string _instanceName ;

      protected override IList<CheckResult> Check1 (string[] tags) {
         return new List<CheckResult> {
                  new CheckResult().Configure (Name, CheckResultKind.Fail,
                                               new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotConfiguredAlert),
                                                                    new XConstantString.Parameter (Strings.Parameters.InstanceName, _instanceName)),
                                               new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotConfiguredAlertDetails),
                                                                    new XConstantString.Parameter (Strings.Parameters.InstanceName, _instanceName)),
                                               new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotConfiguredAlertExtract)))
         } ;
      }

      public override bool ForceAllowInstanceCheck1() {
         return true ;
      }
   }

   public class HardwareSupervisorPackage : InstanceDataListsPackage {
      public new static class Constants {
         public const string CAPTION = "Hardware Supervisor Package" ;
         public static readonly List<string> CREATOR_IDENTIFIERS = new List<string> {Strings.Use.PackageType} ;
      }

      protected List<HardwareSensor> _hardwareInstances = new List<HardwareSensor>() ;

      protected override void SpecificConfigure1 (ConfigurationData configuration) {
         // Load instances
         List<string> hardwareInstanceIds = new List<string>() ;
         var hardwareInstances = configuration.AsArray (SaveConstants.Configuration.INSTANCES) ;

         if (hardwareInstances != null) {
            foreach (var hardwareInstance in hardwareInstances) {
               hardwareInstanceIds.Add (hardwareInstance.AsString (SaveConstants.Configuration.Instance.INSTANCE_ID,
                                                                   hardwareInstance [SaveConstants.Configuration.Instance.ID])) ;

               _hardwareInstances.Add (HardwareSensor.CreateInstance (hardwareInstance)) ;
            }

            SetInstances (hardwareInstanceIds) ;
         }
      }

      protected override List<Checker> CreateCheckers2() {
         //foreach (var instanceState in _hardwareStates) {
         //   HardwareInstanceState state = instanceState.Value ;
         //}

         lock (_instanceDataLists) {
            List<Checker> result = new List<Checker>();

            // If no hardware instance configured, but data already arrived, then alert to configure
            // @todo SzTZ: add warning instead of alert
            if (!_hardwareInstances.Any() && !_instanceDataLists.IsEmpty) {
               foreach (var dataKey in _instanceDataLists.Keys) {
                  if (!_instanceDataLists.Get (dataKey).Any()) continue ;

                  var instance = _instanceDataLists.Get (dataKey).Last() ;

                  result.Add (new TimeoutChecker().Configure (Name, ID, instance.ID, instance.Name)) ;
                  _instances.Add (instance.ID) ;
               }

               return result ;
            }

            result = _hardwareInstances.Select (x => new HardwareInstanceDataChecker().Configure (Name, ID, x.ID, x.Name,
                                                                                                      _timeouts.Ensure (x.ID),
                                                                                                      x,
                                                                                                      _instanceDataLists.Get (x.ID),
                                                                                                      x.CheckKind) as Checker).ToList() ;

            return result ;
         }
      }
   }
}