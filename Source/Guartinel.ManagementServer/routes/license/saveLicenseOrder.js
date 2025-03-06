/**
 * Created by DTAP on 2017.10.05..
 */


var sessionManager = include('guartinel/security/sessionManager.js');
var database = include('guartinel/database/public/databaseConnector.js');

exports.URL = safeGet(managementServerUrls.LICENSE_SAVE_LICENSE_ORDER);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        licenseOrder: req.body[safeGet(commonConstants.ALL_PARAMETERS.LICENSE_ORDER)]
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

        parameters.licenseOrder = JSON.parse(parameters.licenseOrder);

        database.getLicenseOrderByProperty('_id', parameters.licenseOrder.ID, function (err, licenseOrder) {
            if (err) {
                return callback(new ErrorResponse(err));
            }

            if (utils.object.isNull(licenseOrder)) {
                licenseOrder = new database.getLicenseOrderModel()({
                    data: parameters.licenseOrder
                });
            } else {
                licenseOrder.data = parameters.licenseOrder
            }

            licenseOrder.save(function (err) {
                if (err) {
                   var error = new MSInternalServerError()
                      .logMessage(exports.URL + ": Cannot save account")
                      .severe()
                      .innerError(err)
                      .logNow();
                   return callback(new ErrorResponse(error));
                }
                var response = new SuccessResponse();
                response[safeGet(commonConstants.ALL_PARAMETERS.ID)] = licenseOrder._id.toString();
                return callback(response);
            });
        });
    });
}

