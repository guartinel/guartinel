var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var securityTool = include('guartinel/security/tool.js');

exports.URL = safeGet(managementServerUrls.ADMIN_UPDATE);
exports.route = function (req, res) {
    var parameters = {
        userName: req.body[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)],
        webPageURL: req.body[safeGet(commonConstants.ALL_PARAMETERS.WEB_PAGE_URL)],
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        emailProvider: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL_PROVIDER)],
        emailUserName: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL_USER_NAME)],
        emailPassword: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL_PASSWORD)]
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
        config.getAdminAccount(function (err, adminAccount) {
            adminAccount.userName = parameters.userName;//warning! hashed password is dependent from the current username. Must be updated at once the two
            var rehashedPasswordFromClient = securityTool.generatePasswordHash(parameters.userName, parameters.password);
            adminAccount.passwordHash = rehashedPasswordFromClient;

            config.getLocalConfiguration(function (err, xconfig) {
                xconfig.emailConfiguration.emailProvider = parameters.emailProvider;
                xconfig.emailConfiguration.emailUserName = parameters.emailUserName;
                xconfig.emailConfiguration.emailPassword = parameters.emailPassword;
                xconfig.webPageURL =  parameters.webPageURL;

                adminAccount.save(function (err) {
                    xconfig.save(function (err) {
                        return callback(new SuccessResponse());
                    });
                });
            });
        });
    });
}