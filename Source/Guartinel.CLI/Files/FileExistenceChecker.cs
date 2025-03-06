using System.Collections.Generic ;
using System.IO;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;

namespace Guartinel.CLI.Files {
   public class FileExistenceChecker : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string PATTERN = "pattern" ;
            public const string FOLDER = "folder" ;
         }

         public static class Defaults {
            public const string PATTERN = "*.*" ;
         }
      }

      public override string Description => $"Check for existance of file or files." ;

      public override string Command => "checkFileExists" ;

      public string FolderName => Parameters.GetStringValue(Constants.Parameters.FOLDER, string.Empty) ;

      public string Pattern => Parameters.GetStringValue(Constants.Parameters.PATTERN, Constants.Defaults.PATTERN) ;

      protected override List<CheckResult> Run2() {
         bool result = Directory.GetFiles (FolderName, Pattern, SearchOption.TopDirectoryOnly).Length > 0 ;

         string message ;
         string details ;
         string extract ;

         if (result) {
            message = $"File(s) '{Pattern}' found." ;
            details = $"File(s) '{Pattern}' found in folder '{FolderName}'.";
            extract = $"File search is OK." ;
         } else {
            message = $"File(s) '{Pattern}' not found." ;
            details = $"File(s) '{Pattern}' not found in folder '{FolderName}'.";
            extract = $"No file found." ;
         }

         _logger.Info (details) ;

         return new List<CheckResult> {new CheckResult (result, message, details, extract, null) } ;
      }
   }

   public class FileExistenceCheckerCl : SendResultCommandBaseCl<FileExistenceChecker> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption(commandLineParser, FileExistenceChecker.Constants.Parameters.FOLDER, "Path to folder to check the files in.");
         SetupOption(commandLineParser, FileExistenceChecker.Constants.Parameters.PATTERN, "File name or pattern with wildcards to include in the check.");
      }
   }
}