/**
 * Created by DTAP on 2017.08.07..
 */
let utils = global.utils;
import { LOG } from "../../../diagnostics/LoggerFactory";
import { MSError, MSInternalServerError } from "../../../error/Errors";
var sessionManager = global.include("guartinel/security/sessionManager.js");
import { Const } from "../../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import commonConstants = Const.commonConstants;
import plugins = Const.plugins;
import pluginConstants = Const.pluginConstants;
import {ApplicationSupervisorConfiguration} from "../../../guartinel/packages/configurations/applicationSupervisorConfiguration";
import {ErrorResponse, SuccessResponse } from "../../../guartinel/response/Response";
import {APIPackage} from "../../../guartinel/packages/apiPackage";

let traceIfNull = global.utils.string.traceIfNull;

export let URL = traceIfNull(managementServerUrls.API_PACKAGE_SAVE);

var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(URL + " " + message);
}

export function route(req, res) {
   var parameters = {
      token: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.TOKEN)],
      pack: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE)]
   };
   global.myRequestValidation(parameters, req, res, function (err) {
      if (!err) {
         processRequest(parameters, function (result) {
            res.send(result);
         });
      }
   });
};

function processRequest(parameters, callback) {
   sessionManager.validateAPITokenAndGetAccount(parameters.token, function (sessionErr, account) {
      if (sessionErr) {
         return callback(new ErrorResponse(sessionErr));
      }
      // checking the format of the incoming package
      parameters.pack = utils.object.ensureAsObject(parameters.pack);
      if (!utils.object.isObject(parameters.pack)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_PACKAGE_OBJECT)
            .logMessage("Invalid formated package to save: " + parameters.pack)
            .severe()
            .logNow();
         return callback(new ErrorResponse(error));
      }

      //finding out if it is a new package or an existing one to update
      var foundPack;
      for (var index = 0; index < account.packages.length; index++) {
         if (account.packages[index].isDeleted) {
            continue;
         }
         if (account.packages[index].packageName === parameters.pack[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)]) {
            foundPack = account.packages[index];
            break;
         }
      }

      if (utils.object.isNull(foundPack)) {
         debugRoute("Creating package with name" + parameters.pack[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)]);
         if (!account.canAddMorePackage()) {
            var error = new MSError("The maximum package count defined by the account's license is reached.").logNow();
            return callback(new ErrorResponse(error));
         }
         createPackage();
      } else {
         debugRoute("Updating package with name" + parameters.pack[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)]);
         updatePackage(foundPack._id);
      }

      function createPackage() {
         let newPackage = new APIPackage(false, account);
         let createError = newPackage.createFromJSON(parameters.pack);
         if (createError != null) {
            return callback(new ErrorResponse(createError));
         }
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
            debugRoute("After save . No err");
            var response = new SuccessResponse();
            if (newPackage.packageType  === plugins.ALL_PACKAGE_TYPE_VALUES.APPLICATION_SUPERVISOR) {
               response[pluginConstants.APPLICATION_TOKEN] = (newPackage.configuration as ApplicationSupervisorConfiguration).application_token;
            }
            response[commonConstants.ALL_PARAMETERS.VERSION] = 0;
            return callback(response);
         });
      }

      function updatePackage(packageID) {
         let actualPackage = account.packages.id(packageID);
         var accessRule;
         actualPackage.access.forEach(function (item, index) {
            if (item.packageUserEmail === account.email) {
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

         let updatedPackage = new APIPackage(onlyIsEnabledChangeable, account);
         updatedPackage.initFromObject(actualPackage);
         let updateError = updatedPackage.updateFromJSON(parameters.pack);
         if (updateError != null) {
            return callback(new ErrorResponse(updateError));
         }

         account.packages.remove(packageID);
         account.packages.push(updatedPackage);

         account.save(function (err) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(URL + ": Cannot save account")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            debugRoute("After save . No err");
            var response = new SuccessResponse();
            response[commonConstants.ALL_PARAMETERS.VERSION] = updatedPackage.version;
            return callback(response);
         });
      }      
   });
}

