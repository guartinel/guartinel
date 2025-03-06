using Guartinel.WatcherServer.Supervisors.ApplicationSupervisor ;
using Newtonsoft.Json.Linq ;
using System.Collections.Generic ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.InstanceData ;
using RequestConstants = Guartinel.Communication.Supervisors.ApplicationSupervisor.Strings.WatcherServerRoutes.RegisterResult.Request ;
using CheckResultConstants = Guartinel.Communication.Supervisors.ApplicationSupervisor.Strings.WatcherServerRoutes.RegisterResult.Request.CheckResult ;
using SaveConstants = Guartinel.Communication.Supervisors.ApplicationSupervisor.Strings.WatcherServerRoutes.Save ;

namespace Guartinel.WatcherServer.Tests.Supervisors.ApplicatonSupervisor {
   public class Configuration {
      public static void ConfigureChecker (ApplicationInstanceDataChecker checker,
                                           string packageID,
                                           ApplicationInstanceDataLists dataLists,
                                           string instanceID,
                                           string instanceName,
                                           int timeoutInSeconds = 30) {
         checker.Configure ("checker1",
                            packageID,
                            instanceID,
                            instanceName,
                            new Timeout (timeoutInSeconds * 1000),
                            dataLists.Get (instanceID),
                            false) ;
      }

      public static JObject CreateCheckResult (CheckResultKind checkResultKind,
                                               string message = "",
                                               string details = "") {
         JObject checkResult = new JObject() ;

         switch (checkResultKind) {
            case CheckResultKind.Fail:
               checkResult [CheckResultConstants.RESULT] = CheckResultConstants.CheckResultValue.FAIL ;
               break ;
            case CheckResultKind.Success:
               checkResult [CheckResultConstants.RESULT] = CheckResultConstants.CheckResultValue.SUCCESS ;
               break ;
            case CheckResultKind.CriticalFail:
               checkResult [CheckResultConstants.RESULT] = CheckResultConstants.CheckResultValue.CRITICAL ;
               break ;
            case CheckResultKind.WarningFail:
               checkResult [CheckResultConstants.RESULT] = CheckResultConstants.CheckResultValue.WARNING ;
               break ;
         }

         checkResult [CheckResultConstants.MESSAGE] = message ;
         checkResult [CheckResultConstants.DETAILS] = details ;

         return checkResult ;
      }
   }
}