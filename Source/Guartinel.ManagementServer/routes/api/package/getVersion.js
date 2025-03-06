/**
 * Created by DTAP on 2017.08.07..
 */
var sessionManager = include("guartinel/security/sessionManager.js");

exports.URL = safeGet(managementServerUrls.API_PACKAGE_GET_VERSION);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        packageName: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)]
    };
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            processRequest(parameters, function (result) {
                res.send(result);
            });
        }
    });
};

function processRequest(parameters, callback) {
    sessionManager.validateAPITokenAndGetAccount(parameters.token, function (sessionErr, account) {
            if (sessionErr) {
                return callback(new ErrorResponse(sessionErr));
            }

            var foundPack;
            for (var index = 0; index < account.packages.length; index++) {
                if (account.packages[index].packageName === parameters.packageName) {
                    foundPack = account.packages[index];
                    break;
                }
            }

            if (utils.object.isNull(foundPack)) {
               var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_PACKAGE_NAME)
                  .logMessage("Cannot find package by name: " + parameters.packageName)
                  .logNow();
               return callback(new ErrorResponse(error));
             }

            var result = new SuccessResponse();
            result[safeGet(commonConstants.ALL_PARAMETERS.VERSION)] = foundPack.version;
            return callback(result);
        }
    );
}