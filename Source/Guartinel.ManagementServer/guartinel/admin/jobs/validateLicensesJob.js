/**
 * Created by DTAP on 2017.08.09..
 */

var async = require('async');
var TAG = "ValidateLicensesJob";
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


//get all accounts
//iterate over them
//get the biggest valid license for the current package
//if account has active license which is big enough for his licenses then check if any of his license has 0 days remaining
//if user has a license which has 0 days remaining but the current biggest license is enough big for his remaning packages then only alter the minimum check intervals

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

            var hasValidLicense = false;
            //get the biggest active license for account
            var aggregate = account.getLicenseAggregate();
            debugRoute("PackagesCount " +
               account.getPackageCount() +
               " max pack constraint from biggest license : " +
               aggregate.maximumPackages);

            var isUserPackageCountOK = account.getPackageCount() <= aggregate.maximumPackages;
            var isUserPackagePartCountOK = account.getAllPackagePartCount() <= aggregate.maximumPackagePartCount;
            var isUserPackagesCheckIntervalOK = account.getTheSmallestSetPackageCheckInterval() >= aggregate.minimumCheckIntervalSec;
            debugRoute("Minimum check interval from licenses: " + aggregate.minimumCheckIntervalSec);
            debugRoute("isUserPackageCountOK: " + isUserPackageCountOK + " isUserPackagePartCountOK " + isUserPackagePartCountOK + " isUserPackagesCheckIntervalOK " + isUserPackagesCheckIntervalOK); 
            if (isUserPackageCountOK && isUserPackagePartCountOK) {// TODO maybe needed... !utils.object.isNull(aggregate)
               debugRoute("Account " + account.email + " has a big enough license for his packages count.");
               hasValidLicense = true;
            }
            // check if he has any license that is expiring today
            var todayExpiredLicenses = account.getTodayExpiringLicenses();
            var todayExpiredLicense = todayExpiredLicenses[0]; // TODO modify to work with the array
            debugRoute("Today expiring licenses count " + todayExpiredLicenses.length);
            if (hasValidLicense && utils.object.isNull(todayExpiredLicense) && isUserPackagesCheckIntervalOK ) {
               debugRoute("User licenses are OK.");
               return doTheNextAccount();
            }

            //when user has smaller check interval then minimum but no license is expired today 
            if (!isUserPackagesCheckIntervalOK && hasValidLicense && utils.object.isNull(todayExpiredLicense)) {
               debugRoute("User has valid licenses and none of its license expiring today, but one of its packages has bigger check interval then it should. Changing it silently to a valid value");
               alterPackages(aggregate.minimumCheckIntervalSec, false);
               account.save(function (err) {
                  if (err) {
                     new MSInternalServerError()
                        .logMessage(TAG + " Cannot save account after modifiing package check intervals")
                        .innerError(err)
                        .severe()
                        .logNow();
                  }
                  debugRoute("Account saved. Lets do the next one");
                  return doTheNextAccount();
               })
            }
            var messageForTheUser = "";

            if (hasValidLicense && !utils.object.isNull(todayExpiredLicense)) {
               alterPackages(aggregate.minimumCheckIntervalSec, false);
               debugRoute(
                  "User has a big enough license for his packages but also a today expired license too. Alter his packages min check interval");

               debugRoute("Saving his account after db changes");

               messageForTheUser = "Your license : " +
                  todayExpiredLicense.license.caption +
                  " is expired today.\nFortunately  you can keep your remaining packages with your other valid licenses.\n" +
                  " We also changed your packages minimum check interval to " +
                  aggregate.minimumCheckIntervalSec +
                  " sec. If you wish to use a smaller timeout please renew your license.";
               account.save(afterAccountSave);
            }

            if (!hasValidLicense) {
               alterPackages(aggregate.minimumCheckIntervalSec, true);

               messageForTheUser =
                  "Unfortunately your account does not have a valid license and therefore you cannot keep all your existing packages.\n" +
                  " Your packages have been disabled and the minimum check intervals have been set to " +
                  aggregate.minimumCheckIntervalSec +
                  " sec. If you wish to use the current number of packages or smaller check intervals, please renew your license.";

               account.save(afterAccountSave);
            }


            function alterPackages(minimumCheckIntervalSec, shouldDisablePackages) {
               for (var packageIndex = 0; packageIndex < account.packages.length; packageIndex++) {
                  var pack = account.packages[packageIndex];
                  debugRoute("Account " +
                     account.email +
                     " Alter package settings to meet his valid license constraints in package name: " +
                     pack.packageName +
                     " Minimum check interval: " + minimumCheckIntervalSec
                  );
                  if (shouldDisablePackages) {
                     pack.isEnabled = false;
                     pack.watcherServerId = null;
                     debugRoute("Disabling package");
                  }
                  if (pack.checkIntervalSeconds < minimumCheckIntervalSec) {
                     pack.checkIntervalSeconds = minimumCheckIntervalSec;
                     pack.watcherServerId = null;
                     debugRoute("Modifiing package check interval to: " + minimumCheckIntervalSec);
                  }
               }
            }

            function afterAccountSave(err) {
               if (err) {
                  new MSInternalServerError()
                     .logMessage(TAG + " Cannot save account")
                     .innerError(err)
                     .severe()
                     .logNow();
               }

               debugRoute("Account " + account.email + " Sending email");
               //we must send an email about the validation error
               emailer.sendEmailAboutExpiredLicense(account.email, messageForTheUser, afterEmailSend);
            }

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