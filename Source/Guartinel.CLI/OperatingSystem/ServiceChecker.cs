using Guartinel.CLI.ResultSending;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guartinel.CLI.OperatingSystem {
   public class ServiceChecker : SendResultCommandBase {
      public new static class Constants {
         public static class Parameters {
            public const string SERVICE_NAME = "serviceName" ;
         }

         public static class Defaults { }

         public static class Results {
            public const string SERVICE_NAME = "serviceName" ;
         }
      }

      public override string Description => $"Check if a specified service running." ;

      public override string Command => "checkService" ;

      public string ServiceName => Parameters.GetStringValue (Constants.Parameters.SERVICE_NAME) ;

      protected override List<CheckResult> Run2() {
         _logger.Info ($"Starting ServiceChecker for {ServiceName}...") ;
         ServiceState result = OperatingSystemSelector.CurrentOperatingSystem.IsServiceRunning (ServiceName, _logger.Tags) ;

         if (result == ServiceState.Stopped) {
            return new List<CheckResult> {new CheckResult (false,
                                                           $"The service '{ServiceName}' is stopped.",
                                                           null,
                                                           $"Service stopped.",
                                                           null)} ;
         }

         if (result == ServiceState.NotFound) {
            return new List<CheckResult> {new CheckResult (false,
                                                           $"The service '{ServiceName}' is not found.",
                                                           null,
                                                           $"Service not found.",
                                                           null)} ;
         }

         return new List<CheckResult> {new CheckResult (true,
                                                        $"The service '{ServiceName}' is up and running.",
                                                        null,
                                                        $"Service is up.",
                                                        null)} ;
         //return new List<CheckResult> { new CheckResult(false, $"The service '{ServiceName}' is in '{service.Status}' state.", null) };
      }
   }

   public class ServiceCheckerCl : SendResultCommandBaseCl<ServiceChecker> {
      protected override void Setup2 (CommandLineApplication commandLineParser) {
         SetupOption (commandLineParser, ServiceChecker.Constants.Parameters.SERVICE_NAME, "Name of the service.") ;
      }
   }
}