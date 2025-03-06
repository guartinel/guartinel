var sessionManager = include("guartinel/security/sessionManager.js");
var database = include("guartinel/database/public/databaseConnector.js");
var authenticator = include('guartinel/security/authenticator.js');
var securityTool = include('guartinel/security/tool.js');

exports.URL = exports.URL = safeGet(managementServerUrls.ACCOUNT_UPDATE);
exports.route = function (req, res) {

    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)],
        emailNewOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.NEW_EMAIL)],
        firstNameOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.FIRST_NAME)],
        lastNameOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.LAST_NAME)],
        passwordNewOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.NEW_PASSWORD)]
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
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        authenticator.authenticateAndGetAccount(parameters.email, parameters.password, function (authErr, authenticatedAccount) { // auth user just in case...
            if (authErr) {
                return callback(new ErrorResponse(authErr));
            }

            if (utils.object.isNull(parameters.emailNewOPT)) {
                return update(parameters, authenticatedAccount, callback);
            }

            if (!utils.object.isNull(parameters.emailNewOPT)) {
               database.getAccountByMultiplePropertyValues('email', [securityTool.encryptText(parameters.emailNewOPT), parameters.emailNewOPT], function (err, account) {//check if new email is available
                   if (!utils.object.isNull(account)) {
                      var error = new MSError(commonConstants.ALL_ERROR_VALUES.EMAIL_ALREADY_REGISTERED);
                        return callback(new ErrorResponse(error));
                    }
                    return update(parameters, authenticatedAccount, callback);
                });
            }
        });
    });
}
function update(parameters, account, callback) {
    if (!utils.object.isNull(parameters.emailNewOPT)) { // if email is new than the password must be typed again in order to generate new hash from it
        account.email = parameters.emailNewOPT;
    }
    if (!utils.object.isNull(parameters.firstNameOPT)) {
        account.firstName = parameters.firstNameOPT;
    }
    if (!utils.object.isNull(parameters.lastNameOPT)) {
        account.lastName = parameters.lastNameOPT;
    }
    if (!utils.object.isNull(parameters.passwordNewOPT)) {
        account.passwordHash = securityTool.generatePasswordHash(account.email, parameters.passwordNewOPT);
    }

    account.save(function (err) {
        if (err) {
           var error = new MSInternalServerError()
              .logMessage(exports.URL + ": Cannot save account")
              .severe()
              .innerError(err)
              .logNow();
           return callback(new ErrorResponse(error));
        }
        return callback(new SuccessResponse());
    });
}