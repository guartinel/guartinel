using System;
using System.Linq;
using System.Text;

namespace Guartinel.WatcherServer.Checkers {
   public class MeasuredData {
      public string MeasurementType {get ; set ;}
      public DateTime MeasurementDateTime {get ; set ;}
      public string Data {get ; set ;}
   }
}