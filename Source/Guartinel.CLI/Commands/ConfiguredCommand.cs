using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using Guartinel.CLI.Network ;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Network ;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Commands {
   /// <summary>
   /// Command supporting having other commands in one file in JSON format.
   /// </summary>
   public class ConfiguredCommand : SendResultCommandBase {
      public new static class Constants {
        
         public static class Parameters {
            public const string COMMANDS_FILE = "commandsFile";
         }

         public static class ConfigurationProperties {
            public const string COMMON_PARAMETERS = "common";
            public const string COMMANDS_ARRAY = @"commands";
            public const string COMMAND_PARAMETER = "command" ;
         }

         public static class Results {}
      }

      public override string Description => $"Run other commands by configuration file." ;

      public override string Command => $"configured" ;

      public string CommandsFile => Parameters.GetStringValue (Constants.Parameters.COMMANDS_FILE, string.Empty) ;

      protected override List<CheckResult> Run2() {
         var results = new List<CheckResult>();

         var commandsFileName = FilesEx.EnsureFileNameHasFullPath (CommandsFile) ;
         if (!File.Exists (commandsFileName)) return results ;

         //// Process lines in command file
         //var lines = File.ReadLines (commandsFileName) ;
         //foreach (var line in lines) {
         //   if (string.IsNullOrEmpty (line)) continue ;

         //   var correctedLine = line.Trim() ;
         //   if (string.IsNullOrEmpty (correctedLine)) continue ;
         //   // Comment?
         //   if (correctedLine.StartsWith (Constants.COMMENT_PREFIX)) continue ;

         //   // Get command and parameters
         //   var parameters = correctedLine.Split (Constants.PARAMETER_SEPARATOR, StringSplitOptions.RemoveEmptyEntries) ;
         //   if (parameters.Length == 0) continue ;

         //   var command = parameters [0].ToLowerInvariant() ;
         //   if (command == Constants.Commands.PING) {
         //      var result = PingCheckerBase.Ping (new Host (parameters [2], parameters [1])) ;
         //      results.Add (result) ;
         //   }
         //}

         // Process commands
         JObject commandsFile = JObject.Parse (File.ReadAllText (commandsFileName)) ;

         // Get common parameters
         JObject commonConfiguration = (JObject) commandsFile [Constants.ConfigurationProperties.COMMON_PARAMETERS] ;
         // Get commands
         JArray commands = (JArray) commandsFile [Constants.ConfigurationProperties.COMMANDS_ARRAY] ;
         if (commands == null) return results ;

         foreach (var commandToken in commands) {
            if (!(commandToken is JObject)) continue ;
            var commandConfiguration = (JObject) commandToken ;

            var commandName = commandConfiguration.GetStringValue (Constants.ConfigurationProperties.COMMAND_PARAMETER) ;
            if (string.IsNullOrEmpty (commandName)) continue ;

            ICommand command = IoC.Use.Multi.GetInstances<ICommand>().First(x => x.Command == commandName) ;
            if (command == null) continue ;

            command.Setup (commandConfiguration) ;
            command.Setup (commonConfiguration, true) ;

            results.AddRange (command.Run()) ;
         }

         return results ;
      }
   }

   public class ConfiguredCommandCl : SendResultCommandBaseCl<ConfiguredCommand> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, ConfiguredCommand.Constants.Parameters.COMMANDS_FILE, "Commands file in JSON format.") ;
      }
   }
}