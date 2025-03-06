using System ;
using System.Collections.Generic ;
using System.IO ;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Files {
   public class FolderSizeChecker : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string FOLDER = "folder" ;
            public const string PATTERN = "pattern" ;
            public const string MAX_SIZE = "maxSize" ;
            public const string MAX_SIZE_UNIT = "maxSizeUnit" ;
         }

         public static class Defaults {
            public const string PATTERN = FileConstants.ALL_FILES ;
            public const int MAX_SIZE = 0 ;
            public const FileSizeUnit MAX_SIZE_UNIT = FileSizeUnit.GB ;
         }

         public static class Results {
            public const string FOLDER = "folder" ;
            public const string PATTERN = "pattern" ;
            public const string FOLDER_SIZE = "folder_size" ;
            public const string MAX_SIZE = "max_size" ;
            public const string MAX_SIZE_UNIT = "max_size_unit" ;
         }
      }

      public override string Description => $"Check size of a folder." ;

      public override string Command => "checkFolderSize" ;

      public string FolderName => Parameters.GetStringValue(Constants.Parameters.FOLDER, string.Empty) ;

      public string Pattern => Parameters.GetStringValue(Constants.Parameters.PATTERN, Constants.Defaults.PATTERN) ;

      public int MaxSize => Parameters.GetIntegerValue (Constants.Parameters.MAX_SIZE, Constants.Defaults.MAX_SIZE) ;

      public FileSizeUnit MaxSizeUnit {
         get {
            var value = Parameters.GetStringValue (Constants.Parameters.MAX_SIZE_UNIT, Constants.Defaults.MAX_SIZE_UNIT.ToString()) ;
            return EnumEx.Parse (value, Constants.Defaults.MAX_SIZE_UNIT) ;
         }
      }

      protected double CalculateFolderSize() {
         return UnitsEx.ConvertSizeToUnit (CalculateFolderSizeInBytes (FolderName, Pattern, _logger.Tags), MaxSizeUnit) ;
      }

      protected static long CalculateFolderSizeInBytes (string folderName,
                                                        string includePattern,
                                                        string[] tags) {         
         var logger = new TagLogger(tags);

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
               logger.Error ($"Unable to calculate folder size: {exception.GetAllMessages()}") ;
            }
         } catch (UnauthorizedAccessException exception) {
            // Ignore error   

            logger.Error($"No access to folder {folderName}. Message: {exception.GetAllMessages()}") ;
         }
         return folderSizeInBytes ;
      }

      protected override List<CheckResult> Run2() {
         if (MaxSize <= 0f) return new List<CheckResult> {new CheckResult.InvalidParameters()} ;

         var sizeInUnit = CalculateFolderSize() ;
         var sizeIsOK = sizeInUnit <= MaxSize ;
         var maxSizeUnit = MaxSizeUnit.ToString() ;
         CheckResult result ;

         var data = new JObject() ;
         data [Constants.Results.FOLDER] = FolderName ;
         data [Constants.Results.PATTERN] = Pattern ;
         data [Constants.Results.FOLDER_SIZE] = sizeInUnit ;
         data [Constants.Results.MAX_SIZE] = MaxSize ;
         data [Constants.Results.MAX_SIZE_UNIT] = MaxSizeUnit.ToString() ;

         var includePatternString = string.IsNullOrEmpty (Pattern) || Pattern == FileConstants.ALL_FILES ? "files" : $@"'{Pattern}' files" ;

         if (sizeIsOK) {
            result = new CheckResult (true,
                                      $"Size of {includePatternString} is OK ({sizeInUnit} {maxSizeUnit}).",
                                      $@"Size of {includePatternString} in folder '{FolderName}' is {sizeInUnit} {maxSizeUnit}, maximum is {MaxSize} {maxSizeUnit}.",
                                      $"Size is OK ({sizeInUnit} {maxSizeUnit}).",
                                      data) ;
         } else {
            result = new CheckResult (false,
                                      $"Size of {includePatternString} is not OK ({sizeInUnit} {maxSizeUnit}).",
                                      $@"Size of {includePatternString} in folder '{FolderName}' is {sizeInUnit} {maxSizeUnit}, greater than {MaxSize} {maxSizeUnit}.",
                                      $"Size is not OK ({sizeInUnit} {maxSizeUnit}).",
                                      data) ;
         }

         _logger.Info ($"Folder size check. Pattern: {includePatternString}, size: {sizeInUnit} {maxSizeUnit}, max size: {MaxSize} {maxSizeUnit}. Result: {sizeIsOK}") ;

         return new List<CheckResult> {result} ;
      }
   }

   public class FolderSizeCheckerCl : SendResultCommandBaseCl<FolderSizeChecker> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption(commandLineParser, FolderSizeChecker.Constants.Parameters.FOLDER, "Folder path to check the files in..");
         SetupOption(commandLineParser, FolderSizeChecker.Constants.Parameters.PATTERN, "Pattern of files with wildcards to include (all files if not specified).");
         SetupOption(commandLineParser, FolderSizeChecker.Constants.Parameters.MAX_SIZE, "Max size of the folder. If the size exceeds this, an alert will be triggered.");
         SetupOption(commandLineParser, FolderSizeChecker.Constants.Parameters.MAX_SIZE_UNIT, "Unit of max size of the folder (byte, kB, MB, GB, TB).");
      }
   }
}