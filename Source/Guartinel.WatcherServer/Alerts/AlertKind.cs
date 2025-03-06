using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.WatcherServer.Alerts {
   public enum AlertKind {
      None,      
      Alert,
      Warning,
      Critical,
      Recovery
   }
}
