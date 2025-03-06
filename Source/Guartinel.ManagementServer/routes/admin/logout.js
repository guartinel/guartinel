var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var authenticator = include('guartinel/security/authenticator.js');
var config = include('guartinel/admin/configuration/configurationController.js');

exports.URL = safeGet(managementServerUrls.ADMIN_LOGOUT);
exports.route = function (req, res) {
    var parameters = {
        token : req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doLogout(parameters, function (result) {
                res.send(result);
            });
        }
    });
};

function doLogout(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse( err ));
        }

        config.getAdminAccount(function(err,adminAccount){
            for(var i = 0; i< adminAccount.tokens.length;i++){
                if(adminAccount.tokens[i] == parameters.token){
                    adminAccount.tokens.splice(i,1);
                }
            }
            adminAccount.save(function(err){
                return callback(new SuccessResponse());
            });
        });
    });
}