'use strict';
app.service('pluginPackageService',
    [
        'accountService',
        'hostSupervisorPackageService',
        'websiteSupervisorPackageService',
        'hardwareSupervisorPackageService',
        'emailSupervisorPackageService',
        'applicationSupervisorPackageService', function(
            accountService,
            hostSupervisorPackageService,
            websiteSupervisorPackageService,
            hardwareSupervisorService,
            emailSupervisorPackageService,
            applicationSupervisorPackageService) {

            function addPackageBaseDetailsToPackageTemplate(packageTemplate, rawData) {
                packageTemplate.id = rawData.id;

                if (rawData.alertEmails == null) {
                    rawData.alertEmails = [];
                }
                packageTemplate.alertEmails = rawData.alertEmails;
                packageTemplate.packageType = rawData.packageType;

                if (rawData.checkIntervalSeconds == null) {
                    rawData.checkIntervalSeconds = accountService.currentUser.licenseAggregate.minimumCheckIntervalSec;
                }
                packageTemplate.checkIntervalSeconds = rawData.checkIntervalSeconds;

                for (var i = 0; i < checkIntervals.length; i++) {
                    if (checkIntervals[i] == packageTemplate.checkIntervalSeconds) {
                        packageTemplate.checkIntervalLabel = checkIntervalLabels[i];
                        break;
                    }
                }
                if (isNull(packageTemplate.checkIntervalLabel)) {
                    packageTemplate.checkIntervalLabel = packageTemplate.checkIntervalSeconds + " s";
                }

                if (rawData.alertDeviceIds == null) {
                    rawData.alertDeviceIds = [];
                }
                packageTemplate.alertDeviceIds = rawData.alertDeviceIds;

                packageTemplate.packageName = rawData.packageName;
                if (rawData.isEnabled == null) {
                    rawData.isEnabled = true;
                }
                if (rawData.access == null) {
                    rawData.access = [];
                }
                if (rawData.disableAlerts == null) {
                    rawData.disableAlerts = {};
                }

                if (rawData.disableAlerts.schedules == null) {
                    rawData.disableAlerts.schedules = [];
                }

                packageTemplate.disableAlerts = rawData.disableAlerts;
                packageTemplate.access = rawData.access;
                packageTemplate.isEnabled = rawData.isEnabled;
                packageTemplate.cardTemplate = rawData.cardTemplate;
                if (isNull(rawData.lastModificationTimestamp)) {
                    rawData.lastModificationTimestamp = new Date();
                }
                packageTemplate.lastModificationTimestamp = rawData.lastModificationTimestamp;

                if (rawData.state != null) {
                    packageTemplate.state = rawData.state;
                }
            }

            this.translateForSave = function(pack) {
                if (pack.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.APPLICATION_SUPERVISOR) {
                    applicationSupervisorPackageService.translateForSave(pack);
                }
                if (pack.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.HARDWARE_SUPERVISOR) {
                    hardwareSupervisorService.translateForSave(pack);
                }
                if (pack.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.EMAIL_SUPERVISOR) {
                    emailSupervisorPackageService.translateForSave(pack);
                }
                if (pack.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.WEBSITE_SUPERVISOR) {
                    websiteSupervisorPackageService.translateForSave(pack);
                }
                if (pack.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.HOST_SUPERVISOR) {
                    hostSupervisorPackageService.translateForSave(pack);
                }
            };
            this.BasePackageTemplate = function BasePackageTemplate(rawData) {
                if (rawData == null) {
                    rawData = {};
                }
                this.ui = {};

                for (var k in rawData) this[k] = rawData[k];

                this.canUserDeleteThisPackage = function() {
                    var result = false;
                    if (isNull(this.access)) {
                        return true;
                    }
                    this.access.forEach(function(item, index) {
                        if (item.packageUserEmail == accountService.currentUser.email && item.canDelete) {
                            result = true;
                        }
                    });
                    return result;
                };

                this.canUserDisableThisPackage = function() {
                    var result = false;
                    if (isNull(this.access)) {
                        return true;
                    }
                    this.access.forEach(function(item, index) {
                        if (item.packageUserEmail == accountService.currentUser.email && item.canDisable) {
                            result = true;
                        }
                    });
                    return result;
                };

                this.canUserViewThisPackage = function() {
                    var result = false;
                    if (isNull(this.access)) {
                        return true;
                    }
                    this.access.forEach(function(item, index) {
                        if (item.packageUserEmail == accountService.currentUser.email && item.canRead) {
                            result = true;
                        }
                    });
                    return result;
                };

                this.isThisUserTheOwner = function() {
                    var result = false;
                    if (isNull(this.owner)) {
                        return true;
                    }
                    if (this.owner == accountService.currentUser.email) {
                        return true;
                    }
                    return false;
                };

                this.canUserEditThisPackage = function() {
                    var result = false;
                    if (isNull(this.access)) {
                        return true;
                    }
                    this.access.forEach(function(item, index) {
                        if (item.packageUserEmail == accountService.currentUser.email && item.canEdit) {
                            result = true;
                        }
                    });
                    return result;
                };

                this.addAlertDevice = function(alertDeviceId) {
                    if (this.alertDeviceIds.indexOf(alertDeviceId) != -1) { // if already added then return
                        return;
                    }
                    this.alertDeviceIds.push(alertDeviceId);
                };

                this.addAlertEmail = function(email) {
                    if (email == "" || !email) {
                        return;
                    }
                    if (this.alertEmails.indexOf(email) != -1) { // if already added then return
                        return;
                    }
                    this.alertEmails.push(email);
                };

                this.hasEmptyAccessRule = function() {
                    var found = false;
                    this.access.forEach(function(item, index) {
                        if (isEmptyOrNull(item.packageUserEmail)) {
                            found = true;
                        }
                    });
                    return found;
                };

                this.hasMultipliedOwnerAccout = function() {
                    var foundCount = 0;
                    this.access.forEach(function(item, index) {
                        if (accountService.currentUser.email == item.packageUserEmail) {
                            foundCount++;
                        }
                    });
                    if (foundCount > 1) {
                        return true;
                    }
                    return false;
                };

                this.hasAnyAlertMethodSet = function() {
                    var isAnyAlertMail = false;
                    if (!isNull(this.alertEmails)) {
                        isAnyAlertMail = this.alertEmails.length > 0;
                    }
                    var isAnyAlertDevice = false;
                    if (!isNull(this.alertDeviceIds)) {
                        isAnyAlertDevice = this.alertDeviceIds.length > 0;
                    }

                    if (isAnyAlertDevice || isAnyAlertMail) {
                        return true;
                    }
                    return false;
                };

                this.getAlertedPackagePartIds = function() {
                    var result = [];
                    if (isNull(this.state) || isNull(this.state.states)) {
                        return result;
                    }
                    this.state.states.forEach(function(state) {
                        if (state.package_part_state == "alerting") { // TODO repair
                            result.push(state.package_part_identifier);
                        }
                    });
                    return result;
                };
                this.getUnknownPackagePartIds = function() {
                    var result = [];
                    if (isNull(this.state) || isNull(this.state.states)) {
                        return result;
                    }
                    this.state.states.forEach(function(state) {
                        if (state.package_part_state == "unknown") {
                            result.push(state.package_part_identifier);
                        }
                    });
                    return result;
                };

                this.getPackagePartContainerStyle = function(packagePartCount) {
                    var baseStyle = { "max-height": "155px", "overflow-x": "hidden", "background-color": "white" };
                    var isGuestPackage = false;
                    if (this.owner != accountService.currentUser.email) {
                        isGuestPackage = true;
                    }
                    var isAlerting = false;
                    if (this.ui.isStateAlerting) {
                        isAlerting = true;
                    }
                    if (packagePartCount == null) {
                        return baseStyle;
                    }
                    var resultHeight = 80;

                    if (packagePartCount == 1) {
                        resultHeight = 65;
                    }

                    if (packagePartCount == 2) {
                        resultHeight = 110;
                       if (isGuestPackage ) {
                            resultHeight = 100;
                        }
                        if (this.ui.isStateUnknown) {
                            resultHeight = 60;
                        }
                    }
                    if (packagePartCount == 3) {
                        resultHeight = 110;
                        if (isGuestPackage) {
                            resultHeight = 90;
                        }
                    }
                    if (packagePartCount >= 4) {
                        resultHeight = 130;
                        if (isGuestPackage) {
                            resultHeight = 120;
                        }
                    }

                    baseStyle["min-height"] = resultHeight + "px";
                    return baseStyle;
                };

                this.addPackagePartStateAndStyle = function(packagePart, identifier) {
                    var OK_STYLE = { "fill": "green", "margin-left": "16px", "margin-right": "8px" };
                    var UNKNOWN_STYLE = { "fill": "gray", "margin-left": "16px", "margin-right": "8px" };
                    var ALERTED_STYLE = { "fill": "red", "margin-left": "16px", "margin-right": "8px" };
                    packagePart.iconStyle = UNKNOWN_STYLE;
                    var height = 30;

                    //if overall status is unknown then package part is so
                    if (!isNull(this.state) && !isNull(this.state.states)) {
                        for (var index = 0; index < this.state.states.length; index++) {
                            var stateItem = this.state.states[index];
                            if (stateItem.package_part_identifier != identifier) {
                                continue;
                            }
                            packagePart.state = stateItem;
                            if (stateItem.package_part_state == "ok") {
                                packagePart.state.ok = true;
                                packagePart.iconStyle = OK_STYLE;
                                height = 50;
                            }
                            if (stateItem.package_part_state == "alerting") {
                                packagePart.state.alerting = true;
                                packagePart.iconStyle = ALERTED_STYLE;
                                height = 50;
                            }

                            if (stateItem.package_part_state == "unknown") {
                                height = 50;
                                packagePart.iconStyle = UNKNOWN_STYLE;
                            }
                            break;
                        }
                    }


                    if (!this.isEnabled) {
                        packagePart.iconStyle = UNKNOWN_STYLE;
                        height = 30;
                    }
                    // if package is newer then state time stamp = > gray
                    if (this.lastModificationTimestamp > this.state.timeStamp) {
                        packagePart.iconStyle = UNKNOWN_STYLE;
                        height = 30;
                    }
                    if (isEmptyOrNull(packagePart.state) ||isEmptyOrNull(packagePart.state.package_part_extract_built)) {
                        height = 30;
                    }

                    packagePart.rowStyle = { "min-height": height + "px", "height": height + "px;" };
                };
                this.updateUI = function() {
                    this.ui.isStateNotChecked = isStateNotChecked(this);
                    this.ui.notCheckedIconStyle = getNotCheckedIconStyle(this);
                    this.ui.okIconStyle = getOkIconStyle(this);
                    this.ui.alertIconStyle = getAlertIconStyle(this);
                    this.ui.isStateAlerting = isStateAlerting(this);
                    this.ui.isStateUnknown = isStateUnknown(this);
                    this.ui.canUserDisableThisPackage = this.canUserDisableThisPackage();
                    this.ui.canUserEditThisPackage = this.canUserEditThisPackage();
                    this.ui.isThisUserTheOwner = this.isThisUserTheOwner();
                    this.ui.canUserDeleteThisPackage = this.canUserDeleteThisPackage();
                    this.ui.packageNameStyle = getPackageNameStyle(this);
                    this.ui.packageName = getPackageName(this);
                };

                this.updateUI();

                addPackageBaseDetailsToPackageTemplate(this, rawData);

                if (this.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.APPLICATION_SUPERVISOR) {
                    applicationSupervisorPackageService.createAndFillPackageTemplate(this);
                }
                if (this.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.HARDWARE_SUPERVISOR) {
                    hardwareSupervisorService.createAndFillPackageTemplate(this);
                }
                if (this.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.EMAIL_SUPERVISOR) {
                    emailSupervisorPackageService.createAndFillPackageTemplate(this);
                }
                if (this.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.WEBSITE_SUPERVISOR) {
                    websiteSupervisorPackageService.createAndFillPackageTemplate(this);
                }
                if (this.packageType == plugins.ALL_PACKAGE_TYPE_VALUES.HOST_SUPERVISOR) {
                    hostSupervisorPackageService.createAndFillPackageTemplate(this);
                }
                //this function added inside the plugintemplate
                this.updatePluginRelatedUI();
                this.shouldFilter = function(key) {
                    if (this.packageName.indexOf(key) != -1) {
                        return false;
                    }
                    return true;
                };
            };
            function isStateUnknown(pack) {
                var isPropertiesNull = (isNull(pack.lastModificationTimestamp) ||
                    isNull(pack.state.timeStamp));

                if (!isPropertiesNull && (pack.lastModificationTimestamp > pack.state.timeStamp)) {
                    return true;
                }
                if (pack.state == null) {
                    return true;
                }
                return pack.state.name == 'unknown';
            }

            function isStateAlerting(pack) {
                var isPropertiesNull = (isNull(pack.lastModificationTimestamp) ||
                    isNull(pack.state.timeStamp));

                if (!isPropertiesNull && (pack.lastModificationTimestamp > pack.state.timeStamp)) {
                    return false;
                }
                if (pack.state == null) {
                    return false;
                }
                return pack.state.name == 'alerting';
            }

            function getOkIconStyle(pack) {
                if (pack.isEnabled) {
                    return { fill: "green" };
                }
                return { fill: "gray" };
            }

            function getAlertIconStyle(pack) {
                if (pack.isEnabled) {
                    return { fill: "red" };
                }
                return { fill: "gray" };
            }

            function isStateNotChecked(pack) {
                if (isNull(pack.lastModificationTimestamp) || isNull(pack.state.timeStamp)) {
                    return true;
                }
                if (pack.lastModificationTimestamp > pack.state.timeStamp) {
                    return true;
                }
                return pack.state.name == null || pack.state.name == 'unknown';
            }

            function getNotCheckedIconStyle(pack) {
                if (pack.isEnabled) {
                    return { fill: "gray" };
                }
                return { fill: "gray" };
            }

            function getPackageNameStyle(pack) {
                if (isNull(pack.packageName)) {
                    return 'md-headline white_text no_margin';
                }
                if (pack.packageName.length <= 9) {
                    return 'md-headline white_text no_margin';
                }
                if (pack.packageName.length > 9 && pack.packageName.length < 25) {
                    return 'md-headline-smaller white_text no_margin';
                }
                return 'md-headline-smaller white_text no_margin';
            }

            function getPackageName(pack) {
                if (isEmptyOrNull(pack.packageName)) {
                    return "";
                }
                if (pack.packageName.length > 47) {
                    return pack.packageName.substring(0, 47) + "...";
                }
                return pack.packageName;
            }

        }
    ]);