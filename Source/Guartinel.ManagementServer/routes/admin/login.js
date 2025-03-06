var sessionManager = include('guartinel/security/sessionManager.js');
var authenticator = include('guartinel/security/authenticator.js');

exports.URL = safeGet(managementServerUrls.ADMIN_LOGIN);
exports.route = function (req, res) {
    var parameters = {
        userName: req.body[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doLogin(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doLogin(parameters, callback) {
     authenticator.authenticateAdministrator(parameters.userName, parameters.password, function (authErr, token) {
        if (authErr) {
            return callback(new ErrorResponse(authErr));
        }
        var response = new SuccessResponse();
        response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = token;
        return callback(response);
    });
}