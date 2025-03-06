
var timeTool = include('guartinel/utils/timeTool.js');
var async = require("async");
var TAG = "DatabaseJanitor";
var moment = require('moment');

var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(TAG + " : " + message);
}

var database;
exports.initDependencies = function (_database) {
   database = _database;
};


exports.run = function () {
   LOG.info(TAG + "Started!");
   var ExecutionMeterFactory = include("diagnostics/ExecutionMeterFactory.js");
   var meter = ExecutionMeterFactory.getExecutionMeter(TAG);
   try {
      doIt(function () {
         LOG.info(TAG + "Finished!");
         meter.stop();
      });
   } catch (err) {
      new MSInternalServerError().logMessage(TAG + " Job interrupted").innerError(err).severe().logNow();
      meter.stop();
   }
};

function doIt(onFinishCallback) {
   database.getMultipleAccounts(null, null, function (err, accounts) {
      async.eachSeries(accounts, function (account, doNextAccountCallback) {
         account.email = account.email;
         debugRoute("remove old browser token entries");
         for (var i = 0; i < account.browserSessions.length; i++) {
            if (!timeTool.isTokenTimeStampValid(account.browserSessions[i].tokenTimeStamp)) {
               account.browserSessions.id(account.browserSessions[i]._id).remove();
            }
         }
         debugRoute("remove 30 days old deleted packages");
         if (account.packages == null) {
            debugRoute("Account.packages is null");
            account.save(function (err) {
               return doNextAccountCallback();
            });
         } else {
            debugRoute("Searcing for 30 day old packages..");
            var packageIdsToRemove = [];
            for (var packageIndex = 0; packageIndex < account.packages.length; packageIndex++) {
               var pack = account.packages[packageIndex];
               if (!pack.isDeleted) { // we only looking for deleted packages
                   continue;
               }
               debugRoute("Checking deleted package last modificaton date: " + pack.packageName);
               var diff = moment().diff(pack.lastModificationTimeStamp, "days");
               debugRoute("The diff is : " + diff);
               if (diff > 90) {
                  debugRoute("Adding package to be deleted later...");
                  packageIdsToRemove.push(pack._id.toString());
                  LOG.info("Package to remove:\n" + JSON.stringify(pack));
               }
            }

            for (var idIndex = 0; idIndex < packageIdsToRemove.length; idIndex++) {
               debugRoute("Removing: " + packageIdsToRemove[idIndex]);
               account.packages.id(packageIdsToRemove[idIndex]).remove();
            }

            account.save(function (err) {
               return doNextAccountCallback();
            });
         }
      }, function (err) {
         return onFinishCallback();
      });
   });
}

