using System;
using System.Linq ;
using Guartinel.Kernel;
using Guartinel.Kernel.Configuration;
using Guartinel.Kernel.Utility;
using Guartinel.WatcherServer.CheckResults;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares;
using Strings = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors {
   public class LiquidSensor : HardwareSensor {
      protected override void Configure1 (ConfigurationData configuration) {
         _waterPresence = new HardwareCheckOnOff (configuration [MeasuredDataConstants.Water.Presence.Instance.NAME],
                                                  EnumEx.Parse (configuration [MeasuredDataConstants.Water.Presence.Instance.EXPECTED_STATE],
                                                                HardwareCheckBoolean.Any)) ;
      }

      protected HardwareCheckOnOff _waterPresence ;

      protected override CheckResult Check1 (InstanceData.InstanceData instanceData,
                                             string[] tags) {
         var measuredData = instanceData.MeasuredData ;

         var result = CheckResult.CreateUndefined (Name) ;

         int? d1RawValue = measuredData.AsIntegerNull (MeasuredDataConstants.Water.Presence.Measurement.WATER_PRESENCE) ;
         bool? d1Value = d1RawValue == null ? (bool?) null : d1RawValue.Value == MeasuredDataConstants.Water.Presence.Measurement.VALUE_ON_PRESENCE ;

         var waterPresenceCheckResult = _waterPresence.Check (d1Value) ;

         // If any failed
         if (waterPresenceCheckResult == HardwareCheckResult.Fail) {

            result.CheckResultKind = CheckResultKind.Fail ;
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;

            bool d1ValueBool = (d1Value ?? (bool?) false).Value ;
            result.Details = new XConstantString (Strings.Use.Get (d1ValueBool ?
                                                                            Strings.Messages.Use.MeasurementDetailsLiquidSensorYes :
                                                                            Strings.Messages.Use.MeasurementDetailsLiquidSensorNo),
                                                  new XConstantString.Parameter (Strings.Parameters.DeviceName, _waterPresence.Name)) ;
            result.Extract = new XConstantString (Strings.Use.Get (d1ValueBool ?
                                                                            Strings.Messages.Use.MeasurementExtractLiquidSensorYes :
                                                                            Strings.Messages.Use.MeasurementExtractLiquidSensorNo)) ;

            return result ;
         }

         if (waterPresenceCheckResult == HardwareCheckResult.Success) {
            result.CheckResultKind = CheckResultKind.Success ;
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Details = new XSimpleString() ;
            result.Extract = new XSimpleString() ;

            if ((_waterPresence.State ?? HardwareCheckBoolean.Any) != HardwareCheckBoolean.Any) {
               bool d1ValueBool = (d1Value ?? (bool?) false).Value ;
               result.Details = new XConstantString (Strings.Use.Get (d1ValueBool ?
                                                                               Strings.Messages.Use.MeasurementDetailsLiquidSensorYes :
                                                                               Strings.Messages.Use.MeasurementDetailsLiquidSensorNo),
                                                     new XConstantString.Parameter (Strings.Parameters.DeviceName, _waterPresence.Name)) ;
               result.Extract = new XConstantString (Strings.Use.Get (d1ValueBool ?
                                                                               Strings.Messages.Use.MeasurementExtractLiquidSensorYes :
                                                                               Strings.Messages.Use.MeasurementExtractLiquidSensorNo)) ;
            }

            return result ;
         }

         return Undefined ;
      }
   }
}