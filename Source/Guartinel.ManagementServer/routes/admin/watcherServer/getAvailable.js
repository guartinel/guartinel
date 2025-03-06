var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var statusInformer = include('guartinel/admin/statusInformer.js');


exports.URL = safeGet(managementServerUrls.ADMIN_WATCHER_SERVER_GET_AVAILABLE);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }
    
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doGetAvailable(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doGetAvailable(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse( err ));
        }
        config.getWatcherServers(function(err,servers){
            var serversWithPublicInfos = [];
            for (var i = 0; i < servers.length; i++) {
                serversWithPublicInfos.push({
                        id: servers[i].getId(),
                        is_enabled : servers[i].getIsEnabled(),
                        name : servers[i].getName(),
                        address: servers[i].getAddress(),
                        port : servers[i].getPort(),
                        categories :  servers[i].getCategories()
                    });
            }
            //LOG.info(JSON.stringify(serversWithPublicInfos));
            var response = new SuccessResponse();
            response[safeGet(commonConstants.ALL_PARAMETERS.SERVERS)] = serversWithPublicInfos;
            return callback(response);
        });
    });
}