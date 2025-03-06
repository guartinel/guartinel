var timeTool = include("guartinel/utils/timeTool.js");
var config = include('guartinel/admin/configuration/configurationController.js');

var database;
exports.initDependencies = function(_database) {
   database = _database;
}

var TAG = "serverAvailabilityCheckJob";
var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(TAG + " : " + message);
}


exports.run = function () {
   LOG.info(TAG + "Started!");
   var ExecutionMeterFactory = include("diagnostics/ExecutionMeterFactory.js");
   var meter = ExecutionMeterFactory.getExecutionMeter(TAG);
   try {
      checkServerAvailability(function () {
         LOG.info(TAG + "Finished!");
         meter.stop();
      });
   } catch (err) {
      new MSInternalServerError().logMessage(TAG + " Job interrupted").innerError(err).severe().logNow();
      meter.stop();
   } 
     
}

 function checkServerAvailability(onFinished) {
   config.getWatcherServers(function (err, servers) {
      if (utils.object.isNull(servers)) { return onFinished(); }
      debugRoute("Updating watcher server infos.Servers: " + servers.length);

      serverIterator(0);
      function serverIterator(serverIndex) {
         if (serverIndex == servers.length) {
            return onFinished();
         }
         var TEN_MINUTE = 60 * 10;
         if (timeTool.isAmountSecElapsedFromDate(TEN_MINUTE, servers[serverIndex].getStressLevelTimeStamp())) {
            servers[serverIndex].setIsEnabled(false);
            database.getMultipleAccountBySubDocumentProperty("packages", "watcherServerId", servers[serverIndex].id, function (err, accounts) {
               if (utils.object.isNull(accounts) || accounts.length == 0) {
                  return serverIterator(serverIndex + 1);
               }
               accountIterator(0);

               function accountIterator(accountIndex) {
                  if (accountIndex = accounts.length) {
                     return serverIterator(serverIndex + 1);
                  }
                  var account = accounts[accountIndex];
                  if (utils.object.isNull(account.packages)) {
                     return accountIterator(accountIndex + 1);
                  }
                  for (var packageIndex = 0; packageIndex < account.packages.length; packageIndex++) {
                     debugRoute("Package watcherServerId set to null");
                     accounts[accountIndex].packages[packageIndex].watcherServerId = null;
                  }

                  account.save(function (err) {
                     if (err) {
                        new MSInternalServerError()
                           .logMessage(TAG + " Cannot save account after watcherserverid delete from ")
                           .innerError(err)
                           .severe()
                           .logNow();
                     }
                     return accountIterator(accountIndex + 1);
                  });
               }

               return serverIterator(serverIndex + 1);
            });
         }

      }
   });
}