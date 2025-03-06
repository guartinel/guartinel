using System;
using System.Collections.Generic ;
using Fclp;
using Guartinel. CLI. Utility. Commands;
using Guartinel. Core;
using Guartinel. Core. Logging;
using Guartinel. Core. Utility;

namespace Guartinel.CLI.Utility.ResultSending {
   public abstract class SendResultCommandBase : BaseCommand {
      public new static class Constants {
         public static class Parameters {
            public const string SEND = "send" ;
            public const string ADDRESS = "address" ;
            public const string TOKEN = "token" ;
            public const string INSTANCE_ID = "id" ;
            public const string INSTANCE_NAME = "name" ;
            public const string IS_HEARTBEAT = "heartbeat" ;
         }

         public const string SUCCESS_VALUE = "success" ;
         public const string ERROR_VALUE = "error" ;
      }

      public bool Send => _parameters.GetBoolValue (Constants.Parameters.SEND) ;
      public string Address => _parameters [Constants.Parameters.ADDRESS]?.ToString() ;
      public string Token => _parameters [Constants.Parameters.TOKEN]?.ToString() ;
      public string InstanceID => _parameters [Constants.Parameters.INSTANCE_ID]?.ToString() ;
      public string InstanceName => _parameters [Constants.Parameters.INSTANCE_NAME]?.ToString() ;

      public bool IsHeartbeat => _parameters.GetBoolValue (Constants.Parameters.IS_HEARTBEAT) ;

      protected override sealed void Setup1 (FluentCommandLineParser commandLineParser) {
         SetupOptionBool (commandLineParser, Constants.Parameters.SEND) ;
         SetupOptionBool (commandLineParser, Constants.Parameters.IS_HEARTBEAT) ;

         SetupOption (commandLineParser, Constants.Parameters.ADDRESS) ;
         SetupOption (commandLineParser, Constants.Parameters.TOKEN) ;
         SetupOption (commandLineParser, Constants.Parameters.INSTANCE_ID) ;
         SetupOption (commandLineParser, Constants.Parameters.INSTANCE_NAME) ;

         Setup2 (commandLineParser) ;
      }

      protected abstract void Setup2 (FluentCommandLineParser commandLineParser) ;

      protected sealed override List<CheckResult> Run1() {
         var checkResults = Run2() ;
         if (checkResults == null) return new List<CheckResult>() ;

         if (!Send) return checkResults ;
         if (string.IsNullOrEmpty (Address)) throw new Exception ("Address parameter is missing!") ;
         if (string.IsNullOrEmpty (Token)) throw new Exception ("Token parameter is missing!") ;
         if (string.IsNullOrEmpty (InstanceID)) throw new Exception ("ID parameter is missing!") ;
         if (string.IsNullOrEmpty (InstanceName)) throw new Exception ("Name parameter is missing!") ;

         foreach (var checkResult in checkResults) {
            IoC.Use.GetInstance<IResultSender>().SendResult(Address, Token, InstanceID, InstanceName, IsHeartbeat, checkResult);
            var successString = checkResult.Success ? Constants.SUCCESS_VALUE : Constants.ERROR_VALUE;

            string message = $"Result {successString} sent to server.";
            Logger.Log($"{message} {checkResult}");
         }
         // return new CheckResult (true, message, null) ;
         return checkResults ;
      }

      protected abstract List<CheckResult> Run2() ;
   }
}