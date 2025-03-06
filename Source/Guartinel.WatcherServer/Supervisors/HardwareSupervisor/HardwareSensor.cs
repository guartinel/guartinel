using System;
using System.Collections.Generic ;
using System.Linq ;
using Guartinel.Kernel;
using Guartinel.Kernel.Configuration;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.CheckResults;
using Guartinel.WatcherServer.InstanceData ;
using SaveConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.WatcherServerRoutes.Save.Request;
using MeasuredDataConstants = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor {
   public abstract class HardwareSensor {
      public string ID {get ; protected set ;}
      public string Name {get ; protected set ;}

      public static HardwareSensor CreateInstance (ConfigurationData configuration) {
         var type = configuration.AsString (SaveConstants.Configuration.Instance.HARDWARE_TYPE,
                                            configuration [SaveConstants.Configuration.Instance.TYPE]) ;

         var hardwareInstance = IoC.Use.Multi.GetInstance<HardwareSensor> (type) ;
         hardwareInstance.Configure (configuration) ;
         return hardwareInstance ;
      }

      protected HardwareSensor Configure (ConfigurationData configuration) {
         ID = configuration.AsString (SaveConstants.Configuration.Instance.INSTANCE_ID,
                                      configuration [SaveConstants.Configuration.Instance.ID]) ;

         Name = configuration [SaveConstants.Configuration.Instance.NAME] ;

         Configure1 (configuration) ;

         return this ;
      }

      protected abstract void Configure1 (ConfigurationData configuration) ;

      public CheckResult Check (InstanceData.InstanceData instanceData,
                                string[] tags) {
         // if (instanceDataList == null || !instanceDataList.Any()) return new CheckResult().Configure (Name, CheckResultSuccess.Undefined) ;
         return Check1 (instanceData, tags) ;
      }

      protected abstract CheckResult Check1 (InstanceData.InstanceData instanceData,
                                             string[] tags) ;

      protected CheckResult Undefined => CheckResult.CreateUndefined (Name) ;
      public virtual InstanceDataListCheckKind CheckKind => InstanceDataListCheckKind.FailIfLastFails ;

      // protected CheckResultSuccess CheckThreshold (HardwareCheckThreshold threshold,
      //                                             double? value) {

      //   // If no expected value, then we are OK
      //   if (threshold == null) return CheckResultSuccess.Success ;
      //   if (threshold.Value == null) return CheckResultSuccess.Success ;
      //   if (value == null) return CheckResultSuccess.Undefined ;

      //   return threshold.Check (value) ;
      //}

      protected HardwareCheckResult CheckRange (HardwareCheckRange range,
                                                double? value) {
         // If no expected value, then we are OK
         if (range == null) return HardwareCheckResult.Success ;
         if (value == null) return HardwareCheckResult.Undefined ;

         return range.Check (value) ;
      }

      protected static string AsString (double? value,
                                        string unit,
                                        bool separateUnitWithSpace = false) {
         if (value == null) return String.Empty ;
         var space = separateUnitWithSpace ? " " : String.Empty ;
         return $"{Math.Round (value.Value, 2)}{space}{unit}" ;
      }

      //protected string AsString (HardwareCheckThreshold threshold,
      //                           string unit,
      //                           bool separateUnitWithSpace = false) {

      //   if (threshold.Value == null) return String.Empty ;
      //   var space = separateUnitWithSpace ? " " : String.Empty ;
      //   return $"{threshold.Type.ToString().ToLowerInvariant()} {AsString (Math.Round (threshold.Value.Value, 2), unit, separateUnitWithSpace)}" ;
      //}

      protected static string AsString (HardwareCheckRange range,
                                        string unit,
                                        bool separateUnitWithSpace = false) {

         if (range == null) return String.Empty ;
         var space = separateUnitWithSpace ? " " : String.Empty ;
         if (range.MinValue == null && range.MaxValue == null) return String.Empty ;

         var minValue = AsString (range.MinValue, unit, separateUnitWithSpace) ;
         var maxValue = AsString (range.MaxValue, unit, separateUnitWithSpace) ;

         if (range.MinValue != null && range.MaxValue != null) return $"{minValue} - {maxValue}" ;
         if (range.MinValue != null) return $"> {minValue}" ;
         if (range.MaxValue != null) return $"< {maxValue}" ;
         return string.Empty ;
      }

      protected static string AsString (bool? onOff) {
         if (onOff == null) return "undefined" ;

         return onOff.Value ? "on" : "off" ;
      }
   }

   public abstract class SensitiveHardwareSensor : HardwareSensor {
      // Sensitive means that alert happens if the value goes out of range even for a short time
      protected bool _isSensitive = false ;

      public override InstanceDataListCheckKind CheckKind => _isSensitive ? InstanceDataListCheckKind.FailIfAnyFails : InstanceDataListCheckKind.FailIfEveryOneFails ;

      protected sealed override void Configure1 (ConfigurationData configuration) {
         _isSensitive = configuration.AsBoolean (MeasuredDataConstants.CurrentLevel.InstanceProperties.CHECK_MIN_MAX) ;
         Configure2 (configuration) ;
      }

      protected abstract void Configure2 (ConfigurationData configuration) ;
   }
}