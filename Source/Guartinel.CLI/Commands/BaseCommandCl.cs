using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Commands {
   public abstract class BaseCommandCl<T> : ICommandLineCommand where T : BaseCommand, new() {
      protected T _command = new T() ;
      public string Command => _command.Command ;

      public string Description => _command.Description ;

      protected JObject Parameters => _command.Parameters ;

      public List<CheckResult> Run() {
         // Fill values
         foreach (var optionName in _options.Keys) {
            if (_options [optionName].OptionType == CommandOptionType.NoValue) {
               // Switch on
               if (_options [optionName].HasValue()) {
                  _command.SetParameter (optionName, _options [optionName].HasValue()) ;
               }
            } else {
               if (_options [optionName].HasValue()) {
                  var value = _options [optionName].Value() ;

                  // Remove quotes
                  if (!string.IsNullOrEmpty (value) &&
                      value.StartsWith (@"""") &&
                      value.EndsWith (@"""") &&
                      value.Length > 1) {

                     value = value.Remove (0, 1) ;
                     value = value.Remove (value.Length - 1, 1) ;
                  }

                  _command.SetParameter (optionName, value) ;
               }
            }
         }

         return _command.Run() ;
      }

      protected Dictionary<string, CommandOption> _options = new Dictionary<string, CommandOption>() ;

      protected void SetupOption (CommandLineApplication commandLineParser,
                                  string name,
                                  string description) {
         var option = commandLineParser.Option ($"--{name} <{name}>", description, CommandOptionType.SingleValue) ;
         _options.Add (name, option) ;
      }

      protected void SetupOptionBoolean (CommandLineApplication commandLineParser,
                                         string name,
                                         string description) {
         var option = commandLineParser.Option ($"--{name}", description, CommandOptionType.NoValue) ;
         _options.Add (name, option) ;
      }

      //protected void SetupOptionBoolean (CommandLineApplication commandLineParser,
      //                                string name) {
      //   commandParser.AddOption<bool>(name).Callback(value => _parameters[name] = value);
      //}

      public void Setup (JObject parameters) {
         _command.Setup (parameters) ;
      }

      public void Setup (CommandLineApplication commandLineParser,
                         Action<ICommandLineCommand> setCommandToRun) {
         // Add current command
         commandLineParser.Command (Command, commandParser1 => {
            commandParser1.HelpOption ("--help") ;

            // Log folder
            SetupOption (commandParser1, BaseCommand.Constants.Parameters.LOG_FOLDER, "Folder to place the log files.") ;
            SetupOption (commandParser1, BaseCommand.Constants.Parameters.CONFIGURATION_FILE_OBSOLETE, "Configuration file (obsolete).") ;
            SetupOption (commandParser1, BaseCommand.Constants.Parameters.CONFIGURATION_FILE, "Configuration file.") ;

            // commandParser.Setup<bool> (Constants.Parameters.READ_FROM_CONSOLE).Callback (value => _read = value) ;
            SetupOptionBoolean (commandParser1, BaseCommand.Constants.Parameters.WRITE_TO_CONSOLE, "Write result to console.") ;

            Setup1 (commandParser1) ;

            commandParser1.OnExecute (() => {
               setCommandToRun?.Invoke (this) ;

               //if ( logFolderOption.HasValue() ) {
               //   Logger.SetSetting(FileLogger.Constants.SETTING_NAME_FOLDER, logFolderOption.Value());
               //   Console.WriteLine($"Log folder is {logFolderOption.Value()}.");
               //}

               return Application.Constants.ResultCodes.COMMAND_FOUND ;
            }) ;

         }).Description = this.Description ;
      }

      protected virtual void Setup1 (CommandLineApplication commandLineParser) { }
   }
}