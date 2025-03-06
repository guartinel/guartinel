using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Guartinel.CLI.ResultSending;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Guartinel.CLI.Files {
   public class FileChangeChecker : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string FOLDER = "folder" ;
            public const string COOL_DOWN = "coolDown" ;
            public const string COOL_DOWN_UNIT = "coolDownUnit" ;
            public const string PATTERN = "pattern" ;
            public const string SUB_FOLDERS = "subFolders" ;
            public const string RESULTS_FOLDER = "resultsFolder" ;
         }

         public static class Defaults {
            public const string PATTERN = FileConstants.ALL_FILES ;
            public const int COOL_DOWN = 1 ;
            public const TimeUnit COOL_DOWN_UNIT = TimeUnit.Hour ;
            public static readonly string RESULTS_FOLDER = Path.Combine (AssemblyEx.GetAssemblyPath<string>(), "Results", "FileChangeChecker") ;
         }

         public static class Results {
            public const string LAST_MODIFICATION = "last_modification" ;
            public const string IS_COOL_DOWN_IN_PROGRESS = "is_cool_down_in_progress" ;
         }
      }

      public override string Description => "Check and alert if file is modified." ;
      public override string Command => "checkFileChange" ;

      public string FolderName => Parameters.GetStringValue (Constants.Parameters.FOLDER, string.Empty) ;
      public string Pattern => Parameters.GetStringValue (Constants.Parameters.PATTERN, Constants.Defaults.PATTERN) ;
      public bool SubFolders => Parameters.GetBooleanValue (Constants.Parameters.SUB_FOLDERS, false) ;

      public string ResultsFolder => Parameters.GetStringValue (Constants.Parameters.RESULTS_FOLDER, Constants.Defaults.RESULTS_FOLDER) ;
      public int CoolDown => Parameters.GetIntegerValue (Constants.Parameters.COOL_DOWN, Constants.Defaults.COOL_DOWN) ;

      public TimeUnit CoolDownUnit {
         get {
            var value = Parameters.GetStringValue (Constants.Parameters.COOL_DOWN_UNIT, Constants.Defaults.COOL_DOWN_UNIT.ToString()) ;
            return EnumEx.Parse (value, Constants.Defaults.COOL_DOWN_UNIT) ;
         }
      }

      protected override List<CheckResult> Run2() {
         if (CoolDown <= 0) return new List<CheckResult> {new CheckResult.InvalidParameters()} ;
         DateTime lastModificationTimeStamp = FilesEx.GetLatestFileModificationTimeStamp (FolderName, SubFolders, Pattern) ;

         var data = new JObject() ;
         data [Constants.Results.IS_COOL_DOWN_IN_PROGRESS] = false ;
         data [Constants.Results.LAST_MODIFICATION] = lastModificationTimeStamp ;

         MeasurementsContainer measurementsContainer = new MeasurementsContainer (ResultsFolder, InstanceID, _logger.Tags) ;

         // Add cooldown time to last measurement
         DateTime coolDownExpireMoment = measurementsContainer.Result.ModificationTimeStamp.AddSeconds(UnitsEx.ConvertTimeToSeconds(CoolDown, CoolDownUnit));

         // No previous measurement =>FIRST MEASUREMENT
         if (measurementsContainer.Result == null) {
            measurementsContainer.Result = new FileChangeResult() {InstanceId = InstanceID, ModificationTimeStamp = lastModificationTimeStamp} ;
            measurementsContainer.Save() ;
            return new List<CheckResult> {new CheckResult (true, $"File modification date '{lastModificationTimeStamp.ToString()}' is OK.",
                                                                 $"File modification date '{lastModificationTimeStamp.ToString()}' in folder {FolderName} is OK. Maximum is {coolDownExpireMoment.ToString()}.",
                                                                 "File modification date is OK.",
                                                                 data)} ;
         }

         measurementsContainer.Result.ModificationTimeStamp = lastModificationTimeStamp ;
         measurementsContainer.Save() ;

         if (DateTime.UtcNow > coolDownExpireMoment) {
            return new List<CheckResult> {new CheckResult (true,
                                                           $"File modification date '{lastModificationTimeStamp.ToString()}' is OK.",
                                                           $"File modification date '{lastModificationTimeStamp.ToString()}' in folder {FolderName} is OK. Maximum is {coolDownExpireMoment.ToString()}.",
                                                           "File modification date is OK.",
                                                           data)} ;
         }

         // Still cooldown
         data [Constants.Results.IS_COOL_DOWN_IN_PROGRESS] = true ;
         return new List<CheckResult> {new CheckResult (false,
                                                        $"File modification date '{lastModificationTimeStamp.ToString()}' is not OK.",
                                                        $"File modification date '{lastModificationTimeStamp.ToString()}' in folder {FolderName} is not OK. Maximum is {coolDownExpireMoment.ToString()}.",
                                                        "File modification date is not OK.",
                                                        data)} ;
      }

      public class MeasurementsContainer {
         private readonly string _resultsPath ;

         public MeasurementsContainer (string folder,
                                       string instanceID,
                                       string[] tags) {
            var logger = new TagLogger(tags);

            _resultsPath = Path.Combine (folder, $"instance-{instanceID}.json") ;
            if (!Directory.Exists (Path.GetDirectoryName (_resultsPath))) {
               Directory.CreateDirectory (Path.GetDirectoryName (_resultsPath)) ;
            }

            Result = null ;
            if (File.Exists (_resultsPath)) {
               string resultString = File.ReadAllText (_resultsPath) ;
               try {
                  Result = JsonConvert.DeserializeObject<FileChangeResult> (resultString) ;
               } catch (Exception e) {
                  logger.Error ($"Cannot load previous results {e.Message}.") ;
               }
            }
         }

         public void Save() {
            File.WriteAllText (_resultsPath, JsonConvert.SerializeObject (Result)) ;
         }

         public FileChangeResult Result {get ; set ;}
      }

      public class FileChangeResult {
         public string InstanceId {get ; set ;}
         public DateTime ModificationTimeStamp {get ; set ;}
      }
   }

   public class FileChangeCheckerCl : SendResultCommandBaseCl<FileChangeChecker> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, FileChangeChecker.Constants.Parameters.FOLDER, "Folder of the files to check.") ;
         SetupOption (commandLineParser, FileChangeChecker.Constants.Parameters.PATTERN, "File name or pattern with wildcards.") ;
         SetupOptionBoolean (commandLineParser, FileChangeChecker.Constants.Parameters.SUB_FOLDERS, "Check files in in subfolders as well.") ;

         SetupOption (commandLineParser, FileChangeChecker.Constants.Parameters.RESULTS_FOLDER, "Store results in this folder.") ;
         SetupOption (commandLineParser, FileChangeChecker.Constants.Parameters.COOL_DOWN, "If the folder content changed again within the cooldown period no new alert will be sent.") ;
         SetupOption (commandLineParser, FileChangeChecker.Constants.Parameters.COOL_DOWN_UNIT, "The time unit of the cool down value (second, minute, hour, day).") ;
      }
   }
}