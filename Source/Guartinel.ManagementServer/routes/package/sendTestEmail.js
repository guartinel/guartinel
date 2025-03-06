var emailer = include("guartinel/connection/emailer.js");
var sessionManager = include("guartinel/security/sessionManager.js");
var database = include("guartinel/database/public/databaseConnector.js");

exports.URL = "/package/sendtestemail";
exports.route = function (req, res) {
    var parameters = {
        token: req.body.token,
        email: req.body.email
    }

    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doTestEmail(parameters, function (result) {
                return res.send(result);
            });
        }
    });
};

function doTestEmail(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (sessionErr) {
        if (sessionErr) {
            return callback(new ErrorResponse(sessionErr));
        }
        database.getControlledEmailByProperty("email", parameters.email, function (err, controlledEmail) {
            if (!utils.object.isNull(controlledEmail)) {
                return callback(new SuccessResponse()); // email is already tested and inside our system
            }

            emailer.sendTestEmail(parameters.email, function (emailErr) {
                if (emailErr) {
                    return callback(new ErrorResponse(emailErr));
                }
                return callback(new SuccessResponse());
            });
        });
    });
}


