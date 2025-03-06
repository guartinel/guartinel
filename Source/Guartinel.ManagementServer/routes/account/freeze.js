/**
 * Created by DTAP on 2016.07.08..
 */
var constants = include('guartinel/constants.js');
var database = include('guartinel/database/public/databaseConnector.js');
var authenticator = include('guartinel/security/authenticator.js');
var moment = require('moment');
var httpRequester = include('guartinel/connection/httpRequester.js');
var watcherServerController = include('guartinel/admin/configuration/watcherServerController.js');

exports.URL  = safeGet(managementServerUrls.ACCOUNT_FREEZE);
exports.route = function (req, res) {
    var parameters = {
        tokenOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        idOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.ID)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)],
        email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doFreeze(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doFreeze(parameters, callback) {
    authenticator.authenticateAndGetAccount(parameters.email, parameters.password, function (authErr, account) {
        if (authErr) {
           return callback(new ErrorResponse(authErr));
        }

        for (var i = 0; i < account.packages.length; i++) {
            account.packages[i].isEnabled = false;
            account.packages[i].lastModificationTimeStamp = moment().toISOString();
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

            deleteIterator(0);
            function deleteIterator(i) {
                if (i == account.packages.length) {
                    return;
                }
                watcherServerController.getWatcherServerById(account.packages[i].watcherServerId, function (server) {
                    httpRequester.deletePackage(server,  account.packages[i]._id.toString(), function (err) {
                        if (err) {
                           new MSError("CANNOT_DELETE_ACCOUNT_FROM_WS").logMessage("Cannot delete package from WS while freezing account.").innerError(err).logNow();
                        }
                        account.packages[i].watcherServerId = null;
                        account.save(function (err) {
                           if (err) {
                              new MSError("CANNOT_SAVE_ACCOUNT_AFTER_PACKAGE_SAVE").innerError(err).logNow();
                            }
                            deleteIterator(i + 1);
                        });
                    });
                });
            }

            return callback(new SuccessResponse());
        })
    });
}