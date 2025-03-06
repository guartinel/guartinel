using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guartinel.CLI.Commands;
using Guartinel.CLI.ResultSending;
using Guartinel.Kernel;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility;
using Microsoft.Extensions.CommandLineUtils;

namespace Guartinel.CLI {
   public class Application {
      public class Constants {
         public class ResultCodes {
            public const int SUCCESS = 0 ;
            public const int EXCEPTION = 1 ;
            public const int INVALID_PARAMETERS = 2 ;
            public const int HELP_SHOWN = 3 ;
            public const int CHECK_FAIL = 4 ;

            public const int COMMAND_FOUND = 100 ;
         }
      }

      public int Run (string[] arguments) {
         var commandLine = new CommandLineApplication (false) ;

         // Register loggers
         Logger.Setup<SimpleFileLogger> ("Guartinel.CLI", "Guartinel Command Line Interface") ;
         // Logger.RegisterLogger<SimpleConsoleLogger>() ;

         IoC.Use.Multi.Register<ICommand> (typeof(Application).Assembly) ;
         IoC.Use.Multi.Register<ICommandLineCommand> (typeof(Application).Assembly) ;

         IoC.Use.Single.Register<IResultSender, ResultSender>() ;

         // Allow descendants to register commands
         RegisterCommands() ;

         // Verify the registration
         IoC.Use.Verify() ;

         var help = commandLine.HelpOption ("--help") ;
         help.Description = "Display usage information" ;
         // commandLine.VersionOption("--version", "--v", typeof(Application).Assembly.GetName().Version.ToString());

         ICommandLineCommand commandToRun = null ;

         // Loop on commands
         foreach (ICommandLineCommand command in IoC.Use.Multi.GetInstances<ICommandLineCommand>()) {
            // Check command
            command.Setup (commandLine, command1 => {
               commandToRun = command1 ;
            }) ;
         }

         // When running
         commandLine.OnExecute (() => Constants.ResultCodes.INVALID_PARAMETERS) ;

         try {
            // Parse command line
            //; _commandLine.ShowVersion() ;            
            var result = commandLine.Execute (arguments) ;

            if ((result == Constants.ResultCodes.SUCCESS) & commandLine.IsShowingInformation) {
               Logger.Log ("Help is shown.") ;
               return Constants.ResultCodes.HELP_SHOWN ;
            }

            if (result == Constants.ResultCodes.COMMAND_FOUND) {
               if (commandToRun != null) {
                  List<CheckResult> checkResults ;
                  // Run command
                  try {
                     checkResults = commandToRun.Run() ;
                  } catch (Exception e) {
                     var message = $"{commandToRun.Command} exception: {e.Message}" ;
                     Logger.Log (LogLevel.Error, message) ;
                     Console.WriteLine (message) ;

                     return Constants.ResultCodes.EXCEPTION ;
                  }

                  if (checkResults.All (x => x.Success)) {
                     Logger.Log (LogLevel.Info, $"Command '{commandToRun.Command}' check succeeded.") ;
                     return Constants.ResultCodes.SUCCESS ;
                  }

                  if (checkResults.Any (x => !x.Success)) {
                     Logger.Log (LogLevel.Info, $"Command '{commandToRun.Command}' check failed.") ;
                     return Constants.ResultCodes.CHECK_FAIL ;
                  }
               }
            }

            Logger.Log (LogLevel.Error, $"Invalid parameters. Arguments: {string.Join (",", arguments)}") ;
            commandLine.ShowHelp() ;
            return Constants.ResultCodes.INVALID_PARAMETERS ;
         } catch (Exception e) {
            Logger.Log (LogLevel.Error, $"Exception: {e.GetAllMessages()}") ;
            return Constants.ResultCodes.EXCEPTION ;
         }
      }

      protected virtual void RegisterCommands() { }
   }
}