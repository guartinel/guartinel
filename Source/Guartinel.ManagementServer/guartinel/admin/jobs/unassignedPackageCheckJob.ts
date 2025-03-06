
var httpRequester = global.include('guartinel/connection/httpRequester.js');
var watcherServerController = global.include('guartinel/admin/configuration/watcherServerController.js');
var database;

import { LOG } from "../../../diagnostics/LoggerFactory";
import { MSError, MSInternalServerError } from "../../../error/Errors";
import { Package } from "../../packages/package";

exports.initDependencies = function (_database) {
   database = _database;
}

var TAG = "unassignedPackageCheckjob";
var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(TAG + " : " + message);
}


exports.run = function () {
   LOG.info(TAG + " Started!");
   var ExecutionMeterFactory = global.include("diagnostics/ExecutionMeterFactory.js");
   var meter = ExecutionMeterFactory.getExecutionMeter(TAG);
   try {
      assignUnassignedPackages(function () {
         LOG.info(TAG + " Finished!");
         meter.stop();
      });
   } catch (err) {
      new MSInternalServerError().logMessage(TAG + " Job interrupted").innerError(err).severe().logNow();
      meter.stop();
   }
}


function assignUnassignedPackages(onFinished) {
   database.getMultipleAccountBySubDocumentProperty('packages', 'watcherServerId', null, function (err, accounts) {
      if (err) {
         new MSInternalServerError()
            .logMessage(TAG + " Error while fetching all accounts")
            .innerError(err)
            .severe()
            .logNow();
      }

      accountIterator(0);
      function accountIterator(i) {
         if (i === accounts.length) {
            return onFinished();
         }
         var account = accounts[i];
         packageIterator(0);

         function packageIterator(j) {
            if (j === account.packages.length) {
               return accountIterator(i + 1);
            }
             LOG.info("unassignedPackageJob doing account : " + account.email);
             LOG.info("unassignedPackageJob doing package : " + account.packages[j].packageName);

             let pack = new Package(true, account);
            pack.initFromObject(account.packages[j]);

            if (!global.utils.object.isNull(pack.watcherServerId)) { //if package already has a server or disabled  or deleted, skip it
               return packageIterator(j + 1);
            }
            if (!pack.isEnabled) {
               return packageIterator(j + 1);
            }
            if (pack.isDeleted) {
               return packageIterator(j + 1);
            }

            watcherServerController.getBestWatcherServerForThisAccount(account, function (server) {
               if (global.utils.object.isNull(server)) {
                  var error = new MSInternalServerError()
                     .logMessage(TAG + " Cannot find  watcher server for package" + pack._id.toString())
                     .severe()
                     .innerError(err)
                     .logNow();
                  return packageIterator(j + 1);
               }
               httpRequester.savePackage(server, pack, function (err) {
                  if (err) {
                     var error = new MSInternalServerError()
                        .logMessage(TAG + " Error while saving package on watcher server: " + server.getId())
                        .severe()
                        .innerError(err)
                        .logNow();
                     return packageIterator(j + 1);
                  }
                  pack.watcherServerId = server.getId();
                  account.packages.remove(pack._id);
                  account.packages.push(pack);

                  debugRoute("Package: " + pack._id + " WS id is set to: " + server.getId());
                  account.save(function (err) {
                     if (err) {
                        var error = new MSInternalServerError()
                           .logMessage(TAG + " Error while saving package in database.Package id: " + pack._id.toString())
                           .severe()
                           .innerError(err)
                           .logNow();
                     }
                     return packageIterator(j + 1);
                  });
               });
            });
         }
      }
   });
}