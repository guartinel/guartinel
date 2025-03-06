using System;
using System.Collections.Generic ;

namespace Guartinel.Kernel.Logging {
   public class ConsoleLogger : LoggerBase, ILogger {
      public ConsoleLogger (List<string> categories) : base (categories) {}

      public ConsoleLogger(string category) : base (category) { }
      public ConsoleLogger() : this ((List<string>) null) {}

      protected override void DoLog (string timeStamp,
                                     LogLevel level,
                                     string message) {
         Console.WriteLine (FormatLine (timeStamp, level, message)) ;
      }
   }

   public class SimpleConsoleLogger : ConsoleLogger {
      public SimpleConsoleLogger() {}
   }
}