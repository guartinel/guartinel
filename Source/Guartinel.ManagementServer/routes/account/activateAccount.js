var databaseConnector = include("guartinel/database/public/databaseConnector.js");
var licenseController = include("guartinel/license/licenseController.js");
var moment = require('moment');
var emailer = include('guartinel/connection/emailer.js');
var securityTool = include('guartinel/security/tool.js');
var sessionManager = include('guartinel/security/sessionManager.js');


exports.URL = safeGet(managementServerUrls.ACCOUNT_ACTIVATE_ACCOUNT);
exports.route = function (req, res) {
   var parameters = {
      activationCode: req.body[safeGet(commonConstants.ALL_PARAMETERS.ACTIVATION_CODE)],
      email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)]
   }

   myRequestValidation(parameters, req, res, function (err) {
      if (!err) {
         doActivate(parameters, function (result) {
            res.send(result);
         });
      }
   });
}

function doActivate(parameters, callback) {
   databaseConnector.getAccountByMultiplePropertyValues('email', [securityTool.encryptText(parameters.email),parameters.email], function (err, account) {
      if (err) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD)
            .logMessage("Invalid user name " + parameters.email).innerError(err);
         LOG.logMSError(error);
         return callback(new ErrorResponse(error));
      }

      if (account.activationInfo.activationCode != parameters.activationCode) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_ACTIVATION_CODE)
            .logMessage("Invalid activation code attempt:" + parameters.activationCode);
         LOG.logMSError(error);
         return callback(new ErrorResponse(error));
      }
      licenseController.getTrialLicense(afterTrialRetrieval);
      function afterTrialRetrieval(err, trialLicense) {
         var newLicense = {
            startDate: moment().toISOString(),
            expiryDate: moment().add(10, 'days').toISOString(),
            license: trialLicense,
            payment: {
               "amount": 0,
               "paymentInfo": "Free activation"
            }
         }
         account.licenses.push(newLicense);
         account.activationInfo.isActivated = true;

         emailer.sendEmailAboutTrialActivation(account.email, afterEmailNotification);
         function afterEmailNotification() {
            account.save(function (err) {
               if (err) {
                  var error = new MSInternalServerError()
                     .logMessage("Cannot save document").innerError(err);
                  LOG.logMSError(error);
                  return callback(new ErrorResponse(error));
               }

               createSessionForUser();
            });
         }

         function createSessionForUser() {
            sessionManager.createBrowserSession(account, function (err, token) {
               if (err) {
                  return callback(new ErrorResponse(err));
               }
               var response = new SuccessResponse();
               response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = token;
               return callback(response);
            });
         }
      }
   });

}