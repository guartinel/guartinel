/**
 * Created by DTAP on 2016.11.15..
 */
var httpRequester = include('guartinel/connection/httpRequester.js');
var sessionManager = include("guartinel/security/sessionManager.js");
var watcherServerController = include('guartinel/admin/configuration/watcherServerController.js');
var moment = require("moment");
var database = include('guartinel/database/public/databaseConnector.js');

exports.URL = safeGet(managementServerUrls.ALERT_CONFIRM_DEVICE_ALERT);
exports.route = function (req, res) {
    var parameters = {
        alertID: req.body[safeGet(commonConstants.ALL_PARAMETERS.ALERT_ID)],
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    };

    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doConfirmDeviceAlert(parameters, function (result) {
                return res.send(result);
            });
        }
    });
};

function doConfirmDeviceAlert(parameters, callback) {
     sessionManager.validateDeviceTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
           return callback(new ErrorResponse(err));
        }
        var device;
        for (var i = 0; i < account.devices.length; i++) {
            if (account.devices[i].token === (parameters.token)) {
                device = account.devices[i];
                break;
            }
        }

        var foundIndex = -1;
        for (var i = 0; i < device.alerts.length; i++) {
            if (device.alerts[i].alertID == parameters.alertID) {
                foundIndex = i;
                break;
            }
        }
        if (foundIndex == -1) {
           var error = new MSInternalServerError()
              .logMessage(exports.URL + ": Cannot find alertID in device alerts")
              .severe()
              .logNow();
           return callback(new ErrorResponse(error));	
         }
        watcherServerController.getWatcherServerById(device.alerts[foundIndex].watcherServerID, function (server) {
            var alertDetails = device.alerts[foundIndex];
            alertDetails.deviceID = device._id;
            alertDetails.alertId  = parameters.alertID;
            device.alerts.splice(foundIndex, 1);

           database.getAccountModel().update({ 'devices.token': parameters.token },
              {
                 $pull: { 'devices.$.alerts': { 'alertID': parameters.alertID } },
                 $set: { 'devices.$.tokenTimeStamp': moment().toISOString() }
              },
              function(err, success) {
                 if (err) {
                    LOG.info("Cannot save changes for confirmDeviceAlert ERR: " + JSON.stringify(err));
                 }

                 httpRequester.confirmDeviceAlert(server,
                    alertDetails,
                    function(err) {
                       if (err) {
                          LOG.error("Cannot send confirmDeviceAlert req to WS: " + err);
                       }
                       return callback(new SuccessResponse());
                    });
              }
           );
        });
    });
}