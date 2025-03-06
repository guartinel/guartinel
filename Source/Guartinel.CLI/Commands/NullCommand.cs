using System;
using System.Collections.Generic ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Commands {
   public class NullCommand : ICommand {
      public string Description => string.Empty ;
      public string Command => string.Empty ;

      public void Setup (CommandLineApplication commandLineParser,
                         Action<ICommand> setCommandToRun) {
         commandLineParser.OnExecute (() => {
            setCommandToRun?.Invoke (this) ;

            //if ( logFolderOption.HasValue() ) {
            //   Logger.SetSetting(FileLogger.Constants.SETTING_NAME_FOLDER, logFolderOption.Value());
            //   Console.WriteLine($"Log folder is {logFolderOption.Value()}.");
            //}

            return Application.Constants.ResultCodes.COMMAND_FOUND ;
         }) ;
      }

      public List<CheckResult> Run() {
         // Nothing to do
         return new List<CheckResult> {new CheckResult.InvalidParameters()} ;
      }

      public void Setup (JObject parameters,
                         bool merge = false) {
         // Nothing to configure here
      }

      public string Info => string.Empty ;
   }
}
