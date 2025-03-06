using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Logic ;
using Sysment.Watcher.WatcherServer ;

namespace Guartinel.WatcherServer.Connection.Server.Routes.Package {
   class Update {
      public static string Route(Dictionary<string, string> request,
            IWatcherRunnerX iRunnerX) {
         string packageId;
         string agentTresholds;
         string alertDeviceId;
         string alertEmail;
         string checkerId;
         string agentName;
         try {
            packageId = request [ConnectionVars.Parameter.PACKAGE_ID];
            agentTresholds = request [ConnectionVars.Parameter.AGENT_CHECKER_THRESHOLDS];
            alertDeviceId = request [ConnectionVars.Parameter.ALERT_DEVICE_ID];
            alertEmail = request [ConnectionVars.Parameter.ALERT_EMAIL];
            packageId = request [ConnectionVars.Parameter.PACKAGE_ID];
            checkerId = request [ConnectionVars.Parameter.CHECKER_ID];
            agentName = request [ConnectionVars.Parameter.AGENT_NAME];

            } catch (KeyNotFoundException e) {
            MainForm.View.AddMsgToList("Invalid parameter in HTTP request.");
            return ConnectionVars.Content.INVALID_REQUEST_PARAMETERS;
            }
         bool success = iRunnerX.UpdatePackage(packageId, checkerId, agentTresholds, alertDeviceId, alertEmail, agentName);
         MainForm.View.AddMsgToList(String.Format("Incoming package/update request.Package name : {0}  Success: {1}", packageId, success));

         if (success) {
            return ConnectionVars.Content.SUCCESS;
            }
         return ConnectionVars.Content.INTERNAL_SYSTEM_ERROR;
         }
      }
   }
