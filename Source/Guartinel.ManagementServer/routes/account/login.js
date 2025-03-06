var sessionManager = include('guartinel/security/sessionManager.js');
var authenticator = include('guartinel/security/authenticator.js');

exports.URL = safeGet(managementServerUrls.ACCOUNT_LOGIN) ;
exports.route = function(req, res) {
    var parameters = {
        email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)]
    };
    myRequestValidation(parameters,
        req,
        res,
        function(err) {
            if (!err) {
                doLogin(parameters,
                    function(result) {
                        res.send(result);
                    });
            }
        });
};

function doLogin(parameters, callback) {
        authenticator.authenticateAndGetAccount(parameters.email,parameters.password,function(err,account){
            if(err){
               return callback(new ErrorResponse(err));
            }
            sessionManager.createBrowserSession(account, function (err, token) {
               if (err) {
                  return callback(new ErrorResponse(err));
               }
                var response = new SuccessResponse();
                response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = token;
                return callback(response);
            });
        });
}