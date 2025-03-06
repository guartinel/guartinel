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
import { HardwareSupervisorConfiguration } from
   "../../guartinel/packages/configurations/hardwareSupervisorConfiguration";
import { Package } from "../../guartinel/packages/package";
import * as securityTool from "../../guartinel/security/tool";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(managementServerUrls.HARDWARE_SUPERVISOR_REGISTER_MEASUREMENT);

var isRouteDebugEnabled = false;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(URL + " " + message);
}

export function route(req, res) {
   let parameters = {
      hardwareToken: req.body[traceIfNull(pluginConstants.HARDWARE_TOKEN)],
      measurement: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.MEASUREMENT)],
      instanceId: req.body[traceIfNull(pluginConstants.INSTANCE_ID)],
      startupTimeOPT: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.STARTUP_TIME)],//TODO remove OPT after every wemos migrated
      restriction: {}
   };
   parameters.restriction = {
      coolDownSec: 3,
      UUID: URL + parameters.hardwareToken + parameters.instanceId
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
   database.getAccountBySubDocumentProperty('packages', 'hardware_token', parameters.hardwareToken, function (err, account) {
      if (err) {
         return callback(new ErrorResponse(err));
      }
      if (utils.object.isNull(account)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
            .logMessage(URL + " Invalid hardware token" + parameters.hardwareToken)
            .logNow();
         return callback(new ErrorResponse(error));
      }
      debugRoute("Get the package which has the hardware token");
      
      let relatedDBPackage;
      for (let i = 0; i < account.packages.length; i++) {
         let pack = account.packages[i];
         if (!utils.object.isNull(pack.hardware_token) && pack.hardware_token === parameters.hardwareToken && !pack.isDeleted) {
            debugRoute("We have found the package!");
            relatedDBPackage = pack;
            break;
         }
      }
      if (utils.object.isNull(relatedDBPackage)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
            .logMessage(URL + " Invalid hardware token (package deleted) " + parameters.hardwareToken)
            .logNow();
         return callback(new ErrorResponse(error));
      }
      debugRoute("Transform package to custom object from DB object");
      let packageObject = new Package(false, account);
      packageObject.initFromObject(relatedDBPackage);

      debugRoute("Check if it is a new instance for the current package");
      let instance = (packageObject.configuration as HardwareSupervisorConfiguration).getInstance(parameters.instanceId);
      if (utils.object.isNull(instance)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
            .logMessage(URL + " Invalid hardware token" + parameters.hardwareToken)
            .logNow();
         return callback(new ErrorResponse(error));
      }    
 
      watcherServerController.getAllWatcherServers(afterServersLoaded);
      function afterServersLoaded(servers) {
         if (utils.object.isNull(servers)) {
            LOG.debug("There is currently no watcher server registered");
            return callback(new SuccessResponse());
         }
         debugRoute("All watcher servers are loaded.");

         async.eachSeries(servers, sendMeasurementToServer, afterProcessingFinished);

         function sendMeasurementToServer(server, nextServer) {
            debugRoute("Send hardware measurement to WS :" + server.getId());
            if (server.getId() != packageObject.watcherServerId) {
               return nextServer();
            }

            var packID = packageObject._id.toString();
            parameters.measurement = utils.object.ensureAsObject(parameters.measurement);
             httpRequester.forwardHardwareMeasurement(server, parameters.hardwareToken, instance.id, instance.name, packID, parameters.measurement, parameters.startupTimeOPT, function (httpErr) {
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
            var response = new SuccessResponse();
            response["instance_name"] = instance.name;
            return callback(response);
         }
      }    
   }
   );
}

