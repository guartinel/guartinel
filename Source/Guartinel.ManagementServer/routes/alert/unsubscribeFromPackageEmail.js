/**
 * Created by DTAP on 2017.08.18..
 */
exports.URL = safeGet(managementServerUrls.ALERT_UNSUBSCRIBE_FROM_PACKAGE_EMAIL);
var database = include('guartinel/database/public/databaseConnector.js');
var moment = require('moment');

var isRouteDebugEnabled = true;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
    var parameters = {
        blackListToken: req.body[safeGet(commonConstants.ALL_PARAMETERS.BLACK_LIST_TOKEN)],
        packageID: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)]
    };

    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            processRequest(parameters, function (result) {
                return res.send(result);
            });
        }
    });
};

function processRequest(parameters, callback) {
    database.getControlledEmailByProperty('blackListToken', parameters.blackListToken, function (err, controlledEmail) {
          if (err) {
             var error = new MSInternalServerError()
                .logMessage(exports.URL + ": Cannot retrieve controlled email by token :" + parameters.blackListToken)
                .severe()
                .innerError(err)
                .logNow();
             return callback(new ErrorResponse(error));
        }
          if (utils.object.isNull(controlledEmail)) {
             var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
                .logMessage("Invalid blacklist token" + parameters.blackListToken)
                .innerError(err)
                .logNow();
             return callback(new ErrorResponse(error));
          }
        debugRoute("Retrieved controlled email"+ JSON.stringify(controlledEmail));

        if (!database.isIdValid(parameters.packageID)) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
              .logMessage("Invalid package id." + parameters.packageID)
              .innerError(err)
              .logNow();
           return callback(new ErrorResponse(error));
         }

        database.getAccountBySubDocumentProperty('packages', '_id', parameters.packageID, function (err, account) {
             var currentPack = account.packages.id(parameters.packageID);
            if (utils.object.isNull(currentPack)) {
               debugRoute("Invalid package id: " + parameters.packageID);
               var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
                  .logMessage("Invalid package id." + parameters.packageID)
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
             }

            debugRoute("Retrieved controlled email"+ JSON.stringify(controlledEmail));
            debugRoute("Retrieved pack: "+ JSON.stringify(currentPack));
            var indexOfRemoveAbleEmail = currentPack.alertEmails.indexOf(controlledEmail.email);
            if (indexOfRemoveAbleEmail == -1) {
               debugRoute("Something went wrong cannot find alert email between alert emails package id: " + controlledEmail.email);
               var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
                  .logMessage("Something went wrong cannot find alert email between alert emails package id: " + controlledEmail.email)
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            currentPack.lastModificationTimeStamp = moment().toISOString();

            debugRoute("Removing index from alert emails: " + indexOfRemoveAbleEmail);
            currentPack.alertEmails.splice(indexOfRemoveAbleEmail, 1);

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
    });
};