/**
 * Created by DTAP on 2017.09.12..
 */

var database = include('guartinel/database/public/databaseConnector.js');
var sessionManager = include("guartinel/security/sessionManager.js");
var moment = require("moment");
var transactionDatabaseConnector = include("guartinel/database/transaction/databaseConnector.js");

exports.URL = safeGet(managementServerUrls.PACKAGE_GET_STATISTICS);
exports.route = function (req, res) {
    var parameters = {
        packageID: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)],
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    };

    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doGetStatistics(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}

function doGetStatistics(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (sessionErr, account) {
        if (sessionErr) {
            return callback(new ErrorResponse(sessionErr));
        }
        transactionDatabaseConnector.getPackageStatistics(parameters.packageID,function(err,statistics){
            if(err){
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Cannot getPackageStatistics ")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));		
            }
            var response = new SuccessResponse();
            response[safeGet(commonConstants.ALL_PARAMETERS.STATISTICS)] = statistics;
            return callback(response);
        });
    });
}