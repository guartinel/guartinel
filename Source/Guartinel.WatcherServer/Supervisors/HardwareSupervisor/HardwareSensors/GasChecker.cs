using System;
using System.Linq ;
using Guartinel.Kernel;
using Guartinel.Kernel.Configuration;
using Guartinel.Kernel.Utility;
using Guartinel.WatcherServer.CheckResults;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares;
using Strings = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors {
   public class GasMq135Sensor : HardwareSensor {
      protected override void Configure1 (ConfigurationData configuration) {
         _gasPresenceThreshold = new HardwareCheckOnOff (configuration [MeasuredDataConstants.Temperature.Instance.NAME],
                                                         EnumEx.Parse (configuration [MeasuredDataConstants.Gas.MQ135.Instance.EXPECTED_STATE],
                                                                       HardwareCheckBoolean.Any)) ;
      }

      protected HardwareCheckOnOff _gasPresenceThreshold ;

      protected override CheckResult Check1 (InstanceData.InstanceData instanceData,
                                             string[] tags) {
         // var measurement = instanceData.InstanceState.GetChild(Strings.Properties.MEASUREMENT);
         var measuredData = instanceData.MeasuredData ;

         var result = CheckResult.CreateUndefined (Name) ;

         int? d1RawValue = measuredData.AsIntegerNull (MeasuredDataConstants.Gas.MQ135.Measurement.D1) ;
         bool? d1Value = d1RawValue == null ? (bool?) null : d1RawValue.Value == MeasuredDataConstants.Gas.MQ135.Measurement.DIGITAL_VALUE_ON ;

         var gasPresenceCheckResult = _gasPresenceThreshold.Check (d1Value) ;

         // If any failed
         if (gasPresenceCheckResult == HardwareCheckResult.Fail) {
            result.CheckResultKind = CheckResultKind.Fail ;

            string messageCode ;

            switch (result.CheckResultKind) {
               case CheckResultKind.Fail:
                  messageCode = Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage) ;
                  break ;
               case CheckResultKind.WarningFail:
                  messageCode = Strings.Use.Get (Strings.Messages.Use.MeasurementWarningMessage) ;
                  break ;

               case CheckResultKind.CriticalFail:
                  messageCode = Strings.Use.Get (Strings.Messages.Use.MeasurementCriticalMessage) ;
                  break ;

               default:
                  messageCode = Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage) ;
                  break ;
            }

            result.Message = new XConstantString (messageCode,
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.NamedMeasurementAlertDetails),
                                                  new XConstantString.Parameter (Strings.Parameters.DeviceName, _gasPresenceThreshold.Name),
                                                  new XConstantString.Parameter (Strings.Parameters.ReferenceValue, _gasPresenceThreshold.ToString()),
                                                  new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (d1Value))) ;
            result.Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                  new XConstantString.Parameter (Strings.Parameters.ReferenceValue, _gasPresenceThreshold.ToString()),
                                                  new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (d1Value))) ;

            return result ;
         }

         if (gasPresenceCheckResult == HardwareCheckResult.Success) {
            result.CheckResultKind = CheckResultKind.Success ;
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Details = new XSimpleString() ;
            result.Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKExtract),
                                                  new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (d1Value))) ;

            if (_gasPresenceThreshold.IsConfigured) {
               result.Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.NamedMeasurementOKDetails),
                                                     new XConstantString.Parameter (Strings.Parameters.DeviceName, _gasPresenceThreshold.Name),
                                                     new XConstantString.Parameter (Strings.Parameters.ReferenceValue, _gasPresenceThreshold.ToString()),
                                                     new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (d1Value))) ;
            }

            return result ;
         }

         return Undefined ;
      }
   }
}