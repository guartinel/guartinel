/**
 * Created by DTAP on 2016.09.15..
 */
var config = include("guartinel/admin/configuration/configurationController.js");
var securityTool = include("guartinel/security/tool.js");
var watcherServerController = include('guartinel/admin/configuration/watcherServerController.js');

exports.URL = exports.URL = safeGet(managementServerUrls.WATCHER_SERVER_REGISTER);
exports.route = function (req, res) {
   var parameters = {
      uid: req.body[safeGet(commonConstants.ALL_PARAMETERS.UID)],
      token: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)]
   };
   myRequestValidation(parameters, req, res, function (err) {
      if (!err) {
         doRegister(parameters, function (result) {
            res.send(result);
         });
      }
   });
};

function doRegister(parameters, callback) {
   watcherServerController.getWatcherServerByToken(parameters.token, function (server) {
      if (utils.object.isNull(server)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
            .logMessage(exports.URL + ": Cannot get watcher server")
            .severe()
            .logNow();
         return callback(new ErrorResponse(error));
      }
      var watcherServerId = server.getId();
      var passwordHash1 = securityTool.generatePasswordHash(watcherServerId, parameters.uid);
      var passwordHash2 = securityTool.generatePasswordHash(watcherServerId, passwordHash1);
      server.setPasswordHash(passwordHash2);
      var msToken = securityTool.generateRandomString(12);
      server.setMSToken(msToken);
      server.save(function (err) {
         var response = new SuccessResponse();
         response[safeGet(commonConstants.ALL_PARAMETERS.WATCHER_SERVER_ID)] = watcherServerId;
         response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = msToken;
         return callback(response);
      });
   });
}