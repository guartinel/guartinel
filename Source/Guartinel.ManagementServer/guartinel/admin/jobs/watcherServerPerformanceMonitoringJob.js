var httpRequester = include('guartinel/connection/httpRequester.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var async = require('async');
var emailer = include('guartinel/connection/emailer.js');
var moment = require('moment');
var gcmManager = global.include('guartinel/connection/gcmManager.js');

var TAG = "watcherServerPerformanceMonitoringJob";
var isRouteDebugEnabled = false;

function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(TAG + " : " + message);
}

exports.run = function() {
    LOG.info(TAG + "Started!");
    var ExecutionMeterFactory = include("diagnostics/ExecutionMeterFactory.js");
    var meter = ExecutionMeterFactory.getExecutionMeter(TAG);
    try {
        updateServerInfos(function() {
            LOG.info(TAG + "Finished!");
            meter.stop();
        });
    } catch (err) {
        new MSInternalServerError().logMessage(TAG + " Job interrupted").innerError(err).severe().logNow();
        meter.stop();
    }
}

function updateServerInfos(monitorEndedCallback) {
    config.getWatcherServers(whenWatcherServersRetrieved);

    function whenWatcherServersRetrieved(err, servers) {
        if (utils.object.isNull(servers)) {
            return monitorEndedCallback();
        }
        debugRoute(" Updating watcher server infos.Servers: " + servers.length);
        async.eachSeries(servers,
            function(server, nextServer) {
                if (!server.getIsEnabled()) {
                    debugRoute("Server :" + server.getId() + " is disabled. Skipped.");
                    return nextServer();
                }
                httpRequester.getStatus(server, afterStatusReceived);

                function afterStatusReceived(err, status) {
                    if (!err) { //no error just save the status of the server
                        debugRoute("Server is available! ID" + server.getId());
                        server.setIsAvailable(true);
                        server.setStressLevel(status.stress_level);
                        return saveServer();
                    }

                    //server is not available
                    server.setIsAvailable(false);
                    server.setUnavailableTimeStamp(moment().toISOString());
                    debugRoute("Server is NOT available! ID" + server.getId());
                    LOG.critical(new Error(),"Could not reach watcher server"+ server.getId(),function afterHandling(){
                        saveServer();
                    });

                   /* var error = new MSInternalServerError()
                        .logMessage("Cannot update server info.")
                        .severe()
                        .innerError(err)
                        .logNow();                       
                        //it just became unavailable so send alert about it
                    if (server.getIsAvailable()) {                   
                        debugRoute("Server just became offline, send email about it.");
                        server.setIsAvailable(false);
                        server.setUnavailableTimeStamp(moment().toISOString());
                      
                        emailer.sendProblemEmailToAdmin("Cannot connect to Watcher server!",
                            "Cannot connect to WS from MS.ERR: " + JSON.stringify(error),
                            function(err) {
                                return gcmManager.sendGcmToAdminDevices(error.getLogMessage(), saveServer);
                            });

                    } else {
                        debugRoute("Server is already offline. Check last send timeStamp");
                        // the server already unavailable
                        //if server is not available check the last time error sent about it and only send new one if it was 10 minute ago
                        var currentMoment = moment();
                        var diffMinute = currentMoment.diff(moment(server.getUnavailableTimeStamp()), 'minute');
                        debugRoute("Difference in min: " + diffMinute);
                        if (diffMinute < 10) {
                            debugRoute("No email about unavailability");
                            return saveServer();
                        } else {
                            debugRoute("Cooldown is over send email again about the problem");
                            server.setIsAvailable(false);
                            server.setUnavailableTimeStamp(moment().toISOString());
                            emailer.sendProblemEmailToAdmin("Cannot connect to Watcher server!",
                                "Cannot connect to WS from MS.ERR: " + JSON.stringify(error),
                                function(err) {
                                    if (err) {
                                        new MSInternalServerError()
                                            .logMessage("Cannot send email")
                                            .severe()
                                            .innerError(err)
                                            .logNow();
                                    }
                                    debugRoute("Mail sending finished. Stack: " + new Error().stack);
                                    gcmManager.sendGcmToAdminDevices(error, saveServer);
                                });
                        }
                    }*/
                };

                function saveServer() {
                    debugRoute("saveServer starts");
                    server.save(function(err) {
                        if (err) {
                            var error = new MSInternalServerError()
                                .logMessage("Cannot save server info.")
                                .severe()
                                .innerError(err)
                                .logNow();
                        }
                        debugRoute("saveServer finished");
                        return nextServer();
                    });
                }
            },
            function(err) {
                if (err) {
                    var error = new MSInternalServerError()
                        .logMessage("Server Iteration interrupted")
                        .severe()
                        .innerError(err)
                        .logNow();
                }
                debugRoute("Update done.");
                return monitorEndedCallback();
            });
    }
}