using System ;
using System.Collections.Generic;
using System.Linq;
using Fclp;
using Guartinel.CLI.Utility.Commands;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework;

namespace Guartinel.CLI.Utility.Tests {
   public class TestResultSender : IResultSender {
      public static List<JObject> Results = new List<JObject>() ;

      public void SendResult (string address, string token, string instanceID, string instanceName, bool isHeartbeat, CheckResult checkResult) {
         JObject result = new JObject() ;

         result ["address"] = address ;
         result ["token"] = token ;
         result ["instanceID"] = instanceID ;
         result ["instanceName"] = instanceName ;
         result ["checkResult"] = checkResult.ToJObject() ;

         Results.Add (result) ;
      }
   }

   [TestFixture]
   public class SendResultTests : TestsBase {
      protected override void SetUp() {
         base.SetUp() ;
         
         TestResultSender.Results.Clear();
      }

      protected override void Register() {
         base.Register() ;
         IoC.Use.Register<IResultSender>(() => new TestResultSender()) ;
      }

      [Test]
      public void SendResult_CheckResults() {
         CheckResult result = RunCommand() ;
         Assert.IsTrue (result.Success, result.ToString()) ;
         Assert.AreEqual ("test1", result.Message, result.ToString());
         Assert.AreEqual (1, TestResultSender.Results.Count) ;
      }

      //private CheckResult RunPingCommand (string host) {
      //   ICommand pingCommand = IoC.Use.GetAllInstances<ICommand>().First (x => x.Command == "ping") ;
      //   FluentCommandLineParser parser = new FluentCommandLineParser() ;
      //   pingCommand.Setup (parser) ;

      //   List<string> arguments = new List<string>() ;
      //   arguments.Add ("--command:ping") ;
      //   arguments.Add ($"--address:{host}") ;

      //   parser.Parse (arguments.ToArray()) ;
      //   return pingCommand.Run() ;
      //}

      private CheckResult RunCommand() {
         ICommand sendResultCommand = IoC.Use.GetAllInstances<ICommand>().First (x => x.Command == "sendResult") ;
         FluentCommandLineParser parser = new FluentCommandLineParser() ;
         sendResultCommand.Setup (parser) ;

         List<string> arguments = new List<string>() ;
         arguments.Add("--success:success") ;
         arguments.Add("--message:test1");

         arguments.Add ("--command:sendResult") ;
         arguments.Add ("--address:server1") ;
         arguments.Add ("--token:token1") ;
         arguments.Add ("--id:id1") ;
         arguments.Add ("--name:name1") ;

         // arguments.Add ($"--message:") ;
         // arguments.Add ($"--success:success") ;
         // arguments.Add ($"--data:") ;

         parser.Parse (arguments.ToArray()) ;
         return sendResultCommand.Run()[0] ;
      }
   }
}