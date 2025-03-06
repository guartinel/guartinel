/**
 * Created by sysment_dev on 04/09/2015.
 */
import * as admin from "firebase-admin";
import { LOG } from "../../diagnostics/LoggerFactory";
import { Const } from "../../common/constants";
import commonConstants = Const.commonConstants;
import { MSInternalServerError } from "../../error/Errors";

let sender;
var ExecutionMeterFactory = global.include("diagnostics/ExecutionMeterFactory.js");
var config = global.include("guartinel/admin/configuration/configurationController.js");
var async = require("async");
var database = global.include('guartinel/database/public/databaseConnector.js');


export function configure(gcmConfig) {
    admin.initializeApp({ credential: admin.credential.cert(gcmConfig) });
};

export function sendGcmToAdminDevices(message, onFinished) {
    LOG.info("Sending error to admin devices." + config.getBaseConfig().adminDeviceIds);
    var deviceObjectIds = [];
    for (var i = 0; i < config.getBaseConfig().adminDeviceIds.length; i++) {
        deviceObjectIds.push(database.toObjectId(config.getBaseConfig().adminDeviceIds[i]));
    }
    if (deviceObjectIds.length == 0) {
        LOG.info("There are no adminDevices");
        return onFinished();
    }

    database.getNativeAccountsConnection()
        .find({ "devices._id": { $in: deviceObjectIds } }, { "devices.$": 1 })
        .toArray(afterAdminDevicesRetrieved);

    function afterAdminDevicesRetrieved(err, result) {
        var gcmIdsToSend = [];
        for (var i = 0; i < result.length; i++) {
            for (var j = 0; j < result[i].devices.length; j++) {
                gcmIdsToSend.push(result[i].devices[j].gcmId);
            }
        }
        LOG.info("Admin devices gcmids  retrieved." + gcmIdsToSend);
        async.eachSeries(gcmIdsToSend,
            function (gcmId, doNextDeviceCallback) {
                LOG.info("Sending to admin : " + gcmId);
                sendGcm(gcmId,
                    true,
                    "Error inside Management server ",
                    message,
                    "xxxx",
                    "Management Server",
                    false,
                    true,
                    function(err) {
                        return doNextDeviceCallback();
                    });
            },
            function (err) {
                if (err) {
                    LOG.info("Failed gcm send in async foreach." + JSON.stringify(err));
                }
                return onFinished();
            });
    }
};

export function sendGcm(gcmID,
    forcedDeviceAlert,
    alertMessage,
    alertDetails,
    alertID,
    packageName,
    isRecovery,
    isPackageAlerted,
    callback) {
    if (!config.getBaseConfig().isGCMEnabled) {
        LOG.info("GCM is not sent because it is suppressed from config");
        return callback();
    }

    var meter = ExecutionMeterFactory.getExecutionMeter("GCM sending");
    var payload = {
        title: 'Guartinel',
        alert_message: alertMessage,
        alert_details: alertDetails,
        alert_id: alertID,
        package_name: packageName,
        is_package_alerted: JSON.stringify(isPackageAlerted),
        forced_device_alert: JSON.stringify(forcedDeviceAlert),
        is_recovery: JSON.stringify(isRecovery)
    }

    var options = {
        priority: "high"
    };
    var message = {
        data: payload
    };
    admin.messaging().sendToDevice(gcmID, message, options).then(function (result) {
        meter.stop();
        LOG.debug("GCM report: " + JSON.stringify(result));
        LOG.debug("GCM message: " + JSON.stringify(result.results));

        if (result.failureCount > 0) {
            return callback(
                new MSInternalServerError()
                    .logMessage("gcmManager: Invalid gcmId: " + gcmID)
                    .severe()
                    .logNow());
        }
        return callback();
    }).catch(function (err) {
        return callback(
            new MSInternalServerError()
                .logMessage("gcmManager: Cannot send gcm message")
                .innerError(err)
                .severe()
                .logNow());
    });
}