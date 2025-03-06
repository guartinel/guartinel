/**
 * Created by DTAP on 2017.08.18..
  */
import * as moment from "moment";
import * as security from "../../../security/tool";
import { LOG } from "../../../../diagnostics/LoggerFactory";
import PluginConfigurationFactory from "../../../packages/configurations/pluginConfiguratonFactory"

export function getSchema(mongoose) {
    var browser = {
        token: String,
        tokenTimeStamp: Date
    };
    var deviceAlert = {
        watcherServerID: String,
        alertID: String,
        packageID: String,
        observedID: String,
        createdOn: { type: Date, default: Date.now }
    };
    var device = {
        instanceId: String,
        isDisconnected:Boolean,
        hardwareType:String,
        createdOn: { type: Date, default: Date.now },
        gcmId: String,
        deviceType: String,
        name: String,
        categories: [String],
        token: String,
        tokenTimeStamp: String,
        configuration: String,
        configurationTimeStamp: Date,
        passwordHash: String,
        alerts: [deviceAlert],
        properties: mongoose.Schema.Types.Mixed,
        uid:String
    };

    var api = {
        token: String,
        tokenTimeStamp: Date
    };

    var measurement = {
        timeStamp: Date,
        data: mongoose.Schema.Types.Mixed
    };

    var stateItem = {
        package_part_identifier: String,
        package_part_state: String,
        package_part_message: mongoose.Schema.Types.Mixed,
        package_part_message_built: String,
        package_part_details: mongoose.Schema.Types.Mixed,
        package_part_details_built: String,
        package_part_extract_built:String
    };

    var state = {
        name: String,
        message: mongoose.Schema.Types.Mixed,
        message_built: String,
        message_details: mongoose.Schema.Types.Mixed,
        message_details_built: String,
        states: [stateItem],
        timeStamp: Date
    };

    var accessInfo = {
        packageUserEmail: String,
        canRead: Boolean,
        canEdit: Boolean,
        canDisable: Boolean,
        canDelete: Boolean
    };

    var pack = {
        lastModificationTimeStamp: String,
        watcherServerId: String,
        packageName: String,
        packageType: String,
        configuration: { type: mongoose.Schema.Types.Mixed, get: security.decryptObject, set: security.encryptObject },
        checkIntervalSeconds: Number,
        timeoutIntervalSeconds: { type: mongoose.Schema.Types.Mixed },
        alertDeviceIds: [String],
        alertEmails: { type: [String], get: security.decryptArray, set: security.encryptArray },
        usePlainAlertEmail: { type: Boolean, default: false },
        forcedDeviceAlert: {type:Boolean, default:false},
        measurements: [measurement],
        state: state,
        isEnabled: Boolean,
        isDeleted: { type: Boolean, default: false },
        access: [accessInfo],
        version: { type: Number, default: 0 },
        application_token: String,
        hardware_token: String,
        disableAlerts: { type: mongoose.Schema.Types.Mixed }
    };

    var activationInfo = {
        expiryDate: Date,
        activationCode: String,
        isActivated: Boolean,
        lastActivationEmailSentTimeStamp: Date
    };
    var passwordRenew = {
        lastSentTimeStamp: String,
        verificationCode: String
    };
    var payment = {
        createdOn: { type: Date, default: Date.now },
        amount: String,
        paymentInfo: mongoose.Schema.Types.Mixed
    };
    var license = {
        createdOn: { type: Date, default: Date.now },
        startDate: String,
        expiryDate: String,
        license: mongoose.Schema.Types.Mixed,
        payment: payment
    };

    var accountSchema = mongoose.Schema({
        createdOn: { type: Date, default: Date.now },
        language: { type: String, default: "ENGLISH" },
        passwordRenew: passwordRenew,
        firstName: { type: String },
        lastName: String,
        email: { type: String, unique: true, get: security.decryptText, set: security.encryptText },
        passwordHash: String,
        activationInfo: activationInfo,
        browserSessions: [browser],
        packages: [pack],
        accessiblePackageIds: [String],
        devices: [device],
        licenses: [license],
        api: api,
        isDebug: { type: Boolean, default: false },
    });

    accountSchema.methods.getAllPackagePartCount = function () {
        var packagePartCount = 0;
        return packagePartCount;// REMOVE LICENSE HANDLING
        /*LOG.debug("Counting package parts Of package count: " + this.packages.length);
        if (this.packages != null) {
            for (var index = 0; index < this.packages.length; index++) {
                var currentPackage = this.packages[index];
                if (currentPackage.isDeleted || !currentPackage.isEnabled) {// we don't count disabled  or deleted packages
                    LOG.debug("Package " + currentPackage.packageName + " is deleted or not enabled. Skipping");
                    continue;
                }
                var plugin = PluginConfigurationFactory.create(currentPackage.packageType);
                plugin.initFromObject(currentPackage.configuration);
                packagePartCount += plugin.getPackagePartCount();
            }
        }
        LOG.debug("Current account has " + packagePartCount + " package parts.");
        return packagePartCount;*/
    }

    accountSchema.methods.getPackageCount = function () {
        var packageCount = 0;

        if (this.packages != null) {
            for (var index = 0; index < this.packages.length; index++) {
                if (this.packages[index].isDeleted) {
                    continue;
                }
                if (!this.packages[index].isEnabled) { // we don't count disabled packages
                    continue;
                }
                packageCount++;
            }
        }
        return packageCount;
    };

    accountSchema.methods.getTheSmallestSetPackageCheckInterval = function () {
        var minimumCheckIntervalSec = 99999;

        if (this.packages != null) {
            for (var index = 0; index < this.packages.length; index++) {
                if (this.packages[index].checkIntervalSeconds < minimumCheckIntervalSec) {
                    minimumCheckIntervalSec = this.packages[index].checkIntervalSeconds;
                }
            }
        }
        LOG.info("Smallest set check inerval : " + minimumCheckIntervalSec);
        return minimumCheckIntervalSec;
    };

    accountSchema.methods.isTrialAvailable = function () {
        if (this.licenses != null) {
            for (var index = 0; index < this.licenses.length; index++) {
                if (this.licenses[index].license.name === "trial") {
                    return false;
                }
            }
        }
        return true;
    };

    // LICENSE RELATED
    function filterActiveLicenses(licenses) {
        var result = [];
        if (licenses == null) {
            LOG.debug("getActiveLicenses this has 0 licenses returning empty array");
            return result;
        }

        for (var index = 0; index < licenses.length; index++) {
            var expiryMoment = moment(licenses[index].expiryDate);
            var currentMoment = moment();
            var isValid = expiryMoment.isAfter(currentMoment, 'day');
            // var diffInHours = expiryMoment.diff(currentMoment, 'hours');
            //if (diffInHours >= 0) {
            if (isValid) {
                result.push(licenses[index]);
            }
        }
        return result;
    }
    accountSchema.methods.getTodayExpiringLicenses = function () {
        var result = [];
        if (this.licenses == null) {
            LOG.debug("getActiveLicenses this has 0 licenses returning empty array");
            return result;
        }

        for (var index = 0; index < this.licenses.length; index++) {
            var expiryMoment = moment(this.licenses[index].expiryDate);
            var currentMoment = moment();
            var isValid = expiryMoment.isAfter(currentMoment, 'day');
            var diffInDays = expiryMoment.diff(currentMoment, 'days');

            if (!isValid && diffInDays == 0) {
                result.push(this.licenses[index]);
            }
        }

        return result;
    }

    accountSchema.methods.getLicenseWithRemainingDays = function (remainingDays) {
        var result = [];

        LOG.debug("getLicenseWithRemainingDays |  licenses:" + this.licenses.length);
        for (var index = 0; index < this.licenses.length; index++) {
            var license = this.licenses[index];
            var currentMoment = moment();
            var licenseExpiryMoment = moment(license.expiryDate);
            var diff = licenseExpiryMoment.diff(currentMoment, 'days');
            LOG.debug("getLicenseWithRemainingDays | License " + license.license.caption + " Remaining days: " + diff);
            if (remainingDays.indexOf(diff) != -1) { // we can provide multiple remaining days constraints
                license.daysRemaining = diff;
                result.push(license);
            }
        }
        return result;
    }

    accountSchema.methods.canAddMorePackage = function () {
        return true; // REMOVE LICENSE HANDLING
        //var maxPackages = this.getLicenseAggregate().maximumPackages;

        //if (accountSchema.methods.getPackageCount() < maxPackages) {
        //    return true;
        //}
        //return false;
    };


    accountSchema.methods.getActiveLicenses = function () {
        return filterActiveLicenses(this.licenses);
    };

    accountSchema.methods.getBiggestAvailableLicense = function () {
        var activeLicenses = filterActiveLicenses(this.licenses);
        if (activeLicenses.length == 0) {
            return null;
        }
        var maxLicense = activeLicenses[0];
        for (var index = 0; index < activeLicenses.length; index++) {
            var compareLicense = activeLicenses[index];
            if (compareLicense.license.maximumPackages > maxLicense.license.maximumPackages) {
                maxLicense = activeLicenses[index];
            }
        }
        return maxLicense;
    };

    accountSchema.methods.getLicenseAggregate = function () {
        var activeLicenses = filterActiveLicenses(this.licenses);
        var aggregate = { // values doesn't really matters it will be overwritten at least by the free license
            maximumPackages: 0,
            minimumCheckIntervalSec: 99999,
            maximumPackagePartCount: 0,
            canUseAPI: false
        };

        for (var index = 0; index < activeLicenses.length; index++) {
            var license = activeLicenses[index];
            if (license.license.name === "free" && activeLicenses.length > 1) {//skip free license if there are any other kind of license
                continue;
            }

            aggregate.maximumPackages += license.license.maximumPackages;
            aggregate.maximumPackagePartCount += license.license.maximumPackagePartCount;
            if (license.license.minimumCheckIntervalSec < aggregate.minimumCheckIntervalSec) {
                aggregate.minimumCheckIntervalSec = license.license.minimumCheckIntervalSec;
            }
            if (license.license.canUseAPI === true) {
                aggregate.canUseAPI = true;
            }
        }
        return aggregate;
    };

    //!LICENSE RELATED

    accountSchema.methods.getDeviceByName = function (deviceName) {
        for (var index = 0; index < this.devices.length; index++) {
            if (this.devices[index].deviceName === deviceName)
                return this.devices[index];
        }
        return null;
    };

    accountSchema.methods.getUserName = function () {
        var userName = "";
        if (this.firstName == null && this.lastName == null) {
            userName = "Guartinel User";
        }
        if (this.firstName == null && this.lastName != null) {
            userName = this.lastName;
        }
        if (this.firstName != null && this.lastName == null) {
            userName = this.firstName;
        }
        if (this.firstName != null && this.lastName != null) {
            userName = this.firstName + " " + this.lastName;
        }
        return userName;
    }
    return accountSchema;
}
;
