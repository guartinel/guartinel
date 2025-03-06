var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var statusInformer = include('guartinel/admin/statusInformer.js');
var watcherServerController = include('guartinel/admin/configuration/watcherServerController.js');

exports.URL = safeGet(managementServerUrls.ADMIN_WATCHER_SERVER_UPDATE);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
        watcherServerId: req.body[safeGet(commonConstants.ALL_PARAMETERS.WATCHER_SERVER_ID)],
        name: req.body[safeGet(commonConstants.ALL_PARAMETERS.NAME)],
        address: req.body[safeGet(commonConstants.ALL_PARAMETERS.ADDRESS)],
        port: req.body[safeGet(commonConstants.ALL_PARAMETERS.PORT)],
        categories : req.body[safeGet(commonConstants.ALL_PARAMETERS.CATEGORIES)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doUpdate(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doUpdate(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err));
        }

       watcherServerController.getWatcherServerById(parameters.watcherServerId,function(server){
          if (utils.object.isNull(server)) {
             var error = new MSInternalServerError()
                .logMessage(exports.URL + "Invalid watcher server id:" + parameters.watcherServerId)
                .severe()
                .logNow();
             return callback(new ErrorResponse(error));	
           }
           server.setAddress(parameters.address);
           server.setPort(parameters.port);
           server.setName(parameters.name);
           server.setCategories(parameters.categories);
         /*  watcherServerController.updateWatcherServer(server,function(){
               return callback(new SuccessResponse());
           });*/
           server.save(function(err){
               return callback(new SuccessResponse());
           });
       });
    });
}