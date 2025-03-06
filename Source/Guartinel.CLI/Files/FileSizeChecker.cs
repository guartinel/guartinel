using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Files {
   public class FileSizeChecker : SendResultCommandBase {
      public override string Description => $"Check size of a file or files." ;

      public new static class Constants {
         public static class Parameters {
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
            public const string PATTERN = "pattern" ;
            public const string SIZE = "size" ;
            public const string SIZE_UNIT = "sizeUnit" ;
         }
      }

      public override string Command => "checkFileSize" ;

      public string Pattern => Parameters.GetStringValue (Constants.Parameters.PATTERN, Constants.Defaults.PATTERN) ;

      public int MaxSize => Parameters.GetIntegerValue (Constants.Parameters.MAX_SIZE, Constants.Defaults.MAX_SIZE) ;

      public FileSizeUnit MaxSizeUnit {
         get {
            var value = Parameters.GetStringValue (Constants.Parameters.MAX_SIZE_UNIT, Constants.Defaults.MAX_SIZE_UNIT.ToString()) ;
            return EnumEx.Parse (value, Constants.Defaults.MAX_SIZE_UNIT) ;
         }
      }

      protected override List<CheckResult> Run2() {
         var folderName = Path.GetDirectoryName (Pattern) ;
         
         if (string.IsNullOrEmpty (folderName)) return new List<CheckResult> {
                  new CheckResult (false,
                                   $"Cannot get folder name from {Pattern}.",
                                   $"Cannot get folder name from {Pattern}.",
                                   $"Invalid folder name.",
                                   null)
         } ;
         string pattern = Path.GetFileName (Pattern) ;

         if (string.IsNullOrEmpty (pattern)) {
            pattern = Constants.Defaults.PATTERN ;
         }

         var fileNames = Directory.GetFiles (folderName, pattern, SearchOption.TopDirectoryOnly) ;
         if (!fileNames.Any()) return new List<CheckResult> {
                  new CheckResult (true,
                                   $"No file found for {Pattern}.",
                                   $"No file found for {Pattern}.",
                                   $"No file found for {Pattern}.",
                                   null)
         } ;

         var fileSizeInBytes = fileNames.Max (fileName => new FileInfo (fileName).Length) ;
         var fileSize = UnitsEx.ConvertSizeToUnit (fileSizeInBytes, MaxSizeUnit) ;

         var data = new JObject() ;
         data [Constants.Results.SIZE] = fileSize ;
         data [Constants.Results.SIZE_UNIT] = MaxSizeUnit.ToString() ;

         var sizeIsOK = fileSize <= MaxSize ;

         CheckResult result ;
         if (sizeIsOK) {
            result = new CheckResult (true,
                                      $"Size of file '{Pattern}' is OK, it is {fileSize} {MaxSizeUnit}.",
                                      $"Size of file '{Pattern}' is {fileSize} {MaxSizeUnit}, max size is {MaxSize} {MaxSizeUnit}.",
                                      $"File size is OK ({fileSize} {MaxSizeUnit}).",
                                      data) ;
         } else {
            result = new CheckResult (false,
                                      $"File '{Pattern}' is too big, it is {fileSize} {MaxSizeUnit}.",
                                      $"Size of file '{Pattern}' is {fileSize} {MaxSizeUnit}, greater than {MaxSize} {MaxSizeUnit}.",
                                      $"File is too big ({fileSize} {MaxSizeUnit}).",
                                      data) ;
         }

         _logger.Info ($"File size check. Pattern: {Pattern}, maxsize: {MaxSize} {MaxSizeUnit}. Result: {sizeIsOK}") ;

         return new List<CheckResult> {result} ;
      }
   }

   public class FileSizeCheckerCl : SendResultCommandBaseCl<FileSizeChecker> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, FileSizeChecker.Constants.Parameters.PATTERN, "Path of folder to check the files in.") ;
         SetupOption (commandLineParser, FileSizeChecker.Constants.Parameters.MAX_SIZE, "Max size of the files in the folder.") ;
         SetupOption (commandLineParser, FileSizeChecker.Constants.Parameters.MAX_SIZE_UNIT, "Unit of max size of the files (byte, kB, MB, GB, TB).") ;
      }
   }
}