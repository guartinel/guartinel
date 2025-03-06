using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guartinel.Communication.Supervisors.ApplicationSupervisor ;
using Guartinel.Kernel;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.CheckResults;
using Guartinel.WatcherServer.InstanceData ;
using CheckResultConstants = Guartinel.Communication.Supervisors.ApplicationSupervisor.Strings.WatcherServerRoutes.RegisterResult.Request.CheckResult ;

namespace Guartinel.WatcherServer.Supervisors.ApplicationSupervisor {
   public class ApplicationInstanceDataChecker : InstanceDataChecker {
      public class Constants {
         public const string CAPTION = "Application Status Checker" ;
      }

      #region Configuration
      public new ApplicationInstanceDataChecker Configure (string name,
                                                           string packageID,
                                                           string instanceID,
                                                           string instanceName,
                                                           Timeout timeout,
                                                           List<InstanceData.InstanceData> instanceDataList,
                                                           bool isHeartbeat,
                                                           InstanceDataListCheckKind checkKind = InstanceDataListCheckKind.FailIfLastFails) {
         base.Configure (name, packageID, instanceID, instanceName, timeout, instanceDataList, isHeartbeat, checkKind) ;

         return this ;
      }
      #endregion

      protected override CheckResult CreateTimeout => new CheckResult().Configure (Name, CheckResultKind.Fail,
                                                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotAvailableAlertMessage),
                                                                                                        new XConstantString.Parameter (Strings.Parameters.InstanceName, InstanceName)),
                                                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotAvailableAlertDetails),
                                                                                                        new XConstantString.Parameter (Strings.Parameters.InstanceName, InstanceName)),
                                                                                   new XConstantString(Strings.Use.Get(Strings.Messages.Use.InstanceNotAvailableAlertExtract))) ;

      protected CheckResult CreatePackageTimeout => new CheckResult().Configure (Name, CheckResultKind.Fail,
                                                                                 new XConstantString (Strings.Use.Get (Strings.Messages.Use.PackageNotAvailableAlert),
                                                                                                      CreatePackageNameParameter (PackageID)),
                                                                                 new XConstantString (Strings.Use.Get (Strings.Messages.Use.PackageNotAvailableAlertDetails),
                                                                                                      CreatePackageNameParameter (PackageID)),
                                                                                 new XConstantString(Strings.Use.Get(Strings.Messages.Use.PackageNotAvailableAlertExtract))) ;

      public static CheckResultKind GetResultKind (string value) {
         if (string.IsNullOrEmpty (value)) return CheckResultKind.Undefined ;

         if (value.Equals (CheckResultConstants.CheckResultValue.SUCCESS, StringComparison.InvariantCultureIgnoreCase)) return CheckResultKind.Success ;
         if (value.Equals (CheckResultConstants.CheckResultValue.WARNING, StringComparison.InvariantCultureIgnoreCase)) return CheckResultKind.WarningFail ;
         if (value.Equals (CheckResultConstants.CheckResultValue.FAIL, StringComparison.InvariantCultureIgnoreCase)) return CheckResultKind.Fail ;
         if (value.Equals (CheckResultConstants.CheckResultValue.CRITICAL, StringComparison.InvariantCultureIgnoreCase)) return CheckResultKind.CriticalFail ;

         return CheckResultKind.Undefined ;
      }

      public override bool ForceAllowInstanceCheck1() {
         // Allow check for package heartbeat
         return InstanceID == Guartinel.Communication.Strings.Strings.HEARTBEAT_INSTANCE_ID ;
      }

      protected override CheckResult CheckTimeout1(string[] tags) {
         var logger = new TagLogger (tags) ;

         // Check if package heartbeat
         if (InstanceID == Guartinel.Communication.Strings.Strings.HEARTBEAT_INSTANCE_ID) {
            if (CheckTimeout()) {
               // Use special timeout alert when the package timeout happens
               return CreatePackageTimeout ;
            }

            return new CheckResult().Configure (Name, CheckResultKind.Success,
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.PackageAvailableRecoveryMessage),
                                                                     CreatePackageNameParameter (PackageID)),
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.PackageAvailableRecoveryDetails),
                                                                     CreatePackageNameParameter (PackageID)),
                                                new XConstantString(Strings.Use.Get(Strings.Messages.Use.PackageAvailableRecoveryExtract))) ;
         }

         // Timeout
         // var isHeartbeatString = _isHeartbeat ? "heartbeat" : "not heartbeat" ;
         // Logger.Log ($"{instanceID} is {isHeartbeatString}.") ;
         // if (ApplicationInstanceData.IsHeartbeat (InstanceDataList)) {
         if (_isHeartbeat) {
            if (CheckTimeout()) {
               logger.Debug ($"'{InstanceID}' is timed out (heartbeat).") ;
               return CreateTimeout ;
            }
         } else {
            // If not a heartbeat, but timeout, then return undefined
            if (CheckTimeout()) {
               logger.Debug ($"'{InstanceID}' is timed out.") ;
               return CreateUndefined ;
            }
         }

         return null ;
      }

      protected override CheckResult Check3 (InstanceData.InstanceData instanceData,
                                             string[] tags) {
         // Get state for app instance
         var successValue = !string.IsNullOrEmpty (instanceData.MeasuredData [CheckResultConstants.RESULT]) ?
                  instanceData.MeasuredData [CheckResultConstants.RESULT] :
                  instanceData.MeasuredData [CheckResultConstants.SUCCESS] ;
         var message = instanceData.MeasuredData [CheckResultConstants.MESSAGE] ;
         var details = instanceData.MeasuredData [CheckResultConstants.DETAILS] ;

         var checkResult = new CheckResult() ;

         CheckResultKind resultKind = GetResultKind (successValue) ;

         if (resultKind == CheckResultKind.Undefined) {
            return CreateUndefined ;
         }
         
         if (resultKind == CheckResultKind.Success) {
            return checkResult.Configure (Name, CheckResultKind.Success,
                                          new XConstantString (Strings.Use.Get (Strings.Messages.Use.ApplicationMeasurementOKMessage),
                                                               new XConstantString.Parameter (Strings.Parameters.InstanceName, InstanceName)),
                                          new XConstantString (Strings.Use.Get (Strings.Messages.Use.ApplicationMeasurementOKDetails),
                                                               message == null ? null : new XConstantString.Parameter (Strings.Parameters.Message, message),
                                                               details == null ? null : new XConstantString.Parameter (Strings.Parameters.Details, details)),
                                          new XConstantString(Strings.Use.Get(Strings.Messages.Use.ApplicationMeasurementOKExtract))) ;
         }

         string messageCode ;
         string extractCode ;

         switch (resultKind) {
            case CheckResultKind.Fail:
               messageCode = Strings.Use.Get(Strings.Messages.Use.ApplicationMeasurementAlertMessage);
               extractCode = Strings.Use.Get(Strings.Messages.Use.ApplicationMeasurementAlertExtract);
               break ;
            case CheckResultKind.WarningFail:
               messageCode = Strings.Use.Get (Strings.Messages.Use.ApplicationMeasurementWarningMessage) ;
               extractCode = Strings.Use.Get(Strings.Messages.Use.ApplicationMeasurementWarningExtract);
               break ;

            case CheckResultKind.CriticalFail:
               messageCode = Strings.Use.Get (Strings.Messages.Use.ApplicationMeasurementCriticalMessage) ;
               extractCode = Strings.Use.Get(Strings.Messages.Use.ApplicationMeasurementCriticalExtract);
               break ;

            default:
               messageCode = Strings.Use.Get (Strings.Messages.Use.ApplicationMeasurementAlertMessage) ;
               extractCode = Strings.Use.Get(Strings.Messages.Use.ApplicationMeasurementAlertExtract) ;
               break ;
         }

         checkResult.Configure (Name, resultKind,
                                new XConstantString (messageCode,
                                                     new XConstantString.Parameter (Strings.Parameters.InstanceName, InstanceName)),
                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.ApplicationMeasurementAlertDetails),
                                                     message == null ? null : new XConstantString.Parameter (Strings.Parameters.Message, message),
                                                     details == null ? null : new XConstantString.Parameter (Strings.Parameters.Details, details)),
                                new XConstantString (extractCode)) ;


         // Drop instance state
         // Logger.Log ($"{instanceID} instance state removed.") ;

         // _instanceStates.Remove (instanceID) ;

         return checkResult ;
      }
   }
}