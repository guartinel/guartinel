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

export let URL = traceIfNull(managementServerUrls.DEVICE_DISCONNECT);

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
        instanceId: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.DEVICE_UUID)]
    }
    global.myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doDisconnect(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}

function doDisconnect(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        database.getHardwareByProperty('instanceId', parameters.instanceId, afterHardwareDBQuery);
        function afterHardwareDBQuery(err, hardware) {
            if (utils.object.isNull(hardware)) {
                debugRoute("There is no hardware with the id: " + parameters.instanceId);

                var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_ID)
                    .logMessage(URL + " Invalid instanceId " + parameters.instanceId)
                    .logNow();
                return callback(new ErrorResponse(error));
            }

            let foundDevice;
            for (var deviceIndex = 0; deviceIndex < account.devices.length; deviceIndex++) {
                if (account.devices[deviceIndex].instanceId == parameters.instanceId) {
                    foundDevice = account.devices[deviceIndex];                    
                    break;
                }
            }
            foundDevice.isDisconnected = true;
            account.save(function (err) {
                var response = new SuccessResponse();
                return callback(response);
            });
        }
    });
}

