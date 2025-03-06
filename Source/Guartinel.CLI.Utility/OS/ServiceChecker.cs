using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using System.ServiceProcess;

namespace Guartinel.CLI.Utility.OS {
   internal class ServiceChecker : ResultSending.SendResultCommandBase {
      private string _serviceName ;
      public override string Description => $"Check if a specified service running." ;

      public override string Command => "checkService" ;


      protected override void Setup2 (FluentCommandLineParser commandLineParser) {
         commandLineParser.Setup<string> ("serviceName").Required().Callback (value => _serviceName = value) ;

      }

      protected override List<CheckResult> Run2() {
         ServiceController[] services = ServiceController.GetServices() ;
         ServiceController service = services.FirstOrDefault (s => s.ServiceName == _serviceName) ;

         if (service == null) {
            service = services.FirstOrDefault (s => s.DisplayName == _serviceName) ;
         }

         if (service == null) {
            return new List<CheckResult> {new CheckResult (false, $"The service '{_serviceName}' does not exist.", null)} ;
         }

         if (service.Status.Equals (ServiceControllerStatus.Running)) {
            return new List<CheckResult> {new CheckResult (true, $"The service '{_serviceName}' is up and running.", null)} ;
         }

         return new List<CheckResult> {new CheckResult (false, $"The service '{_serviceName}' is in '{service.Status}' state.", null)} ;
      }
   }
}