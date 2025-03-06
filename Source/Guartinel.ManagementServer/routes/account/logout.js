var constants = include('guartinel/constants.js');
var database = include('guartinel/database/public/databaseConnector.js');
var sessionManager = include("guartinel/security/sessionManager.js");

exports.URL = exports.URL = safeGet(managementServerUrls.ACCOUNT_LOGOUT);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }
    
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
           doLogout(parameters,function(result){
               res.send(result);
           });
        }
    });
}

function doLogout(parameters,callback){
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
       if(err){
          return callback(new ErrorResponse(err));
       }
        for (var i in account.browserSessions) {
            var session = account.browserSessions[i];
            if (session.token === parameters.token) {
                session.remove();
                break;
            }
        }
        account.save(function (err) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Cannot save account")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            return callback(new SuccessResponse());
        });
    });
}

