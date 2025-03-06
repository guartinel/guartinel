
var authenticator = include('guartinel/security/authenticator.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var database = include('guartinel/database/public/databaseConnector.js');
var securityTool = include('guartinel/security/tool.js');
var moment = require('moment');

exports.URL = safeGet(managementServerUrls.DEVICE_LOGIN);
exports.route = function (req, res) {
    var parameters = {
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)],
        deviceUUID: req.body[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_UUID)],
        gcmIdOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.GCM_ID)]
    };

    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doLogin(parameters, function (result) {
                return res.send(result);
            });
        }
    });
};

function doLogin(parameters, callback) {
    if (!database.isIdValid(parameters.deviceUUID)) {
        var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DEVICE_UUID)
            .logMessage(exports.URL + ": Cannot login device. Invalid device id: " + parameters.deviceUUID)
            .logNow();
        return callback(new ErrorResponse(error));
    }

    database.getAccountBySubDocumentProperty('devices', '_id', parameters.deviceUUID, function (err, account) {
        if (utils.object.isNull(account)) {
            var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DEVICE_UUID)
                .logMessage(exports.URL + ": Cannot login device. Invalid device id: " + parameters.deviceUUID)
                .logNow();
            return callback(new ErrorResponse(error));
        }

        var device = account.devices.id(parameters.deviceUUID);
        if (!device) {
            var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DEVICE_UUID)
                .logMessage(exports.URL + ": Cannot login device. Invalid device id: " + parameters.deviceUUID)
                .logNow();
            return callback(new ErrorResponse(error));
        }

        var incomingPasswordHash = securityTool.generatePasswordHash(device._id, parameters.password);
        if (incomingPasswordHash != device.passwordHash) {
            var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD)
                .logMessage(exports.URL + ": Cannot login device. Invalid password  deviceID: " + parameters.deviceUUID)
                .logNow();
            return callback(new ErrorResponse(error));
        }


        if (!utils.object.isNull(parameters.gcmIdOPT)) {
            device.gcmId = parameters.gcmIdOPT;
        }

        sessionManager.createBrowserSession(account, function (err, token) {
            if (err) {
                return callback(new ErrorResponse(err));
            }
            saveTokenForDevice(token);
        });

        function saveTokenForDevice(token) {
            device.token = token;
            device.tokenTimeStamp = moment().toISOString();

            account.save(function (err) {
                if (err) {
                    var error = new MSInternalServerError()
                        .logMessage(exports.URL + ": Cannot save account")
                        .severe()
                        .innerError(err)
                        .logNow();
                    return callback(new ErrorResponse(error));
                }
                var response = new SuccessResponse();
                response[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_NAME)] = device.name;
                response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = token;
                return callback(response);
            });          
        }
    });
}

