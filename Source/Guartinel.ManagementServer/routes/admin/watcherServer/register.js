var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var httpRequester = include('guartinel/connection/httpRequester.js');
var watcherServerController = include('guartinel/admin/configuration/watcherServerController.js');
var securityTool = include("guartinel/security/tool.js")

exports.URL = safeGet(managementServerUrls.ADMIN_WATCHER_SERVER_REGISTER);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        name: req.body[safeGet(commonConstants.ALL_PARAMETERS.NAME)],
        address: req.body[safeGet(commonConstants.ALL_PARAMETERS.ADDRESS)],
        port: req.body[safeGet(commonConstants.ALL_PARAMETERS.PORT)],
        userName: req.body[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)],
        categories: req.body[safeGet(commonConstants.ALL_PARAMETERS.CATEGORIES)],
        newPassword: req.body[safeGet(commonConstants.ALL_PARAMETERS.NEW_PASSWORD)],// Coming from the admin account stored in the adminwebsite backend
        newUserName: req.body[safeGet(commonConstants.ALL_PARAMETERS.NEW_USER_NAME)]// Coming from the admin account stored in the adminwebsite backend
    };

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doRegister(parameters, function (result) {
                res.send(result);
            });
        }
    });
};

function doRegister(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        config.getLocalConfiguration(function (err, config) {
            parameters.managementServerAddress = config.address.fullHTTP;
            var server = watcherServerController.getNewWatcherServer(parameters.address, parameters.port, parameters.name, parameters.categories);

            httpRequester.registerWatcherServer(server, parameters, function (err, token, managementServerId) {
                if (err) {
                    return callback(new ErrorResponse(err));
                }
                server.setWSToken(token);
                server.setManagementServerId(managementServerId);
                server.save(function (err) {
                    return callback(new SuccessResponse());
                });
            });
        });
    });
}