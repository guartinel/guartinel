import { Const } from "../../../common/constants";
import { LOG } from "../../../diagnostics/LoggerFactory";
import { MSInternalServerError } from "../../../error/Errors";
var async = require('async');


var TAG = "DailyPackageAlerter";
var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(TAG + " : " + message);
}

var database;
var emailer;
exports.initDependencies = function (_database, _emailer) {
   emailer = _emailer;
   database = _database;
};


exports.run = function () {
   LOG.info(TAG + "Started!");
   var ExecutionMeterFactory = global.include("diagnostics/ExecutionMeterFactory.js");
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


//get all accounts
//iterate over them
//find for packges where package.state == alerted
//send email to the account owner about it

function doIt(onFinished) {
   debugRoute("Starting the job");
   debugRoute("Getting all accounts.");
   //get all the accounts
   database.getMultipleAccounts(null, null, afterAllAccountRetrieved);

   function afterAllAccountRetrieved(err, accounts) {
      if (err) {
         new MSInternalServerError()
            .logMessage(TAG + " Cannot get multiple accounts")
            .innerError(err)
            .severe()
            .logNow();
         return onFinished();
      }
      debugRoute("Accounts length: " + accounts.length);
      //iterate over all account
      async.eachSeries(accounts,
         function (account, doTheNextAccount) {
            debugRoute("Account " + account.email);
            if (account.packages == null) {
               return doTheNextAccount();
            }
            var alertedPackageNames = [];
            for (var packageIndex = 0; packageIndex < account.packages.length; packageIndex++) {
               if (!account.packages[packageIndex].isEnabled) {
                  debugRoute("Package is disabled");
                  continue;
               }
               if (account.packages[packageIndex].isDeleted) {
                  debugRoute("Package is deleted ");
                  continue;
               }
               if (account.packages[packageIndex].state == null) {
                  debugRoute("Package is missing state");
                  continue;
               }
             
               if (account.packages[packageIndex].state.name == Const.commonConstants.ALL_PARAMETERS.PACKAGE_STATE_ALERTING) {
                  debugRoute("Alerted package is found.");
                  alertedPackageNames.push(account.packages[packageIndex].packageName);
               }
            }

            if (alertedPackageNames.length == 0) {
               debugRoute("There is no alerted package.");
               return doTheNextAccount();
            }

            debugRoute("Account " + account.email + " Sending email");
            //we must send an email about the validation error
            emailer.sendDailyAlertedPackageSummary(account.email, alertedPackageNames, afterEmailSend);

            function afterEmailSend(err) {
               if (err) {
                  new MSInternalServerError()
                     .logMessage(TAG + " Cannot send email " + account.email)
                     .innerError(err)
                     .severe()
                     .logNow();
               } else {
                  debugRoute("Account " + account.email + " Email sent.");

               }
               return doTheNextAccount();
            }
         },

         function onAccountIterationFinished(err) {
            if (err) {
               new MSInternalServerError()
                  .innerError(err)
                  .logMessage(TAG + " onAccountIterationFinished with error")
                  .severe()
                  .logNow();
            }
            debugRoute("All account finished.");
            return onFinished();
         });
   }
}