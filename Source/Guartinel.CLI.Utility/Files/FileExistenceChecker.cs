using System.Collections.Generic ;
using System.IO;
using Fclp;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core.Logging ;

namespace Guartinel.CLI.Utility.Files {
   public class FileExistenceChecker : SendResultCommandBase {
      private string _folderName ;
      private string _pattern ;


      public new static class Constants {
         public static class Parameters {
            public const string PATTERN = "pattern" ;
            public const string FOLDER = "folder" ;
         }
      }

      public override string Description => $"Check for existance of file or files." ;

      public override string Command => "checkFileExists" ;

      protected override void Setup2 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<string> (Constants.Parameters.PATTERN).Required().Callback (value => _pattern = value) ;
         commandLineParser.Setup<string> (Constants.Parameters.FOLDER).Required().Callback (value => _folderName = value) ;
      }

      protected override List<CheckResult> Run2() {
         bool result = Directory.GetFiles (_folderName, _pattern, SearchOption.TopDirectoryOnly).Length > 0 ;

         string message ;

         if (result) {
            message = $"File(s) {_pattern} found in folder {_folderName}." ;
            Logger.Log (LogLevel.Info, message) ;
         } else {
            message = $"File(s) {_pattern} not found in folder {_folderName}." ;
            Logger.Log (LogLevel.Info, message) ;
         }

         return new List<CheckResult> {new CheckResult (result, message, null)} ;
      }
   }
}