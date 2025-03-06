/**
 * Created by DTAP on 2016.10.15..
 */
/**
 * Created by DTAP on 2016.06.29..
 */

var sessionManager = include('guartinel/security/sessionManager.js');
var database = include('guartinel/database/public/databaseConnector.js');
var moment = require('moment');

exports.URL = safeGet(managementServerUrls.LICENSE_ACTIVATE_LICENSE);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        licenseID: req.body[safeGet(commonConstants.ALL_PARAMETERS.LICENSE_ID)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doActivateLicense(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doActivateLicense(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        database.getLicenseByProperty('_id', parameters.licenseID, function (err, license) {
           if (utils.object.isNull(license)) {
              var error = new MSInternalServerError()
                 .logMessage(exports.URL + ":Invalid trial activation attempt. Account id: " + account._id.toString())
                 .severe()
                 .logNow();
              return callback(new ErrorResponse(error));
             }

            if (license.name === "trial") {
                return handleTrialActivation(account, license, callback);
            }
        });
    });
}

function handleTrialActivation(account, license, callback) {
    for (var i = 0; i < account.licenses.length; i++) {
       if (account.licenses[i].license.name === "trial") {
          var error = new MSInternalServerError()
             .logMessage(exports.URL + ":Invalid trial activation attempt. Account id: " + account._id.toString())
             .severe()
             .logNow();
          return callback(new ErrorResponse(error));
          }
    }

    var newLicense = {
        startDate: moment().toISOString(),
        expiryDate: moment().add(1, 'months').toISOString(),
        license: license,
        payment: {
            "amount":0,
            "paymentInfo":"Free activation"
        }
    }
    account.licenses.push(newLicense);
    account.save(function (err) {
       if (err) {
          var error = new MSInternalServerError()
             .logMessage(exports.URL + ": Cannot save account")
             .severe()
             .logNow();
          return callback(new ErrorResponse(error));
        }
        return callback(new SuccessResponse());
    });
}

