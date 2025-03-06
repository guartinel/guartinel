using System ;
using System.Collections.Generic ;
using Fclp ;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core ;
using Guartinel.Core.Logging ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility.Commands {
   public class HeartbeatCommand : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {}

         public static class Results {}
      }

      public override string Description => $"Simple heartbeat command." ;

      public override string Command => $"heartbeat" ;

      protected override void Setup2 (FluentCommandLineParser commandLineParser) {}

      protected override List<CheckResult> Run2() {
         // Specify heartbeat
         _parameters [SendResultCommandBase.Constants.Parameters.IS_HEARTBEAT] = true ;

         //JObject checkResultDetails = new JObject() ;
         //checkResultDetails [Constants.Results.TARGET] = _target ;
         //checkResultDetails [Constants.Results.IP_ADDRESS] = pingResult.IPAddress ;
         //checkResultDetails [Constants.Results.PING_TIME_MS] = pingResult.RoundtripMilliseconds ;

         // CheckResult result = new CheckResult (true, string.Empty, checkResultDetails) ;
         CheckResult result = new CheckResult (true, string.Empty, null) ;

         //  Logger.DisableLogger<ConsoleLogger>() ;
         Logger.Log (LogLevel.Info, $"Heartbeat check executed.") ;

         return new List<CheckResult> {result} ;
      }
   }
}