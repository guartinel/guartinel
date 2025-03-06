/**
 * Created by DTAP on 2016.09.15..
 */
var config = include("guartinel/admin/configuration/configurationController.js");
var securityTool = include("guartinel/security/tool.js");
var watcherServerController = include("guartinel/admin/configuration/watcherServerController.js");

exports.URL = exports.URL = safeGet(managementServerUrls.WATCHER_SERVER_LOGIN);
exports.route = function (req, res) {
    var parameters = {
        watcherServerId: req.body[safeGet(commonConstants.ALL_PARAMETERS.WATCHER_SERVER_ID)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)]
    };
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doLogin(parameters, function (result) {
                res.send(result);
            });
        }
    });
};

function doLogin(parameters, callback) {
    watcherServerController.getWatcherServerById(parameters.watcherServerId, function (server) {
       if (utils.object.isNull(server)) {
          var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
             .logMessage(exports.URL + ": Cannot get WatcherServer by id: " + parameters.watcherServerId)
             .logNow();
          return callback(new ErrorResponse(error));
        }
        var passwordHash2 = securityTool.generatePasswordHash(parameters.watcherServerId, parameters.password);
        if (server.getPasswordHash() != passwordHash2) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD)
              .logMessage(exports.URL + ": Invalid password")
              .severe()
              .logNow();
           return callback(new ErrorResponse(error));
         }
        var msToken = securityTool.generateRandomString(12);
        server.setMSToken(msToken);
        LOG.info("WS\\Login Token is for ws:"+ msToken);
        server.save(function(err){
            if(err) {
                LOG.info("WS\\Login Cannot save server: "+ err);
            }
            var response = new SuccessResponse();
            response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = msToken;
            LOG.info("watcherServer/login response:" + JSON.stringify(response));
            return callback(response);
        });

    });
}