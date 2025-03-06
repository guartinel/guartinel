/**
 * Created by DTAP on 2017.03.13..
 */
var database = include('guartinel/database/public/databaseConnector.js');
var sessionManager = include("guartinel/security/sessionManager.js");
var moment = require("moment");
var alertMessageBuilder = include("guartinel/utils/alertMessageBuilder.js");
var transactionDatabaseConnector = include("guartinel/database/transaction/databaseConnector.js");

exports.URL = safeGet(managementServerUrls.PACKAGE_STORE_STATE);

var isRouteDebugEnabled = false;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(URL + " " + message);
}
exports.route = function (req, res) {
    var parameters = {
        packageID: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)],
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        state: req.body[safeGet(commonConstants.ALL_PARAMETERS.STATE)]
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
           if ( utils.object.isNull(account)) {
              var error = new MSInternalServerError()
                 .logMessage(exports.URL + ": Invalid package ID " + parameters.packageID )
                 .severe()
                 .logNow();
              return callback(new ErrorResponse(error));
            }
            var dbTimeStamp = moment().format("YYYY-MM-DD HH:mm:ss");
            var stateMessageBuilt = alertMessageBuilder.build(account, parameters.state.message);
            var stateMessageDetailsBuilt = "";
            if (!utils.object.isNull(parameters.state.message_details)) {
               stateMessageDetailsBuilt = alertMessageBuilder.build(account, parameters.state.message_details);
            }
           var stateName = parameters.state.name;
           transactionDatabaseConnector.storeState(parameters.packageID, dbTimeStamp, stateName, stateMessageBuilt, stateMessageDetailsBuilt, afterMySQLFinished);

            function afterMySQLFinished() {
                var package = account.packages.id(parameters.packageID);
               
                for (var index = 0; index < parameters.state.states.length; index++) {
                    var statePart = parameters.state.states[index];
                    statePart.package_part_message_built = alertMessageBuilder.build(account, statePart.package_part_message);   
                    statePart.package_part_details_built = alertMessageBuilder.build(account, statePart.package_part_details);
                    statePart.package_part_extract_built = alertMessageBuilder.build(account, statePart.package_part_extract);
                }
                var state = {
                    name: parameters.state.name,
                    states: parameters.state.states,
                    message: parameters.state.message,
                    message_built: stateMessageBuilt,
                    message_details: parameters.state.message_details,
                    message_details_built: stateMessageDetailsBuilt,
                    timeStamp: moment().toISOString()
                };
                database.getAccountModel().update(
                    {'packages._id': database.toObjectId(parameters.packageID)},
                    {$set: {'packages.$.state': state}}, function (err, success) {
                        if (err) {
                           var error = new MSInternalServerError()
                              .logMessage(exports.URL + ": Cannot save account after state set.")
                              .severe()
                              .innerError(err)
                              .logNow();
                           return callback(new ErrorResponse(error));	
                        }
                        return callback(new SuccessResponse());
                    });
            }
        });
    });
}