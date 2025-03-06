using System ;
using System.Diagnostics ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel.Utility {
   /// <summary>
   /// Class to store updating information.
   /// </summary>
   public static class StopwatchEx {
      /// <summary>
      /// Measure the time of an action.
      /// </summary>
      public static TimeSpan TimeIt (Action action) {
         Stopwatch stopwatch = new Stopwatch() ;
         stopwatch.Start() ;

         action?.Invoke() ;

         stopwatch.Stop() ;

         return stopwatch.Elapsed ;
      }

      public static double GetSeconds (this TimeSpan elapsed,
                                       int secondsRounding = 1) {
         return elapsed.Seconds + Math.Round (elapsed.AllMilliseconds() / 1000.0, secondsRounding) ;
      }
   }
}
