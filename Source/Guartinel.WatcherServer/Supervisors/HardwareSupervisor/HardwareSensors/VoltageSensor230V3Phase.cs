using System;
using System.Linq ;
using Guartinel.Kernel;
using Guartinel.Kernel.Configuration;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility;
using Guartinel.WatcherServer.CheckResults;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares;
using Strings = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors {

   public class VoltageChecker230V3Channel : VoltageSensor {
      protected override void Configure2 (ConfigurationData configuration) {
         var channel1 = configuration.GetChild (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.CHANNEL_1) ;
         _channel1 = new HardwareCheckRange (channel1 [MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel1.NAME],
                                             channel1.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel1.MIN_THRESHOLD),
                                             channel1.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel1.MAX_THRESHOLD)) ;

         var channel2 = configuration.GetChild (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.CHANNEL_2) ;
         _channel2 = new HardwareCheckOnOff (channel2 [MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel2.NAME],
                                             EnumEx.Parse (channel2 [MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel2.EXPECTED_STATE],
                                                           HardwareCheckBoolean.Any)) ;

         var channel3 = configuration.GetChild (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.CHANNEL_3) ;
         _channel3 = new HardwareCheckOnOff (channel3 [MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel3.NAME],
                                             EnumEx.Parse (channel3 [MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Instance.Channel3.EXPECTED_STATE],
                                                           HardwareCheckBoolean.Any)) ;
      }

      protected HardwareCheckRange _channel1 ;

      // protected HardwareCheckOnOff _channel1OnOff ;
      protected HardwareCheckOnOff _channel2 ;
      protected HardwareCheckOnOff _channel3 ;

      public static void Check (HardwareCheckOnOff channel,
                                bool isSensitive,
                                bool? value,
                                bool? valueMin,
                                bool? valueMax,
                                CheckResult result,
                                XStrings successDetails,
                                XStrings successExtract,
                                XStrings errorDetails,
                                XStrings errorExtract) {
         if (channel.IsConfigured) {
            var resultValue = channel.Check (value) ;
            if (resultValue == HardwareCheckResult.Fail) {
               result.CheckResultKind = CheckResultKind.Fail ;
               errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.NamedMeasurementAlertDetails),
                                                      new XConstantString.Parameter (Strings.Parameters.DeviceName, channel.Name),
                                                      new XConstantString.Parameter (Strings.Parameters.ReferenceValue, channel.ToString()),
                                                      new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (value)))) ;
               errorExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                      new XConstantString.Parameter (Strings.Parameters.ReferenceValue, channel.ToString()),
                                                      new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (value)))) ;
            }

            if (resultValue == HardwareCheckResult.Success) {
               successDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.NamedMeasurementOKDetails),
                                                        new XConstantString.Parameter (Strings.Parameters.DeviceName, channel.Name),
                                                        new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (value)))) ;
               successExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.NamedMeasurementOKExtract),
                                                        new XConstantString.Parameter(Strings.Parameters.DeviceName, channel.Name),
                                                        new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (value)))) ;
            }

            if (isSensitive) {
               var minResult = channel.Check (valueMin) ;
               if (minResult == HardwareCheckResult.Fail) {
                  result.CheckResultKind = CheckResultKind.Fail ;
                  if (channel.State != null) {
                     errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementSensitiveAlertDetails),
                                                            new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (channel.State.Value == HardwareCheckBoolean.On)),
                                                            new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (valueMin)))) ;
                     errorExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                            new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (channel.State.Value == HardwareCheckBoolean.On)),
                                                            new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (valueMin)))) ;
                  }
               }

               var maxResult = channel.Check (valueMax) ;
               if (maxResult == HardwareCheckResult.Fail) {
                  result.CheckResultKind = CheckResultKind.Fail ;
                  if (channel.State != null) {
                     errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementSensitiveAlertDetails),
                                                            new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (channel.State.Value == HardwareCheckBoolean.On)),
                                                            new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (valueMax)))) ;
                     errorExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                            new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (channel.State.Value == HardwareCheckBoolean.On)),
                                                            new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (valueMax)))) ;
                  }
               }
            }
         }
      }

      protected bool? ConvertDigitalValue (int? rawValue) {
         return rawValue == null ? (bool?) null : rawValue.Value == MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.DIGITAL_VALUE_ON ;
      }

      protected override CheckResult Check1 (InstanceData.InstanceData instanceData,
                                             string[] tags) {
         var logger = new TagLogger (tags) ;
         // var measurement = instanceData.InstanceState.GetChild (Strings.Properties.MEASUREMENT) ;
         var measuredData = instanceData.MeasuredData ;
         var channel1Value = measuredData.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.A0) ;
         // Convert value to volts
         double? channel1Volts = ConvertTo230Volts (channel1Value) ;

         // int? d1RawValue = measurement.AsIntegerNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Measurement.D1) ;
         // bool? d1Value = d1RawValue == null ? (bool?) null : d1RawValue.Value == MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.Measurement.DIGITAL_VALUE_ON ;

         int? d2RawValue = measuredData.AsIntegerNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.D2) ;
         int? d3RawValue = measuredData.AsIntegerNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.D3) ;

         logger.Debug ("Voltage 3-phase 230V check; " +
                       $"Channel 1: range {_channel1}, value {channel1Volts};" +
                       $"Channel 2 state: {_channel2.State}, value {d2RawValue}; " +
                       $"Channel 3 state: {_channel3.State}, value {d3RawValue}") ;

         var result = CheckResult.CreateUndefined (Name) ;

         // If nothing is configured, then return undefined
         if (!_channel1.IsConfigured &&
             !_channel2.IsConfigured &&
             !_channel3.IsConfigured) {
            return result ;
         }

         result.CheckResultKind = CheckResultKind.Success ;

         // Collect messages
         XStrings errorDetails = new XStrings() ;
         XStrings errorExtract = new XStrings() ;
         XStrings successDetails = new XStrings() ;
         XStrings successExtract = new XStrings() ;

         // Check items one by one
         // Channel 1
         if (_channel1.IsConfigured) {
            var resultA0 = CheckRange (_channel1, channel1Volts) ;
            if (resultA0 == HardwareCheckResult.Fail) {
               result.CheckResultKind = CheckResultKind.Fail ;
               errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.NamedMeasurementAlertDetails),
                                                      new XConstantString.Parameter (Strings.Parameters.DeviceName, _channel1.Name),
                                                      new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_channel1, Constants.UNIT_VOLTS)),
                                                      new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (channel1Volts, Constants.UNIT_VOLTS)))) ;
               errorExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                      new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_channel1, Constants.UNIT_VOLTS)),
                                                      new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (channel1Volts, Constants.UNIT_VOLTS)))) ;
            }

            if (resultA0 == HardwareCheckResult.Success) {
               successDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.NamedMeasurementOKDetails),
                                                        new XConstantString.Parameter (Strings.Parameters.DeviceName, _channel1.Name),
                                                        new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (channel1Volts, Constants.UNIT_VOLTS)))) ;
               successExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.NamedMeasurementOKExtract),
                                                        new XConstantString.Parameter (Strings.Parameters.DeviceName, _channel1.Name),
                                                        new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (channel1Volts, Constants.UNIT_VOLTS)))) ;
            }

            if (_isSensitive) {
               var valueMin = instanceData.MeasuredData.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.A0_MIN) ;
               var valueMax = instanceData.MeasuredData.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.A0_MAX) ;
               double? voltsMin = ConvertTo230Volts (valueMin) ;
               double? voltsMax = ConvertTo230Volts (valueMax) ;

               var minResult = CheckRange (_channel1, voltsMin) ;
               if (minResult == HardwareCheckResult.Fail) {
                  result.CheckResultKind = CheckResultKind.Fail ;
                  errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementSensitiveAlertDetails),
                                                         new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_channel1, Constants.UNIT_VOLTS)),
                                                         new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (voltsMin, Constants.UNIT_VOLTS)))) ;
                  errorExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                         new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_channel1, Constants.UNIT_VOLTS)),
                                                         new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (voltsMin, Constants.UNIT_VOLTS)))) ;
               }

               var maxResult = CheckRange (_channel1, voltsMax) ;
               if (maxResult == HardwareCheckResult.Fail) {
                  result.CheckResultKind = CheckResultKind.Fail ;
                  errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementSensitiveAlertDetails),
                                                         new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_channel1, Constants.UNIT_VOLTS)),
                                                         new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (voltsMax, Constants.UNIT_VOLTS)))) ;
                  errorExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                         new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_channel1, Constants.UNIT_VOLTS)),
                                                         new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (voltsMin, Constants.UNIT_VOLTS)))) ;
               }
            }
         }

         Check (_channel2, _isSensitive,
                ConvertDigitalValue (d2RawValue),
                ConvertDigitalValue (instanceData.MeasuredData.AsIntegerNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.D2_MIN)),
                ConvertDigitalValue (instanceData.MeasuredData.AsIntegerNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.D2_MAX)),
                result,
                successDetails,
                successExtract,
                errorDetails,
                errorExtract) ;

         Check (_channel3, _isSensitive,
                ConvertDigitalValue (d3RawValue),
                ConvertDigitalValue (instanceData.MeasuredData.AsIntegerNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.D3_MIN)),
                ConvertDigitalValue (instanceData.MeasuredData.AsIntegerNull (MeasuredDataConstants.VoltageLevel.Max230V.ThreeChannel.MeasuredDataProperties.D3_MAX)),
                result,
                successDetails,
                successExtract,
                errorDetails,
                errorExtract) ;

         if (result.CheckResultKind == CheckResultKind.Fail) {
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Details = errorDetails ;
            result.Extract = errorExtract ;
            return result ;
         } else if (result.CheckResultKind == CheckResultKind.Success) {
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Details = successDetails ;
            result.Extract = successExtract ;
            return result ;
         }

         return Undefined ;
      }
   }
}