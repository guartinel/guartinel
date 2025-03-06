/**
 * Created by DTAP on 2017.08.07..
 */
import { APIPackage } from "../../guartinel/packages/apiPackage";
let utils = global.utils;
import { LOG } from "../../diagnostics/LoggerFactory";
import { MSError, MSInternalServerError } from "../../error/Errors";
var sessionManager = global.include("guartinel/security/sessionManager.js");
import { Const } from "../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import commonConstants = Const.commonConstants;
import plugins = Const.plugins;
import pluginConstants = Const.pluginConstants;

let traceIfNull = global.utils.string.traceIfNull;

exports.getPackageDTO = function (packageDBO, account) {
   var packageObject = new APIPackage(true, account);
   packageObject.initFromObject(packageDBO);
   packageObject.configuration.maskSensitiveInfo();

    var pack = {};
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)] = packageDBO.packageName;
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.VERSION)] = packageDBO.version;
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_TYPE)] = packageDBO.packageType;
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.CONFIGURATION)] = alterConfiguration(packageObject.configuration, account);
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.CHECK_INTERVAL_SECONDS)] = packageDBO.checkIntervalSeconds;
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.ALERT_EMAILS)] = packageDBO.alertEmails;
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.ALERT_DEVICES)] = getDevicesNameArrayFromIdsArray(packageDBO.alertDeviceIds, account);
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.IS_ENABLED)] = packageDBO.isEnabled;
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.ACCESS)] = packageDBO.access;
    pack[traceIfNull(commonConstants.ALL_PARAMETERS.USE_PLAIN_ALERT_EMAIL)] = packageDBO.usePlainAlertEmail;
    return pack;
}

 function alterConfiguration(configuration, account) {  
    return configuration;
}

function getDevicesNameArrayFromIdsArray(ids, account) {
    var result = [];
    for (var index = 0; index < ids.length; index++) {
        var device = account.devices.id(ids[index]);
        result.push(device.name);
    }
    return result;
}