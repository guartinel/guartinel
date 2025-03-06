/**
 * Created by DTAP on 2017.04.28..

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
var alertMessageBuilder = global.include("guartinel/utils/alertMessageBuilder.js");

import * as async from "async";
import { HardwareSupervisorConfiguration } from
    "../../guartinel/packages/configurations/hardwareSupervisorConfiguration";
import { Package } from "../../guartinel/packages/package";
import * as securityTool from "../../guartinel/security/tool";
import * as config from "../../guartinel/admin/configuration/configurationController"
import * as slack from "../../guartinel/connection/slack";
let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(managementServerUrls.ALERT_SEND_SLACK);

var isRouteDebugEnabled = true;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
    var parameters = {
        token: req.body[traceIfNull(pluginConstants.TOKEN)],
        channelID: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.CHANNEL_ID)],
        alertMessage: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.ALERT_MESSAGE)],
        packageID: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_ID)]
    }
    global.myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doSendSlackAlert(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}

function doSendSlackAlert(parameters, callback) {
    var pack;
    var alertMessage;
    var account;
    sessionManager.validateMSTokenAndGetWatcherServer(parameters.token, function (err, server) {
        if (err) {
            return callback(new ErrorResponse(err));
        }

        database.getAccountBySubDocumentProperty('packages', '_id', parameters.packageId, afterAccountRetrieved);
        function afterAccountRetrieved(err, _account) {
            if (err) {
                return callback(new ErrorResponse(err));
            }
            account = _account;
            if (utils.object.isNull(account)) {
                var error = new MSInternalServerError()
                    .logMessage(exports.URL + ": PackageID is invalid " + parameters.packageId)
                    .severe()
                    .logNow();
                return callback(new ErrorResponse(error));
            }
            try {
                alertMessage = alertMessageBuilder.build(account, parameters.alertMessage);
            } catch (err) {
                var error = new MSInternalServerError()
                    .logMessage(exports.URL + ": Cannot build alert message.")
                    .severe()
                    .innerError(err)
                    .logNow();
                return callback(new ErrorResponse(error));
            }
            slack.send(parameters.channelID, alertMessage, pack, account, afterSlackSent);
        }

        function afterSlackSent(slackSendError) {
            if (slackSendError) {
                var error = new MSInternalServerError()
                    .logMessage(exports.URL + slackSendError)
                    .severe()
                    .logNow();
            }
            return callback(new SuccessResponse());
        }
    });
}

 */
