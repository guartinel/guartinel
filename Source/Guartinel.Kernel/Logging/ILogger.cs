using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace Guartinel.Kernel.Logging {
   public interface ILogger {
      List<string> Categories {get ;}

      void Log (LogLevel level,
                string message,
                List<string> categories) ;
   }
}