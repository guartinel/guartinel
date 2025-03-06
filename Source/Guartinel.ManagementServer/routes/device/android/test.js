var sessionManager = include('guartinel/security/sessionManager.js');
var gcmManager = include('guartinel/connection/gcmManager.js');

exports.URL = safeGet(managementServerUrls.DEVICE_ANDROID_TEST);
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
                doTest(parameters,
                    function(result) {
                        return res.send(result);
                    });
            }
        });
};

function doTest(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (sessionErr, account) {
        if (sessionErr) {
            return callback(new ErrorResponse(sessionErr));
        }

        var device = account.devices.id(parameters.deviceUUID);
        if (utils.object.isNull(device)) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DEVICE_UUID)
              .logMessage(exports.URL + "Invalid device uuid " + parameters.deviceUUID)
              .logNow();
            return callback(new ErrorReponse(error));
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
            gcmManager.sendGcm(device.gcmId, false, "This is a test alert.", "(Ignore it.)", utils.string.generateUUID(),"Test Alert",false,false, function (gcmErr) {
                if (gcmErr) {
                   var error = new MSError(commonConstants.ALL_ERROR_VALUES.CANNOT_TEST_DEVICE)
                      .logMessage(exports.URL + "Invalid device uuid " + device.gcmId)
                      .innerError(gcmErr)
                      .logNow();
                   return callback(new ErrorResponse(error));
                  }
                return callback(new SuccessResponse());
            });
        });

    });
}