/**
 * Created by DTAP on 2016.06.21..
 */

var database = include("guartinel/database/public/databaseConnector.js");
var emailer = include('guartinel/connection/emailer.js');
var timeTool = include('guartinel/utils/timeTool.js');
var securityTool = include("guartinel/security/tool.js");
var moment = require('moment');

exports.URL = exports.URL = safeGet(managementServerUrls.ACCOUNT_SEND_NEW_PASSWORD);
exports.route = function (req, res) {
    var parameters = {
        email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)],
        address : req.body[safeGet(commonConstants.ALL_PARAMETERS.ADDRESS)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doSendNewPassword(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doSendNewPassword(parameters, callback) {
   database.getAccountByMultiplePropertyValues('email', [securityTool.encryptText(parameters.email), parameters.email], function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        if (utils.object.isNull(account)) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD)
              .logMessage("Invalid email to for send new password " + parameters.email).logNow();
           return callback(new ErrorResponse(error));
        }
        var notSentEmailYet = utils.object.isNull(account.passwordRenew.lastSentTimeStamp);
        var oneHourElapsedAfterLast = timeTool.isOneHourElapsedFromThisTimeStamp(account.passwordRenew.lastSentTimeStamp);
        if (!notSentEmailYet && !oneHourElapsedAfterLast) {
            var error = new MSError(commonConstants.ALL_ERROR_VALUES.ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND)
              .logMessage("To frequent send password request for account " + parameters.email).logNow();
           return callback(new ErrorResponse(error));              
        }

        var addressInfo = "";
        addressInfo=   addressInfo+ "IP: "+ parameters.address.ip+ "\n";
        addressInfo= addressInfo+ " Country: "+ parameters.address.country_name+ "\n";
        addressInfo = addressInfo + " Region: "+ parameters.address.region_name+ "\n";
        addressInfo = addressInfo + " Device: "+ parameters.address.user_agent+ "\n";

        account.passwordRenew.lastSentTimeStamp = moment().toISOString();
        account.passwordRenew.verificationCode = securityTool.generateRandomString(10);
        emailer.sendVerifyPasswordSendEmail(account.email,account.passwordRenew.verificationCode,addressInfo, function (err) {
            if (err) {
               LOG.error("Cannot send passwordrenew verification email to address "+ account.email);
            }
            account.save(function (err) {
                if (err) {
                   var error = new MSInternalServerError()
                      .logMessage(exports.URL + " Cannot save account")
                      .severe()
                      .innerError(err)
                      .logNow();
                   return callback(new ErrorResponse(error));	
                }
                return callback(new SuccessResponse());
            });
        });

        return callback(new SuccessResponse());
    });
}



