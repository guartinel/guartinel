var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var securityTool = include('guartinel/security/tool.js');
var database = include('guartinel/database/public/databaseConnector.js');

exports.URL = safeGet(managementServerUrls.ADMIN_DATABASE_UPDATE);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        databaseUrl: req.body[safeGet(commonConstants.ALL_PARAMETERS.DATABASE_URL)],
        userName: req.body[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)]
    }
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doUpdate(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doUpdate(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        config.getLocalConfiguration(function (err, config) {
            var previousConfig = config.databaseConfiguration;

            config.databaseConfiguration.passwordEncrypted = securityTool.encryptText(parameters.password);
            config.databaseConfiguration.email = parameters.email;
            config.databaseConfiguration.url = parameters.databaseUrl;

            database.disconnect(function (err) {
                database.connect(function (err) {
                    if (err) { // if connection failed than try again with the previously working setup
                        config.databaseConfiguration = previousConfig;
                        database.connect(function (err) {
                           config.save(function (err) {
                              var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DATABASE_URL)
                                 .logMessage("Invalid database url : " + parameters.databaseUrl)
                                 .logNow();
                              return callback(new ErrorResponse(error));   
                            });
                        });
                    }
                    config.save(function (err) { // everything fine save the new config
                        return callback(new SuccessResponse());
                    });
                });
            });
        });
    });
}