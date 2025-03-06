using System ;
using System.Collections.Generic ;
using Fclp;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core.Logging;
using Guartinel.Core.Network ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility.Network {
   public abstract class PingCheckerBase : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string RETRY_COUNT = "retries" ;
            public const string RETRY_WAIT_SECONDS = "wait_seconds" ;
         }

         public static class Results {
            public const string TARGET = "target" ;
            public const string IP_ADDRESS = "ip_address" ;
            public const string PING_TIME_MS = "ping_time_ms" ;
         }
      }

      protected int _retries = 4 ;
      protected int _waitTimeSeconds = 4 ;
      protected int _timeoutSeconds = 5 ;

      protected sealed override void Setup2 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<int> (Constants.Parameters.RETRY_COUNT).Callback (value => _retries = value) ;
         commandLineParser.Setup<int> (Constants.Parameters.RETRY_WAIT_SECONDS).Callback (value => _waitTimeSeconds = value) ;

         Setup3 (commandLineParser) ;
      }

      protected virtual void Setup3 (FluentCommandLineParser commandLineParser) {}

      protected CheckResult Ping (string target) {
         PingResult pingResult = new Pinger().Ping (target, _retries, _waitTimeSeconds, _timeoutSeconds, message => Logger.Log (LogLevel.Info, message)) ;

         JObject checkResultDetails = new JObject() ;
         checkResultDetails [Constants.Results.TARGET] = target ;
         checkResultDetails [Constants.Results.IP_ADDRESS] = pingResult.Address ;
         checkResultDetails [Constants.Results.PING_TIME_MS] = pingResult.RoundtripMilliseconds ;

         CheckResult result = new CheckResult (pingResult.Success == PingSuccess.Success,
                                               pingResult.Message.ToString(),
                                               checkResultDetails) ;

         //  Logger.DisableLogger<ConsoleLogger>() ;
         Logger.Log (LogLevel.Info, $"Ping check. Target: {target}, retries: {_retries}, wait time: {_waitTimeSeconds} seconds. Result: {result}") ;

         return result ;
      }
   }

   public class PingChecker : PingCheckerBase {
      public new static class Constants {
         public static class Parameters {
            public const string TARGET = "target" ;
         }
      }

      private string _target ;

      public override string Description => $"Check ping of a target." ;

      public override string Command => $"ping" ;

      protected override void Setup3 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<string> (Constants.Parameters.TARGET).Required().Callback (value => _target = value) ;
      }

      protected override List<CheckResult> Run2() {
         return new List<CheckResult> {Ping (_target)} ;
      }
   }
}