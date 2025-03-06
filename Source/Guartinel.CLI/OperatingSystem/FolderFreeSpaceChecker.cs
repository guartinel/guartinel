using System;
using Guartinel.CLI.Files ;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;

namespace Guartinel.CLI.OperatingSystem {
   public class FolderFreeSpaceChecker : FreeSpaceCheckerBase {

      public new static class Constants {
         public static class Parameters {
            public const string FOLDER = "folder" ;
         }

         public static class Defaults { }

         public static class Results { }
      }

      public override string Description => $"Check size of a folder." ;

      public override string Command => "checkFolderFreeSpace" ;

      public string FolderName => Parameters.GetProperty<string> (Constants.Parameters.FOLDER) ;

      protected override string Target => FolderName ;

      protected override FreeSpace GetSpaceInfo() {
         return OperatingSystemSelector.CurrentOperatingSystem.GetFolderSpace (FolderName, _logger.Tags) ;
      }
   }

   public class FolderFreeSpaceCheckerCl : FreeSpaceCheckerBaseCl<FolderFreeSpaceChecker> {
      protected override void Setup3 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, FolderFreeSpaceChecker.Constants.Parameters.FOLDER, "Full path of folder to check.") ;
      }
   }
}