using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace Guartinel.WatcherServer.Instances {
   public class InstanceStateList : Dictionary<string, InstanceState> {
      public InstanceStateList() {}

      public InstanceStateList (Dictionary<string, InstanceState> source) {
         foreach (var sourceItem in source) {
            Add (sourceItem.Key, sourceItem.Value) ;
         }         
      }
      
      public override string ToString() {
         return Kernel.Utility.StringEx.Concat (this.Select (x => $"{x.Key}: {x.Value.Name}"), Environment.NewLine) ;
      }
   }
}
