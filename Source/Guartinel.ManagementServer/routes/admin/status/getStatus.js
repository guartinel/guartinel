var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var securityTool = include('guartinel/security/tool.js');
var statusInformer = include('guartinel/admin/statusInformer.js');

exports.URL = safeGet(managementServerUrls.ADMIN_STATUS_GET_STATUS);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }
    
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
			doGetStatus(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doGetStatus(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err ));
        }
        statusInformer.getServerInfos(function (result) {
            var response = new SuccessResponse();
            response[safeGet(commonConstants.ALL_PARAMETERS.STATUS)] = result;
            return callback(response);
        });
    });
}