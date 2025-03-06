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

export let URL = traceIfNull(managementServerUrls.HARDWARE_SUPERVISOR_REGISTER);

var isRouteDebugEnabled = true;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
    var parameters = {
        token: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.TOKEN)],
        instanceId: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.INSTANCE_ID)],
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
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        database.getHardwareByProperty('instanceId', parameters.instanceId, afterHardwareDBQuery);
        function afterHardwareDBQuery(err, hardware) {
            if (utils.object.isNull(hardware)) {
                debugRoute("There is no hardware with the id: " + parameters.instanceId);

                var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
                    .logMessage(URL + " Invalid instanceId " + parameters.instanceId)
                    .logNow();
                return callback(new ErrorResponse(error));
            }

            let isAlreadyExists = false;
            let foundDevice;
            for (var deviceIndex = 0; deviceIndex < account.devices.length; deviceIndex++) {
                if (account.devices[deviceIndex].instanceId == parameters.instanceId) {
                    foundDevice = account.devices[deviceIndex];
                    foundDevice.name = parameters.instanceName;
                    foundDevice.hardwareType = hardware.type;
                    isAlreadyExists = true;
                    break;
                }
            }

            if (!isAlreadyExists) {
                foundDevice = {
                    deviceType: commonConstants.ALL_DEVICE_TYPE_VALUES.HARDWARE_SENSOR,
                    name: parameters.instanceName,
                    instanceId: parameters.instanceId,
                    hardwareType: hardware.type
                };
                account.devices.push(foundDevice);
            }
            foundDevice.isDisconnected = false;

            config.getLocalConfiguration(afterLocalConfigRetrieved);

            function afterLocalConfigRetrieved(err, configResult) {
                account.save(function (err) {
                    var response = new SuccessResponse();
                    response[traceIfNull(commonConstants.ALL_PARAMETERS.UPDATE_SERVER_PORT)] = configResult.updateServer.port;
                    response[traceIfNull(commonConstants.ALL_PARAMETERS.UPDATE_SERVER_HOST)] = configResult.updateServer.host;
                    response[traceIfNull(commonConstants.ALL_PARAMETERS.UPDATE_SERVER_PROTOCOL_PREFIX)] = configResult.updateServer.protocolPrefix;
                    response[traceIfNull(pluginConstants.TYPE)] = foundDevice.hardwareType;

                    return callback(response);
                });
            }
        }
    });
}

