using Guartinel.Kernel.Utility ;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor {
   public enum HardwareCheckThresholdType {
      Any,
      Minimum,
      Maximum
   }

   public class HardwareCheckThreshold {
      public HardwareCheckThreshold (string name, 
                                     double? value,
                                     HardwareCheckThresholdType type) {
         Name = name ;
         Value = value ;
         Type = type ;
      }

      public HardwareCheckThreshold (string name,
                                     double? value,
                                     string type) : this (name, value, EnumEx.Parse (type, HardwareCheckThresholdType.Any)) {
      }      
      public string Name { get; }
      public double? Value {get ;}
      public HardwareCheckThresholdType Type {get ;}

      public HardwareCheckResult Check (double? thresholdValue) {
         if (Value == null) return HardwareCheckResult.Undefined ;
         if (thresholdValue == null) return HardwareCheckResult.Undefined ;

         switch (Type) {
            case HardwareCheckThresholdType.Any:
               return HardwareCheckResult.Success ;

            case HardwareCheckThresholdType.Minimum:
               return thresholdValue.Value >= Value ? HardwareCheckResult.Success : HardwareCheckResult.Fail ;

            case HardwareCheckThresholdType.Maximum:
               return thresholdValue.Value <= Value ? HardwareCheckResult.Success : HardwareCheckResult.Fail ;
         }

         return HardwareCheckResult.Undefined ;
      }

      public override string ToString() {
         return $"{Type.ToString().ToLowerInvariant()} {Value}" ;
      }
   }
}