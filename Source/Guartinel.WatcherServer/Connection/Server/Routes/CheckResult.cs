using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.WatcherServer.Communication ;
using Guartinel.WatcherServer.Logic ;

namespace Guartinel.WatcherServer.Connection.Server.Routes {
   class CheckResult {
      public static string Route(Dictionary<string, string> request, IWatcherRunnerX iRunnerX) {

         List<string> checkerIDs = new List<string>();
         string checkResult;
         try {
            foreach (KeyValuePair<string, string> keyValuePair in request) {
               if (keyValuePair.Key.Contains(ConnectionVars.Parameter.CHECKER_ID)) {
                  checkerIDs.Add(keyValuePair.Value);
                  }
               }

            checkResult = request [ConnectionVars.Parameter.CHECK_RESULT];
            } catch (KeyNotFoundException e) {
            return ConnectionVars.Content.INVALID_REQUEST_PARAMETERS;
            }
         bool success = iRunnerX.RouteAgentCheckResult(checkerIDs, checkResult);
         string checkerIdsString = string.Join(",", checkerIDs.ToArray());
      
      if (success) {
            return ConnectionVars.Content.SUCCESS;
            }
         return ConnectionVars.Content.INTERNAL_SYSTEM_ERROR;
         }
      }
   }
