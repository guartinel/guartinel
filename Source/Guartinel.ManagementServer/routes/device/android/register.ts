import { Const } from "../../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import commonConstants = Const.commonConstants;
import pluginConstants = Const.pluginConstants;

import { ErrorResponse, SuccessResponse } from "../../../guartinel/response/Response";
import { MSInternalServerError, MSError } from "../../../error/Errors";
import { LOG } from "../../../diagnostics/LoggerFactory";
import * as securityTool from "../../../guartinel/security/tool";
import { isNullOrUndefined, isNull, debug, debuglog } from "util";

var sessionManager = global.include("guartinel/security/sessionManager.js");
var database = global.include("guartinel/database/public/databaseConnector.js");
var authenticator = global.include('guartinel/security/authenticator.js');

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

var isRouteDebugEnabled = false;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(URL + " " + message);
}

export let URL = traceIfNull(managementServerUrls.DEVICE_ANDROID_REGISTER);
export function route(req, res) {
    var parameters = {
        email: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.EMAIL)],
        password: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.PASSWORD)],
        deviceUID: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.DEVICE_UUID)],
        gcmId: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.GCM_ID)],
        deviceName: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.DEVICE_NAME)]
    };

    global.myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doRegister(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}

function doRegister(parameters, callback) {
    authenticator.authenticateAndGetAccount(parameters.email, parameters.password, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        var device = null;
        for (var i = 0; i < account.devices.length; i++) {
            if (account.devices[i].uid != null && account.devices[i].uid === parameters.deviceUID) {
                device = account.devices[i];
                break;
            }
        }
        var isNew = false;
        if (utils.object.isNull(device)) {
            device = {};
            isNew = true;
        }

        device.deviceType = Const.commonConstants.ALL_DEVICE_TYPE_VALUES.ANDROID_DEVICE,
        device.name = parameters.deviceName;
        device.uid = parameters.deviceUID;
        device.gcmId = parameters.gcmId;
        device.properties = parameters.properties

        if (isNew) {
             account.devices.push(device);
        }
        sessionManager.createBrowserSession(account, function (err, token) {
            if (err) {
                return callback(new ErrorResponse(err));
            }

            var response = new SuccessResponse();
            response[traceIfNull(commonConstants.ALL_PARAMETERS.TOKEN)] = token;
            return callback(response);
        });
    });

}

