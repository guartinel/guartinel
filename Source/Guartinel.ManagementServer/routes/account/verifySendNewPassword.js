/**
 * Created by DTAP on 2016.07.25..
 */

var database = include('guartinel/database/public/databaseConnector.js');
var moment = require('moment');
var emailer = include('guartinel/connection/emailer.js');
var securityTool = include("guartinel/security/tool.js");

exports.URL = exports.URL = safeGet(managementServerUrls.ACCOUNT_VERIFY_SEND_NEW_PASSWORD);
exports.route = function (req, res) {
    var parameters = {
        verificationCode: req.body[safeGet(commonConstants.ALL_PARAMETERS.VERIFICATION_CODE)],
        email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)]
    };

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doVerify(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doVerify(parameters, callback) {
    database.getAccountByProperty('passwordRenew.verificationCode', parameters.verificationCode, function (err, account) {
       if (utils.object.isNull(account)) {
          var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_VERIFICATION_CODE)
             .logMessage("Invalid send new password verification code" + parameters.verificationCode).logNow();
          return callback(new ErrorResponse(error));   
        }

        var newPassword = securityTool.generateRandomString(12);
        var hash1 = securityTool.generatePasswordHash(account.email, newPassword);
        var hash2 = securityTool.generatePasswordHash(account.email, hash1);

        account.browserSessions = [];
        account.passwordHash = hash2;

        account.passwordRenew.lastSentTimeStamp = moment().toISOString();
        account.passwordRenew.verificationCode = "";
        emailer.sendNewPasswordToEmail(account.email, newPassword, function (err) {
            if (err) {
                LOG.error("Cannot send new password to email address "+ account.email);
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
    });
}