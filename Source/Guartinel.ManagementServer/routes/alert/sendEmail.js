var emailer = include("guartinel/connection/emailer.js");
var sessionManager = include("guartinel/security/sessionManager.js");
var database = include('guartinel/database/public/databaseConnector.js');
var alertMessageBuilder = include("guartinel/utils/alertMessageBuilder.js");

exports.URL = safeGet(managementServerUrls.ALERT_SEND_EMAIL);
exports.route = function (req, res) {
   var parameters = {
      token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
      alertEmail: req.body[safeGet(commonConstants.ALL_PARAMETERS.ALERT_EMAIL)],
      packageId: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)],
      alertMessage: req.body[safeGet(commonConstants.ALL_PARAMETERS.ALERT_MESSAGE)],
      alertDetails: req.body[safeGet(commonConstants.ALL_PARAMETERS.ALERT_DETAILS)],
      isRecovery: req.body[safeGet(commonConstants.ALL_PARAMETERS.IS_RECOVERY)],
      instanceID: req.body[safeGet(commonConstants.ALL_PARAMETERS.INSTANCE_ID)],
      packageState: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_STATE)]
   };

   myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         doSendEmail(parameters, function (result) {
            res.send(result);
         });
      }
   });
};

function doSendEmail(parameters, callback) {
   var isPartiallyRecoverd;
   var pack;
   var usePlainAlertEmail;
   var alertMessage;
   var alertDetails;
   var account;
   sessionManager.validateMSTokenAndGetWatcherServer(parameters.token, function (err, server) {
      if (err) {
         return callback(new ErrorResponse(err));
      }

      database.getAccountBySubDocumentProperty('packages', '_id', parameters.packageId, afterAccountRetrieved);
      function afterAccountRetrieved(err, _account) {
         if (err) {
            return callback(new ErrorResponse(err));
         }
         account = _account;
         if (utils.object.isNull(account)) {
            var error = new MSInternalServerError()
               .logMessage(exports.URL + ": PackageID is invalid " + parameters.packageId)
               .severe()
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
         pack = account.packages.id(parameters.packageId);
          usePlainAlertEmail = pack.usePlainAlertEmail;
          //this is a recovery but the state remains in alerted state so this is a partial recovery if the state is alerting
         isPartiallyRecoverd = false;
         if (parameters.packageState === safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_STATE_ALERTING)) {
            isPartiallyRecoverd = true;
         }
         emailer.sendPackageStatusChangeEmail(parameters.alertEmail, pack.packageName, pack.usePlainAlertEmail, parameters.isRecovery, isPartiallyRecoverd, alertMessage, alertDetails,"", afterMailSend);
      }

      function afterMailSend(emailErr) {
         if (emailErr) {
            return sendToAccountOwner();
         }
         return callback(new SuccessResponse());
      }

      function sendToAccountOwner() {
         var cannotReachDeviceError = "\\n(You received this email because your alert email " +
            parameters.alertEmail +
            " is not available. Please remove that alert email address from you package)";
          emailer.sendPackageStatusChangeEmail(account.email, pack.packageName, usePlainAlertEmail, parameters.isRecovery, isPartiallyRecoverd, alertMessage, alertDetails, cannotReachDeviceError, function (emailErr) {
            if (emailErr) {
                 return callback(new ErrorResponse(emailErr));
            }
            callback(new SuccessResponse());
         });
      }
   });
}
