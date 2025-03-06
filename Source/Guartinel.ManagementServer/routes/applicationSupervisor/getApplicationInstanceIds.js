/**
 * Created by DTAP on 2017.04.28..
 */
var database = include("guartinel/database/public/databaseConnector.js");
var sessionManager = include("guartinel/security/sessionManager.js");

exports.URL = safeGet(managementServerUrls.APPLICATION_SUPERVISOR_GET_APPLICATION_INSTANCE_IDS);

var isRouteDebugEnabled = false;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        packageId: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)]
    }
    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doGetInstanceIds(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}

function doGetInstanceIds(parameters, callback) {
    sessionManager.validateMSTokenAndGetWatcherServer(parameters.token, afterSessionValidation);
    function afterSessionValidation(sessionErr, server) {
        debugRoute("Session retrieved");
        if (sessionErr) {
             return callback(new ErrorResponse(sessionErr));
        }
        if (!database.isIdValid(parameters.packageId)) {
           LOG.debug("The packageId is invalid");

           var error = new MSInternalServerError()
              .logMessage(exports.URL + ": Invalid package id:" + parameters.packageId)
              .severe()              
              .logNow();
           return callback(new ErrorResponse(error));
         }
        database.getAccountBySubDocumentProperty('packages', '_id', database.toObjectId(parameters.packageId), afterPackageRetrieval);

        function afterPackageRetrieval(err, account) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ":Error with package retrieval. ")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            if (utils.object.isNull(account)) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Invalid package id:" + parameters.packageId)
                  .severe()
                  .logNow();
               return callback(new ErrorResponse(error));
              }

            var pack = account.packages.id(parameters.packageId);

            var instanceIds = [];
            if (!utils.object.isNullOrEmpty(pack.configuration.instances)) {
                for (var instanceId = 0; instanceId < pack.configuration.instances.length; instanceId++) {
                    instanceIds.push(pack.configuration.instances[instanceId].id);
                }
            }

            var response = new SuccessResponse();
            response[safeGet(pluginConstants.INSTANCE_IDS)] = instanceIds;
            return callback(response);
        }
    }
}

