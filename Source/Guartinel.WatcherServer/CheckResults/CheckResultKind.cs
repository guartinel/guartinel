using System ;
using System.Linq ;
using System.Text ;

namespace Guartinel.WatcherServer.CheckResults {
   public enum CheckResultKind {
      Undefined = 0,
      Success = 1,
      Fail = 2,
      WarningFail = 3,
      CriticalFail = 4
   }
}