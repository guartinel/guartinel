import { Const } from "../../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import commonConstants = Const.commonConstants;
import pluginConstants = Const.pluginConstants;
import * as moment from "moment";
import { ErrorResponse, SuccessResponse } from "../../../guartinel/response/Response";
import { MSInternalServerError, MSError } from "../../../error/Errors";
import { LOG } from "../../../diagnostics/LoggerFactory";
import * as securityTool from "../../../guartinel/security/tool";
import { isNullOrUndefined, isNull, debug, debuglog } from "util";
import * as configController from "../../../guartinel/admin/configuration/configurationController";


var sessionManager = global.include("guartinel/security/sessionManager.js");
var database = global.include("guartinel/database/public/databaseConnector.js");
var authenticator = global.include('guartinel/security/authenticator.js');

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

var isRouteDebugEnabled = true;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(URL + " " + message);
}

export let URL = traceIfNull(managementServerUrls.DEVICE_ANDROID_LOGIN);
export function route(req, res) {
    var parameters = {
        email: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.EMAIL)],
        password: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.PASSWORD)],
        deviceUID: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.DEVICE_UUID)],
        gcmId: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.GCM_ID)],
        versionCodeOPT : req.body[traceIfNull(commonConstants.ALL_PARAMETERS.VERSION_CODE)]
    };

    global.myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doLogin(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}


function doLogin(parameters, callback) {
    authenticator.authenticateAndGetAccount(parameters.email, parameters.password, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        var device = null;
        for(var i = 0; i< account.devices.length; i++) {
            if (account.devices[i].uid != null && account.devices[i].uid === parameters.deviceUID) {
                device = account.devices[i];
                break;
            }
        }

        if (utils.object.isNull(device)) {
            return callback(new ErrorResponse(new MSError(commonConstants.ALL_ERROR_VALUES.DEVICE_NOT_REGISTERED)));
        }

        debugRoute("Version code of client: " + parameters.versionCodeOPT);
        debugRoute("Minimum version code of server: " + configController.getBaseConfig().minimumAndroidAppVersionCode);

        if (!utils.object.isNull(parameters.versionCodeOPT) &&
            parameters.versionCodeOPT < configController.getBaseConfig().minimumAndroidAppVersionCode) {
            return callback(new ErrorResponse(new MSError(commonConstants.ALL_ERROR_VALUES.UPDATE_NOW)));
        }

        if (!utils.string.isNullOrEmpty(parameters.gcmId)) {
            device.gcmId = parameters.gcmId;
        }

        sessionManager.createBrowserSession(account, function (err, token) {
            if (err) {
                return callback(new ErrorResponse(err));
            }
            device.token = token;
            device.tokenTimeStamp = moment().toISOString();
            account.save(function(err) {
                var response = new SuccessResponse();
                response[traceIfNull(commonConstants.ALL_PARAMETERS.TOKEN)] = token;
                response[traceIfNull(commonConstants.ALL_PARAMETERS.DEVICE_NAME)] = device.name;

                return callback(response);
            });
        });
    });

}

