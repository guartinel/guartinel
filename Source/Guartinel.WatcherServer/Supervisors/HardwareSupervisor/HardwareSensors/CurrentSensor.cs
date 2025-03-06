using System;
using System.Linq ;
using Guartinel.Kernel;
using Guartinel.Kernel.Configuration;
using Guartinel.WatcherServer.CheckResults;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares;
using Strings = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors {
   public abstract class CurrentSensor : SensitiveHardwareSensor {
      public static class Constants {
         public const string UNIT_AMPERS = "A" ;
         public const double MEASUREMENT_LIMIT = 0.25 ;
      }

      protected override void Configure2 (ConfigurationData configuration) {
         _range = new HardwareCheckRange (configuration [MeasuredDataConstants.CurrentLevel.InstanceProperties.NAME],
                                          configuration.AsDoubleNull (MeasuredDataConstants.CurrentLevel.InstanceProperties.MIN_THRESHOLD),
                                          configuration.AsDoubleNull (MeasuredDataConstants.CurrentLevel.InstanceProperties.MAX_THRESHOLD)) ;
      }

      protected HardwareCheckRange _range ;

      protected override CheckResult Check1 (InstanceData.InstanceData instanceData,
                                             string[] tags) {
         var value = instanceData.MeasuredData.AsDoubleNull (MeasuredDataConstants.CurrentLevel.MeasuredDataProperties.CURRENT_A) ;
         if (value == null) return Undefined ;

         // Convert value to ampers
         var realValue = ConvertToAmpers (value.Value) ;
         var result = CheckRange (_range, realValue) ;

         switch (result) {
            case HardwareCheckResult.Success:
               return new CheckResult().Configure (Name, CheckResultKind.Success,
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKMessage),
                                                                        new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)),
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementOKDetails),
                                                                        new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (realValue, Constants.UNIT_AMPERS))),
                                                   new XConstantString(Strings.Use.Get(Strings.Messages.Use.MeasurementOKExtract),
                                                                       new XConstantString.Parameter(Strings.Parameters.MeasuredValue, AsString(realValue, Constants.UNIT_AMPERS)))) ;
            case HardwareCheckResult.Fail:
               return new CheckResult().Configure (Name, CheckResultKind.Fail,
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertMessage),
                                                                        new XConstantString.Parameter (Strings.Parameters.InstanceName, Name)),
                                                   new XConstantString (Strings.Use.Get (Strings.Messages.Use.MeasurementAlertDetails),
                                                                        new XConstantString.Parameter (Strings.Parameters.ReferenceValue, AsString (_range, Constants.UNIT_AMPERS)),
                                                                        new XConstantString.Parameter (Strings.Parameters.MeasuredValue, AsString (realValue, Constants.UNIT_AMPERS))),
                                                   new XConstantString(Strings.Use.Get(Strings.Messages.Use.MeasurementAlertExtract),
                                                                       new XConstantString.Parameter(Strings.Parameters.ReferenceValue, AsString(_range, Constants.UNIT_AMPERS)),
                                                                       new XConstantString.Parameter(Strings.Parameters.MeasuredValue, AsString(realValue, Constants.UNIT_AMPERS)))) ;
         }

         return Undefined ;
      }

      protected double ConvertToAmpers (double value) {
         // Small value should be handled as zero
         if (value < Constants.MEASUREMENT_LIMIT) return 0.0 ;

         // No conversion needed
         return value ;

      }
   }

   public class CurrentChecker30A : CurrentSensor {
   }

   public class CurrentChecker100A : CurrentSensor {
   }
}