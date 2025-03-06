/**
 * Created by DTAP on 2016.05.28..
 */
var httpRequester = include('guartinel/connection/httpRequester.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var adminDatabaseConnector = include('guartinel/database/admin/databaseConnector.js');
var securityTool = include("guartinel/security/tool.js");

exports.removeWatcherServer = function (serverId, callback) {
    config.getWatcherServers(function (err, servers) {
        var foundServer;
        for (var i = 0; i < servers.length; i++) {
            if (servers[i].getId() === serverId) {
                foundServer = servers[i];
                break;
            }
        }
        if (!utils.object.isNull(foundServer)) {
            foundServer.remove(function (err) {
                return callback();
            });
        } else {
           return callback(new MSInternalServerError()
              .logMessage("WatcherServerController: Cannot find watcher server with :" + serverId)
              .severe()
              .logNow());
        }
    });
}


exports.getBestWatcherServerForThisAccount = function (account, onFinishCallback) {
    config.getWatcherServers(function (err, servers) {
        var bestServer;
        // iterate over all servers to find the best one
        for (var i = 0; i < servers.length; i++) {
            var server = servers[i];
            //if sever is disabled than we skip it
            if (!server.getIsEnabled()) {
                continue;
            }
            //get the categories of the current server and iterate over it
            for (var j = 0; j < server.getCategories().length; j++) {
                //select the current category from iteration as a variable
                var category = server.getCategories()[j];
                var activeLicenses = account.getActiveLicenses();

                for (var k = 0; k < activeLicenses.length; k++) {

                    if (category == activeLicenses[k].license.name) {
                        if (utils.object.isNull(bestServer)) {
                            bestServer = server;
                            continue;
                        }
                        if (server.getStressLevel() < bestServer.getStressLevel()) {
                            bestServer = server;
                            continue;
                        }
                    }
                }
            }
        }
        return onFinishCallback(bestServer);
    });
}

exports.getLeastBusyServer = function (onFinishCallback) {
    config.getWatcherServers(function (err, servers) {
        if (utils.object.isNull(servers)) {
            return null;
        }
        var bestServer = servers[0];
        for (var i = 1; i < servers.length; i++) {
            if (!servers[i].getIsEnabled()) {
                continue;
            }
            if (servers[i].getStressLevel() < bestServer.getStressLevel()) {
                bestServer = servers[i];
            }
        }
        return onFinishCallback(bestServer);
    });
}

exports.getNewWatcherServer = function (address, port, name, categories) {
    /* address: {
     ip: String,
     port: String
     },
     properties: {
     createdOn: {type: Date, default: Date.now},
     name: String,
     isEnabled: {type: Boolean, default: true},
     categories: [String]
     },*/
    var model = adminDatabaseConnector.getWatcherServerModel();
    model.address.ip = address;
    model.address.port = port;
    model.properties = {};
    model.properties.name = name;
    model.properties.categories = categories;
    model.wsToMSAuth = {};
    model.wsToMSAuth.oneTimeRegistrationToken = securityTool.generateRandomString(10);
    model.wsToMSAuth.watcherServerId = securityTool.generateRandomString(10);

    return model;
};
exports.getAllWatcherServers = function (callback) {
    config.getWatcherServers(function (err, servers) {
      return  callback(servers);
    });
}

exports.getWatcherServerById = function (serverId, callback) {
    config.getWatcherServers(function (err, servers) {
        callback(getServerById(servers, serverId));
    });
}

exports.getWatcherServerByToken = function (token, onFinishCallback) {
    config.getWatcherServers(function (err, servers) {
        var foundServer;
        for (var i = 0; i < servers.length; i++) {
            var isWSToken = servers[i].getWSToken() == token;
            var isMSToken = servers[i].getMSToken() == token;
            var isOneTimeRegistrationToken = servers[i].getOneTimeRegistrationToken() == token;

            if (isWSToken || isMSToken || isOneTimeRegistrationToken) {
                foundServer = servers[i];
                break;
            }
        }
        return onFinishCallback(foundServer);
    });
};

function getServerById(servers, id) {
    for (var i = 0; i < servers.length; i++) {
        if (servers[i].getId() === id) {
            return servers[i];
        }
    }
    return null;
}

exports.updateWatcherServer = function (server, onFinishCallback) {
    config.getWatcherServers(function (err, servers) {
        var foundServer = getServerById(server, server.getId());
        if (utils.object.isNull(foundServer)) {
           new MSError("CANNOT_FOUND_WATCHER_SERVER")
              .logMessage("Cannot find ws with id: " + server.getId())
              .severe()
              .logNow();
            return onFinishCallback();
        }
        foundServer = server;
        foundServer.isEnabled = server.getIsEnabled();
        foundServer.address = server.getAddress();
        foundServer.port = server.getPort();
        foundServer.token = server.getWSToken();
        foundServer.name = server.getName();
        foundServer.stressLevel = server.getStressLevel();
        foundServer.stressLevelTimeStamp = server.getStressLevelTimeStamp();
        foundServer.categories = server.getCategories();

        foundServer.save(function (err) {
            return onFinishCallback();
        });
    })
}
/*
 exports.isAnyServerAvailable = function (onFinishCallback) {
 config.getWatcherServers(function (err, servers) {
 for (var i = 0; i < servers.length; i++) {
 if (servers[i].getIsEnabled()) {
 return onFinishCallback(true);
 }
 }
 return onFinishCallback(false);
 });
 }

 exports.notifyWatcherServerStateChanged = function () {
 if (config.USE().getWatcherServers().length > 0) {
 watcherServerPerformanceMonitor.start();
 return;
 }
 LOG.info("There is 0 WatcherServer");
 watcherServerPerformanceMonitor.stop();
 }*/