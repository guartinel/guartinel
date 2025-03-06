var httpRequester = include('guartinel/connection/httpRequester.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var moment = require('moment');
var async = require('async');

var database;

exports.initDependencies = function (_database) {
   database = _database;
};
var TAG = "packageTimeStampCheckJob";
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
      monitorPackageTimeStampsOnAllServers(function () {
         LOG.info(TAG + "Finished!");
         meter.stop();
      });
   } catch (err) {
      new MSInternalServerError().logMessage(TAG + " Job interrupted").innerError(err).severe().logNow();
      meter.stop();
   } 
}
function monitorPackageTimeStampsOnAllServers(finishedCallback) {
   /*
    iterate over all ws
    ---get the timestamps
    ------ get all packages for this ws. IF package is not in the list coming from the ws THEN delete ws association from the package in the db
    -------iterate over the timestamps
    ----------get the package for the timestamp package id
    -------------IF nothing found THEN  delete package from ws
    -------------else
    -------------IF package timestamp is different then ws timestamp THEN delete ws id from the db for this package and delete package from WS and continue
    -------------IF package timestamp is the same then the one from WS then continue
    */
   config.getWatcherServers(function (err, servers) {
      if (utils.object.isNull(servers)) {
         debugRoute("Job cannot run because the lack of servers");
         return finishedCallback();
      }

      // iterate over all ws
      async.eachSeries(servers, function (server, nextServer) {
         // get the timestamps
         httpRequester.getPackagesWithTimeStamp(server, function (err, timeStamps) {
            if (err || utils.object.isNull(timeStamps)) {
               new MSInternalServerError()
                  .logMessage(TAG + " Cannot get timestamps from WS")
                  .innerError(err)
                  .severe()
                  .logNow();
            }

            //get all packages for this ws. IF package is not in the list coming from the ws THEN delete ws association from the package in the db
            checkDbPackagesPresenceInTimeStampsAsync(timeStamps, server);

            // iterate over the timestamps
            async.eachSeries(timeStamps, function (timeStamp, nextTimeStamp) {

               //  get the package for the timestamp package id
               database.getAccountByProperty('packages._id', timeStamp.packageId, function (err, account) {
                  if (err) {
                     new MSInternalServerError()
                        .logMessage(TAG + " Cannot get accounts from DB")
                        .innerError(err)
                        .severe()
                        .logNow();
                  }
                  // if nothing found then  delete package from ws
                  if (utils.object.isNull(account)) {
                     httpRequester.deletePackage(server, timeStamp.packageId, function (err) {
                        if (err) {
                           new MSInternalServerError()
                              .logMessage(TAG + " Cannot delete packages from WS")
                              .innerError(err)
                              .severe()
                              .logNow();
                        }
                        return nextTimeStamp();
                     });
                  } else {
                     var pack = account.packages.id(timeStamp.packageId);

                     //if package timestamp is different then ws timestamp then delete ws id from the db  and delete from it from WS    and process the next timeStamp
                     var packageTimeStampMoment = moment(pack.lastModificationTimeStamp).milliseconds(0);
                     var wsTimeStampMoment = moment.utc(timeStamp.timeStamp).milliseconds(0);

                     if (!packageTimeStampMoment.isSame(wsTimeStampMoment)) {
                        handleIfTimeStampsAreNotMatching(server, pack, timeStamp, account, nextTimeStamp);
                     } else {//else package timestamp is the same then the one from WS then continue
                        return nextTimeStamp();
                     }
                  }
               });
            }, function (err) {
               if (err) {
                  new MSInternalServerError()
                     .logMessage(TAG + " Timestamp check failed for package " + timeStamp.packageId)
                     .innerError(err)
                     .severe()
                     .logNow();
               }
               return nextServer();
            });
         });
      }, function (err) {
         if (err) {
            new MSInternalServerError()
               .logMessage(TAG + " Iteration interrupted")
               .innerError(err)
               .severe()
               .logNow();
         }
         return finishedCallback();
      });
   });
}


function checkDbPackagesPresenceInTimeStampsAsync(timeStamps, server) {
   if (utils.object.isNull(timeStamps)) {
      return;
   }
   //get all packages for this ws.
   database.getMultipleAccountBySubDocumentProperty('packages', 'watcherServerId', server.getId(), function (err, accounts) {
         if (err) {
            var error = new MSInternalServerError()
               .logMessage(TAG + "getMultipleAccountBySubDocumentProperty failed")
               .severe()
               .innerError(err)
               .logNow();
      }
     
      async.eachSeries(accounts, function (account, nextAccount) {
         //IF package is not in the list coming from the ws THEN delete ws association from the package in the db
         for (var packageIndex = 0; packageIndex < account.packages.length; packageIndex++) {
            var package = account.packages[packageIndex];
            if (package.isDeleted) { continue; }
            if (!package.isEnabled) { continue; }

            debugRoute("Doing package: " + package._id.toString());
            var isIdenticalPackageFoundInTimeStampList = false;
            for (var timeStampIndex = 0; timeStampIndex < timeStamps.length; timeStampIndex++) {
               var timeStamp = timeStamps[timeStampIndex];
               if (timeStamp.packageId === package._id.toString()) {
                  isIdenticalPackageFoundInTimeStampList = true;
                  break;
               }
            }
            if (!isIdenticalPackageFoundInTimeStampList) {
               debugRoute("Package watcherServerId set to null");
               debugRoute("Package id is not found in timestamps: " + JSON.stringify(timeStamps));
               package.watcherServerId = null;
            }
         }
         if (account.isModified()) {
            account.save(function (err) {
               if (err) {
                  var error = new MSInternalServerError()
                     .logMessage(TAG + " Cannot save account")
                     .severe()
                     .innerError(err)
                     .logNow();
               }
               return nextAccount();
            });
         } else {
            return nextAccount();
         }
      }, function (err) {
         if (err) {
            var error = new MSInternalServerError()
               .logMessage(TAG + " Account iteration interrupted")
               .severe()
               .innerError(err)
               .logNow();
         }
      });
   }
   );
}

function handleIfTimeStampsAreNotMatching(server, pack, timeStamp, account, nextTimeStamp) {
   pack.watcherServerId = null;
   debugRoute("Package watcherServerId set to null");
   async.series({
      deletePackageFromWS: function (deletePackageFromWSCallback) {
         httpRequester.deletePackage(server, timeStamp.packageId, function (err) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(TAG + " Cannot delete package from server.")
                  .severe()
                  .innerError(err)
                  .logNow();
            }
            return deletePackageFromWSCallback();
         });
      },
      saveAccount: function (saveAccountCallback) {
         account.save(function(err) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(TAG + " Cannot save account")
                  .severe()
                  .innerError(err)
                  .logNow();
            }
            return saveAccountCallback();
         });
      }
   }, function (err, results) {
      if (err) {
         var error = new MSInternalServerError()
            .logMessage(TAG +  " handleIfTimeStampsAreNotMatching interrupted")
            .severe()
            .innerError(err)
            .logNow();
      }
      return nextTimeStamp();
   });
}