var sessionManager = global.include("guartinel/security/sessionManager.js");
var database = global.include('guartinel/database/public/databaseConnector.js');
var watcherServerController = global.include('guartinel/admin/configuration/watcherServerController.js');
var httpRequester = global.include("guartinel/connection/httpRequester.js");
import { Package } from "../../guartinel/packages/package";
import { Const } from "../../common/constants";
import commonConstants = Const.commonConstants;
import { ErrorResponse, SuccessResponse } from "../../guartinel/response/Response";
import { MSInternalServerError, MSError } from "../../error/Errors";
import { LOG } from "../../diagnostics/LoggerFactory";
import * as securityTool from "../../guartinel/security/tool.js";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(Const.managementServerUrls.PACKAGE_REMOVE_ACCESS);

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
        packageId: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_ID)]
    };
    global.myRequestValidation(parameters,
        req,
        res,
        function(requestErr) {
            if (!requestErr) {
                doRemoveAccess(parameters,
                    req.body,
                    function(result) {
                        return res.send(result);
                    });
            }
        });
};

function doRemoveAccess(parameters, requestJSON, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token,
        function(sessionErr, account) {
            if (sessionErr) {
                return callback(new ErrorResponse(sessionErr));
            }
            debugRoute("Session is OK");
            if (!database.isIdValid(parameters.packageId)) {
                var error = new MSInternalServerError()
                    .logMessage(URL + ": removeAccess invalid package id:" + parameters.packageId)
                    .severe()
                    .logNow();
                return callback(new ErrorResponse(error));
            }
            debugRoute("Package id is valid");
          
            database.getNativeAccountsConnection().updateMany(
                { 'email': { $in: [securityTool.encryptText(account.email), account.email] } },
                { $pull: { 'accessiblePackageIds': parameters.packageId } },
                afterPull);

            function afterPull(afterPullError) {
                if (afterPullError) {
                    var error = new MSInternalServerError()
                        .logMessage(URL + ": afterPull  cannot done pull.")
                        .severe()
                        .innerError(afterPullError)
                        .logNow();
                    return callback(new ErrorResponse(error));
                }
                debugRoute("Pulling is done");
                database.getNativeAccountsConnection().update(
                    {
                        "packages": { $elemMatch: { "_id": database.toObjectId(parameters.packageId) } }
                    },
                    { $pull: { 'packages.$.access': { "packageUserEmail": account.email } } },
                    afterRemove); // TODO warning if this is changed to encrypted it could be a problem                     
            }

            function afterRemove(afterRemoveError) {
                if (afterRemoveError) {
                    var error = new MSInternalServerError()
                        .logMessage(URL + ": afterRemove  cannot done pull.")
                        .severe()
                        .innerError(afterRemoveError)
                        .logNow();
                    return callback(new ErrorResponse(error));
                }
                debugRoute("Removing is done");
                return callback(new SuccessResponse());
            }
        });
}