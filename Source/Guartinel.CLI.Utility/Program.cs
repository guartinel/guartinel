using System;

namespace Guartinel.CLI.Utility {
   internal class Program {
      private static int Main (string[] arguments) {
         Application application = new Application (arguments) ;
         int result = application.Run();

         if (System.Diagnostics.Debugger.IsAttached) {
            Console.ReadKey() ;
         }

         return result ;
      }
   }
}