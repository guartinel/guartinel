using System;
using System.Collections.Generic ;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Fclp;
using Guartinel.CLI.Utility.Commands;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core;
using Guartinel.Core.Logging;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility.Files
{
   public class FreeSpaceChecker : SendResultCommandBase {
      private string _folderName ;
      private double _minAvailableSpaceGBs ;
      private int _minAvailableSpacePercents ;

      public override string Description => $"Check size of a drive for a given path." ;
      public new static class Constants {
         public static class Parameters {
            public const string FOLDER = "folder";
            public const string MIN_SPACE_GBS = "minSpaceGBs";
            public const string MIN_SPACE_PERCENTS = "minSpacePercents";
         }

         public static class Results{
            public const string FREE_SPACE_GB = "freeSpaceGbs";
            public const string FREE_SPACE_PERCENTS = "freeSpacePercents";
         }
      }

      public override string Command => "checkFreeSpace" ;

      protected override void Setup2 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<string> (Constants.Parameters.FOLDER).Required().Callback (value => _folderName = value) ;
         commandLineParser.Setup<double> (Constants.Parameters.MIN_SPACE_GBS).Callback (value => _minAvailableSpaceGBs = value) ;
         commandLineParser.Setup<int> (Constants.Parameters.MIN_SPACE_PERCENTS).Callback (value => _minAvailableSpacePercents = value) ;
      }

      protected class FreeSpaceInfo {
         public FreeSpaceInfo (ulong freeSpaceBytes,
                               int freeSpacePercents) {
            FreeSpaceBytes = freeSpaceBytes ;
            FreeSpaceGBs = Math.Round (freeSpaceBytes / 1024.0 / 1024.0 / 1024.0, 2) ;
            FreeSpacePercents = freeSpacePercents ;
         }

         public double FreeSpaceGBs {get ; private set ;}
         public double FreeSpaceBytes {get ; private set ;}
         public int FreeSpacePercents {get ; private set ;}
      }

      protected static FreeSpaceInfo GetSpaceInfo (string folderName) {
         ulong freeBytesAvailable ;
         ulong totalNumberOfBytes ;
         ulong totalNumberOfFreeBytes ;

         bool success = WinAPI.GetDiskFreeSpaceEx (folderName,
                                                   out freeBytesAvailable,
                                                   out totalNumberOfBytes,
                                                   out totalNumberOfFreeBytes) ;
         if (!success) {
            throw new Win32Exception (Marshal.GetLastWin32Error()) ;
         }

         // Logger.Log ($"Folder free space check: {freeBytesAvailable}, {totalNumberOfBytes}, {totalNumberOfFreeBytes}") ;

         return new FreeSpaceInfo (totalNumberOfFreeBytes, (int) ((double) totalNumberOfFreeBytes / totalNumberOfBytes * 100)) ;
      }

      protected override List<CheckResult> Run2() {
         if ((_minAvailableSpaceGBs <= 0f) &&
             (_minAvailableSpacePercents <= 0)) return new List<CheckResult> {new CheckResult.InvalidParameters()} ;

         var spaceInfo = GetSpaceInfo (_folderName) ;
         var checkData = new JObject() ;
         checkData [Constants.Results.FREE_SPACE_GB] = spaceInfo.FreeSpaceGBs ;
         checkData [Constants.Results.FREE_SPACE_PERCENTS] = spaceInfo.FreeSpacePercents ;

         if ((_minAvailableSpaceGBs > 0f) && (spaceInfo.FreeSpaceGBs < _minAvailableSpaceGBs)) {
            string message = $"Free space for {_folderName} is {spaceInfo.FreeSpaceGBs} GBs, less than {_minAvailableSpaceGBs} GBs." ;
            Logger.Log ($"Free GBs check fail. {message}") ;
            return new List<CheckResult> { new CheckResult (false, message, checkData)} ;
         } else if ((_minAvailableSpacePercents > 0) && (spaceInfo.FreeSpacePercents < _minAvailableSpacePercents)) {
            string message = $"Free space for {_folderName} is {spaceInfo.FreeSpacePercents}%, less than {_minAvailableSpacePercents}%." ;
            Logger.Log ($"Free percent check fail. {message}") ;
            return new List<CheckResult> { new CheckResult (false, message, checkData)} ;
         } else {
            string message = $@"Free space for {_folderName} is {spaceInfo.FreeSpaceGBs} GBs ({spaceInfo.FreeSpacePercents}%)." ;
            Logger.Log ($"Check successful, free space for {_folderName} is {spaceInfo.FreeSpaceGBs} GBs ({spaceInfo.FreeSpacePercents}%).") ;

            return new List<CheckResult> { new CheckResult (true, message, checkData)} ;
         }
      }
   }
}