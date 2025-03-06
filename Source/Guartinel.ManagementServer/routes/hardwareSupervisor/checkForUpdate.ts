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
var database = global.include("guartinel/database/public/databaseConnector.js");
import * as securityTool from "../../guartinel/security/tool";
import * as config from "../../guartinel/admin/configuration/configurationController"
var moment = require('moment');
let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(managementServerUrls.HARDWARE_SUPERVISOR_CHECK_FOR_UPDATE);

var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
   var parameters = {
      hardwareTokenOPT: req.body[traceIfNull(pluginConstants.HARDWARE_TOKEN)],
      instanceId: req.body[traceIfNull(pluginConstants.INSTANCE_ID)],
      instanceName: req.body[traceIfNull(pluginConstants.INSTANCE_NAME)],
      version: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.VERSION)]
   }
   global.myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         doCheckForUpdate(parameters, function (result) {
            return res.send(result);
         });
      }
   });
}

function doCheckForUpdate(parameters, callback) {
   database.getHardwareByProperty('instanceId', parameters.instanceId, afterHardwareDBQuery);
   function afterHardwareDBQuery(err, hardware) {
      if (utils.object.isNull(hardware)) {
         debugRoute("There is no hardware with the id: " + parameters.instanceId);

         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
            .logMessage(URL + " Invalid instanceId " + parameters.hardwareToken)
            .logNow();
         return callback(new ErrorResponse(error));
      }
      config.getLocalConfiguration(afterLocalConfigRetrieved);

      function afterLocalConfigRetrieved(err, configResult) {
         getUpdateUrl(configResult, afterURLBuild);
      }

      function getUpdateUrl(localConfiguration, onFinished) {
         debugRoute("Building update url");
         /* "updateServer" : {
                  "host" : "backend2.guartinel.com",
                 "port" : "8889",
                "protocolPrefix" : "http://"
              },*/
         var updateURL = "";
         updateURL += localConfiguration.updateServer.protocolPrefix;
         updateURL += localConfiguration.updateServer.host;
         updateURL += ":";
         updateURL += localConfiguration.updateServer.port;
         debugRoute("Update server url base: " + updateURL);
         debugRoute("Found hardware instance: " + JSON.stringify(hardware));
            /** custom firmware
             {}
                    "minVersion" : 21,
                    "path" : "/HardwareSupervisor/unified/testv22.bin"
                }*/
         if (!utils.object.isNull(hardware.firmware)) {
            debugRoute("Hardware has a special firmware so using it for update check");
            debugRoute("Hardware has a min version : " + hardware.firmware.minVersion + " and the  current hardware instance has version : " + parameters.version);

            if (hardware.firmware.minVersion > parameters.version) {
               debugRoute("Version WRONG");
               updateURL += hardware.firmware.path;
               afterURLBuild(updateURL);
            } else {
               afterURLBuild("");
               debugRoute("Version OK");
            }
         } else {
            debugRoute("Hardrware instance using the general firmware versions. Lets query it by type: "+ hardware.type);
            database.getHardwareFirmwareByProperty('type', hardware.type, afterHardwareFirmwareFound);
            function afterHardwareFirmwareFound(err, firmware) {
               if (err) {
                  return callback(new ErrorResponse(err));
               }
               if (utils.object.isNull(firmware)) {
                 return afterURLBuild("");
               }
               debugRoute("Found firmware: " + JSON.stringify(firmware));
               debugRoute("Hardware has a min version : " + firmware.minVersion + " and the current hardware instance has version : " + parameters.version);
               if (firmware.minVersion > parameters.version) {
                  debugRoute("Version WRONG");
                  updateURL += firmware.path;
                  afterURLBuild(updateURL);
               } else {
                  afterURLBuild("");
                  debugRoute("Version OK");
               }
            }
         }
      }
      function afterURLBuild(url) {
         debugRoute("URL after build : " + url);
         var response = new SuccessResponse();
         response[traceIfNull(commonConstants.ALL_PARAMETERS.URL)] = url;
         response[traceIfNull(commonConstants.ALL_PARAMETERS.REMOTE_DEBUG_URL)] = hardware.remoteDebugURL;   
         response[traceIfNull(commonConstants.ALL_PARAMETERS.TIME)] = moment().toISOString();   
         return callback(response);
      }
   }
}

