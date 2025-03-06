using System;
using System.Collections.Generic ;
using Guartinel.CLI.ResultSending ;
using Guartinel.Kernel.Utility ;
using Microsoft.Extensions.CommandLineUtils ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.CLI.Files {
   public abstract class FreeSpaceCheckerBase : SendResultCommandBase {

      public new static class Constants {
         public static class Parameters {
            public const string MIN_SPACE_GBS = "minSpaceGBs" ;
            public const string MIN_SPACE_PERCENTS = "minSpacePercents" ;
         }

         public static class Defaults {
            // public const int MIN_SPACE_GBS = 1 ;
            // public const int MIN_SPACE_PERCENTS = 20 ;
         }

         public static class Results {
            public const string FREE_SPACE_GB = "freeSpaceGbs" ;
            public const string FREE_SPACE_PERCENTS = "freeSpacePercents" ;
         }
      }

      public int? MinSpaceGBs => Parameters.GetIntegerValueNull (Constants.Parameters.MIN_SPACE_GBS) ;

      public int? MinSpacePercents => Parameters.GetIntegerValueNull (Constants.Parameters.MIN_SPACE_PERCENTS) ;

      protected abstract string Target {get ;}

      protected abstract FreeSpace GetSpaceInfo() ;

      protected override List<CheckResult> Run2() {
         if (MinSpaceGBs <= 0f &&
             MinSpacePercents <= 0) return new List<CheckResult> {new CheckResult.InvalidParameters()} ;

         var spaceInfo = GetSpaceInfo() ;
         if (spaceInfo == null)
            return new List<CheckResult> {
                     new CheckResult (false, "Space info not available.",
                                      "Space info not implemented on this system.",
                                      "Not available.", null)
            } ;

         var checkData = new JObject() ;
         checkData [Constants.Results.FREE_SPACE_GB] = spaceInfo.FreeSpaceGBs ;
         checkData [Constants.Results.FREE_SPACE_PERCENTS] = spaceInfo.FreeSpacePercents ;

         string message ; 
         string details ;
         string extract ;

         if (MinSpaceGBs != null && spaceInfo.FreeSpaceGBs < MinSpaceGBs) {
            message = $@"Free space is not enough {spaceInfo.FreeSpaceGBs} GBs, less than {MinSpaceGBs} GBs." ;
            details = $@"Free space is not enough for {Target} is {spaceInfo.FreeSpaceGBs} GBs, less than {MinSpaceGBs} GBs." ;
            extract = $@"Space is {spaceInfo.FreeSpaceGBs} GBs, < {MinSpaceGBs} GBs." ;
            _logger.Info ($@"Free space GBs check fail. {details}") ;
            return new List<CheckResult> {new CheckResult (false, message, details, extract, checkData)} ;
         }

         if (MinSpacePercents != null && spaceInfo.FreeSpacePercents < MinSpacePercents) {
            message = $@"Free space is not enough is {spaceInfo.FreeSpacePercents}%, less than {MinSpacePercents}%." ;
            details = $@"Free space is not enough for {Target} is {spaceInfo.FreeSpacePercents}%, less than {MinSpacePercents}%." ;
            extract = $@"Space is {spaceInfo.FreeSpacePercents}%, < {MinSpacePercents}%." ;
            _logger.Info ($@"Free space percent check fail. {details}") ;
            return new List<CheckResult> {new CheckResult (false, message, details, extract, checkData)} ;
         }

         message = $@"Free space is {spaceInfo.FreeSpaceGBs} GBs ({spaceInfo.FreeSpacePercents}%)." ;
         details = $@"Free space for {Target} is {spaceInfo.FreeSpaceGBs} GBs ({spaceInfo.FreeSpacePercents}%)." ;
         extract = $@"Space is OK {spaceInfo.FreeSpaceGBs} GBs ({spaceInfo.FreeSpacePercents}%)." ;
         _logger.Info ($@"Check successful, free space for {Target} is {spaceInfo.FreeSpaceGBs} GBs ({spaceInfo.FreeSpacePercents}%).") ;

         return new List<CheckResult> {new CheckResult (true, message, details, extract, checkData)} ;

      }
   }

   public abstract class FreeSpaceCheckerBaseCl<T> : SendResultCommandBaseCl<T> where T : FreeSpaceCheckerBase, new() {
      protected sealed override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption(commandLineParser, FreeSpaceCheckerBase.Constants.Parameters.MIN_SPACE_GBS, "Minimum free space on drive in GBs.");
         SetupOption(commandLineParser, FreeSpaceCheckerBase.Constants.Parameters.MIN_SPACE_PERCENTS, "Minimum free space on drive in percents.");

         Setup3(commandLineParser);
      }

      protected abstract void Setup3 (CommandLineApplication commandLineParser);
   }
}