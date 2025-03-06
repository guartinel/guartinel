var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var statusInformer = include('guartinel/admin/statusInformer.js');
var httpRequester = include("guartinel/connection/httpRequester.js");
var watcherServerController = include("guartinel/admin/configuration/watcherServerController.js");


exports.URL = safeGet(managementServerUrls.ADMIN_WATCHER_SERVER_GET_EVENTS);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        watcherServerId: req.body[safeGet(commonConstants.ALL_PARAMETERS.WATCHER_SERVER_ID)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doGetEvents(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doGetEvents(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        watcherServerController.getWatcherServerById(parameters.watcherServerId, function (server) {
            httpRequester.getEvents(server, function (err, result) {
                if (err) {
                    return callback(new ErrorResponse(err));
                }

                var response = new SuccessResponse();
                response[safeGet(commonConstants.ALL_PARAMETERS.EVENTS)] = result;
                return callback(response);
            });
        });
    });
}