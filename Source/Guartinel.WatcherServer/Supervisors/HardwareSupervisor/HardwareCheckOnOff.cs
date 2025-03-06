using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor {
   public enum HardwareCheckBoolean {
      Any,
      On,
      Off
   }

   public class HardwareCheckOnOff : IHardwareCheck {
      public HardwareCheckOnOff (string name,
                                 HardwareCheckBoolean state) {
         Name = name ;
         State = state;
      }

      public string Name { get; }
      public HardwareCheckBoolean? State {get ;}

      public bool IsConfigured => (State ?? HardwareCheckBoolean.Any) != HardwareCheckBoolean.Any ;

      public HardwareCheckResult Check (bool? value) {
         // If no expected state, then OK!
         if (State == null) return HardwareCheckResult.Success;
         if (State == HardwareCheckBoolean.Any) return HardwareCheckResult.Success;

         if (value == null) return HardwareCheckResult.Undefined ;

         switch (State) {
            case HardwareCheckBoolean.On:
               return value.Value ? HardwareCheckResult.Success : HardwareCheckResult.Fail ;

            case HardwareCheckBoolean.Off:
               return value.Value ? HardwareCheckResult.Fail : HardwareCheckResult.Success ;
         }

         return HardwareCheckResult.Undefined ;
      }

      public override string ToString() {
         if (State == null) return "undefined" ;

         return State.Value.ToString().ToLowerInvariant() ;
      }
   }
}