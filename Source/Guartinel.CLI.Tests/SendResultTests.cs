using System ;
using System.Collections.Generic;
using System.Linq;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests {
   public class TestResultSender : IResultSender {
      public static List<JObject> Results = new List<JObject>() ;

      public void SendResult (string address, string token, string instanceID, string instanceName, bool isHeartbeat, CheckResult checkResult) {
         JObject result = new JObject() ;

         result ["address"] = address ;
         result ["token"] = token ;
         result ["instanceID"] = instanceID ;
         result ["instanceName"] = instanceName ;
         result ["checkResult"] = checkResult.AsJObject() ;

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
         
         IoC.Use.Single.Register<IResultSender>(() => new TestResultSender()) ;
      }

      [Test]
      public void SendResult_CheckResults() {
         CheckResult result = RunCommand() [0] ;
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

      private List<CheckResult> RunCommand() {
         return RunCommand ("sendResult", CreateArguments) ;
      }

      private List<string> CreateArguments() {
         List<string> arguments = new List<string>();
         arguments.Add("sendResult");

         arguments.Add("--success=success");
         arguments.Add("--message=test1");

         arguments.Add("--address=server1");
         arguments.Add("--token=token1");
         arguments.Add("--id=id1");
         arguments.Add("--name=name1");
         return arguments;
      }
   }
}