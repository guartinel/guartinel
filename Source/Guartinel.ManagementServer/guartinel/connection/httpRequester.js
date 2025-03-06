var request = require('request');
var securityTool = include('guartinel/security/tool.js');
var statusInformer = include("guartinel/admin/statusInformer.js");
var moment = require('moment');
//var timeTool = include('guartinel/utils/timeTool.js');
var ExecutionMeterFactory = include("diagnostics/ExecutionMeterFactory.js");


function concanateAddress(address, port, url) {
    var resultAddress = "";
    resultAddress = address + ":" + port;

    if (resultAddress[resultAddress.length] != '/') {
        resultAddress = resultAddress + '/';
    }

    if (url.startsWith('/')) {
        LOG.error("URL is starting with / but it shouldnt" + url);
    }
    resultAddress = resultAddress + url;
    return resultAddress;
}

function sendRequestToWatcherServer(server, url, requestForm, callback) {
    if (utils.object.isNull(server)) {
        return callback(new MSInternalServerError()
            .logMessage("httpRequester:Server is null. Cannot send request to nothing")
            .severe()
            .logNow());
    }
    requestForm.token = server.getWSToken();
    var _url = concanateAddress(server.getAddress(), server.getPort(), url);
    process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
    var meter = ExecutionMeterFactory.getExecutionMeter("Request to WS. URL: " + _url + " Request Form: " + JSON.stringify(requestForm));
    LOG.info("Request to WS. URL: " + _url + " Request Form: " + JSON.stringify(requestForm));
    request({
        url: _url,
        json: true,
        method: 'POST',
        body: requestForm,
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        }
    }, onResponseReceived);

    function onResponseReceived(err, response, body) {
        meter.stop();
        if (err) { // catch connection layer errors
            LOG.error("Cannot send request to WS URL: " + _url + " ERR:" + JSON.stringify(err));
            // notifyGuartinelAdminIfNeeded(JSON.stringify(err), function () {
            return callback(
                new MSError(commonConstants.ALL_ERROR_VALUES.CANNOT_CONNECT_TO_REMOTE_HOST)
                    .logMessage("Error while sending request to: " + url)
                    .innerError(err)
                    .logNow()
            );
            //   });
        } else {
            LOG.info("Response from WS. URL: " + _url + " Response: " + JSON.stringify(body));

            if (isSuccess(body)) { // everything fine continue
                return callback(null, body);
            }

            if (isTokenIssue(body)) {  // check and handle if token issue exists
                LOG.info("Ms token for WS in invalid. Trying to relogin.");
                module.exports.loginToWatcherServerForToken(server, afterLoginToWS);

                function afterLoginToWS(err, token) {
                    if (err) {//login failed...
                        new MSError("Cannot login to WS.").innerError(err).logNow();
                        return callback(err);
                    }
                    //Login successful lets save the token and retry the previous request
                    server.setWSToken(token);
                    server.save(function (err) {
                        return sendRequestToWatcherServer(server, url, requestForm, callback);
                    });
                }
            } else { // handle everything else error
                return callback(body);
            }
        }
    }
}

function isSuccess(body) {
    return body[safeGet(commonConstants.ALL_PARAMETERS.SUCCESS)] == safeGet(commonConstants.ALL_SUCCESS_VALUES.SUCCESS);
}

function isTokenIssue(body) {
    var isInvalid = body[safeGet(commonConstants.ALL_PARAMETERS.ERROR)] == safeGet(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN);
    var isTokenExpired = body[safeGet(commonConstants.ALL_PARAMETERS.ERROR)] == safeGet(commonConstants.ALL_ERROR_VALUES.TOKEN_EXPIRED);
    return isInvalid || isTokenExpired;
}

exports.registerWatcherServer = function (server, parameters, callback) {
    statusInformer.getCpuId(function (cpuId) {
        var requestBody = {};
        requestBody[safeGet(commonConstants.ALL_PARAMETERS.NEW_USER_NAME)] = parameters.newUserName;
        requestBody[safeGet(commonConstants.ALL_PARAMETERS.NEW_PASSWORD)] = parameters.newPassword;
        requestBody[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)] = parameters.userName;
        requestBody[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = parameters.password;//securityTool.generatePasswordHash(parameters.userName,parameters.password);
        requestBody[safeGet(commonConstants.ALL_PARAMETERS.ONE_TIME_REGISTRATION_TOKEN)] = server.getOneTimeRegistrationToken();
        requestBody[safeGet(commonConstants.ALL_PARAMETERS.MANAGEMENT_SERVER_ADDRESS)] = parameters.managementServerAddress;
        requestBody[safeGet(commonConstants.ALL_PARAMETERS.UID)] = cpuId;
        requestBody[safeGet(commonConstants.ALL_PARAMETERS.CATEGORIES)] = server.getCategories();

        sendRequestToWatcherServer(server, safeGet(watcherServersUrls.ADMIN_REGISTER), requestBody, function (err, result) {
            if (err) {
                return callback(err);
            }
            if (!result.hasOwnProperty(safeGet(commonConstants.ALL_PARAMETERS.TOKEN))) {
                return callback(new MSInternalServerError()
                    .logMessage("httpRequester.registerWatcherServer: response is wrong  token")
                    .severe()
                    .logNow());
            }
            if (!result.hasOwnProperty(safeGet(commonConstants.ALL_PARAMETERS.MANAGEMENT_SERVER_ID))) {
                return callback(new MSInternalServerError()
                    .logMessage("httpRequester.registerWatcherServer: response is wrong  MANAGEMENT_SERVER_ID")
                    .severe()
                    .logNow());
            }

            return callback(err, result.token, result.management_server_id);
        });
    });
}

exports.loginToWatcherServerForToken = function (server, callback) {
    statusInformer.getCpuId(function (cpuId) {
        var requestBody = {};
        // requestBody[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)] = adminAccount.userName;
        var loginPassword = securityTool.generatePasswordHash(server.getManagementServerId(), cpuId);
        requestBody[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = loginPassword;

        sendRequestToWatcherServer(server, safeGet(watcherServersUrls.ADMIN_LOGIN), requestBody, function (err, result) {
            if (err) {
                return callback(err);
            }
            if (!result.hasOwnProperty(safeGet(commonConstants.ALL_PARAMETERS.TOKEN))) {
                return callback(new MSInternalServerError()
                    .logMessage("httpRequester.registerWatcherServer: response is wrong  TOKEN")
                    .logNow());
            }
            return callback(err, result.token);
        });
    });
}

exports.getPackagesWithTimeStamp = function (server, callback) {
    var requestBody = {};
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = server.getWSToken();

    sendRequestToWatcherServer(server, "packages/getAllWithTimeStamp", requestBody, function (err, result) {
        if (err) {
            return callback(err);
        }
        var timeStamps = result[safeGet(commonConstants.ALL_PARAMETERS.TIMESTAMPS)];

        if (utils.object.isNull(timeStamps)) {
            return callback(
                new MSInternalServerError()
                    .logMessage("Invalid incoming message. Missing fields.")
                    .innerError(err)
                    .logNow());
        }
        var translatedTimeStamps = [];
        for (var i = 0; i < timeStamps.length; i++) {
            var packageId = timeStamps[i][safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)];
            var timeStamp = timeStamps[i][safeGet(commonConstants.ALL_PARAMETERS.TIMESTAMP)];

            timeStamp = timeStamp.replaceAll('\"', '');
            if (utils.object.isNull(packageId) || utils.object.isNull(timeStamp)) {
                return callback(
                    new MSInternalServerError()
                        .logMessage("Invalid incoming message. Missing fields.")
                        .innerError(err));
            }

            translatedTimeStamps.push({ packageId: packageId, timeStamp: timeStamp });
        }
        return callback(null, translatedTimeStamps);
    });
};

exports.confirmDeviceAlert = function (server, alertDetails, callback) {
    var requestBody = {};
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = alertDetails.packageID;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.ALERT_DEVICE_ID)] = alertDetails.deviceID;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.ALERT_ID)] = alertDetails.alertID;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.INSTANCE_ID)] = alertDetails.instanceID;

    sendRequestToWatcherServer(server, safeGet(watcherServersUrls.ADMIN_CONFIRM_DEVICE_ALERT), requestBody, function (err, result) {
        if (err) {
            return callback(err);
        }

        return callback(null);
    });
};

exports.savePackage = function (server, _package, callback) {
    var requestBody = {};
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = _package._id.toString();
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_TYPE)] = _package.packageType;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)] = _package.packageName;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.CHECK_INTERVAL_SECONDS)] = _package.checkIntervalSeconds;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.LAST_MODIFICATION_TIMESTAMP)] = _package.lastModificationTimeStamp;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS)] = _package.alertDeviceIds;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.ALERT_EMAILS)] = _package.alertEmails;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.CONFIGURATION)] = _package.configuration;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.STATE)] = _package.state;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.DISABLE_ALERTS)] = _package.disableAlerts;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.TIMEOUT_INTERVAL_SECONDS)] = _package.timeoutIntervalSeconds;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.FORCED_DEVICE_ALERT)] = _package.forcedDeviceAlert;


    sendRequestToWatcherServer(server, safeGet(watcherServersUrls.PACKAGE_SAVE), requestBody, function (err, result) {
        if (err) {
            return callback(err);
        }
        return callback(null);
    });
};

exports.getPublicIP = function (callback) {
    var _url = "http://api.ipify.org";
    request({
        url: _url,
        method: 'GET'
    }, function (err, response, body) {
        LOG.debug("Started http request to: " + _url);
        if (err) {
            return callback(new MSInternalServerError()
                .logMessage("httpRequester.getPublicIP: cannot get IP. "
                    .innerError(err)
                    .severe()
                    .logNow()));
        }
        if (response.statusCode === 200) {
            return callback(null, body);
        }
        return callback(new MSInternalServerError()
            .logMessage("httpRequester.getPublicIP: cannot get IP. Response Body: " + body + " response: " + response)
            .innerError(err)
            .severe()
            .logNow());
    });
}

exports.deletePackage = function (server, packageId, callback) {
    var requestBody = {};
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = packageId;

    sendRequestToWatcherServer(server, safeGet(watcherServersUrls.PACKAGE_DELETE), requestBody, function (err) {
        return callback(err);
    });
}

exports.forwardApplicationMeasurement = function (server, applicationToken, instanceID, instanceName, packageId, measurement, isHeartBeat, callback) {
    var requestBody = {};
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = packageId;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.CHECK_RESULT)] = measurement;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.MEASUREMENT_TIMESTAMP)] = moment().toISOString();
    requestBody[safeGet(pluginConstants.INSTANCE_ID)] = instanceID;
    requestBody[safeGet(pluginConstants.APPLICATION_TOKEN)] = applicationToken;
    requestBody[safeGet(pluginConstants.IS_HEARTBEAT)] = isHeartBeat;
    requestBody[safeGet(pluginConstants.INSTANCE_NAME)] = instanceName;


    sendRequestToWatcherServer(server, safeGet(watcherServersUrls.APPLICATION_SUPERVISOR_CHECK_RESULTS), requestBody, function (err) {
        return callback(err);
    });
}
exports.forwardHardwareMeasurement = function (server, instanceID, instanceName, packageIds, measurement, startupTime, callback) {
    var requestBody = {};
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.STARTUP_TIME)] = startupTime;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_IDS)] = packageIds;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = packageIds[0];

    requestBody[safeGet(commonConstants.ALL_PARAMETERS.MEASUREMENT)] = measurement;
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.MEASUREMENT_TIMESTAMP)] = moment().toISOString();
    requestBody[safeGet(pluginConstants.INSTANCE_ID)] = instanceID;
    requestBody[safeGet(pluginConstants.INSTANCE_NAME)] = instanceName;

    sendRequestToWatcherServer(server, safeGet(watcherServersUrls.HARDWARE_SUPERVISOR_REGISTER_MEASUREMENTS), requestBody, function (err) {
        return callback(err);
    });
}

exports.getEvents = function (server, callback) {
    var requestBody = {};
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = server.getWSToken();

    sendRequestToWatcherServer(server, safeGet(watcherServersUrls.ADMIN_GET_EVENTS), requestBody, function (err, result) {
        if (err) {
            return callback(err);
        }
        if (!result.hasOwnProperty(safeGet(commonConstants.ALL_PARAMETERS.EVENTS))) {
            return callback(new MSInternalServerError()
                .logMessage("httpRequester.getEvents: response is wrong missing events")
                .severe()
                .logNow());
        }
        return callback(null, result.events);
    });
}

exports.getStatus = function (server, callback) {
    var requestBody = {};
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = server.getWSToken();

    sendRequestToWatcherServer(server, safeGet(watcherServersUrls.ADMIN_GET_STATUS), requestBody, function (err, result) {
        if (err) {
            return callback(err);
        }
        return callback(null, result[safeGet(commonConstants.ALL_PARAMETERS.STATUS)]);
    });
}

exports.getVersion = function (server, callback) {
    var requestBody = {};
    requestBody[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = server.getWSToken();

    sendRequestToWatcherServer(server, safeGet(watcherServersUrls.ADMIN_GET_VERSION), requestBody, function (err, result) {
        if (err) {
            return callback(err);
        }
        return callback(null, result[safeGet(commonConstants.ALL_PARAMETERS.VERSION)]);
    });
}
