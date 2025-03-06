var sessionManager = global.include("guartinel/security/sessionManager.js");
var database = global.include('guartinel/database/public/databaseConnector.js');
var watcherServerController = global.include('guartinel/admin/configuration/watcherServerController.js');
var httpRequester = global.include("guartinel/connection/httpRequester.js");
import { Package } from "../../guartinel/packages/package";
import {Const} from "../../common/constants";
import commonConstants = Const.commonConstants;
import {ErrorResponse, SuccessResponse } from "../../guartinel/response/Response";
import {MSInternalServerError, MSError } from "../../error/Errors";
import { LOG } from "../../diagnostics/LoggerFactory";
import * as securityTool from "../../guartinel/security/tool.js";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(Const.managementServerUrls.PACKAGE_SAVE);

var isRouteDebugEnabled = false;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(URL + " " + message);
}

export function route (req, res) {
   var parameters = {
      token: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.TOKEN)],
      packageIdOPT: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_ID)],
      packageType: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_TYPE)],
      packageName: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)],
      checkIntervalSeconds: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.CHECK_INTERVAL_SECONDS)],
      alertEmails: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.ALERT_EMAILS)],
      alertDeviceIds: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS)],
      configuration: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.CONFIGURATION)],
      isEnabled: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.IS_ENABLED)],
      access: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.ACCESS)],
      usePlainAlertEmail: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.USE_PLAIN_ALERT_EMAIL)]
   };

   global.myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         doSave(parameters, req.body, function (result) {
            return res.send(result);
         });
      }
   });
};

function doSave(parameters, requestJSON, callback) {
   sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (sessionErr, account) {
      if (sessionErr) {
         return callback(new ErrorResponse(sessionErr));
      }
      if (utils.object.isNull(parameters.packageIdOPT)) {
         handleCreate(requestJSON, account, callback);
      } else {
         handleUpdate(parameters, requestJSON, account, callback);
      }
   });
}

function handleCreate(requestJSON, account, callback) {
   debugRoute("Starting package creation");
   if (!account.canAddMorePackage()) { 
      var error = new MSInternalServerError()
         .logMessage(URL + ": Account reached maximum package count defined by its license.")
         .severe()
         .logNow();
      return callback(new ErrorResponse(error));
   }
   let newPackage = new Package(false, account);
   var createError = newPackage.createFromJSON(requestJSON);
   if (createError != null) {
      debugRoute("Failed package configuration from json");
      //Oooops something is wrong. Break the whole precess and return an internal server error
    /*  return callback(new ErrorResponse(new MSInternalServerError()
         .logMessage(URL + " handleCreate. Something went wrong while creating package.")
         .innerError(createError)
         .logNow()));*/
      return callback(new ErrorResponse(createError));
   }
   debugRoute("Package configuration is finished without error.");

   account.packages.push(newPackage);
   account.save(function (err) {
      if (err) {
         var error = new MSInternalServerError()
            .logMessage(URL + ": Cannot save account")
            .severe()
            .innerError(err)
            .logNow();
         return callback(new ErrorResponse(error));
      }
      debugRoute("Package saved after creation. Lets update other accounts acces for it");

      updateOtherAccountsAccess(account.packages[account.packages.length - 1], account, function (err) {
         if (err) {
            debugRoute("Failed to add other accounts access");
            return callback(new ErrorResponse(err));
         }
         debugRoute("Finished adding other accounts access");
         return callback(new SuccessResponse());
      });
   });
}

function handleUpdate(parameters, requestJSON, accountForUser, callback) {
   if (!database.isIdValid(parameters.packageIdOPT)) {
      var error = new MSInternalServerError()
         .logMessage(URL + ": handleUpdate invalid package id:" + parameters.packageIdOPT)
         .severe()
         .logNow();
      return callback(new ErrorResponse(error));
   }
   database.getAccountBySubDocumentProperty('packages', '_id', database.toObjectId(parameters.packageIdOPT), afterPackageRetrieval);

   function afterPackageRetrieval(err, account) {
      if (err) {
         return callback(new ErrorResponse(err));
      }
      if (utils.object.isNull(account)) {
         var error = new MSInternalServerError()
            .logMessage(URL + ":afterPackageRetrieval Invalid package ID " + parameters.packageIdOPT)
            .severe()
            .logNow();
         return callback(new ErrorResponse(error));
      }

      var actualPackage = account.packages.id(parameters.packageIdOPT);
      var accessRule;
      actualPackage.access.forEach(function (item, index) {
         if (item.packageUserEmail === accountForUser.email) {
            accessRule = item;
         }
      });

      if (utils.object.isNull(accessRule) || (!accessRule.canEdit && !accessRule.canDisable)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.PERMISSION_DENIED)
            .logMessage(URL + ": Account dont have permission to edit this package :" + account.email)
            .logNow();
         return callback(new ErrorResponse(error));
      }

      var onlyIsEnabledChangeable = false;
      if (!accessRule.canEdit && accessRule.canDisable) {
         onlyIsEnabledChangeable = true;
      }
       let updatedPackage = new Package(onlyIsEnabledChangeable, account);
      updatedPackage.initFromObject(actualPackage);
      let updateError = updatedPackage.updateFromJSON(requestJSON);
      if (updateError != null) {
         /*return callback(new ErrorResponse(
            new MSInternalServerError()
            .logMessage("Error while updating package")
            .innerError(updateError)
            .logNow()));*/
         return callback(new ErrorResponse(updateError));
      }
      account.packages.remove(parameters.packageIdOPT);
      account.packages.push(updatedPackage);
     
      watcherServerController.getWatcherServerById(updatedPackage.watcherServerId, startPackageUpdateProcess);

      function startPackageUpdateProcess(currentServer) {
         if (!utils.object.isNull(currentServer)) {
            httpRequester.deletePackage(currentServer, parameters.packageIdOPT, afterPackageDeleted);
         } else {
            afterPackageDeleted(null);
         }
      }

      function afterPackageDeleted(err) {
         if (err) {
            new MSInternalServerError().logMessage(URL + " Afterpackage deleted. Cannot delete from WS")
               .innerError(err).severe().logNow();
         }
         account.save(afterPackageSave);
      }

      function afterPackageSave(err) {
         if (err) {
            var error = new MSInternalServerError()
               .logMessage(URL + ": afterPackageSave Cannot save account")
               .severe()
               .innerError(err)
               .logNow();
            return callback(new ErrorResponse(error));
         }
         updateOtherAccountsAccess(updatedPackage,
            account,
            afterAccessListUpdate);
      }

      function afterAccessListUpdate(err) {
         if (err) {
            return callback(new ErrorResponse(err));
         }
         return callback(new SuccessResponse());
      }
   }
}


function updateOtherAccountsAccess(affectedPackage, ownerAccount, finishedCallback) {
   //get current emails for access rules
   var relatedAccountPlainEmails = [];

   debugRoute("Affected package access length :" + affectedPackage.access.length);
   for (var accessIndex = 0; accessIndex < affectedPackage.access.length; accessIndex++) {
      if (affectedPackage.access[accessIndex].packageUserEmail === ownerAccount.email) {
         continue;
      }
      relatedAccountPlainEmails.push(affectedPackage.access[accessIndex].packageUserEmail);
   }
   var relatedAccountEncryptedAndPlain = [];

   for (var index = 0; index < relatedAccountPlainEmails.length; index++) {    
      relatedAccountEncryptedAndPlain.push(relatedAccountPlainEmails[index]); // add plain version
      relatedAccountEncryptedAndPlain.push(securityTool.encryptText(relatedAccountPlainEmails[index]));// add encrypted version
   }
   debugRoute("updateOtherAccountsAccess: relatedAccountEmail:" + relatedAccountPlainEmails.toString());
   // add new package id for the accounts that are related to the access list emails
   database.getNativeAccountsConnection().updateMany(
      { 'email': { $in: relatedAccountEncryptedAndPlain } },
      { $addToSet: { 'accessiblePackageIds': affectedPackage._id.toString() } }
      , afterNewPackageIdsPush);
    
   function afterNewPackageIdsPush(err, success) {
      if (err) {
         var error = new MSInternalServerError()
            .logMessage(URL + ": afterNewPackageIdsPush  cannot done push.")
            .severe()
            .innerError(err)
            .logNow();
         return finishedCallback(error);
      }
      if (!utils.object.isNull(success)) {
         try {
            debugRoute(JSON.stringify(success));
         } catch (err) {
         }

      }
      //update every other account WHERE newPackageId is present AND the account email is NOT included in the newPackage.accesslist emails
      database.getNativeAccountsConnection().updateMany({
         $and: [
            { 'email': { $nin: relatedAccountEncryptedAndPlain } },
            { 'accessiblePackageIds': affectedPackage._id.toString() }
         ]
      },
         { $pull: { 'accessiblePackageIds': affectedPackage._id.toString() } },
         afterNewPackageIdsPull);
   }

   function afterNewPackageIdsPull(err, success) {
      if (err) {
         var error = new MSInternalServerError()
            .logMessage(URL + ": Cannot save account")
            .severe()
            .innerError(err)
            .logNow();
         return finishedCallback(error);
      }
      return finishedCallback();
   }
}