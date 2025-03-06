var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var eventsInformer = include('guartinel/admin/eventsInformer.js');

exports.URL = safeGet(managementServerUrls.ADMIN_STATUS_GET_EVENTS);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }
    
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doGetEvents(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doGetEvents(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err ));
        }
        eventsInformer.getEvents(function ( result) {
            var response = new SuccessResponse();
            response[safeGet(commonConstants.ALL_PARAMETERS.EVENTS)] = result;
            return callback(response);
        });
    });
}