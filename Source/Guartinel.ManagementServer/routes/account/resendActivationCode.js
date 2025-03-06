var database = include("guartinel/database/public/databaseConnector.js");
var emailer = include("guartinel/connection/emailer.js");
var timeTool = include('guartinel/utils/timeTool.js');
var moment = require('moment');

var securityTool = include('guartinel/security/tool.js');
exports.URL = exports.URL = safeGet(managementServerUrls.ACCOUNT_RESEND_ACTIVATION_CODE);
exports.route = function (req, res) {
    var parameters = {
        email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doResend(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doResend(parameters, callback) {
   database.getAccountByMultiplePropertyValues('email', [securityTool.encryptText(parameters.email), parameters.email], function (err, account) {
        if (err) {
           return callback(new ErrorResponse(err));
        }

        if (!timeTool.isOneHourElapsedFromThisTimeStamp(account.activationInfo.lastActivationEmailSentTimeStamp)) {
           return callback(new ErrorResponse(
              new MSError(commonConstants.ALL_ERROR_VALUES.ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND)
                 .logMessage("Account email: " + parameters.email)
                 .logNow()));
        }

        if(account.activationInfo.isActivated){
            LOG.error("Someone trying to reactivate an already activated account");
            return callback(new SuccessResponse());
        }

        emailer.sendActivationEmail(account, function (emailErr) {
           if (emailErr) {
              return callback(
                 new ErrorResponse(
                    new MSInternalServerError()
                       .logMessage("Cannot resend activation code to "+ parameters.email)
                       .innerError(emailErr)
                       .logNow()));
             }
            account.activationInfo.lastActivationEmailSentTimeStamp = moment().toISOString();
            account.save(function (err) {
               if (err) {
                  var error = new MSInternalServerError()
                     .logMessage("Cannot save account after resend activation code.").innerError(err).logNow();
                  return callback(new ErrorResponse(error));
               }
                return callback(new SuccessResponse());
            });
        });
    });
}
