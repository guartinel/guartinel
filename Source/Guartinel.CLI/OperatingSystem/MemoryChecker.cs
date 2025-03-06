using Guartinel.CLI.ResultSending;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guartinel.Kernel ;

namespace Guartinel.CLI.OperatingSystem {
   public class MemoryChecker : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string MIN_FREE_MEMORY_GBS = "minFreeMemoryGBs" ;
            public const string MIN_FREE_MEMORY_PERCENTS = "minFreeMemoryPercents" ;
         }

         public static class Defaults {
            public const double MIN_FREE_MEMORY_GBS = 0.5 ;
            public const int MIN_FREE_MEMORY_PERCENTS = 15 ;
         }

         public static class Results {
            public const string FREE_MEMORY_GBS = "freeMemoryGbs" ;
            public const string FREE_MEMORY_PERCENTS = "freeMemoryPercents" ;
         }
      }

      public override string Description => $"Check free amount of memory." ;

      public override string Command => "checkMemory" ;

      public double MinFreeMemoryGBs => Parameters.GetDoubleValue (Constants.Parameters.MIN_FREE_MEMORY_GBS, Constants.Defaults.MIN_FREE_MEMORY_GBS) ;

      public int MinFreeMemoryPercents => Parameters.GetIntegerValue (Constants.Parameters.MIN_FREE_MEMORY_PERCENTS, Constants.Defaults.MIN_FREE_MEMORY_PERCENTS) ;

      protected override List<CheckResult> Run2() {
         if (MinFreeMemoryGBs <= 0 &&
             MinFreeMemoryPercents <= 0)
            return new List<CheckResult> {
                     new CheckResult.InvalidParameters()
            } ;

         long totalMemoryByte = OperatingSystemSelector.CurrentOperatingSystem.GetTotalMemoryBytes(_logger.Tags) ;
         double totalMemoryGBs = UnitsEx.ConvertSizeToUnit (totalMemoryByte, FileSizeUnit.GB) ;

         long freeMemoryByte = OperatingSystemSelector.CurrentOperatingSystem.GetFreeMemoryBytes (_logger.Tags) ;
         double freeMemoryGBs = UnitsEx.ConvertSizeToUnit (freeMemoryByte, FileSizeUnit.GB) ;

         int freeMemoryPercents = Converter.Percent (freeMemoryGBs, totalMemoryGBs) ;

         var checkData = new JObject() ;
         checkData [Constants.Results.FREE_MEMORY_GBS] = freeMemoryGBs.NormalizeValue() ;
         checkData [Constants.Results.FREE_MEMORY_PERCENTS] = freeMemoryPercents ;

         if ((MinFreeMemoryGBs > 0) && (freeMemoryGBs < MinFreeMemoryGBs)) {
            string message = $@"Free memory ({freeMemoryGBs} GBs) is not enough." ;
            string details = $@"Free memory is {freeMemoryGBs} GBs, less than {MinFreeMemoryGBs} GBs.";
            string extract = $@"Insufficient memory {freeMemoryGBs} GBs.";
            _logger.Info($@"Free memory GBs check fail. {message}") ;
            return new List<CheckResult> {new CheckResult (false, message, details, extract, checkData) } ;
         } else if ((MinFreeMemoryPercents > 0) && (freeMemoryPercents < MinFreeMemoryPercents)) {
            string message = $@"Free memory ({freeMemoryPercents}%) is not enough.";
            string details = $@"Free memory is {freeMemoryPercents}%, less than {MinFreeMemoryPercents}%.";
            string extract = $@"Insufficient memory {freeMemoryPercents}%.";

            _logger.Info ($@"Free memory percent check fail. {message}") ;
            return new List<CheckResult> {new CheckResult (false, message, details, extract, checkData)} ;
         } else {
            string message = $@"Free memory ({freeMemoryGBs} GBs and {freeMemoryPercents}%) is OK." ;
            string details = $@"Free memory is {freeMemoryGBs} GBs ({freeMemoryPercents}%). Minimum values are {MinFreeMemoryGBs} GBs and {MinFreeMemoryPercents}%." ;
            string extract = $@"Memory OK {freeMemoryGBs} GBs ({freeMemoryPercents}%).";
            _logger.Info (message) ;

            return new List<CheckResult> {new CheckResult (true, message, details, extract, checkData) } ;
         }
      }
   }

   public class MemoryCheckerCl : SendResultCommandBaseCl<MemoryChecker> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, MemoryChecker.Constants.Parameters.MIN_FREE_MEMORY_GBS, "Minimum free memory in GBs.") ;
         SetupOption (commandLineParser, MemoryChecker.Constants.Parameters.MIN_FREE_MEMORY_PERCENTS, "Minimum free memory in percents.") ;
      }
   }
}