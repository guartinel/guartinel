using System;
using System.Linq ;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors {

   public abstract class VoltageSensor : SensitiveHardwareSensor {
      public static class Constants {
         public const string UNIT_VOLTS = "V" ;
         public const double MEASURED_DATA_LIMIT = 100.0 ;
         public const int REFERENCE_VOLTAGE = 240 ;
      }

      protected double? ConvertTo230Volts (double? value) {
         if (value == null) return null ;

         // Small value should be handled as zero
         if (value < Constants.MEASURED_DATA_LIMIT) return 0.0 ;

         // return Math.Round (value.Value / (double) 1023 * (double) 230) ;
         // Empyrical value: ~960 is reported when 240V is present
         return Math.Round (value.Value / 960 * Constants.REFERENCE_VOLTAGE) ;
      }
   }
}