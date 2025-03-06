var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var statusInformer = include('guartinel/admin/statusInformer.js');
var httpRequester = include("guartinel/connection/httpRequester.js");
var watcherServerController = include('guartinel/admin/configuration/watcherServerController.js');

exports.URL = safeGet(managementServerUrls.ADMIN_WATCHER_SERVER_GET_STATUS);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        watcherServerId: req.body[safeGet(commonConstants.ALL_PARAMETERS.WATCHER_SERVER_ID)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doGetStatus(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doGetStatus(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        watcherServerController.getWatcherServerById(parameters.watcherServerId,function(server){
           if (utils.object.isNull(server)) {
              var error = new MSInternalServerError()
                 .logMessage(exports.URL + ": Cannot find watcher server with id:"+ parameters.watcherServerId)
                 .severe()
                 .logNow();
              return callback(new ErrorResponse(error));	
            }
            httpRequester.getStatus(server, function (err, status) {
                if (err) {
                    return callback(new ErrorResponse(err));
                }
                var response = new SuccessResponse();
                response[safeGet(commonConstants.ALL_PARAMETERS.STATUS)] = status;
                return callback(response);
            });
        });
    });
}