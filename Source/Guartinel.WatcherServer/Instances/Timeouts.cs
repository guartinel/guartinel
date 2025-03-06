using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;

namespace Guartinel.WatcherServer.Instances {
   public class Timeouts : Dictionary<string, Timeout> {
      private readonly object _lock = new object() ;
      private int TimeoutSeconds {get ; set ;}

      public void Configure (int checkIntervalSeconds,
                             int timeoutIntervalSeconds) {
         lock (_lock) {
            TimeoutSeconds = timeoutIntervalSeconds ;
         }
      }

      public Timeout Ensure (string instanceID) {
         lock (_lock) {
            if (!ContainsKey (instanceID)) {
               var timeoutName = Guid.NewGuid().ToString() ;
               // Logger.Log ($"There is no timeout for instance {instanceID}, creating a new one: {timeoutName}.", nameof (Timeout)) ;
               Add (instanceID, new Timeout (TimeSpan.FromSeconds (TimeoutSeconds), timeoutName)) ;
            }

            return this [instanceID] ;
         }
      }

      //public Timeout Get (string instanceID) {
      //   lock (_lock) {
      //      //if (!this.ContainsKey (instanceID)) {
      //      //   var timeoutName = Guid.NewGuid().ToString() ;
      //      //   // Logger.Log ($"There is no timeout for instance {instanceID}, creating a new one: {timeoutName}.", nameof (Timeout)) ;
      //      //   this.Add (instanceID, new Timeout(TimeoutSeconds, timeoutName)) ;
      //      //}
      //      //return this [instanceID] ;

      //      return !ContainsKey (instanceID) ? null : this [instanceID] ;
      //   }
      //}
   }
}