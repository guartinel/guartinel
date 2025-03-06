using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel.Logging {
   public class NullLogger : ILogger {
      public List<string> Categories => new List<string>() ;

      public void Log (LogLevel level,
                       string message,
                       List<string> categories) {
         // Nothing to do
      }
   }
}
