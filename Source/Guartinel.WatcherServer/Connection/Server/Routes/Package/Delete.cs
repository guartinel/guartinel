using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Logic ;
using Sysment.Watcher.WatcherServer ;

namespace Guartinel.WatcherServer.Connection.Server.Routes.Package {
   class Delete {
      public static string Route(Dictionary<string, string> request,
            IWatcherRunnerX iRunnerX) {
         string packageId;

         try {
            packageId = request [ConnectionVars.Parameter.PACKAGE_ID];
            } catch (KeyNotFoundException e) {
            return ConnectionVars.Content.INVALID_REQUEST_PARAMETERS + " : " + e.Message;
            }
         bool success = iRunnerX.DeletePackage(packageId);
         MainForm.View.AddMsgToList(String.Format("Incoming package/delete request.Package name : {0}  Success: {1}", packageId, success));

         if (success) {
            return ConnectionVars.Content.SUCCESS;
            }
         return ConnectionVars.Content.INTERNAL_SYSTEM_ERROR;
         }

      }
   }
