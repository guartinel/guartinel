using System;
using System.Linq ;
using Guartinel.Kernel;
using Guartinel.Kernel.Configuration;
using Guartinel.Kernel.Logging;
using Guartinel.WatcherServer.CheckResults;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares;
using Strings = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors {
   public abstract class TemperatureSensor : HardwareSensor {
      public static class Constants {
         public const string UNIT_CELSIUS = "°C" ;
         public const int ERROR_VALUE = -1000 ;
      }

      protected override void Configure1 (ConfigurationData configuration) {
         var temperatureConfiguration = configuration.GetChild (MeasuredDataConstants.Temperature.Instance.TEMPERATURE_CELSIUS) ;
         if (temperatureConfiguration != null) {
            _temperatureRange = new HardwareCheckRange (configuration [MeasuredDataConstants.Temperature.Instance.NAME],
                                                        temperatureConfiguration.AsDoubleNull (MeasuredDataConstants.Temperature.Instance.TemperatureCelsius.MIN_THRESHOLD),
                                                        temperatureConfiguration.AsDoubleNull (MeasuredDataConstants.Temperature.Instance.TemperatureCelsius.MAX_THRESHOLD)) ;

            _temperatureWarningRange = new HardwareCheckRange (configuration [MeasuredDataConstants.Temperature.Instance.NAME],
                                                               temperatureConfiguration.AsDoubleNull (MeasuredDataConstants.Temperature.Instance.TemperatureCelsius.MIN_WARNING_THRESHOLD),
                                                               temperatureConfiguration.AsDoubleNull (MeasuredDataConstants.Temperature.Instance.TemperatureCelsius.MAX_WARNING_THRESHOLD)) ;
         }
      }

      protected HardwareCheckRange _temperatureRange ;
      protected HardwareCheckRange _temperatureWarningRange ;

      protected override CheckResult Check1 (InstanceData.InstanceData instanceData,
                                             string[] tags) {
         if (instanceData == null) return Undefined ;
         var measuredData = instanceData.MeasuredData ;

         var temperatureCelsius = measuredData.AsDoubleNull (MeasuredDataConstants.Temperature.Measurement.TEMPERATURE_CELSIUS) ;

         var result = CheckResult.CreateUndefined (Name) ;

         // Check for errors
         if ((temperatureCelsius != null && (int) temperatureCelsius.Value == Constants.ERROR_VALUE)) {

            result.CheckResultKind = CheckResultKind.Fail ;
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementErrorAlertDetails),
                                                   new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementErrorAlertExtract)) ;
            return result ;
         }

         // If not configured yet, then return undefined
         if (!_temperatureRange.IsConfigured) {
            return new CheckResult().Configure (Name, CheckResultKind.Fail,
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotConfiguredAlert),
                                                                     new XConstantString.Parameter (Strings.Parameters.InstanceName, instanceData.Name)),
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotConfiguredAlertDetails),
                                                                     new XConstantString.Parameter (Strings.Parameters.InstanceName, instanceData.Name)),
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotConfiguredAlertExtract))) ;
         }

         Logger.Log ($"Temperature check, range {_temperatureRange}, warning range {_temperatureWarningRange}, value {temperatureCelsius}.") ;
         var temperatureResult = CheckRange (_temperatureRange, temperatureCelsius) ;

         // Success
         if (temperatureResult == HardwareCheckResult.Success) {
            result.CheckResultKind = CheckResultKind.Success ;
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKDetails),
                                                   new XConstantString.Parameter (Strings.Parameters.MeasuredValue,
                                                                                  AsString (temperatureCelsius, Constants.UNIT_CELSIUS, true))) ;
            result.Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKExtract),
                                                  new XConstantString.Parameter (Strings.Parameters.MeasuredValue,
                                                                                 AsString (temperatureCelsius, Constants.UNIT_CELSIUS, false))) ;
            return result ;
         }

         // Failed, check if warning
         if (_temperatureWarningRange.IsConfigured) {
            var temperatureResultWarning = CheckRange (_temperatureWarningRange, temperatureCelsius) ;

            if (temperatureResultWarning == HardwareCheckResult.Success) {
               // Warning level
               result.CheckResultKind = CheckResultKind.Fail ;
               result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementWarningMessage),
                                                     new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
               result.Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertDetails),
                                                      new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_temperatureWarningRange, Constants.UNIT_CELSIUS, true)),
                                                      new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (temperatureCelsius, Constants.UNIT_CELSIUS, true))) ;
               result.Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKExtract),
                                                     new XConstantString.Parameter (Strings.Parameters.MeasuredValue,
                                                                                    AsString (temperatureCelsius, Constants.UNIT_CELSIUS, true))) ;

               return result ;
            }

            // Not warning, alert
            result.CheckResultKind = CheckResultKind.Fail ;
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertDetails),
                                                   new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_temperatureRange, Constants.UNIT_CELSIUS, true)),
                                                   new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (temperatureCelsius, Constants.UNIT_CELSIUS, true))) ;
            result.Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                  new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_temperatureWarningRange, Constants.UNIT_CELSIUS, true),
                                                                                 new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (temperatureCelsius, Constants.UNIT_CELSIUS, true)))) ;

            return result ;
         }

         // Fallback
         return Undefined ;
      }
   }

   public abstract class TemperatureAndHumiditySensor : TemperatureSensor {
      public new static class Constants {
         public const string UNIT_RELATIVE_HUMIDITY_PERCENT = "%" ;
      }

      protected override void Configure1 (ConfigurationData configuration) {
         base.Configure1 (configuration) ;

         var humidity = configuration.GetChild (MeasuredDataConstants.Temperature.Instance.RELATIVE_HUMIDITY_PERCENT) ;
         if (humidity != null) {
            _humidityRange = new HardwareCheckRange (configuration [MeasuredDataConstants.Temperature.Instance.NAME],
                                                     humidity.AsDoubleNull (MeasuredDataConstants.Temperature.Instance.RelativeHumidityPercent.MIN_THRESHOLD),
                                                     humidity.AsDoubleNull (MeasuredDataConstants.Temperature.Instance.RelativeHumidityPercent.MAX_THRESHOLD)) ;

            _humidityWarningRange = new HardwareCheckRange (configuration [MeasuredDataConstants.Temperature.Instance.NAME],
                                                            humidity.AsDoubleNull (MeasuredDataConstants.Temperature.Instance.RelativeHumidityPercent.MIN_WARNING_THRESHOLD),
                                                            humidity.AsDoubleNull (MeasuredDataConstants.Temperature.Instance.RelativeHumidityPercent.MAX_WARNING_THRESHOLD)) ;
         }
      }

      protected HardwareCheckRange _humidityRange ;
      protected HardwareCheckRange _humidityWarningRange ;

      protected sealed override CheckResult Check1 (InstanceData.InstanceData instanceData,
                                                    string[] tags) {
         if (instanceData == null) return Undefined ;
         var measuredData = instanceData.MeasuredData ;

         var temperatureCelsius = measuredData.AsDoubleNull (MeasuredDataConstants.Temperature.Measurement.TEMPERATURE_CELSIUS) ;
         var humidityPercent = measuredData.AsDoubleNull (MeasuredDataConstants.Temperature.Measurement.RELATIVE_HUMIDITY_PERCENT) ;

         var result = CheckResult.CreateUndefined (Name) ;

         // Check for errors
         if ((temperatureCelsius != null && (int) temperatureCelsius.Value == TemperatureSensor.Constants.ERROR_VALUE) ||
             (humidityPercent != null && (int) humidityPercent.Value == TemperatureSensor.Constants.ERROR_VALUE)) {

            result.CheckResultKind = CheckResultKind.Fail ;
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Details = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementErrorAlertDetails),
                                                   new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            result.Extract = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementErrorAlertDetails),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;
            return result ;
         }

         // If not configured yet, then return undefined
         if (!_temperatureRange.IsConfigured &&
             !_humidityRange.IsConfigured) {

            return new CheckResult().Configure (Name, CheckResultKind.Fail,
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotConfiguredAlert),
                                                                     new XConstantString.Parameter (Strings.Parameters.InstanceName, instanceData.Name)),
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotConfiguredAlertDetails),
                                                                     new XConstantString.Parameter (Strings.Parameters.InstanceName, instanceData.Name)),
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.InstanceNotConfiguredAlertExtract))) ;
         }

         Logger.Log ($"Temperature check, range {_temperatureRange}, warning range {_temperatureWarningRange} value {temperatureCelsius}.") ;
         Logger.Log ($"Humidity check, range {_humidityRange}, warning range {_humidityWarningRange} value {humidityPercent}.") ;

         var temperatureResult = CheckRange (_temperatureRange, temperatureCelsius) ;
         var humidityResult = CheckRange (_humidityRange, humidityPercent) ;

         // If any failed
         if (temperatureResult == HardwareCheckResult.Fail ||
             humidityResult == HardwareCheckResult.Fail) {

            result.CheckResultKind = CheckResultKind.Fail ;
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;

            // Collect messages
            XStrings details = new XStrings() ;
            XStrings extract = new XStrings() ;

            if (temperatureResult == HardwareCheckResult.Fail) {
               details.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertDetails),
                                                 new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_temperatureRange, TemperatureSensor.Constants.UNIT_CELSIUS, true)),
                                                 new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (temperatureCelsius, TemperatureSensor.Constants.UNIT_CELSIUS, true)))) ;
               extract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                 new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_temperatureRange, TemperatureSensor.Constants.UNIT_CELSIUS, true)),
                                                 new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (temperatureCelsius, TemperatureSensor.Constants.UNIT_CELSIUS, true))), false) ;
            }

            if (humidityResult == HardwareCheckResult.Fail) {
               details.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertDetailsRelativeHumidity),
                                                 new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_humidityRange, Constants.UNIT_RELATIVE_HUMIDITY_PERCENT)),
                                                 new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (humidityPercent, Constants.UNIT_RELATIVE_HUMIDITY_PERCENT)))) ;
               extract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                 new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_humidityRange, Constants.UNIT_RELATIVE_HUMIDITY_PERCENT)),
                                                 new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (humidityPercent, Constants.UNIT_RELATIVE_HUMIDITY_PERCENT))), false) ;
            }

            result.Details = details ;
            result.Extract = extract ;
            return result ;
         }

         // If all successful
         if ((!_temperatureRange.IsConfigured || temperatureResult == HardwareCheckResult.Success) &&
             (!_humidityRange.IsConfigured || humidityResult == HardwareCheckResult.Success)) {

            result.CheckResultKind = CheckResultKind.Success ;
            result.Message = new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKMessage),
                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)) ;

            // Collect details
            XStrings details = new XStrings() ;
            XStrings extract = new XStrings() ;

            if (_temperatureRange.IsConfigured) {
               details.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKDetails),
                                                 new XConstantString.Parameter (Strings.Parameters.MeasuredValue,
                                                                                AsString (temperatureCelsius, TemperatureSensor.Constants.UNIT_CELSIUS, true)))) ;
               extract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementExtractTemperature),
                                                 new XConstantString.Parameter (Strings.Parameters.MeasuredValue,
                                                                                AsString (temperatureCelsius, TemperatureSensor.Constants.UNIT_CELSIUS, true))), false) ;
            }

            if (_humidityRange.IsConfigured) {
               details.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKDetailsRelativeHumidity),
                                                 new XConstantString.Parameter (Strings.Parameters.MeasuredValue,
                                                                                AsString (humidityPercent, Constants.UNIT_RELATIVE_HUMIDITY_PERCENT)))) ;
               extract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementExtractRelativeHumidity),
                                                 new XConstantString.Parameter (Strings.Parameters.MeasuredValue,
                                                                                AsString (humidityPercent, Constants.UNIT_RELATIVE_HUMIDITY_PERCENT))), false) ;
            }

            result.Details = details ;
            result.Extract = extract ;

            return result ;
         }

         return Undefined ;
      }
   }

   public class TemperatureSensorDht11 : TemperatureAndHumiditySensor { }

   public class TemperatureSensorDht22 : TemperatureAndHumiditySensor { }

   public class TemperatureSensorDs8B20 : TemperatureSensor { }
}