using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using Fclp ;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core ;
using Guartinel.Core.Logging ;
using Guartinel.Core.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility.Files {
   public class FolderSizeChecker : SendResultCommandBase {
      public new static class Constants {
         public const string ALL_FILES = "*.*" ;
         public const long BYTES_IN_MB = 1024 * 1024 ;
         public const long BYTES_IN_GB = 1024 * BYTES_IN_MB ;
         public const long BYTES_IN_TB = 1024 * BYTES_IN_GB ;

         public const int ROUNDING = 2 ;

         public static class Parameters {
            public const string FOLDER = "folder" ;
            public const string MAX_SIZE = "maxSize" ;
            public const string MAX_SIZE_UNIT = "maxSizeUnit" ;
            public const string INCLUDE = "include" ;
         }

         public static class Results {
            public const string FOLDER = "folder" ;
            public const string INCLUDE = "include" ;
            public const string FOLDER_SIZE = "folder_size" ;
            public const string MAX_SIZE = "max_size" ;
            public const string MAX_SIZE_UNIT = "max_size_unit" ;
         }
      }

      public enum SizeUnit {
         Byte,
         MB,
         GB,
         TB
      };

      public static long ConvertToBytes (long size,
                                         SizeUnit timeUnit) {
         switch (timeUnit) {
            case SizeUnit.MB:
               return size * Constants.BYTES_IN_MB ;

            case SizeUnit.GB:
               return size * Constants.BYTES_IN_GB ;

            case SizeUnit.TB:
               return size * Constants.BYTES_IN_TB ;
         }

         return size ;
      }

      public static double ConvertToUnit (long sizeInBytes,
                                          SizeUnit sizeUnit) {
         switch (sizeUnit) {
            case SizeUnit.MB:
               return Math.Round (1.0 * sizeInBytes / Constants.BYTES_IN_MB, Constants.ROUNDING) ;

            case SizeUnit.GB:
               return Math.Round (1.0 * sizeInBytes / Constants.BYTES_IN_GB, Constants.ROUNDING) ;

            case SizeUnit.TB:
               return Math.Round (1.0 * sizeInBytes / Constants.BYTES_IN_TB, Constants.ROUNDING) ;
         }

         return sizeInBytes ;
      }

      private string _folderName ;
      private double _maxSize ;
      private SizeUnit _maxSizeUnit = SizeUnit.Byte ;
      private string _includePattern ;
      private string IncludePattern => string.IsNullOrEmpty (_includePattern) ? Constants.ALL_FILES : _includePattern ;

      public override string Description => $"Check size of a folder." ;

      public override string Command => "checkFolderSize" ;

      protected override void Setup2 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<string> (Constants.Parameters.FOLDER).Required().Callback (value => _folderName = value) ;
         commandLineParser.Setup<double> (Constants.Parameters.MAX_SIZE).Required().Callback (value => _maxSize = value) ;
         commandLineParser.Setup<string> (Constants.Parameters.MAX_SIZE_UNIT).Callback (value => _maxSizeUnit = EnumEx.Parse (value, SizeUnit.Byte)) ;
         commandLineParser.Setup<string> (Constants.Parameters.INCLUDE).Callback (value => _includePattern = value) ;
      }

      protected double CalculateFolderSize() {
         return ConvertToUnit (CalculateFolderSizeInBytes (_folderName, IncludePattern), _maxSizeUnit) ;
      }

      protected static long CalculateFolderSizeInBytes (string folderName,
                                                        string includePattern) {
         long folderSizeInBytes = 0 ;
         try {
            //Checks if the path is valid or not
            if (!Directory.Exists (folderName)) {
               return folderSizeInBytes ;
            }

            try {
               //foreach (string file in Directory.GetFiles (folderName, includePattern, SearchOption.TopDirectoryOnly)) {
               //   if (File.Exists (file)) {
               //      FileInfo fileInfo = new FileInfo (file) ;
               //      folderSizeInBytes += fileInfo.Length ;
               //   }
               //}

               //foreach (string directory in Directory.GetDirectories (folderName)) {
               //   folderSizeInBytes += CalculateFolderSizeInBytes (directory, includePattern) ;
               //}

               foreach (string file in Directory.GetFiles (folderName, includePattern, SearchOption.AllDirectories)) {
                  if (File.Exists (file)) {
                     FileInfo fileInfo = new FileInfo (file) ;
                     folderSizeInBytes += fileInfo.Length ;
                  }
               }
            } catch (NotSupportedException exception) {
               // Ignore error   
               Logger.Log (LogLevel.Error, $"Unable to calculate folder size: {exception.GetAllMessages()}") ;
            }
         } catch (UnauthorizedAccessException exception) {
            // Ignore error   

            Logger.Log (LogLevel.Error, $"No access to folder {folderName}. Message: {exception.GetAllMessages()}") ;
         }
         return folderSizeInBytes ;
      }

      protected override List<CheckResult> Run2() {
         if (_maxSize <= 0f) return new List<CheckResult> {new CheckResult.InvalidParameters()} ;

         var sizeInUnit = CalculateFolderSize() ;
         var sizeIsOK = sizeInUnit <= _maxSize ;
         var maxSizeUnit = _maxSizeUnit.ToString().ToLowerInvariant() ;
         CheckResult result ;

         var data = new JObject() ;
         data [Constants.Results.FOLDER] = _folderName ;
         data [Constants.Results.INCLUDE] = IncludePattern ;
         data [Constants.Results.FOLDER_SIZE] = sizeInUnit ;
         data [Constants.Results.MAX_SIZE] = _maxSize ;
         data [Constants.Results.MAX_SIZE_UNIT] = _maxSizeUnit.ToString() ;

         var includePatternString = string.IsNullOrEmpty (IncludePattern) || IncludePattern == Constants.ALL_FILES ? "files" : $@"'{IncludePattern}' files" ;

         if (sizeIsOK) {
            result = new CheckResult (true, $@"Size of {includePatternString} in folder '{_folderName}' is {sizeInUnit} {maxSizeUnit}.", data) ;
         } else {
            result = new CheckResult (false, $@"Size of {includePatternString} in folder '{_folderName}' is {sizeInUnit} {maxSizeUnit}, greater than {_maxSize} {maxSizeUnit}.", data) ;
         }

         Logger.Log (LogLevel.Info, $"Folder size check. Pattern: {includePatternString}, size: {sizeInUnit} {maxSizeUnit}, max size: {_maxSize} {maxSizeUnit}. Result: {sizeIsOK}") ;

         return new List<CheckResult> {result} ;
      }
   }
}