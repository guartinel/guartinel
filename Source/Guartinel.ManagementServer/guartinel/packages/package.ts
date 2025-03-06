import { MSError, LicenseError } from "../../error/Errors";
import { Const } from "../../common/constants";
import * as moment from "moment";
import PluginConfigurationFactory from "./configurations/pluginConfiguratonFactory";
import { PluginPackageConfigurationBase } from "./configurations/pluginPackageConfiguration";
import { ApplicationSupervisorConfiguration } from "./configurations/applicationSupervisorConfiguration";
import { HardwareSupervisorConfiguration } from "./configurations/hardwareSupervisorConfiguration";
import { LOG } from "../../diagnostics/LoggerFactory";
let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export interface IConfigurable {
    initFromObject(object): MSError;
    createFromJSON(json): MSError;
    updateFromJSON(json): MSError;
    toJSON(): any;
}

export class Package implements IConfigurable {
    _id: any;
    packageType: string;
    isEnabled: boolean;
    isDeleted: boolean;
    packageName: string;
    alertEmails: Array<string> = new Array<string>();
    alertDeviceIds: Array<string> = new Array<string>();
    checkIntervalSeconds: number;
    timeoutIntervalSeconds: number;
    lastModificationTimeStamp: string;
    configuration: PluginPackageConfigurationBase;
    access: any;
    watcherServerId: any;
    version: number;
    application_token: string;
    hardware_token: string;
    usePlainAlertEmail: boolean;
    forcedDeviceAlert :boolean;
    disableAlerts: any;
    state: any;

    protected onlyIsEnabledChangeable: boolean;
    protected account; // todo extends license aggregate with current packagepart and other info and replace account with a license aggregate
    protected licenseAggregate;

    constructor(onlyIsEnabledChangeable: boolean, account) {
        this.lastModificationTimeStamp = moment().toISOString();
        this.onlyIsEnabledChangeable = onlyIsEnabledChangeable;
        this.account = account;
        this.licenseAggregate = this.account.getLicenseAggregate();
    }

    getCheckIntervalLabel = function (currentInterval) {
        var checkIntervalLabels = ["5 s", "15 s", "30 s", "45 s", "1 min", "1 min 30 s", "2 min", "5 min", "10 min", "15 min", "30 min", "1 h", "1h 30min", "2h", "3h", "4h"];
        var checkIntervals = [5, 15, 30, 45, 60, 90, 120, 300, 600, 900, 1800, 3600, 5400, 7200, 10800, 14400];
        return checkIntervalLabels[checkIntervals.indexOf(currentInterval)];
    }
    getAccount() {
        return this.account;
    }

    initFromObject(obj): MSError {
        this._id = obj._id;
        this.packageType = obj.packageType;
        this.isEnabled = obj.isEnabled;
        this.isDeleted = obj.isDeleted;
        this.packageName = obj.packageName;
        this.alertEmails = obj.alertEmails;
        this.alertDeviceIds = obj.alertDeviceIds;
        this.checkIntervalSeconds = obj.checkIntervalSeconds;
        this.timeoutIntervalSeconds = obj.timeoutIntervalSeconds;
        this.lastModificationTimeStamp = obj.lastModificationTimeStamp;
        this.access = obj.access;
        this.disableAlerts = obj.disableAlerts;
        this.watcherServerId = obj.watcherServerId;
        this.version = obj.version;
        this.configuration = PluginConfigurationFactory.create(this.packageType);
        this.usePlainAlertEmail = obj.usePlainAlertEmail;
        this.forcedDeviceAlert = obj.forcedDeviceAlert;
        this.application_token = obj.application_token;
        this.hardware_token = obj.hardware_token;
        this.state = obj.state;
        let resultError = this.configuration.initFromObject(obj.configuration);
        if (resultError != null) {
            return resultError;
        }
        return null;
    }

    createFromJSON(sourceJSON: any): MSError {
        if (utils.object.isNull(sourceJSON[Const.commonConstants.ALL_PARAMETERS.PACKAGE_TYPE])) {
            return new MSError("Missing 'package_type' from package.").logNow();
        }
        this.packageType = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.PACKAGE_TYPE)];

        if (utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.PACKAGE_NAME)])) {
            return new MSError("Missing 'package_name' from package.").logNow();
        }
        this.packageName = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.PACKAGE_NAME)];

        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_EMAILS)])) {
            this.alertEmails = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_EMAILS)];
        }
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS)])) {
            this.alertDeviceIds = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS)];
        }
        if (utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.IS_ENABLED)])) {
            return new MSError("Missing 'is_enabled' from package.").logNow();
        }
        this.isEnabled = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.IS_ENABLED)];

        if (utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CHECK_INTERVAL_SECONDS)])) {
            return new MSError("Missing 'check_interval_seconds' from package.").logNow();
        }

        this.checkIntervalSeconds = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CHECK_INTERVAL_SECONDS)];
        if (this.checkIntervalSeconds < this.licenseAggregate.minimumCheckIntervalSec) {
            return new LicenseError(`The selected check interval (${this.getCheckIntervalLabel(this.checkIntervalSeconds)}) is smaller than the minimum one allowed in the package owner's license (${this.getCheckIntervalLabel(this.licenseAggregate.minimumCheckIntervalSec)}). `);
        }

        if (utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CONFIGURATION)])) {
            return new MSError("Missing 'configuration' from package.").logNow();
        }

        this.configuration = PluginConfigurationFactory.create(this.packageType);
        let resultError = this.configuration.createFromJSON(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CONFIGURATION)]);
        if (resultError != null) {
            return resultError;
        }
        if (this.account.getLicenseAggregate().maximumPackagePartCount < (this.account.getAllPackagePartCount() + this.configuration.getPackagePartCount())) {
            return new LicenseError(`The package owner's license provides only: ${this.licenseAggregate.maximumPackagePartCount} package parts.`);
        }
           this.timeoutIntervalSeconds = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.TIMEOUT_INTERVAL_SECONDS)];
     
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.USE_PLAIN_ALERT_EMAIL)])) {
            this.usePlainAlertEmail = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.USE_PLAIN_ALERT_EMAIL)];
        }
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.FORCED_DEVICE_ALERT)])) {
            this.forcedDeviceAlert = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.FORCED_DEVICE_ALERT)];
        }
        this.access = this.ensureOwnerAccess(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ACCESS)], this.account);
        this.disableAlerts = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.DISABLE_ALERTS)];
        this.watcherServerId = null;
        if (this.configuration instanceof ApplicationSupervisorConfiguration) {
            this.application_token = (this.configuration as ApplicationSupervisorConfiguration).application_token;
        }
        if (this.configuration instanceof HardwareSupervisorConfiguration) {
            this.hardware_token = (this.configuration as HardwareSupervisorConfiguration).hardware_token;
        }
        return null; // everything OK we are HAPPY
    }

    updateFromJSON(sourceJSON): MSError {
        let packageIsEnabledBeforeUpdate = this.isEnabled;
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.PACKAGE_NAME)])) {
            this.packageName = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.PACKAGE_NAME)];
        }

        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_EMAILS)])) {
            this.alertEmails = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_EMAILS)];
        }
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS)])) {
            this.alertDeviceIds = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS)];
        }
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.IS_ENABLED)])) {
            this.isEnabled = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.IS_ENABLED)];
        }


        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CHECK_INTERVAL_SECONDS)])) {
            this.checkIntervalSeconds = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CHECK_INTERVAL_SECONDS)];
            if (this.checkIntervalSeconds < this.licenseAggregate.minimumCheckIntervalSec) {
                return new LicenseError(`The selected check interval (${this.getCheckIntervalLabel(this.checkIntervalSeconds)}) is smaller than the minimum one allowed in the package owner's license (${this.getCheckIntervalLabel(this.licenseAggregate.minimumCheckIntervalSec)}). `);
            }
        }
         this.timeoutIntervalSeconds = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.TIMEOUT_INTERVAL_SECONDS)];
        
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CONFIGURATION)])) {

            let resultError = this.configuration.updateFromJSON(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CONFIGURATION)]);
            if (resultError != null) {
                return resultError;
            }
            let newPackagePartsCount = 0; // we only need to add this current configuration count if the package was disabled before, beacuse getAllPackagePartCount doesnt count disabled packages
            if (!packageIsEnabledBeforeUpdate) {
                newPackagePartsCount = this.configuration.getPackagePartCount();
            }
            if (this.account.getLicenseAggregate().maximumPackagePartCount < (this.account.getAllPackagePartCount() + newPackagePartsCount)) {
                return new LicenseError(`The package owner's license provides only: ${this.licenseAggregate.maximumPackagePartCount} package parts.`);
            }
        }
        if (this.configuration instanceof ApplicationSupervisorConfiguration) {
            this.application_token = (this.configuration as ApplicationSupervisorConfiguration).application_token;
        }

        if (this.configuration instanceof HardwareSupervisorConfiguration) {
            this.hardware_token = (this.configuration as HardwareSupervisorConfiguration).hardware_token;
        }

        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.USE_PLAIN_ALERT_EMAIL)])) {
            this.usePlainAlertEmail = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.USE_PLAIN_ALERT_EMAIL)];
        }
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.FORCED_DEVICE_ALERT)])) {
            this.forcedDeviceAlert = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.FORCED_DEVICE_ALERT)];
        }
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.DISABLE_ALERTS)])) {
            this.disableAlerts = sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.DISABLE_ALERTS)];
        }

        this.lastModificationTimeStamp = moment().toISOString();
        if (!utils.object.isNull(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ACCESS)])) {
            this.access = this.ensureOwnerAccess(sourceJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ACCESS)], this.account);
        }
        //state nulling removal  this.state = null;
        this.watcherServerId = null;
        return null; // everything OK we are HAPPY
    }

    toJSON(): any {
        this.configuration.maskSensitiveInfo();
        var packJSON = {};
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.PACKAGE_TYPE)] = this.packageType;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.PACKAGE_NAME)] = this.packageName;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CONFIGURATION)] = this.configuration;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ID)] = this._id.toString();
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.IS_ENABLED)] = this.isEnabled;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.CHECK_INTERVAL_SECONDS)] = this.checkIntervalSeconds;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.TIMEOUT_INTERVAL_SECONDS)] = this.timeoutIntervalSeconds;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_EMAILS)] = this.alertEmails;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS)] = this.alertDeviceIds;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ACCESS)] = this.access;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.LAST_MODIFICATION_TIMESTAMP)] = this.lastModificationTimeStamp;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.USE_PLAIN_ALERT_EMAIL)] = this.usePlainAlertEmail;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.FORCED_DEVICE_ALERT)] = this.forcedDeviceAlert;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.OWNER)] = this.account.email;
        packJSON[traceIfNull(Const.commonConstants.ALL_PARAMETERS.DISABLE_ALERTS)] = this.disableAlerts;
        return packJSON;
    }

 /*   getTimeoutInterval(checkIntervalSeconds) { timeout is set by the user otherwise WS will handle it
        var checkInterval = checkIntervalSeconds;
        var timeoutInterval = checkInterval * 1.2;
        var ONE_MINUTE = 60;
        if (timeoutInterval < ONE_MINUTE) {
            timeoutInterval = ONE_MINUTE;
        }
        return timeoutInterval;
    }*/
    ensureOwnerAccess(accessList, account) {
        if (utils.object.isNull(accessList)) {
            accessList = [];
        }
        var isOwnerRuleFound = false;
        accessList.forEach(function (item, index) {
            if (item.packageUserEmail === account.email) {
                isOwnerRuleFound = true;
            }
        });
        if (!isOwnerRuleFound) {
            accessList.push({
                packageUserEmail: account.email,
                canEdit: true,
                canDelete: true,
                canDisable: true,
                canRead: true
            });
        }
        return accessList;
    }
}