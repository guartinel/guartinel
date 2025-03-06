'use strict';
app.service('websiteSupervisorPackageService', ['dialogService', function (dialogService) {

    var createDialog = {
        title: 'Create Website Supervisor Package',
        controller: 'websiteSupervisorPackageSaveDialogController',
        template: 'plugins/websiteSupervisor/templates/saveDialog/dialog.html',
        needConfirmToClose: true
    }
    this.showCreateDialog = function (packageData) {
        return dialogService.showDialog(createDialog, packageData); //1313this.createAndFillPackageTemplate());
    }

    var editDialog = {
        title: 'Edit Website Supervisor Package',
        controller: 'websiteSupervisorPackageSaveDialogController',
        template: 'plugins/websiteSupervisor/templates/saveDialog/dialog.html',
        needConfirmToClose: true
    }
    this.showEditDialog = function (paramPackage) {
        editDialog.title = "Edit " + paramPackage.packageName;
        return dialogService.showDialog(editDialog, paramPackage);
    }

    function addLastMeasuremntsToPackageTemplate(packageTemplate, rawData) {
        if (rawData.lastMeasurementTimeStamp === null || rawData.lastMeasurement === null) {
            return;
        }
        packageTemplate.lastMeasurementTimeStamp = rawData.lastMeasurementTimeStamp;


        packageTemplate.lastMeasurement = {};
        if (!isNull(rawData.lastMeasurement)) {
            packageTemplate.lastMeasurement.message = rawData.lastMeasurement.message;
            packageTemplate.lastMeasurement.success = rawData.lastMeasurement.success;
            packageTemplate.lastMeasurement.loadTimeMilliseconds = rawData.lastMeasurement.load_time_milliseconds;
        }
    }

    function addConfigurationDataToPackageTemplate(packageTemplate, rawData) {
        if (rawData.configuration == null) {
            rawData.configuration = {};
        }
        if (isNull(rawData.configuration.detailed_websites)) {
            rawData.configuration.detailed_websites = [];
        }

        var alertedIds = packageTemplate.getAlertedPackagePartIds();
        var unknownIds = packageTemplate.getUnknownPackagePartIds();

        var alertedWebsites = [];
        var unknownWebsites = [];
        var okWebsites = [];

        rawData.configuration.detailed_websites.forEach(function (website) {
            if (alertedIds.indexOf(website.address) !== -1) {
                alertedWebsites.push(website);
                return;
            }
            if (unknownIds.indexOf(website.address) !== -1) {
                unknownWebsites.push(website);
                return;
            }
            okWebsites.push(website);
        });
       // packageTemplate.configuration = {}; remove reinit
        packageTemplate.configuration.detailed_websites = alertedWebsites.concat(unknownWebsites).concat(okWebsites);
        packageTemplate.configuration.retryCount = rawData.configuration.retry_count;
        packageTemplate.configuration.checkTextPatternIsEnabled = !isEmptyOrNull(rawData.configuration.check_text_pattern);
        packageTemplate.configuration.checkTextPattern = rawData.configuration.check_text_pattern;

        packageTemplate.configuration.checkCertificateMinimumDaysIsEnabled = !isEmptyOrNull(rawData.configuration.check_certificate_minimum_days);
        packageTemplate.configuration.checkCertificateMinimumDays = rawData.configuration.check_certificate_minimum_days;

        packageTemplate.configuration.checkLoadTimeSecondsIsEnabled = !isEmptyOrNull(rawData.configuration.check_load_time_seconds);
        packageTemplate.configuration.checkLoadTimeSeconds = rawData.configuration.check_load_time_seconds;
        packageTemplate.updatePluginRelatedUI = function() {

            packageTemplate.ui.packageParts = {
                data: [],
                style: packageTemplate.getPackagePartContainerStyle(packageTemplate.configuration.detailed_websites
                    .length)
            };

            for (var packagePartIndex = 0;
                packagePartIndex < packageTemplate.configuration.detailed_websites.length;
                packagePartIndex++) {
                var currentPackagePart = packageTemplate.configuration.detailed_websites[packagePartIndex];
                var packagePart = {
                    data: packageTemplate.configuration.detailed_websites[packagePartIndex]
                };
                packageTemplate.addPackagePartStateAndStyle(packagePart, currentPackagePart.address);
                packageTemplate.ui.packageParts.data.push(packagePart);
            }
        };
    }

        this.createAndFillPackageTemplate = function(rawData) {
            if (rawData === null) {
                rawData = {};
            }
            //1313var baseTemplate = new pluginPackageService.BasePackageTemplate(rawData);
            //1313 baseTemplate.packageType = safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.WEBSITE_SUPERVISOR);

            addConfigurationDataToPackageTemplate(rawData, rawData);
            addLastMeasuremntsToPackageTemplate(rawData, rawData);
            rawData.getPackagePartCount = function() {
                return rawData.configuration.detailed_websites.length;
            };
            rawData.ui.packagePartCount = rawData.configuration.detailed_websites.length;
            rawData.isDeviceUsed = function (id) { return false; }
            return rawData;
        };

    this.translateForSave = function (paramPackage) { // first translate the package keys
        var configuration = {
            detailed_websites: paramPackage.configuration.detailed_websites
        };

        if (paramPackage.configuration.checkTextPatternIsEnabled) {
            configuration.check_text_pattern = paramPackage.configuration.checkTextPattern;
        }
        if (paramPackage.configuration.checkCertificateMinimumDaysIsEnabled) {
            configuration.check_certificate_minimum_days = paramPackage.configuration.checkCertificateMinimumDays;
        }
        if (paramPackage.configuration.checkLoadTimeSecondsIsEnabled) {
            configuration.check_load_time_seconds = paramPackage.configuration.checkLoadTimeSeconds;
        }
        configuration.retry_count = paramPackage.configuration.retryCount;
        paramPackage.configuration = configuration;

       /*1313 return packageService.save(translatedPackage).then(function () {
            return $q.resolve();
        }, function (error) {
            return $q.reject(error);
        });*/
    }
}
]);