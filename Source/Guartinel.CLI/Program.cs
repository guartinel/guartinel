using System;
using Guartinel.Kernel.Logging ;

// Consider using McMaster.Extensions.CommandLineUtils instead of the MS version
// Download .net Core https://www.microsoft.com/net/core#windowscmd
// RID Catalog https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
// Deploy .net Core apps with VS https://docs.microsoft.com/en-us/dotnet/core/deploying/deploy-with-vs

namespace Guartinel.CLI {
   public class Program {
      static int Main (string[] arguments) {

         if (System.Diagnostics.Debugger.IsAttached) {
            Logger.RegisterLogger<ConsoleLogger>() ;
         }

         int result = new Application().Run (arguments) ;

         if (System.Diagnostics.Debugger.IsAttached) {
            Console.ReadKey() ;
         }

         return result ;
      }
   }
}