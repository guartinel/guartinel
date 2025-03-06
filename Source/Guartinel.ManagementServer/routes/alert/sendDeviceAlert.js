var gcmManager = include('guartinel/connection/gcmManager.js');
var database = include('guartinel/database/public/databaseConnector.js');
var sessionManager = include("guartinel/security/sessionManager.js");
var alertMessageBuilder = include("guartinel/utils/alertMessageBuilder.js");
var emailer = include("guartinel/connection/emailer.js");
var httpRequester = include('guartinel/connection/httpRequester.js');

exports.URL = safeGet(managementServerUrls.ALERT_SEND_DEVICE_ALERT);

var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
   var parameters = {
      token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
      packageID: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)],
      alertID: req.body[safeGet(commonConstants.ALL_PARAMETERS.ALERT_ID)],
      alertDeviceId: req.body[safeGet(commonConstants.ALL_PARAMETERS.ALERT_DEVICE_ID)],
      alertMessage: req.body[safeGet(commonConstants.ALL_PARAMETERS.ALERT_MESSAGE)],
      alertDetails: req.body[safeGet(commonConstants.ALL_PARAMETERS.ALERT_DETAILS)],
      instanceID: req.body[safeGet(commonConstants.ALL_PARAMETERS.INSTANCE_ID)],
      isRecovery: req.body[safeGet(commonConstants.ALL_PARAMETERS.IS_RECOVERY)],
      packageState: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_STATE)],
      forcedDeviceAlertOPT : req.body[safeGet(commonConstants.ALL_PARAMETERS.FORCED_DEVICE_ALERT)]
   };

   myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         doSendDeviceAlert(parameters, function (result) {
            return res.send(result);
         });
      }
   });
};

function doSendDeviceAlert(parameters, callback) {
   var alertMessage;
   var alertDetails;
   var alertDevice;
   var packageName;
   var server;
   var account;
   var isPackageAlerted;
   var newAlert;
   var usePlainAlertEmail;

   sessionManager.validateMSTokenAndGetWatcherServer(parameters.token, afterSessionValidated);
   function afterSessionValidated(err, _server) {
      if (err) {
         return callback(err);
      }
      server = _server;
      database.getAccountBySubDocumentProperty('devices', '_id', parameters.alertDeviceId, afterAccountForDeviceRetrieved);
   }

   function afterAccountForDeviceRetrieved(err, _account) {
      account = _account;
      if (err) {
         var error = new MSInternalServerError()
            .logMessage(exports.URL + ": Cannot get device by id from DB")
            .severe()
            .innerError(err)
            .logNow();
         return callback(new ErrorResponse(error));
      }
      if (!database.isIdValid(parameters.alertDeviceId) || utils.object.isNull(account)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DEVICE_UUID)
            .logMessage("Cannot send gcm. Invalid device id: " + parameters.alertDeviceId)
            .logNow();
         return callback(new ErrorResponse(error));
      }

      alertDevice = account.devices.id(parameters.alertDeviceId);

      if (utils.object.isNull(alertDevice)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_DEVICE_UUID)
            .logMessage("Cannot send gcm. Invalid device id: " + parameters.alertDeviceId)
            .logNow();
         return callback(new ErrorResponse(error));
      }
      debugRoute("Alert device found.");
      if (utils.object.isNull(alertDevice.gcmId)) {
         var error = new MSInternalServerError()
            .logMessage("Missing GCM id for device:" + parameters.alertDeviceId)
            .severe()
            .logNow();
         return callback(new ErrorResponse(error));
      }

      debugRoute("GCM id found found.");

      newAlert = {
         alertID: parameters.alertID,
         watcherServerID: server.getId(),
         packageID: parameters.packageID,
         instanceID: parameters.instanceID
      };
      debugRoute("New alert created: " + JSON.stringify(newAlert));
    

      var length = alertDevice.alerts.unshift(newAlert);
      debugRoute("New alert added to alerts. Length: " + length);
      if (length >= 10) {
         debugRoute("Slicing device alerts because it was longer than 10 element.");
         alertDevice.alerts = alertDevice.alerts.slice(0, 10);
         debugRoute("After slice. Length: " + alertDevice.alerts.length);
      }
      isPackageAlerted = false;
      if (parameters.packageState === safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_STATE_ALERTING)) {
         isPackageAlerted = true;
      }
      debugRoute("Sending gcm.");

      database.getAccountBySubDocumentProperty('packages', '_id', parameters.packageID, afterAccountForPackageRetrieved);
   }

   function afterAccountForPackageRetrieved(err, packageAccount) {
      if (err) {
         var error = new MSInternalServerError()
            .logMessage(exports.URL + ": Cannot get device by id from DB")
            .severe()
            .innerError(err)
            .logNow();
         return callback(new ErrorResponse(error));
       }
       try {
           alertMessage = alertMessageBuilder.build(account, parameters.alertMessage);
           alertDetails = alertMessageBuilder.build(account, parameters.alertDetails);
       } catch (err) {
           var error = new MSInternalServerError()
               .logMessage(exports.URL + ": Cannot build alert message.")
               .severe()
               .innerError(err)
               .logNow();
           return callback(new ErrorResponse(error));
       }
       debugRoute("Alert message created: " + alertMessage);
      packageName = packageAccount.packages.id(parameters.packageID).packageName;
      usePlainAlertEmail = packageAccount.packages.id(parameters.packageID).usePlainAlertEmail;
       gcmManager.sendGcm(alertDevice.gcmId, parameters.forcedDeviceAlertOPT, alertMessage, alertDetails, parameters.alertID, packageName, parameters.isRecovery, isPackageAlerted, afterGCMSend);
   }

   function afterGCMSend(gcmErr) {
      if (gcmErr) {
         debugRoute("Cannot send gcm. " + JSON.stringify(gcmErr));
         var error = new MSInternalServerError()
            .logMessage("Cannot send gcm. Invalid device id: " + parameters.alertDeviceId)
            .severe()
            .logNow();
         return sendAlertAsEmail(); // try to send alert as email
      } else {
         debugRoute("GC sent without error");
         account.save(function (err) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Cannot save account")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            debugRoute("Save done.");
            return callback(new SuccessResponse());
         });
      }
   }

   function sendAlertAsEmail() {
      var cannotReachDeviceError = "(You received this email because your alert device " +
         alertDevice.name +
         " is not available. Please try to start the Guartinel application on it or remove it from the alert devices in your package.)";
      debugRoute("Sending the following alert email: " + alertMessage);

      emailer.sendPackageStatusChangeEmail(account.email, packageName, usePlainAlertEmail, parameters.isRecovery, isPackageAlerted, alertMessage , alertDetails, cannotReachDeviceError, function (emailErr) {
         if (emailErr) {
            debugRoute("Cannot sent as email either..");
            return callback(new ErrorResponse(emailErr));
         }
         debugRoute("Sent as email and returning request successful.");
         callback(new SuccessResponse());
         debugRoute("Starting confirm deviceAlert request to prevent alert send retry.");
         httpRequester.confirmDeviceAlert(server,
            newAlert,
            function (err) {
               if (err) {
                  LOG.error("Cannot send confirmDeviceAlert req to WS: " + err);
                  return;
               }
               debugRoute("Device alert confirmed.");
               return;
            });
      });
   }
}