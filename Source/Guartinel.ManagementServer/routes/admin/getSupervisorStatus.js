/**
 * Created by DTAP on 2016.11.18..
 */
exports.URL = safeGet(managementServerUrls.ADMIN_GET_SUPERVISOR_STATUS);

var adminDB = include('guartinel/database/admin/databaseConnector.js');
var publicDB = include('guartinel/database/public/databaseConnector.js');
var transactionDB = include('guartinel/database/transaction/databaseConnector.js');
var inMemoryDB = include('guartinel/database/inMemory/databaseConnector.js');
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doGetSupervisorStatus(parameters, function (result) {
                res.send(result);
            });
        }
    });
};

function doGetSupervisorStatus(parameters, callback) {
   if (parameters.token != "engedjbeSzepenKerlek001") {
      var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
         .logMessage("Invalid supervisorStatus token : " + parameters.token)
         .logNow();
      return callback(new ErrorResponse(error));   
    }
    var response = new SuccessResponse();
    var responseMessage = "Public Database: " + publicDB.getConnectionState().state + "\n";
    responseMessage += "Admin database : " + adminDB.getConnectionState().state + "\n";
    responseMessage += "Transaction database : " + transactionDB.getConnectionState().state + "\n";
    responseMessage += "In memory database : " + inMemoryDB.getConnectionState().state + "\n";

    response.message = responseMessage;
    return callback(response);
}