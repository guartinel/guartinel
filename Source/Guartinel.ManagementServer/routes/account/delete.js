var constants = include('guartinel/constants.js');
var database = include('guartinel/database/public/databaseConnector.js');
var authenticator = include('guartinel/security/authenticator.js');

exports.URL  = safeGet(managementServerUrls.ACCOUNT_DELETE);
exports.route = function (req, res) {
    var parameters = {
        tokenOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        idOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.ID)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)],
        email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doDelete(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doDelete(parameters, callback) {
    authenticator.authenticateAndGetAccount(parameters.email, parameters.password, function (authErr, account) {
        if (authErr) {
           return callback(new ErrorResponse(authErr));
        }
        account.remove(function (err) {
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
    });
}