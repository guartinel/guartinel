var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var securityTool = include('guartinel/security/tool.js');
var requestStat = include('guartinel/admin/requestStat.js');

exports.URL = safeGet(managementServerUrls.ADMIN_STATUS_GET_INVALID_REQUESTS);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }
    
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doGetInvalidRequests(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doGetInvalidRequests(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse( err ));
        }
        requestStat.getInvalidRequests(function (result) {
            var response = new SuccessResponse();
            response[safeGet(commonConstants.ALL_PARAMETERS.INVALID_REQUESTS)] = result;
            return callback(response);
        });
    });
}