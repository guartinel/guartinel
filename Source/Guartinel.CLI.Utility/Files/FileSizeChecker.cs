using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using Fclp ;
using Guartinel.CLI.Utility.Commands ;
using Guartinel.CLI.Utility.ResultSending ;
using Guartinel.Core.Logging ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Utility.Files {
   public class FileSizeChecker : SendResultCommandBase {
      private string _pattern;
      private double _maxSize ;

      public override string Description => $"Check size of a file or files." ;
      public new static class Constants {
         public const string ALL_FILES = "*.*";

         public static class Parameters {
            public const string PATTERN = "pattern";
            public const string MAX_SIZE = "maxSize";            
         }
      }
      public override string Command => "checkFileSize" ;

      protected override void Setup2 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<string> (Constants.Parameters.PATTERN).Required( ).Callback (value => _pattern = value) ;
         commandLineParser.Setup<double> (Constants.Parameters.MAX_SIZE).Required().Callback (value => _maxSize = value) ;
      }

      protected override List<CheckResult> Run2 () {
         var folderName = Path.GetDirectoryName (_pattern) ;
         if (string.IsNullOrEmpty (folderName)) return new List<CheckResult> { new CheckResult (false, $"Cannot get folder name from {_pattern}.", null)} ;
         string pattern = Path.GetFileName (_pattern) ;
         if (string.IsNullOrEmpty (pattern)) {
            pattern = Constants.ALL_FILES ;
         }

         var fileNames = Directory.GetFiles (folderName, pattern, SearchOption.TopDirectoryOnly) ;
         // if (string.IsNullOrEmpty (fileName)) return new CheckResult (true, $"No file found for {_pattern}.", null) ;
         if (!fileNames.Any()) return new List<CheckResult> {new CheckResult (true, $"No file found for {_pattern}.", null)} ;

         var fileSize = fileNames.Max (fileName => new FileInfo (fileName).Length) ;
         
         var data = new JObject() ;
         data ["file_size"] = fileSize ;

         var sizeIsOK = fileSize <= _maxSize;

         CheckResult result ;         
         if (sizeIsOK) {
            result = new CheckResult (true, $"Size of file '{_pattern}' is {fileSize}.", data) ;
         } else {
            result = new CheckResult (false, $"Size of file '{_pattern}' is {fileSize}, greater than {_maxSize}.", data) ;
         }

         Logger.Log (LogLevel.Info, $"File size check. Pattern: {_pattern}, maxsize: {_maxSize}. Result: {sizeIsOK}") ;

         return new List<CheckResult> { result} ;
      }
   }
}