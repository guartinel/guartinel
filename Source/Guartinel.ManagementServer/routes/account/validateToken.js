/**
 * Created by DTAP on 2016.10.11..
 */
var sessionManager = include('guartinel/security/sessionManager.js');

exports.URL =  safeGet(managementServerUrls.ACCOUNT_VALIDATE_TOKEN);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doValidate(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doValidate(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
            return callback(new SuccessResponse());
    });
}