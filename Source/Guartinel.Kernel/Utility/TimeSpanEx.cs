using System;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel.Utility {
   public static class TimeSpanEx {
      public static int AllMilliseconds (this TimeSpan timeSpan) {
         return (int) (timeSpan.TotalMilliseconds) ;
      }
   }
}