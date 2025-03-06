using System;
using System.Collections.Generic ;
using Guartinel.CLI.Commands;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;

namespace Guartinel.CLI.ResultSending {
   public abstract class SendResultCommandBase : BaseCommand {
      public new static class Constants {
         public const string SUCCESS_VALUE = "success";
         public const string ERROR_VALUE = "error";

         public static class Parameters {
            public const string SEND = "send" ;
            public const string ADDRESS = "address" ;
            public const string TOKEN = "token" ;
            public const string INSTANCE_ID = "id" ;
            public const string INSTANCE_NAME = "name" ;
            public const string IS_HEARTBEAT = "heartbeat" ;
         }
      }

      public virtual bool Send => Parameters.GetBoolValue (Constants.Parameters.SEND) ;
      public string Address => Parameters [Constants.Parameters.ADDRESS]?.ToString() ;
      public string Token => Parameters [Constants.Parameters.TOKEN]?.ToString() ;
      public string InstanceID => Parameters [Constants.Parameters.INSTANCE_ID]?.ToString() ;
      public string InstanceName => Parameters [Constants.Parameters.INSTANCE_NAME]?.ToString() ;

      public bool IsHeartbeat => Parameters.GetBoolValue (Constants.Parameters.IS_HEARTBEAT) ;

      protected sealed override List<CheckResult> Run1() {
         List<CheckResult> checkResults = Run2() ;
         if (checkResults == null) return new List<CheckResult>() ;

         if (!Send) { return checkResults ; }

         if (string.IsNullOrEmpty (Address)) throw new Exception ("Address parameter is missing!") ;
         if (string.IsNullOrEmpty (Token)) throw new Exception ("Token parameter is missing!") ;
         if (string.IsNullOrEmpty (InstanceID)) throw new Exception ("ID parameter is missing!") ;
         if (string.IsNullOrEmpty (InstanceName)) throw new Exception ("Name parameter is missing!") ;

         foreach (CheckResult checkResult in checkResults) {
            // var name = string.IsNullOrEmpty (checkResult.Name) ? InstanceID : $"{InstanceID} - {checkResult.Name}" ;
            string name = checkResults.Count == 1 ? InstanceName : $"{InstanceName}-{checkResults.IndexOf(checkResult).ToString()}" ;
            string instanceID = checkResults.Count == 1 ? InstanceID : $"InstanceID-{checkResults.IndexOf(checkResult).ToString()}" ;
            Logger.Info ($"Instance name: {name}. Instance ID: {instanceID}. Details: {checkResults.Count}, {InstanceID}, {InstanceName}, {checkResult.Name}") ;
            IoC.Use.Single.GetInstance<IResultSender>().SendResult (Address, Token, instanceID, name, IsHeartbeat, checkResult) ;
            var successString = checkResult.Success ? Constants.SUCCESS_VALUE : Constants.ERROR_VALUE ;

            string message = $"Result {successString} sent to server." ;
            Logger.Log ($"{message} {checkResult}") ;
         }

         // return new CheckResult (true, message, null) ;
         return checkResults ;
      }

      protected abstract List<CheckResult> Run2() ;
   }

   public abstract class SendResultCommandBaseCl<T> : BaseCommandCl<T> where T : SendResultCommandBase, new() {
      public virtual bool Send => Parameters.GetBoolValue (SendResultCommandBase.Constants.Parameters.SEND) ;
      public string Address => Parameters [SendResultCommandBase.Constants.Parameters.ADDRESS]?.ToString() ;
      public string Token => Parameters [SendResultCommandBase.Constants.Parameters.TOKEN]?.ToString() ;
      public string InstanceID => Parameters [SendResultCommandBase.Constants.Parameters.INSTANCE_ID]?.ToString() ;
      public string InstanceName => Parameters [SendResultCommandBase.Constants.Parameters.INSTANCE_NAME]?.ToString() ;

      public bool IsHeartbeat => Parameters.GetBoolValue (SendResultCommandBase.Constants.Parameters.IS_HEARTBEAT) ;

      protected sealed override void Setup1 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, SendResultCommandBase.Constants.Parameters.ADDRESS, "Guartinel server address.") ;
         SetupOption (commandLineParser, SendResultCommandBase.Constants.Parameters.TOKEN, "Guartinel package token.") ;
         SetupOption (commandLineParser, SendResultCommandBase.Constants.Parameters.INSTANCE_ID, "Check instance unique identifier.") ;
         SetupOption (commandLineParser, SendResultCommandBase.Constants.Parameters.INSTANCE_NAME, "Check instance name.") ;

         SetupOptionBoolean (commandLineParser, SendResultCommandBase.Constants.Parameters.SEND, "Send results to server? By default, yes.") ;
         SetupOptionBoolean (commandLineParser, SendResultCommandBase.Constants.Parameters.IS_HEARTBEAT, "If set, it causes alert if no results sent to server in time.") ;

         Setup2 (commandLineParser) ;
      }

      protected abstract void Setup2 (CommandLineApplication commandLineParser) ;
   }
}