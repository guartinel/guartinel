using System;
using System.Collections.Generic ;
using System.Linq ;
using System.Reflection ;
using Fclp;
using Guartinel.CLI.Utility.Commands ;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core ;
using Guartinel.Core.Logging ;

namespace Guartinel.CLI.Utility {
   public class Application {
      public class Constants {
         public class ErrorCodes {
            public const int SUCCESS = 0 ;
            public const int EXCEPTION = 1 ;
            public const int INVALID_PARAMETERS = 2 ;
            public const int HELP_SHOWN = 3 ;
            public const int CHECK_FAIL = 4 ;
         }
      }

      public Application (string[] arguments) {
         _arguments = arguments ;

         // Register loggers
         Logger.Setup<SimpleFileLogger> ("Guartinel.CLI", "Guartinel Command Line Interface") ;
         // Logger.RegisterLogger<SimpleConsoleLogger>() ;

         IoC.RegisterImplementations<ICommand> (Assembly.GetAssembly (typeof (Application))) ;
         IoC.Use.Register<IResultSender> (() => new ResultSender()) ;

         // Verify the registration
         IoC.Use.Verify() ;

         _commandLineParser.SetupHelp ("h", "help", "?").Callback (message => {
            Console.WriteLine ("Usage:") ;
            Console.WriteLine (message) ;

            // Loop on all possible commands
            Console.WriteLine ("Available commands:") ;
            foreach (var command in IoC.Use.GetAllInstances<ICommand>()) {
               Console.WriteLine ($"   {command.Command.PadRight (20)}: {command.Description}") ;
            }
         }) ;

         _commandLineParser.Setup<string> ("command").Callback (value => _command = value).WithDescription ("Command to run.") ;
         _commandLineParser.Setup<string> ("logFolder").Callback (value => Logger.SetSetting (FileLogger.Constants.SETTING_NAME_FOLDER, value)).WithDescription ("Folder to place the log files.") ;
      }

      private readonly string[] _arguments ;
      private readonly FluentCommandLineParser _commandLineParser = CommandLineParserEx.Create() ;
      private string _command ;

      public int Run() {
         // const string DATE_FORMAT = "YYYY-MM-DD" ;
         try {
            // Parse command line
            var parseResult = _commandLineParser.Parse (_arguments) ;
            if (parseResult.HasErrors) {
               throw new Exception (parseResult.ErrorText) ;
            }

            if (parseResult.HelpCalled) {
               return Constants.ErrorCodes.HELP_SHOWN ;
            }

            // Loop on commands
            foreach (ICommand command in IoC.Use.GetAllInstances<ICommand>()) {
               // Check command
               if (string.IsNullOrEmpty (command.Command)) continue ;
               if (!command.Command.Equals (_command, StringComparison.InvariantCultureIgnoreCase)) continue ;

               // Setup arguments for this command
               var commandLineParser = CommandLineParserEx.Create();
               commandLineParser.SetupHelp ("commandHelp", "??").Callback (message => {
                  Console.WriteLine ($"Usage of {command.Command}:") ;
                  Console.WriteLine (message) ;
               }) ;

               command.Setup (commandLineParser) ;

               // Parse arguments
               parseResult = commandLineParser.Parse (_arguments) ;
               if (parseResult.HasErrors) {
                  throw new Exception (parseResult.ErrorText) ;
               }

               if (parseResult.HelpCalled) {
                  return Constants.ErrorCodes.HELP_SHOWN ;
               }

               List<CheckResult> checkResults ;
               // Run command
               try {
                  checkResults = command.Run() ;
               } catch (Exception e) {
                  var message = $"{command.Command} exception: {e.Message}" ;
                  Logger.Log (LogLevel.Error, message) ;
                  Console.WriteLine (message) ;
                  return Constants.ErrorCodes.EXCEPTION ;
               }

               if (checkResults.All (x => x.Success) ) {
                  // Logger.Log (LogLevel.Info, $"{command.Command}: success.") ;
                  return Constants.ErrorCodes.SUCCESS ;
               }

               if (checkResults.Any (x => !x.Success)) {
                  // Logger.Log (LogLevel.Info, $"{command.Command}: fail.") ;
                  return Constants.ErrorCodes.CHECK_FAIL ;
               }
            }

            // Not processed
            Logger.Log (LogLevel.Error, $"Invalid parameters.") ;
            _commandLineParser.HelpOption.ShowHelp (_commandLineParser.Options) ;

            return Constants.ErrorCodes.INVALID_PARAMETERS ;
         } catch (Exception e) {
            Logger.Log (LogLevel.Error, $"Exception: {e.GetAllMessages()}") ;
            return Constants.ErrorCodes.EXCEPTION ;
         }
      }
   }
}