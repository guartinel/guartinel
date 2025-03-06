using System ;
using System.Collections.Generic ;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel.Logging ;
using Microsoft.Extensions.CommandLineUtils ;

namespace Guartinel.CLI.Commands {
   public class HeartbeatCommand : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
         }

         public static class Results { }
      }

      public override string Description => $"Simple heartbeat command." ;

      public override string Command => SendResultCommandBase.Constants.Parameters.IS_HEARTBEAT;

      // protected override void Setup2 (CommandLineApplication commandLineParser) {}

      protected override List<CheckResult> Run2() {
         // Specify heartbeat
         Parameters [SendResultCommandBase.Constants.Parameters.IS_HEARTBEAT] = true ;

         //JObject checkResultDetails = new JObject() ;
         //checkResultDetails [Constants.Results.TARGET] = _target ;
         //checkResultDetails [Constants.Results.IP_ADDRESS] = pingResult.IPAddress ;
         //checkResultDetails [Constants.Results.PING_TIME_MS] = pingResult.RoundtripMilliseconds ;

         // CheckResult result = new CheckResult (true, string.Empty, checkResultDetails) ;
         CheckResult result = new CheckResult (true, string.Empty, string.Empty, String.Empty, null) ;

         //  Logger.DisableLogger<ConsoleLogger>() ;
         _logger.Info ($"Heartbeat check executed.") ;

         return new List<CheckResult> {result} ;
      }
   }

   public class HeartbeatCommandCl : SendResultCommandBaseCl<HeartbeatCommand> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
      }
   }
}