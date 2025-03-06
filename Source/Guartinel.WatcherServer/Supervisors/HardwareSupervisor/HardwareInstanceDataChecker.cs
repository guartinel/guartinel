using System;
using System.Collections.Generic ;
using System.Linq;
using System.Text;
using Guartinel.Kernel;
using Guartinel.WatcherServer.CheckResults;
using Guartinel.WatcherServer.InstanceData ;
using Strings = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings ;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor {
   public class HardwareInstanceDataChecker : InstanceDataChecker {
      public class Constants {
         public const string CAPTION = "Hardware Status Checker" ;
      }

      #region Construction
      //public static Creator GetCreator() {
      //   return new Creator (typeof (HardwareInstanceDataChecker), () => new HardwareInstanceDataChecker(), typeof (Checker), Constants.CAPTION) ;
      //}
      #endregion

      private HardwareSensor _instance ;

      #region Configuration
      public HardwareInstanceDataChecker Configure (string name,
                                                    string packageID,
                                                    string instanceID,
                                                    string instanceName,
                                                    Timeout timeout,
                                                    HardwareSensor instance,
                                                    List<InstanceData.InstanceData> instanceDataList,
                                                    InstanceDataListCheckKind checkKind = InstanceDataListCheckKind.FailIfEveryOneFails) {
         Configure (name, packageID, instanceID, instanceName, timeout, instanceDataList, true, checkKind) ;
         _instance = instance ;

         return this ;
      }
      #endregion

      protected override CheckResult CreateTimeout => 
         new CheckResult().Configure (Name, CheckResultKind.Fail,
                                      new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotAvailableAlert),
                                                           new XConstantString.Parameter (Strings.Parameters.InstanceName, InstanceName)),
                                      new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotAvailableAlertDetails),
                                                           new XConstantString.Parameter (Strings.Parameters.InstanceName, InstanceName)),
                                      new XConstantString(Strings.Use.Get(Strings.Messages.Use.InstanceNotAvailableAlertExtract))) ;

      public override bool ForceAllowInstanceCheck1 () {
         // Allow check anyway
         return true ;
      }

      protected override CheckResult Check3 (InstanceData.InstanceData instanceData,
                                             string[] tags) {
         return _instance.Check (instanceData,
                                 tags) ;
      }
   }
}