var sessionManager = include("guartinel/security/sessionManager.js");
var constants = include("guartinel/constants.js");
var database = include("guartinel/database/public/databaseConnector.js");

exports.URL = exports.URL = safeGet(managementServerUrls.DEVICE_DELETE);
exports.route = function(req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        deviceUUID: req.body[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_UUID)]
    };

    myRequestValidation(parameters,
        req,
        res,
        function(requestErr) {
            if (!requestErr) {
                doDelete(parameters,
                    function(result) {
                        return res.send(result);
                    });
            }
        });
};

function doDelete(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (sessionErr, account) {
        if (sessionErr) {
            return callback(new ErrorResponse(sessionErr));
        }
        if (!database.isIdValid(parameters.deviceUUID)) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DEVICE_UUID)
              .logMessage(exports.URL + "Cannot delete device. Invalid device id: " + parameters.deviceUUID)
              .logNow();
           return callback(new ErrorResponse(error));
         }
        var device = account.devices.id(parameters.deviceUUID);
        if (utils.object.isNull(device)) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DEVICE_UUID)
              .logMessage(exports.URL + "Cannot delete device. Invalid device id: " + parameters.deviceUUID)
              .logNow();
           return callback(new ErrorResponse(error));
        }
        device.remove();
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