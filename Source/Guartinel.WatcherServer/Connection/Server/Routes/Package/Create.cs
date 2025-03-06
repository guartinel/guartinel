using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Logic ;
using Sysment.Watcher.WatcherServer ;

namespace Guartinel.WatcherServer.Connection.Server.Routes.Package {
   class Create {
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
            checkerId = request [ConnectionVars.Parameter.CHECKER_ID];
            agentName = request [ConnectionVars.Parameter.AGENT_NAME];

            } catch (KeyNotFoundException e) {
            MainForm.View.AddMsgToList("Invalid parameter in HTTP request.");
            return ConnectionVars.Content.INVALID_REQUEST_PARAMETERS + " : " + e.Message;
            }
         bool success = iRunnerX.CreatePackage(packageId, agentTresholds, alertDeviceId, alertEmail, checkerId, agentName);
         MainForm.View.AddMsgToList(String.Format("Incoming package/create request.Package name : {0}  Success: {1}", packageId, success));

         if (success) {
            return ConnectionVars.Content.SUCCESS;
            }
         return ConnectionVars.Content.INTERNAL_SYSTEM_ERROR;
         }
      }
   }
