import { Const } from "../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import { LOG } from "../../diagnostics/LoggerFactory";
import pluginConstants = Const.pluginConstants;
import commonConstants = Const.commonConstants;
import { ErrorResponse, SuccessResponse } from "../../guartinel/response/Response";
import { MSError } from "../../error/Errors";
var httpRequester = global.include('guartinel/connection/httpRequester.js');
var watcherServerController = global.include('guartinel/admin/configuration/watcherServerController.js');
var database = global.include("guartinel/database/public/databaseConnector.js");
import * as async from "async";
import { ApplicationSupervisorConfiguration } from
   "../../guartinel/packages/configurations/applicationSupervisorConfiguration";
import { Package } from "../../guartinel/packages/package";
import * as securityTool from "../../guartinel/security/tool";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(managementServerUrls.APPLICATION_SUPERVISOR_REGISTER_MEASUREMENT);

var isRouteDebugEnabled = false;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(URL + " " + message);
}

export function route(req, res) {
   let parameters = {
      applicationToken: req.body[traceIfNull(pluginConstants.APPLICATION_TOKEN)],
      measurement: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.MEASUREMENT)],
      instanceId: req.body[traceIfNull(pluginConstants.INSTANCE_ID)],
      instanceName: req.body[traceIfNull(pluginConstants.INSTANCE_NAME)],
      isHeartBeatOPT: req.body[traceIfNull(pluginConstants.IS_HEARTBEAT)],
      restriction: {}
   };
   parameters.restriction = {
      coolDownSec: 30,
      UUID: URL + parameters.applicationToken + parameters.instanceId
   };

   global.myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         doRegisterMeasurement(parameters, function (result) {
            return res.send(result);
         });
      }
   });
};
//validate token for package
//check if package has the same instance in the instance list
// if not add it
//forward to WS
function doRegisterMeasurement(parameters, callback) {
   database.getAccountBySubDocumentProperty('packages', 'application_token', parameters.applicationToken, function (err, account) {
      if (err) {
         return callback(new ErrorResponse(err));
      }
      if (utils.object.isNull(account)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
            .logMessage(URL + " Invalid application token" + parameters.applicationToken)
            .logNow();
         return callback(new ErrorResponse(error));
      }
      debugRoute("Get the package which has the application token");

      if (utils.object.isNull(parameters.isHeartBeatOPT)) {
         parameters.isHeartBeatOPT = false;
      }
      let relatedDBPackage;
      for (let i = 0; i < account.packages.length; i++) {
         let pack = account.packages[i];
         if (!utils.object.isNull(pack.application_token) && pack.application_token === parameters.applicationToken) {
            debugRoute("We have found the package!");
            relatedDBPackage = pack;
            break;
         }
      }

      debugRoute("Transform package to custom object from DB object");
      let packageObject = new Package(false, account);
      packageObject.initFromObject(relatedDBPackage);

      debugRoute("Check if it is a new instance for the current package");
      let isAlreadyAdded = (packageObject.configuration as ApplicationSupervisorConfiguration).hasInstance(parameters.instanceId);

      if (isAlreadyAdded) {
         debugRoute("Instance already exists. Lets update it!");
         (packageObject.configuration as ApplicationSupervisorConfiguration).updateInstance(
            parameters.instanceId,
            parameters.instanceName,
            parameters.isHeartBeatOPT);
      }
      if (isAlreadyAdded === false) {
         debugRoute("This is a new instance check if the account has remaining item in its license to add this.");
         if (account.getLicenseAggregate().maximumPackagePartCount < account.getAllPackagePartCount()) {
            debugRoute("Nice try but nope. Refuse requets with license error.");
            var error = new MSError(commonConstants.ALL_ERROR_VALUES.MAXIMUM_PACKAGE_PART_COUNT_REACHED)
               .logMessage(URL + "Maximum package part count reached for app token: " + parameters.applicationToken)
               .logNow();
            return callback(new ErrorResponse(error));
         }
         debugRoute("License is OK add this new instance!");
         (packageObject.configuration as ApplicationSupervisorConfiguration).addInstance(
            parameters.instanceId,
            parameters.instanceName,
            parameters.isHeartBeatOPT);
      }

      debugRoute("Instance added lets save only the config of the package to optimize performance to the db!");

      debugRoute("Saving package:  " + packageObject._id);
      database.getAccountModel().update(
         { 'packages._id': packageObject._id },
         { $set: { 'packages.$.configuration': securityTool.encryptObject(packageObject.configuration) } }, function (err, success) {
            if (err) {
               debugRoute("Cannot save related package: " + packageObject._id);
            } else {
               debugRoute("Package saved:  " + packageObject._id);
            }
            watcherServerController.getAllWatcherServers(afterServersLoaded);
         });

      function afterServersLoaded(servers) {
         if (utils.object.isNull(servers)) {
            LOG.debug("There is currently no watcher server registered");
            return callback(new SuccessResponse());
         }
         debugRoute("All watcher servers are loaded.");

         async.eachSeries(servers, sendMeasurementToServer, afterProcessingFinished);

         function sendMeasurementToServer(server, nextServer) {
            debugRoute("Send application measurement to WS :" + server.getId());
            if (server.getId() != packageObject.watcherServerId) {
               return nextServer();
            }

            var packID = packageObject._id;
            parameters.measurement = utils.object.ensureAsObject(parameters.measurement);
            httpRequester.forwardApplicationMeasurement(server, parameters.applicationToken, parameters.instanceId,parameters.instanceName, packID, parameters.measurement, parameters.isHeartBeatOPT, function (httpErr) {
               if (httpErr) {
                  LOG.error("Cannot forward measurements to server:" + server.getId());
                  debugRoute("Cannot forward measurements to server:" + server.getId());
               } else {
                  debugRoute("Forward successful");
               }
               return nextServer();
            });
         }

         function afterProcessingFinished(err) {
            if (err) {
               LOG.error("Error while processing servers and related package : " + JSON.stringify(err));
            } else {
               debugRoute("Route finished without error");
            }
            return callback(new SuccessResponse());
         }
      }

      //end
   }
   );
}

