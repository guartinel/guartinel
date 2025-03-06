using System;
using System.IO ;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;

namespace Guartinel.CLI.Files {
   public class FreeSpaceChecker : FreeSpaceCheckerBase {

      public override string Description => $"Check size of a drive." ;

      public new static class Constants {
         public static class Parameters {
            public const string DRIVE = "drive" ;
         }

         public static class Defaults { }

         public static class Results { }
      }

      public override string Command => "checkFreeSpace" ;
      public string Drive => Parameters.GetStringValue (Constants.Parameters.DRIVE, string.Empty) ;

      protected override string Target => Drive ;

      protected override FreeSpace GetSpaceInfo() {
         var driveInfo = new DriveInfo (Drive) ;

         // Logger.Log ($"Folder free space check: {freeBytesAvailable}, {totalNumberOfBytes}, {totalNumberOfFreeBytes}") ;
         _logger.Info ($"Drive '{Drive}', format is {driveInfo.DriveFormat}, free space is {driveInfo.TotalFreeSpace} bytes of {driveInfo.TotalSize}.") ;

         return new FreeSpace ((ulong) driveInfo.TotalFreeSpace, (ulong) driveInfo.TotalSize) ;
      }
   }

   public class FreeSpaceCheckerCl : FreeSpaceCheckerBaseCl<FreeSpaceChecker> {
      protected override void Setup3 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, FreeSpaceChecker.Constants.Parameters.DRIVE, "Drive to check.") ;
      }
   }
}