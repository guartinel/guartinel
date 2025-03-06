/**
 * Created by DTAP on 2016.06.29..
 */

var sessionManager = include('guartinel/security/sessionManager.js');
var database = include('guartinel/database/public/databaseConnector.js');

exports.URL = safeGet(managementServerUrls.LICENSE_ADD_TO_ACCOUNT);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        licenseID: req.body[safeGet(commonConstants.ALL_PARAMETERS.LICENSE_ID)],
        startDate: req.body[safeGet(commonConstants.ALL_PARAMETERS.START_DATE)],
        expiryDate: req.body[safeGet(commonConstants.ALL_PARAMETERS.EXPIRY_DATE)],
        payment: req.body[safeGet(commonConstants.ALL_PARAMETERS.PAYMENT)]
    };

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doAddToAccount(parameters, function (result) {
                res.send(result);
            });
        }
    });
};

function doAddToAccount(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }

        database.getLicenseByProperty('_id',parameters.licenseID,function(err,license){
            var newLicense = {
                startDate: parameters.startDate,
                expiryDate: parameters.expiryDate,
                license: license,
                payment:parameters.payment
            }
            account.licenses.push(newLicense);
            account.save(function (err) {
                if (err) {
                   var error = new MSInternalServerError()
                      .logMessage(exports.URL + ": Cannot save account")
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

