var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var fs = require('fs');

exports.URL = safeGet(managementServerUrls.ADMIN_STATUS_GET_LOG);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }
    
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doGetLog(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doGetLog(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(  err ));
        }
        fs.readFile(getGuartinelHome()+'logs\\management-server.log', 'utf8',function (err, result) {
            var response = new SuccessResponse();
            response[safeGet(commonConstants.ALL_PARAMETERS.LOG)] = result;
            return callback(response);
        });
    });
}