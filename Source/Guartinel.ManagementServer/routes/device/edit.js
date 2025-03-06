/**
 * Created by DTAP on 2016.10.27..
 */

var sessionManager = include('guartinel/security/sessionManager.js');
var moment = require('moment');

exports.URL = safeGet(managementServerUrls.DEVICE_EDIT);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        deviceUUID: req.body[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_UUID)],
        deviceName: req.body[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_NAME)],
        categories : req.body[safeGet(commonConstants.ALL_PARAMETERS.CATEGORIES)]
    };

    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doRename(parameters, function (result) {
                return res.send(result);
            });
        }
    });
};

function doRename(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (sessionErr, account) {
        if (sessionErr) {
            return callback(new ErrorResponse(sessionErr));
        }
        var device = account.devices.id(parameters.deviceUUID);

        if (utils.object.isNull(device)) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DEVICE_UUID)
              .logMessage(exports.URL + "Cannot edit device. Invalid device id: " + parameters.deviceUUID)
              .logNow();
           return callback(new ErrorResponse(error));
        }

        device.name = parameters.deviceName;
        device.categories = parameters.categories;

        device.tokenTimeStamp = moment().subtract(1, 'years');
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
}

