/**
 * Created by DTAP on 2017.08.07..
 */
var sessionManager = include("guartinel/security/sessionManager.js");

exports.URL = safeGet(managementServerUrls.API_PACKAGE_GET_ALL);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            processRequest(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function processRequest(parameters, callback) {
    sessionManager.validateAPITokenAndGetAccount(parameters.token, function (sessionErr, account) {
            if (sessionErr) {
                return callback(new ErrorResponse(sessionErr));
            }

            var resultPackages = [];
            for (var index = 0; index < account.packages.length; index++) {
                var pack = account.packages[index];
               if (pack.isDeleted) {
                  continue;
               }
                resultPackages.push({
                    package_name: pack.packageName,
                    version: pack.version,
                    is_enabled : pack.isEnabled
                });
            }

            var result = new SuccessResponse();
            result[safeGet(commonConstants.ALL_PARAMETERS.PACKAGES)] = resultPackages;
            return callback(result);
        }
    );
}

