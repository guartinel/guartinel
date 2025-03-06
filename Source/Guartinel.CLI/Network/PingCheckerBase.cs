using System ;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Network {
   public abstract class PingCheckerBase : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            [Obsolete]
            public const string RETRY_COUNT = "retries" ;
            public const string TRY_COUNT = "try_count" ;
            [Obsolete]
            public const string RETRY_WAIT_SECONDS = "wait_seconds" ;
            public const string RETRY_WAIT_SECONDS1 = "retry_wait_seconds" ;
         }

         public static class Defaults {
            public const int TRY_COUNT = 4 ;
            public const int RETRY_WAIT_SECONDS = 3 ;
            public const int TIMEOUT_SECONDS = 5 ;
         }

         public static class Results {
            public const string TARGET = "target" ;
            public const string IP_ADDRESS = "ip_address" ;
            public const string PING_TIME_MS = "ping_time_ms" ;
         }
      }

      public int Retries => Parameters.GetIntegerValue (Constants.Parameters.TRY_COUNT, Parameters.GetIntegerValue(Constants.Parameters.RETRY_COUNT, Constants.Defaults.TRY_COUNT)) ;
      public int WaitTimeSeconds => Parameters.GetIntegerValue (Constants.Parameters.RETRY_WAIT_SECONDS1, Parameters.GetIntegerValue(Constants.Parameters.RETRY_WAIT_SECONDS, Constants.Defaults.RETRY_WAIT_SECONDS)) ;

      public int TimeoutSeconds => Constants.Defaults.TIMEOUT_SECONDS ;

      public CheckResult Ping (Host target,
                                      int tryCount = Constants.Defaults.TRY_COUNT,
                                      int retryWaitSeconds = Constants.Defaults.RETRY_WAIT_SECONDS,
                                      int timeoutSeconds = Constants.Defaults.TIMEOUT_SECONDS) {
         PingResult pingResult = new Pinger().Ping (target, tryCount, false,
                                                    retryWaitSeconds, timeoutSeconds,
                                                    message => _logger.Error (message)) ;

         JObject checkResultDetails = new JObject() ;
         checkResultDetails [Constants.Results.TARGET] = target.DisplayText ;
         checkResultDetails [Constants.Results.IP_ADDRESS] = pingResult.Host ;
         checkResultDetails [Constants.Results.PING_TIME_MS] = pingResult.RoundtripMilliseconds ;

         CheckResult result = new CheckResult (pingResult.Success == PingSuccess.Success,
                                               pingResult.Message.ToString(),
                                               pingResult.Details.ToString(),
                                               String.Empty,
                                               checkResultDetails) ;

         //  Logger.DisableLogger<ConsoleLogger>() ;
         _logger.Info ($"Ping check. Target: {target.DisplayText}, tries: {tryCount}, retry wait time: {retryWaitSeconds} seconds. Result: {result}") ;

         return result ;
      }
   }

   public abstract class PingCheckerBaseCl<T> : SendResultCommandBaseCl<T> where T : PingCheckerBase, new() {
      protected sealed override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, PingCheckerBase.Constants.Parameters.RETRY_COUNT, "Obsolete: Retry count if the ping is not successful.") ;
         SetupOption(commandLineParser, PingCheckerBase.Constants.Parameters.TRY_COUNT, "Retry count if the ping is not successful.");
         SetupOption (commandLineParser, PingCheckerBase.Constants.Parameters.RETRY_WAIT_SECONDS, "Obsolete: Wait in seconds between two retry attempts.") ;
         SetupOption(commandLineParser, PingCheckerBase.Constants.Parameters.RETRY_WAIT_SECONDS1, "Wait in seconds between two retry attempts.");

         Setup3 (commandLineParser) ;
      }

      protected abstract void Setup3 (CommandLineApplication commandLineParser) ;
   }
}