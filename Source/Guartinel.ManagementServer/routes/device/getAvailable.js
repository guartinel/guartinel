var sessionManager = include("guartinel/security/sessionManager.js");

exports.URL =  safeGet(managementServerUrls.DEVICE_GET_AVAILABLE);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }

    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doAvailable(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}

function doAvailable(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (sessionErr, account) {
        if (sessionErr) {
            return callback(new ErrorResponse(sessionErr));
        }

        var DTODevices = [];
        for (var i = 0; i < account.devices.length; i++) {
            var dtoDevice = {};
            dtoDevice[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_TYPE)] = account.devices[i].deviceType;
            dtoDevice[safeGet(commonConstants.ALL_PARAMETERS.NAME)] = account.devices[i].name;
            dtoDevice[safeGet(commonConstants.ALL_PARAMETERS.ID)] = account.devices[i]._id.toString();
            dtoDevice[safeGet(commonConstants.ALL_PARAMETERS.CREATED_ON)] = account.devices[i].createdOn;
            dtoDevice[safeGet(commonConstants.ALL_PARAMETERS.CATEGORIES)] = account.devices[i].categories;
            dtoDevice[safeGet(commonConstants.ALL_PARAMETERS.IS_DISCONNECTED)] = account.devices[i].isDisconnected;
            dtoDevice[safeGet(pluginConstants.HARDWARE_TYPE)] = account.devices[i].hardwareType;
            dtoDevice[safeGet(commonConstants.ALL_PARAMETERS.INSTANCE_ID)] = account.devices[i].instanceId;

            DTODevices.push(dtoDevice);
        }

        var response = new SuccessResponse();
        response[safeGet(commonConstants.ALL_PARAMETERS.DEVICES)] = DTODevices;

        account.save(function(err){
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Cannot save account")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            return callback(response);
        });
    });
}