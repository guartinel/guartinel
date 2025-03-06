using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using Fclp ;
using Guartinel.CLI.Utility.Commands ;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core ;
using Guartinel.Core.Logging ;
using Guartinel.Core.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility.Files {
   public class LatestFileAgeChecker : SendResultCommandBase {
      public new static class Constants {
         public const int SECONDS_IN_MINUTE = 60 ;
         public const int SECONDS_IN_HOUR = SECONDS_IN_MINUTE * 60 ;
         public const int SECONDS_IN_DAY = SECONDS_IN_HOUR * 24;

         public const int ROUNDING = 2 ;
         public const string ALL_FILES = "*.*";

         public static class Parameters {            
            public const string FOLDER = "folder";
            public const string MAX_AGE = "maxAge" ;
            public const string MAX_AGE_UNIT = "maxAgeUnit";
            public const string INCLUDE = "include" ;
            public const string SUB_FOLDERS = "subFolders" ;
         }

         public static class Results {            
            public const string FILE_AGE = "file_age" ;
            public const string AGE_UNIT = "age_unit" ;
         }         
      }

      public enum TimeUnit {
         Second,
         Minute,
         Hour,
         Day
      };

      public static int ConvertToSeconds (int interval,
                                          TimeUnit timeUnit) {
         switch (timeUnit) {
            case TimeUnit.Minute:
               return interval * Constants.SECONDS_IN_MINUTE ;

            case TimeUnit.Hour:
               return interval * Constants.SECONDS_IN_HOUR ;
            
            case TimeUnit.Day :
               return interval * Constants.SECONDS_IN_DAY ;
         }

         return interval ;
      }

      public static double ConvertToUnit (int intervalInSeconds,
                                          TimeUnit timeUnit) {
         switch (timeUnit) {
            case TimeUnit.Minute:
               return Math.Round (1.0 * intervalInSeconds / Constants.SECONDS_IN_MINUTE, Constants.ROUNDING) ;

            case TimeUnit.Hour:
               return Math.Round (1.0 * intervalInSeconds / Constants.SECONDS_IN_HOUR, Constants.ROUNDING) ;

            case TimeUnit.Day:
               return Math.Round (1.0 * intervalInSeconds / Constants.SECONDS_IN_DAY, Constants.ROUNDING) ;
         }

         return intervalInSeconds ;
      }

      private string _folderName ;
      private int _maxAge ;
      private TimeUnit _maxAgeUnit = TimeUnit.Second ;
      private string _includePattern ;      
      private string IncludePattern => string.IsNullOrEmpty (_includePattern) ? Constants.ALL_FILES : _includePattern ;

      private bool _subFolders ;

      public override string Description => "Check latest file in a folder." ;

      public override string Command => "checkLatestFileInFolder" ;

      protected override void Setup2 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<string> (Constants.Parameters.FOLDER).Required().Callback (value => _folderName = value) ;
         commandLineParser.Setup<int> (Constants.Parameters.MAX_AGE).Required().Callback (value => _maxAge = value) ;
         commandLineParser.Setup<string> (Constants.Parameters.MAX_AGE_UNIT).Callback (value => _maxAgeUnit = EnumEx.Parse (value, TimeUnit.Second)) ;
         commandLineParser.Setup<string> (Constants.Parameters.INCLUDE).Callback (value => _includePattern = value) ;
         commandLineParser.Setup<bool> (Constants.Parameters.SUB_FOLDERS).Callback (value => _subFolders = value) ;
      }

      protected override List<CheckResult> Run2() {
         if (_maxAge <= 0) return new List<CheckResult> { new CheckResult.InvalidParameters()} ;

         var maxAgeInSeconds = ConvertToUnit (_maxAge, _maxAgeUnit) ;

         var ageInSeconds = FilesEx.GetAgeOfLatestFile (_folderName, _subFolders, IncludePattern) ;
         var ageInUnit = ConvertToUnit (ageInSeconds, _maxAgeUnit) ;
         var ageUnit = _maxAgeUnit.ToString().ToLowerInvariant() ;

         var data = new JObject();
         data [Constants.Results.FILE_AGE] = ageInUnit;
         data [Constants.Results.AGE_UNIT] = ageUnit ;

         var ageIsOK = ageInSeconds <= ConvertToSeconds (_maxAge, _maxAgeUnit) ;
         Logger.Log(LogLevel.Info, $"Latest file check. Pattern: {IncludePattern}, age: {ageInUnit} {ageUnit}, max age: {_maxAge}.");

         CheckResult result ;
         
         if (ageIsOK) {
            return new List<CheckResult> { new CheckResult (true, $"Age of latest file in folder {_folderName} is less than {_maxAge} {ageUnit}.", data)} ;
         } else {
            return new List<CheckResult> { new CheckResult (false, $"Age of latest file in folder {_folderName} is {ageInUnit} {ageUnit}, more than {_maxAge}.", data)} ;
         }
      }
   }
}