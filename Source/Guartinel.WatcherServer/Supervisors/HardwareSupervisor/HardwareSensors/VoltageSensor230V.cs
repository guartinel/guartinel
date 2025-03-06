using System;
using System.Linq ;
using Guartinel.Kernel;
using Guartinel.Kernel.Configuration;
using Guartinel.Kernel.Logging;
using Guartinel.WatcherServer.CheckResults;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares;
using Strings = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors {
   public class VoltageSensor230V : VoltageSensor {
      protected override void Configure2 (ConfigurationData configuration) {
         _range = new HardwareCheckRange (configuration [MeasuredDataConstants.VoltageLevel.Max230V.OneChannel.InstanceProperties.NAME],
                                          configuration.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.OneChannel.InstanceProperties.MIN_THRESHOLD),
                                          configuration.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.OneChannel.InstanceProperties.MAX_THRESHOLD)) ;
      }

      protected HardwareCheckRange _range ;

      protected override CheckResult Check1 (InstanceData.InstanceData instanceData,
                                             string[] tags) {
         var logger = new TagLogger(tags);
         var value = instanceData.MeasuredData.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.OneChannel.MeasuredDataProperties.A0) ;

         // Convert value to volts
         double? volts = ConvertTo230Volts (value) ;
         logger.Debug ($"Voltage 230V check, range {_range}, value {volts}.") ;

         var result = CheckRange (_range, volts) ;
         // If fail, then return
         if (result == HardwareCheckResult.Fail) {
            return new CheckResult().Configure (Name, CheckResultKind.Fail,
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage),
                                                                     new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)),
                                                new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertDetails),
                                                                     new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_range, Constants.UNIT_VOLTS)),
                                                                     new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (volts, Constants.UNIT_VOLTS))),
                                                new XConstantString(Strings.Use.Get(Strings.Messages.Use.MeasurementAlertExtract),
                                                                    new XConstantString.Parameter(Strings.Parameters.ReferenceValue, AsString(_range, Constants.UNIT_VOLTS)),
                                                                    new XConstantString.Parameter(Strings.Parameters.MeasuredValue, AsString(volts, Constants.UNIT_VOLTS)))) ;
         }

         XStrings errorDetails = new XStrings() ;
         XStrings errorExtract = new XStrings();

         if (_isSensitive) {
            var valueMin = instanceData.MeasuredData.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.OneChannel.MeasuredDataProperties.A0_MIN) ;
            var valueMax = instanceData.MeasuredData.AsDoubleNull (MeasuredDataConstants.VoltageLevel.Max230V.OneChannel.MeasuredDataProperties.A0_MAX) ;
            double? voltsMin = ConvertTo230Volts (valueMin) ;
            double? voltsMax = ConvertTo230Volts (valueMax) ;

            var minResult = CheckRange (_range, voltsMin) ;
            if (minResult == HardwareCheckResult.Fail) {
               errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementSensitiveAlertDetails),
                                                      new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_range, Constants.UNIT_VOLTS)),
                                                      new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (voltsMin, Constants.UNIT_VOLTS)))) ;
               errorExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                      new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_range, Constants.UNIT_VOLTS)),
                                                      new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (voltsMin, Constants.UNIT_VOLTS)))) ;
            }

            var maxResult = CheckRange (_range, voltsMax) ;
            if (maxResult == HardwareCheckResult.Fail) {
               errorDetails.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementSensitiveAlertDetails),
                                                      new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_range, Constants.UNIT_VOLTS)),
                                                      new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (voltsMax, Constants.UNIT_VOLTS)))) ;
               errorExtract.Add (new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertExtract),
                                                      new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_range, Constants.UNIT_VOLTS)),
                                                      new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (voltsMax, Constants.UNIT_VOLTS)))) ;
            }
         }

         if (errorDetails.IsEmpty) {
            if (result == HardwareCheckResult.Success) {
               return new CheckResult().Configure (Name, CheckResultKind.Success,
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKMessage),
                                                                        new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)),
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKDetails),
                                                                        new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (volts, Constants.UNIT_VOLTS))),
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKExtract),
                                                                        new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (volts, Constants.UNIT_VOLTS)))) ;
            }

            return Undefined ;
         }

         return new CheckResult().Configure (Name, CheckResultKind.Fail,
                                             new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage),
                                                                  new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)),
                                             errorDetails,
                                             errorExtract) ;
      }
   }
}