using Guartinel.CLI.ResultSending;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guartinel.CLI.OperatingSystem {
   public class ProcessorChecker : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string MAX_PROCESSOR_USAGE_PERCENTS = "maxProcessorUsagePercents" ;
         }

         public static class Defaults {
            public const double MAX_PROCESSOR_USAGE_PERCENTS = 50 ;
         }

         public static class Results {
            public const string PROCESSOR_USAGE_PERCENTS = "processorUsagePercents" ;
         }
      }

      public ProcessorChecker() { }

      public override string Description => $"Check processor utilization." ;

      public override string Command => "checkProcessor" ;

      public double MaximumProcessorUsage => Parameters.GetDoubleValue (Constants.Parameters.MAX_PROCESSOR_USAGE_PERCENTS,
                                                                        Constants.Defaults.MAX_PROCESSOR_USAGE_PERCENTS) ;

      protected override List<CheckResult> Run2() {
         if (MaximumProcessorUsage <= (double) 0)
            return new List<CheckResult> {
                     new CheckResult.InvalidParameters()
            } ;

         double processorUsagePercent = OperatingSystemSelector.CurrentOperatingSystem.GetProcessorUsage (_logger.Tags) ;

         var checkData = new JObject() ;
         checkData [Constants.Results.PROCESSOR_USAGE_PERCENTS] = processorUsagePercent.NormalizeValue() ;

         if (processorUsagePercent > MaximumProcessorUsage) {
            string message = $@"CPU usage is {processorUsagePercent}% is too high." ;
            string details = $@"Processor utilization is too high ({processorUsagePercent}%), higher than {MaximumProcessorUsage}%.";
            string extract = $@"CPU is too high ({processorUsagePercent}%).";
            return new List<CheckResult> {new CheckResult (false, message, details, extract, checkData)} ;
         } else {
            string message = $@"CPU usage is OK ({processorUsagePercent}%)." ;
            string details = $@"Processor utilization is OK ({processorUsagePercent}%), maximum value is {MaximumProcessorUsage}%." ;
            string extract = $@"CPU is OK ({processorUsagePercent}%).";
            return new List<CheckResult> {new CheckResult (true, message, details, extract, checkData)} ;
         }
      }
   }

   public class ProcessorCheckerCl : SendResultCommandBaseCl<ProcessorChecker> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, ProcessorChecker.Constants.Parameters.MAX_PROCESSOR_USAGE_PERCENTS, "Maximum processor utilization percent.") ;
      }
   }
}