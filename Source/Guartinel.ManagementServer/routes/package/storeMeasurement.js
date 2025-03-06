var database = include('guartinel/database/public/databaseConnector.js');
var sessionManager = include("guartinel/security/sessionManager.js");
var moment = require("moment");
var transactionDatabaseConnector = include("guartinel/database/transaction/databaseConnector.js");

var isRouteDebugEnabled = true;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(URL + " " + message);
}

exports.URL = safeGet(managementServerUrls.PACKAGE_STORE_MEASUREMENT);
exports.route = function (req, res) {
   var parameters = {
      measurement: req.body[safeGet(commonConstants.ALL_PARAMETERS.MEASUREMENT)],
      measurementTimeStamp: req.body[safeGet(commonConstants.ALL_PARAMETERS.MEASUREMENT_TIMESTAMP)],
      packageID: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)],
      token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
   };

   myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         doStoreMeasurement(parameters, function (result) {
            return res.send(result);
         });
      }
   });
}

function doStoreMeasurement(parameters, callback) {
   sessionManager.validateMSTokenAndGetWatcherServer(parameters.token, function (err, server) {
      if (err) {
         return callback(err);
      }
      database.getAccountBySubDocumentProperty('packages', '_id', parameters.packageID, function (err, account) {
         if (err) {
            return callback(new ErrorResponse(err));
         }

         if (utils.object.isNull(account)) {
            var error = new MSInternalServerError()
               .logMessage(exports.URL + ": Invalid packageID " + parameters.packageID)
               .severe()
               .logNow();
            return callback(new ErrorResponse(error));
         }

         var dbTimeStamp = moment(parameters.measurementTimeStamp).format("YYYY-MM-DD HH:mm:ss");
         transactionDatabaseConnector.storeMeasurement(parameters.packageID, dbTimeStamp, utils.string.tryStringify(parameters.measurement), afterMySQLFinished);

         function afterMySQLFinished() {
          /*  var package = account.packages.id(parameters.packageID);
            var measurement = {
               data: parameters.measurement,
               timeStamp: moment().toISOString()
            };        
            package.measurements = [];
            package.measurements[0] = measurement;
            database.getAccountModel().update(
               { 'packages._id': database.toObjectId(parameters.packageID) },
               { $set: { 'packages.$.measurements': package.measurements } }, function (err, success) {
                  if (err) {
                     var error = new MSInternalServerError()
                        .logMessage(exports.URL + ": Cannot save account after measurement set")
                        .severe()
                        .innerError(err)
                        .logNow();
                     return callback(new ErrorResponse(error));
                  }
                  return callback(new SuccessResponse());
               });*/
            return callback(new SuccessResponse());
         }
      });
   });
}