/**
 * Created by DTAP on 2017.04.28..
 */
import { Const } from "../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import { LOG } from "../../diagnostics/LoggerFactory";
import pluginConstants = Const.pluginConstants;
import commonConstants = Const.commonConstants;
import { ErrorResponse, SuccessResponse } from "../../guartinel/response/Response";
import { MSError, MSInternalServerError } from "../../error/Errors";
var httpRequester = global.include('guartinel/connection/httpRequester.js');
var watcherServerController = global.include('guartinel/admin/configuration/watcherServerController.js');
var database = global.include("guartinel/database/public/databaseConnector.js");
var sessionManager = global.include("guartinel/security/sessionManager.js");

import * as async from "async";
import { HardwareSupervisorConfiguration } from
   "../../guartinel/packages/configurations/hardwareSupervisorConfiguration";
import { Package } from "../../guartinel/packages/package";
import * as securityTool from "../../guartinel/security/tool";
import * as config from "../../guartinel/admin/configuration/configurationController"

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(managementServerUrls.HARDWARE_SUPERVISOR_REGISTER_HARDWARE);

var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
   var parameters = {
      hardwareToken: req.body[traceIfNull(pluginConstants.HARDWARE_TOKEN)],
      instanceId: req.body[traceIfNull(pluginConstants.INSTANCE_ID)],
      instanceName: req.body[traceIfNull(pluginConstants.INSTANCE_NAME)]
   }
   global.myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         doRegisterHardware(parameters, function (result) {
            return res.send(result);
         });
      }
   });
}

function doRegisterHardware(parameters, callback) {
   database.getHardwareByProperty('instanceId', parameters.instanceId, afterHardwareDBQuery);
   function afterHardwareDBQuery(err, hardware) {
      if (utils.object.isNull(hardware)) {
         debugRoute("There is no hardware with the id: " + parameters.instanceId);       

          var error = new MSInternalServerError()
              .logMessage(URL + " Invalid instanceId " + parameters.hardwareToken)
            .severe()
              .innerError(err)
              .logNow();
          return callback(new ErrorResponse(error));
       }
        

      database.getAccountBySubDocumentProperty('packages', 'hardware_token', parameters.hardwareToken, afterAccountQuery);

      function afterAccountQuery(err, account) {
         if (err) {
            return callback(new ErrorResponse(err));
         }
         if (utils.object.isNull(account)) {
             var error = new MSInternalServerError()
                 .logMessage(URL + " Invalid hardware token " + parameters.hardwareToken)
                 .severe()
                 .innerError(err)
                 .logNow();
             return callback(new ErrorResponse(error));
         }
         debugRoute("Get the package which has the hardware token");

         let relatedDBPackage;
         for (let i = 0; i < account.packages.length; i++) {
            let pack = account.packages[i];
            if (!utils.object.isNull(pack.hardware_token) && pack.hardware_token === parameters.hardwareToken) {
               debugRoute("We have found the package!");
               relatedDBPackage = pack;
               break;
            }
         }

         debugRoute("Transform package to custom object from DB object");
         let packageObject = new Package(false, account);
         packageObject.initFromObject(relatedDBPackage);

         debugRoute("Check if it is a new instance for the current package");
         let instance = (packageObject.configuration as HardwareSupervisorConfiguration).getInstance(parameters.instanceId);

         if (!utils.object.isNull(instance)) {
            instance.name = parameters.instanceName;
            instance.type = hardware.type;
            saveAccount();
         } else {
            debugRoute("This is a new instance check if the account has remaining item in its license to add this.");
            if (account.getLicenseAggregate().maximumPackagePartCount < account.getAllPackagePartCount()) {
               debugRoute("Nice try but nope. Refuse requets with license error.");
               var error = new MSError(commonConstants.ALL_ERROR_VALUES.MAXIMUM_PACKAGE_PART_COUNT_REACHED)
                  .logMessage(URL + "Maximum package part count reached for app token: " + parameters.applicationToken)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            debugRoute("License is OK add this new instance!");
            (packageObject.configuration as HardwareSupervisorConfiguration).addInstance(
               hardware.instanceId,
               parameters.instanceName,
               hardware.type);
            debugRoute("Instance added lets save only the config of the package to optimize performance to the db!");
            saveAccount();
         }

         function saveAccount() {
            debugRoute("Saving package:  " + packageObject._id);
            database.getAccountModel().update(
               { 'packages._id': packageObject._id },
               { $set: { 'packages.$.configuration': securityTool.encryptObject(packageObject.configuration) } }, afterAccountSaved);
         }

         function afterAccountSaved(err, success) {
            if (err) {
               debugRoute("Cannot save related package: " + packageObject._id);
            } else {
               debugRoute("Package saved:  " + packageObject._id);
            }
            config.getLocalConfiguration(afterLocalConfigRetrieved);
         }

         function afterLocalConfigRetrieved(err, configResult) {
            var response = new SuccessResponse();
            response[traceIfNull(commonConstants.ALL_PARAMETERS.UPDATE_SERVER_PORT)] = configResult.updateServer.port;
            response[traceIfNull(commonConstants.ALL_PARAMETERS.UPDATE_SERVER_HOST)] = configResult.updateServer.host;
            response[traceIfNull(commonConstants.ALL_PARAMETERS.UPDATE_SERVER_PROTOCOL_PREFIX)] = configResult.updateServer.protocolPrefix;
            response[traceIfNull(pluginConstants.TYPE)] = hardware.type;

            return callback(response);
         }
      };
   }
}

