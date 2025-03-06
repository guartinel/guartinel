using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;
using NUnit.Framework ;

namespace Guartinel.CLI.Tests {
   [TestFixture]
   public class ConfiguredCommandTests : TestsBase {
      protected override void SetUp() {
         base.SetUp() ;

         TestResultSender.Results.Clear() ;
      }

      [Test]
      public void MultipleCommands_Run_CheckResults() {
         JObject configuration = new JObject() ;

         JObject common = new JObject() ;
         common ["address"] = "server1" ;
         common ["token"] = "token1" ;

         configuration["common"] = common ;

         JArray commands = new JArray() ;

         // Ping
         JObject pingCommand = new JObject() ;
         pingCommand["id"] = "id1";
         pingCommand["name"] = "name1";

         pingCommand["command"] = "ping" ;
         pingCommand ["target"] = "index.hu" ;

         commands.Add (pingCommand) ;

         // Check file existence - exists!
         string tempFile1 = Path.GetTempFileName() ;
         File.WriteAllText (tempFile1, "Test1") ;
         JObject fileExistsCommand1 = new JObject() ;

         fileExistsCommand1 ["id"] = "id2";
         fileExistsCommand1 ["name"] = "name2";

         fileExistsCommand1 ["command"] = "checkFileExists" ;
         fileExistsCommand1 ["folder"] = Path.GetDirectoryName (tempFile1) ;
         fileExistsCommand1 ["pattern"] = Path.GetFileName (tempFile1) ;
         commands.Add (fileExistsCommand1) ;

         // Check file existence - does not exist
         string tempFile2 = tempFile1 + "1" ;
         JObject fileExistsCommand2 = new JObject() ;
         fileExistsCommand2["id"] = "id3";
         fileExistsCommand2["name"] = "name3";

         fileExistsCommand2["command"] = "checkFileExists" ;
         fileExistsCommand2 ["folder"] = Path.GetDirectoryName (tempFile2) ;
         fileExistsCommand2 ["pattern"] = Path.GetFileName (tempFile2) ;
         commands.Add (fileExistsCommand2) ;

         configuration ["commands"] = commands ;

         List<CheckResult> results = RunCommand (configuration) ;
         Assert.AreEqual (3, results.Count) ;

         // Ping
         Asserts (results [0], true, "index.hu", "is accessible") ;
         // Check file existence - exists!
         Asserts (results [1], true, Path.GetDirectoryName (tempFile1), Path.GetFileName (tempFile1), "found in folder") ;
         // Check file existence - does not exist
         Asserts (results [2], false, Path.GetDirectoryName (tempFile2), Path.GetFileName (tempFile2), "not found in folder") ;

         File.Delete (tempFile1) ;
      }

      private void Asserts (CheckResult result,
                            bool success,
                            params string[] messageParts) {
         Assert.AreEqual (success, result.Success, result.ToString()) ;

         foreach (var messagePart in messageParts) {
            if (!string.IsNullOrEmpty (messagePart)) {
               Assert.IsTrue (result.Message.Contains (messagePart), result.ToString()) ;
            }
         }
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

      private List<CheckResult> RunCommand (JObject commands) {
         string commandsFileName = Path.GetTempFileName();
         File.WriteAllText (commandsFileName, commands.ToString (Formatting.Indented)) ;

         return RunCommand ("configured", () => CreateArguments (commandsFileName)) ;
      }

      private List<string> CreateArguments (string commandsFileName) {
         List<string> arguments = new List<string>() ;
         arguments.Add ("configured") ;

         arguments.Add ($@"--commandsFile=""{commandsFileName}""") ;

         return arguments ;
      }
   }
}