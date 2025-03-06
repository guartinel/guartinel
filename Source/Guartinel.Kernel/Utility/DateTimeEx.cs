using System;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel.Utility {
   public static class DateTimeEx {
      public static DateTime TruncateMilliSeconds (this DateTime dateTime) {

         return dateTime.AddTicks (-(dateTime.Ticks % TimeSpan.FromSeconds (1).Ticks)) ;
      }
   }
}