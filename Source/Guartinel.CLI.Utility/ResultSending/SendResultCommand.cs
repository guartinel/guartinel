using System ;
using System.Collections.Generic ;
using Fclp ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility.ResultSending {
   public class SendResultCommand : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string SUCCESS = "success";
            public const string SUCCESS_VALUE = "success";
            public const string ERROR_VALUE = "error";

            public const string MESSAGE = "message" ;
            public const string DATA = "data" ;
         }
      }

      public bool Success => _parameters [Constants.Parameters.SUCCESS]?.ToString() == Constants.Parameters.SUCCESS_VALUE ;
      public string Message => _parameters [Constants.Parameters.MESSAGE]?.ToString() ;
      public JObject Data => _parameters [Constants.Parameters.DATA] as JObject ;

      public override string Description => "Send result to server." ;
      public override string Command => "sendResult" ;

      protected override void Setup2 (FluentCommandLineParser commandLineParser) {
         SetupOption (commandLineParser, Constants.Parameters.SUCCESS) ;
         SetupOption (commandLineParser, Constants.Parameters.MESSAGE) ;

         // Add send parameter
         _parameters [SendResultCommandBase.Constants.Parameters.SEND] = true ;
      }

      protected override List<CheckResult> Run2() {
         return new List<CheckResult>() { new CheckResult (Success, Message, Data)} ;
      }
   }
}