/**
 * Created by DTAP on 2017.08.09..
 */

var async = require('async');
var timeTool = include('guartinel/utils/timeTool.js');
var moment = require('moment');

var TAG = "WarnAboutLicenseExpirationJob";
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
}
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


function doIt(onFinished) {
   //get all the accounts
   database.getMultipleAccounts(null, null, afterAllAccountRetrieved);
   function afterAllAccountRetrieved(err, accounts) {
      if (err) {
         if (err) {
            new MSInternalServerError()
               .logMessage(TAG + " Error in  afterAllAccountRetrieved")
               .innerError(err)
               .severe()
               .logNow();
         }
         return onFinished();
      }
      //iterate over all account
      async.eachSeries(accounts, onNextAccount, onAccountIterationFinished);

      function onNextAccount(account, doTheNextAccount) {
         debugRoute("Account " + account.email);

         var expiringLicenses = account.getLicenseWithRemainingDays([7, 1]);
         debugRoute("Expiring licenses count: " + expiringLicenses.length);
         if (expiringLicenses.length == 0) {
            debugRoute("Licenses are OK for this account");
            return doTheNextAccount();
         }
         debugRoute("Licenses are going to be expired for this account");

         emailer.sendWarningAboutLicenseExpiration(account.email, expiringLicenses, afterEmailSend);

         function afterEmailSend(err) {
            if (err) {
               new MSInternalServerError()
                  .logMessage(TAG + " Failed to send a warning email " + account.email)
                  .innerError(err)
                  .severe()
                  .logNow();
            }
            return doTheNextAccount();
         }
      }

      function onAccountIterationFinished(err) {
         if (err) {
            new MSInternalServerError()
               .logMessage(TAG + " onAccountIterationFinished with err")
               .innerError(err)
               .severe()
               .logNow();
         }
         debugRoute("All account finished");
         return onFinished();
      }
   }
}