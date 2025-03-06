using Guartinel.Kernel.Utility ;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor {
   public class HardwareCheckRange : IHardwareCheck {
      public HardwareCheckRange (string name,
                                 double? minValue,
                                 double? maxValue) {
         Name = name;
         MinValue = minValue ;
         MaxValue = maxValue;
      }

      public string Name { get; }
      public double? MinValue { get; }
      public double? MaxValue { get; }

      public bool IsConfigured => MinValue != null || MaxValue != null ;

      public HardwareCheckResult Check (double? thresholdValue) {
         if (MinValue == null &&
             MaxValue == null) return HardwareCheckResult.Undefined;
         if (thresholdValue == null) return HardwareCheckResult.Undefined;

         if (MinValue.HasValue && thresholdValue.Value < MinValue.Value) return HardwareCheckResult.Fail ;
         if (MaxValue.HasValue && thresholdValue.Value > MaxValue.Value) return HardwareCheckResult.Fail ;

         return HardwareCheckResult.Success ;
      }

      public override string ToString () {
         return $"{MinValue}-{MaxValue}";
      }
   }
}