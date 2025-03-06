import { Const } from "../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import { LOG } from "../../diagnostics/LoggerFactory";
import pluginConstants = Const.pluginConstants;
import commonConstants = Const.commonConstants;
import { ErrorResponse, SuccessResponse } from "../../guartinel/response/Response";
import { MSError } from "../../error/Errors";
var httpRequester = global.include('guartinel/connection/httpRequester.js');
var watcherServerController = global.include('guartinel/admin/configuration/watcherServerController.js');
var database = global.include("guartinel/database/public/databaseConnector.js");
import * as async from "async";
import { HardwareSupervisorConfiguration } from
    "../../guartinel/packages/configurations/hardwareSupervisorConfiguration";
import { Package } from "../../guartinel/packages/package";
import * as securityTool from "../../guartinel/security/tool";
import { parallel } from "async";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(managementServerUrls.HARDWARE_SUPERVISOR_REGISTER_MEASURED_DATA);

var isRouteDebugEnabled = true;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(URL + " " + message);
}

export function route(req, res) {
    let parameters = {
        measurement: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.MEASUREMENT)],
        instanceId: req.body[traceIfNull(pluginConstants.INSTANCE_ID)],
        startupTimeOPT: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.STARTUP_TIME)],//TODO remove OPT after every wemos migrated
        restriction: {}
    };
    parameters.restriction = {
        coolDownSec: 3,
        UUID: URL + parameters.instanceId
    };

    global.myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doRegisterMeasuredData(parameters, function (result) {
                return res.send(result);
            });
        }
    });
};

function doRegisterMeasuredData(parameters, callback) {
    database.getAccountBySubDocumentProperty('devices', 'instanceId', parameters.instanceId, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        if (utils.object.isNull(account)) {
            var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_ID)
                .logMessage(URL + " Invalid instance id" + parameters.instanceId)
                .logNow();
            return callback(new ErrorResponse(error));
        }
        let device;
        for (var deviceIndex = 0; deviceIndex < account.devices.length; deviceIndex++) {
            if (account.devices[deviceIndex].instanceId != null && account.devices[deviceIndex].instanceId == parameters.instanceId) {
                device = account.devices[deviceIndex];
                debugRoute("Device found");
            }
        }
        if (device.isDisconnected) {
            var error = new MSError(commonConstants.ALL_ERROR_VALUES.DISCONNECTED)
                .logMessage(URL + "Instance is disconnected")
                .logNow();
            return callback(new ErrorResponse(error));
        }
       
        debugRoute("Get the package which has the  instance id.");
        let relatedDBPackageIds = [];
        let relatedWSId;
        for (let i = 0; i < account.packages.length; i++) {
            let pack = account.packages[i];
            if (pack.isDeleted || !pack.isEnabled) {   // TODO should we skip the disabled packages?              
                continue;
            }
            debugRoute("Transform package to custom object from DB object");
            let packageObject = new Package(false, account);
            packageObject.initFromObject(pack);
            if (!(packageObject.configuration instanceof HardwareSupervisorConfiguration)) {
                continue;
            }
            debugRoute("Check if it is a new instance for the current package");
            let instance = (packageObject.configuration as HardwareSupervisorConfiguration).getInstance(parameters.instanceId);
            if (utils.object.isNull(instance)) {
                continue; // TODO currently this code only handle one to one mapping between instances and packages
            }
            debugRoute("Found related package and instance");
            relatedDBPackageIds.push(pack._id.toString());
            //#WARNING
            relatedWSId = pack.watcherServerId;    //TODO  fix to sort requests per watcherserver        
        }

        if (relatedDBPackageIds.length == 0) {
            debugRoute("This sensor is not registered to any packages yet.")
            var response = new SuccessResponse();
            response["instance_name"] = device.name;
            return callback(response);
        }

        watcherServerController.getAllWatcherServers(afterServersLoaded);
        function afterServersLoaded(servers) {
            if (utils.object.isNull(servers)) {
                LOG.debug("There is currently no watcher server registered");
                return callback(new SuccessResponse());
            }
            debugRoute("All watcher servers are loaded.");

            async.eachSeries(servers, sendMeasurementToServer, afterProcessingFinished);

            function sendMeasurementToServer(server, nextServer) {
                debugRoute("Send hardware measurement to WS :" + server.getId());
                if (server.getId() != relatedWSId && !utils.object.isNull(relatedWSId)) { 
                    return nextServer();
                }
                parameters.measurement = utils.object.ensureAsObject(parameters.measurement);
                httpRequester.forwardHardwareMeasurement(server, device.instanceId, device.name, relatedDBPackageIds, parameters.measurement, parameters.startupTimeOPT, function (httpErr) {
                    if (httpErr) {
                        LOG.error("Cannot forward measurements to server:" + server.getId());
                        debugRoute("Cannot forward measurements to server:" + server.getId());
                    } else {
                        debugRoute("Forward successful");
                    }
                    return nextServer();
                });
            }

            function afterProcessingFinished(err) {
                if (err) {
                    LOG.error("Error while processing servers and related package : " + JSON.stringify(err));
                } else {
                    debugRoute("Route finished without error");
                }
                var response = new SuccessResponse();
                response["instance_name"] = device.name;
                return callback(response);
            }
        }
    }
    );
}

