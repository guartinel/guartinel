var sessionManager = include('guartinel/security/sessionManager.js');
var constants = include('guartinel/constants.js');
var database = include("guartinel/database/public/databaseConnector.js");
var loginAuth = include("guartinel/security/authenticator.js");
var securityTool = include('guartinel/security/tool.js');
var moment = require('moment');

exports.URL = safeGet(managementServerUrls.DEVICE_REGISTER);
exports.route = function (req, res) {
    var parameters = {
        email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)],
        deviceType: req.body[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_TYPE)],
        deviceName: req.body[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_NAME)],
        gcmIdOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.GCM_ID)],
        uid: req.body[safeGet(commonConstants.ALL_PARAMETERS.UID)],
        categoryOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.CATEGORY)],
        overWrite : req.body[safeGet(commonConstants.ALL_PARAMETERS.OVER_WRITE)],
        properties : req.body[safeGet(commonConstants.ALL_PARAMETERS.PROPERTIES)]
    };

    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doRegister(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}

function doRegister(parameters, callback) {
    loginAuth.authenticateAndGetAccount(parameters.email, parameters.password, function (authErr, account) {
        if (authErr) {
            return callback(new ErrorResponse(authErr));
        }

        for (var i = 0; i < account.devices.length; i++) {
            if (account.devices[i].name === parameters.deviceName ) {
               if (!parameters.overWrite) {
                  var error = new MSError(commonConstants.ALL_ERROR_VALUES.DEVICE_NAME_TAKEN)
                     .logMessage(exports.URL + ": Cannot register device. Device name is taken.")
                     .logNow();
                  return callback(new ErrorResponse(error));
               }else {
                  LOG.info("Overwriting device.");
                  return  overWriteDevice(parameters,account,account.devices[i]._id,callback);
                }
           }
        }
        if (account.getAllPackagePartCount() + 1 > account.getLicenseAggregate().maximumPackagePartCount) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.MAXIMUM_DEVICE_COUNT_REACHED)
              .logMessage(exports.URL + ": Cannot register device. Account reached its maximum device count. ID:" + account._id)
              .logNow();
           return callback(new ErrorResponse(error));
         }

        var tokenHash = securityTool.generateToken();
        var newDevice = {
            deviceType: parameters.deviceType,
            name: parameters.deviceName,
            token: tokenHash,
            tokenTimeStamp: moment().toISOString(),
            uid: parameters.uid
        };

        if (!utils.object.isNull(parameters.properties)) {
            newDevice.properties = parameters.properties;
        }

        if (!utils.object.isNull(parameters.gcmIdOPT)) {
            newDevice.gcmId = parameters.gcmIdOPT;
        }
        if(!utils.object.isNull(parameters.categoryOPT)){
            newDevice.categories = [];
            newDevice.categories.push(parameters.categoryOPT);
        }
        account.devices.push(newDevice);

        for (var i = 0; i < account.devices.length; i++) {
            if (account.devices[i].name == newDevice.name) {
                newDevice.id = account.devices[i]._id.toString();
            }
        }

        var passwordHash1 = securityTool.generatePasswordHash(newDevice.id,parameters.uid);
        var passwordHash2 = securityTool.generatePasswordHash(newDevice.id,passwordHash1);
        account.devices.id(newDevice.id).passwordHash = passwordHash2;

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
            response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = newDevice.token;
            response[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_UUID)] = newDevice.id;// account.devices[account.devices.length - 1]._id.toString();
            return callback(response);
        });
    });
}

function overWriteDevice(parameters,account,deviceID,callback){
    var device = account.devices.id(deviceID);
    device.deviceType = parameters.deviceType;
    device.name = parameters.deviceName;
    device.token = securityTool.generateToken();
    device.tokenTimeStamp = moment().toISOString();
    device.gcmId = parameters.gcmIdOPT;
    device.categories = parameters.categories;

    var passwordHash1 = securityTool.generatePasswordHash(device._id,parameters.uid);
    var passwordHash2 = securityTool.generatePasswordHash(device._id,passwordHash1);
    device.passwordHash = passwordHash2;

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
        response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = device.token;
        response[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_UUID)] = device._id;
        return callback(response);
    });
}