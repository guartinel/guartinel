var config = include('guartinel/admin/configuration/configurationController.js');
var async = require('async');

var database;
exports.initDependencies = function (_database) {
   database = _database;
}
var TAG = "monitorWatchersJob";
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
      monitorWatcherServerIds(function () {
         LOG.info(TAG + "Finished!");
         meter.stop();
      });
   } catch (err) {
      new MSInternalServerError().logMessage(TAG + " Job interrupted").innerError(err).severe().logNow();
      meter.stop();
   }
}

function monitorWatcherServerIds(finishedCallback) {
   //6 LOG.info("monitorWatcherServerIds started")
   database.getMultipleAccounts(null, null, function (err, accounts) {
      if (err) {
         var error = new MSInternalServerError()
            .logMessage(TAG + "Cannot get multiple accounts")
            .severe()
            .innerError(err)
            .logNow();
      }
      config.getWatcherServers(function (err, servers) {
         if (err) {
            var error = new MSInternalServerError()
               .logMessage(TAG + "Cannot get watcher servers")
               .severe()
               .innerError(err)
               .logNow();
         }
         debugRoute("monitorWatcherServerIds accounts length:" + accounts.length + "servers length: " + servers.length);

         async.eachSeries(accounts, function (account, nextAccount) {
            debugRoute("doing account: " + account.email);
            debugRoute("packages length: " + account.packages.length);
            var isModified = false;
            for (var i = 0; i < account.packages.length; i++) {
               if (account.packages[i].watcherServerId == null) { continue; }
               var foundServerForPackage = false;
               for (var j = 0; j < servers.length; j++) {
                  if (account.packages[i].watcherServerId === servers[j].getId()) {
                     debugRoute(" found watcher server for package:" + account.packages[i]._id);
                     foundServerForPackage = true;
                     break;
                  }
               }
               if (!foundServerForPackage) {
                  debugRoute("NOT found watcher server for package:" + account.packages[i]._id);
                  debugRoute("Package watcherServerId set to null");
                  account.packages[i].watcherServerId = null;
                  isModified = true;
               }
            }

            if (isModified) {
               debugRoute("account is modified. saving: "+ account.email) ;
               account.save(function (err) {
                  if (err) {
                     var error = new MSInternalServerError()
                        .logMessage(TAG + " Cannot save account")
                        .severe()
                        .innerError(err)
                        .logNow();
                  }
                  nextAccount();
               });
            } else {
               debugRoute("account is NOT  modified.cotinue : "+ account.email) ;
               nextAccount();
            }
         }, function (err) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(TAG + " Account iteration interrupted")
                  .severe()
                  .innerError(err)
                  .logNow();
            }
            return finishedCallback();
         });
      });
   });
}
