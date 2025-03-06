using System ;
using System.Collections.Generic ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.ResultSending {
   public class SendResultCommand : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string SUCCESS = "success" ;
            public const string SUCCESS_VALUE = "success" ;
            public const string ERROR_VALUE = "error" ;

            public const string MESSAGE = "message" ;
            public const string DETAILS = "details" ;
            public const string EXTRACT = "extract" ;
            public const string DATA = "data" ;
         }
      }

      public override bool Send => true ;
      public bool Success => Parameters [Constants.Parameters.SUCCESS]?.ToString() == Constants.Parameters.SUCCESS_VALUE ;
      public string Message => Parameters [Constants.Parameters.MESSAGE]?.ToString() ;
      public string Details => Parameters [Constants.Parameters.DETAILS]?.ToString() ;
      public string Extract => Parameters [Constants.Parameters.EXTRACT]?.ToString() ;
      public JObject Data => Parameters [Constants.Parameters.DATA] as JObject ;

      public override string Description => "Send result to server." ;
      public override string Command => "sendResult" ;

      protected override List<CheckResult> Run2() {
         return new List<CheckResult>() {new CheckResult (Success, Message, Details, Extract, Data)} ;
      }
   }

   public class SendResultCommandCl : SendResultCommandBaseCl<SendResultCommand> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, SendResultCommand.Constants.Parameters.SUCCESS, "Set success value of check: success or error.") ;
         SetupOption (commandLineParser, SendResultCommand.Constants.Parameters.MESSAGE, "Text message of the check.") ;
         SetupOption (commandLineParser, SendResultCommand.Constants.Parameters.DETAILS, "Optional: detailed text message of the check.") ;
         SetupOption (commandLineParser, SendResultCommand.Constants.Parameters.EXTRACT, "Optional: extracted message of the check.") ;

         // Add send parameter
         // _parameters [SendResultCommandBase.Constants.Parameters.SEND] = true ;
      }
   }
}