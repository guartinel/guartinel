using System ;
using System.Collections.Generic ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;

namespace Guartinel.CLI.Network {
   public class PingChecker : PingCheckerBase {
      public new static class Constants {
         public static class Parameters {
            public const string TARGET = "target" ;
            public const string CAPTION = "caption" ;
         }
      }

      public string Target => Parameters.GetStringValue (Constants.Parameters.TARGET) ;
      public string Caption => Parameters.GetStringValue (Constants.Parameters.CAPTION) ;

      public override string Description => $"Check ping of a target." ;

      public override string Command => $"ping" ;

      protected override List<CheckResult> Run2() {
         return new List<CheckResult> {Ping (new Host (Target, Caption), Retries, WaitTimeSeconds, TimeoutSeconds)} ;
      }
   }

   public class PingCheckerCl : PingCheckerBaseCl<PingChecker> {
      protected override void Setup3 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, PingChecker.Constants.Parameters.TARGET, $"Ping target as address{Pinger.Constants.PORT_SEPARATOR}port.") ;
      }
   }
}