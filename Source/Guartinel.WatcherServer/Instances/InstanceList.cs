using System ;
using System.Collections.Generic ;
using System.Linq ;

namespace Guartinel.WatcherServer.Instances {
   public class InstanceList : Dictionary<string, Instance> {
      private readonly object _lock = new object() ;

      public void Synchronize (IList<string> identifiers) {
         lock (_lock) {
            if (identifiers == null) {
               identifiers = new List<string>() ;
            }

            // System.Diagnostics.Debug.WriteLine ("instances synchronized.") ;

            // Remove all which are not in the new list
            var toDelete = Keys.Where (x => !identifiers.Contains (x)).ToList() ;
            toDelete.ForEach (x => {
               // System.Diagnostics.Debug.WriteLine ($"instance {x} removed.") ;
               Remove (x) ;
            }) ;

            // Add new ones
            var toAdd = identifiers.Where (x => !Keys.Contains (x)).ToList() ;
            toAdd.ForEach (x => {
               if (!Keys.Contains (x)) {
                  Add (x, new Instance (x)) ;
               }
            }) ;
         }
      }

      public void Add (string identifier) {
         if (Keys.Contains (identifier)) return ;

         List<string> keys = Keys.ToList() ;
         keys.Add (identifier) ;
         Synchronize (keys) ;
      }
   }
}