using System ;
using System.Collections.Generic ;
using System.Linq ;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Files {
   public class LatestFileAgeChecker : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string FOLDER = "folder" ;
            public const string MAX_AGE = "maxAge" ;
            public const string MAX_AGE_UNIT = "maxAgeUnit" ;
            public const string PATTERN = "pattern" ;
            public const string SUB_FOLDERS = "subFolders" ;
         }

         public static class Defaults {
            public const string PATTERN = FileConstants.ALL_FILES ;
            public const int MAX_AGE = 1 ;
            public const TimeUnit MAX_AGE_UNIT = TimeUnit.Day ;
         }

         public static class Results {
            public const string FILE_AGE = "file_age" ;
            public const string AGE_UNIT = "age_unit" ;
         }
      }

      public string FolderName => Parameters.GetStringValue (Constants.Parameters.FOLDER, string.Empty) ;

      public string Pattern => Parameters.GetStringValue (Constants.Parameters.PATTERN, Constants.Defaults.PATTERN) ;

      public bool SubFolders => Parameters.GetBooleanValue (Constants.Parameters.SUB_FOLDERS, false) ;

      public int MaxAge => Parameters.GetIntegerValue (Constants.Parameters.MAX_AGE, Constants.Defaults.MAX_AGE) ;

      public TimeUnit MaxAgeUnit {
         get {
            var value = Parameters.GetStringValue (Constants.Parameters.MAX_AGE_UNIT, Constants.Defaults.MAX_AGE_UNIT.ToString()) ;
            return EnumEx.Parse (value, Constants.Defaults.MAX_AGE_UNIT) ;
         }
      }

      public override string Description => "Check latest file in a folder." ;

      public override string Command => "checkLatestFileInFolder" ;

      protected override List<CheckResult> Run2() {
         if (MaxAge <= 0) return new List<CheckResult> {new CheckResult.InvalidParameters()} ;

         var ageInSeconds = FilesEx.GetAgeOfLatestFile (FolderName, SubFolders, Pattern) ;
         var ageInUnit = UnitsEx.ConvertTimeToUnit (ageInSeconds, MaxAgeUnit) ;
         var ageUnit = MaxAgeUnit.ToString().ToLowerInvariant() ;

         var data = new JObject() ;
         data [Constants.Results.FILE_AGE] = ageInUnit ;
         data [Constants.Results.AGE_UNIT] = ageUnit ;

         var ageIsOK = ageInSeconds <= UnitsEx.ConvertTimeToSeconds (MaxAge, MaxAgeUnit) ;
         _logger.Info ($"Latest file check. Pattern: {Pattern}, age: {ageInUnit} {ageUnit}, max age: {MaxAge}.") ;

         if (ageIsOK) {
            return new List<CheckResult> {new CheckResult (true,
                                                           $"Age of latest file is {ageInUnit} {ageUnit.Plural()}, less than {MaxAge} {ageUnit.Plural()}.",
                                                           $"Age of latest file in folder {FolderName} is {ageInUnit} {ageUnit.Plural()}, less than {MaxAge} {ageUnit.Plural()}.",
                                                           $"File age is OK ({ageInUnit} {ageUnit.Plural()}.",
                                                           data)} ;
         } else {
            return new List<CheckResult> {new CheckResult (false,
                                                           $"Age of latest file is {ageInUnit} {ageUnit.Plural()}, more than {MaxAge} {ageUnit.Plural()}.",
                                                           $"Age of latest file in folder {FolderName} is {ageInUnit} {ageUnit.Plural()}, more than {MaxAge} {ageUnit.Plural()}.",
                                                           $"File age is not OK ({ageInUnit} {ageUnit.Plural()}.",
                                                           data)} ;
         }
      }
   }

   public class LatestFileAgeCheckerCl : SendResultCommandBaseCl<LatestFileAgeChecker> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, LatestFileAgeChecker.Constants.Parameters.FOLDER, "Path to folder where the latest file is checked.") ;
         SetupOption (commandLineParser, LatestFileAgeChecker.Constants.Parameters.PATTERN, "File name or pattern with wildcards to include in the check.") ;
         SetupOptionBoolean (commandLineParser, LatestFileAgeChecker.Constants.Parameters.SUB_FOLDERS, "Check files in in subfolders as well.") ;

         SetupOption (commandLineParser, LatestFileAgeChecker.Constants.Parameters.MAX_AGE, "Max age of the latest file in the folder.") ;
         SetupOption (commandLineParser, LatestFileAgeChecker.Constants.Parameters.MAX_AGE_UNIT, "Unit of max age of file (second, minute, hour, day).") ;
      }
   }
}