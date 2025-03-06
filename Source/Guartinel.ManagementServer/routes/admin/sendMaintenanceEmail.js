/**
 * Created by DTAP on 2017.08.21..
 */
var sessionManager = include('guartinel/security/sessionManager.js');
var database = include('guartinel/database/public/databaseConnector.js');
var emailer = include('guartinel/connection/emailer.js');
var async = require('async');

exports.URL = safeGet(managementServerUrls.ADMIN_SEND_MAINTENANCE_EMAIL);
exports.route = function (req, res) {
    var parameters = {
        message: req.body[safeGet(commonConstants.ALL_PARAMETERS.MESSAGE)],//We are going to do some maintenance on the Guartinel servers this Friday afternoon (the 27th) between the hours of 14:00 am and 16:00 am.
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    };

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doSetWebsiteAddress(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doSetWebsiteAddress(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        database.getMultipleAccounts(null, null, afterAllAccountRetrieved);
        var successSend = [];
        var failedSend = [];

        function afterAllAccountRetrieved(err, accounts) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Cannot get accounts from db")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            async.eachSeries(accounts, onNextAccount, onIterationFinished);

            function onNextAccount(account, doTheNextAccount) {
                emailer.sendMaintenanceEmail(account.email, account.getUserName(), parameters.message, afterSend);

                function afterSend(err) {
                    if (err) {
                        LOG.error("Cannot send maintenance email to address : "+ account.email);
                        failedSend.push(account.email);
                    } else {
                        successSend.push(account.email);
                    }

                    return doTheNextAccount();
                }
            }

            function onIterationFinished(err) {
                if (err) {
                   var error = new MSInternalServerError()
                      .logMessage(exports.URL + ": Cannot send maintenance email.Iteration interrupted.")
                      .severe()
                      .innerError(err)
                      .logNow();
                   return callback(new ErrorResponse(error));	
                }
                var response = new SuccessResponse();
                response[commonConstants.ALL_PARAMETERS.FAILED] = JSON.stringify(failedSend);
                response[commonConstants.ALL_PARAMETERS.SENT] = JSON.stringify(successSend);
                return callback(response);
            }
        }
    });
}