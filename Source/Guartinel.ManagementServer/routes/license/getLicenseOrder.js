/**
 * Created by DTAP on 2017.10.05..
 */


var sessionManager = include('guartinel/security/sessionManager.js');
var database = include('guartinel/database/public/databaseConnector.js');

exports.URL = safeGet(managementServerUrls.LICENSE_GET_LICENSE_ORDER);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        licenseOrderID: req.body[safeGet(commonConstants.ALL_PARAMETERS.ID)]
    };

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            processRequest(parameters, function (result) {
                res.send(result);
            });
        }
    });
};

function processRequest(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }

        database.getLicenseOrderByProperty('_id',parameters.licenseOrderID,function(err,licenseOrder){
            if (err) {
                return callback(new ErrorResponse(err));
            }

            if (utils.object.isNull(licenseOrder)) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Invalid licenseID:" + parameters.licenseOrderID)
                  .severe()
                  .logNow();
               return callback(new ErrorResponse(error));
            }

           var response = new SuccessResponse();
            response[safeGet(commonConstants.ALL_PARAMETERS.LICENSE_ORDER)] = licenseOrder.data;
            return callback(response);
        });
    });
}

