'use strict';
app.service('hostSupervisorPackageService',
    [
        'dialogService', '$q', function(dialogService, $q) {

            var createDialog = {
                title: 'Create Host Supervisor Package',
                controller: 'hostSupervisorPackageSaveDialogController',
                template: 'plugins/hostSupervisor/templates/saveDialog/dialog.html',
                needConfirmToClose: true
            }
            this.showCreateDialog = function (packageData) {
                return dialogService.showDialog(createDialog, packageData); //1313this.createAndFillPackageTemplate());
            }
            var editDialog = {
                title: 'Edit Host Supervisor Package',
                controller: 'hostSupervisorPackageSaveDialogController',
                template: 'plugins/hostSupervisor/templates/saveDialog/dialog.html',
                needConfirmToClose: true
            }
            this.showEditDialog = function(paramPackage) {
                editDialog.title = "Edit " + paramPackage.packageName;
                return dialogService.showDialog(editDialog, paramPackage);
            }

            function addLastMeasuremntsToPackageTemplate(packageTemplate, rawData) {
                if (rawData.lastMeasurementTimeStamp == null || rawData.lastMeasurement == null) {
                    return;
                }
                packageTemplate.lastMeasurementTimeStamp = rawData.lastMeasurementTimeStamp;

                packageTemplate.lastMeasurement = {};
                if (!isNull(rawData.lastMeasurement)) {
                    packageTemplate.lastMeasurement.message = rawData.lastMeasurement.message;
                    packageTemplate.lastMeasurement.pingTime = rawData.lastMeasurement.ping_time;
                    packageTemplate.lastMeasurement.success = rawData.lastMeasurement.success;
                }
            }

            function addConfigurationDataToPackageTemplate(packageTemplate, rawData) {
                if (rawData.configuration == null) {
                    rawData.configuration = {};
                }
                if (rawData.configuration.detailed_hosts == null) {
                    rawData.configuration.detailed_hosts = [];
                }

                var alertedIds = packageTemplate.getAlertedPackagePartIds();
                var unknownIds = packageTemplate.getUnknownPackagePartIds();

                var alertedHosts = [];
                var unknownHosts = [];
                var okHosts = [];

                rawData.configuration.detailed_hosts.forEach(function(host) {
                    if (alertedIds.indexOf(host.address) != -1) {
                        alertedHosts.push(host);
                        return;
                    }
                    if (alertedIds.indexOf(host.address) != -1) {
                        unknownHosts.push(host);
                        return;
                    }
                    okHosts.push(host);

                });
                // packageTemplate.configuration = {}; remove reinit
                packageTemplate.configuration.detailed_hosts = alertedHosts.concat(unknownHosts).concat(okHosts);
                packageTemplate.configuration.retryCount = rawData.configuration.retry_count;

                packageTemplate.updatePluginRelatedUI = function () {

                    packageTemplate.ui.packageParts = {
                        data: [],
                        style: packageTemplate.getPackagePartContainerStyle(packageTemplate.configuration.detailed_hosts
                            .length)
                    };

                    for (var packagePartIndex = 0;
                        packagePartIndex < packageTemplate.configuration.detailed_hosts.length;
                        packagePartIndex++) {
                        var currentPackagePart = packageTemplate.configuration.detailed_hosts[packagePartIndex];
                        var packagePart = {
                            data: packageTemplate.configuration.detailed_hosts[packagePartIndex]
                        };
                        packageTemplate.addPackagePartStateAndStyle(packagePart, currentPackagePart.address);
                        packageTemplate.ui.packageParts.data.push(packagePart);
                    }
                };

            }

            this.createAndFillPackageTemplate = function(rawData) {
                if (rawData == null) {
                    rawData = {};
                }
                //1313var baseTemplate = new pluginPackageService.BasePackageTemplate(rawData);
                //1313 baseTemplate.packageType = safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.HOST_SUPERVISOR);

                addConfigurationDataToPackageTemplate(rawData, rawData);
                addLastMeasuremntsToPackageTemplate(rawData, rawData);

                rawData.getPackagePartCount = function() {
                    return rawData.configuration.detailed_hosts.length;
                };
                rawData.isDeviceUsed = function(id) { return false; }
                return rawData;
            }

            this.translateForSave = function(paramPackage) { // first translate the package keys
                 var configuration = {
                    detailed_hosts: paramPackage.configuration.detailed_hosts,
                     retry_count: paramPackage.configuration.retryCount
                };

                paramPackage.configuration = configuration;

                /*1313return packageService.save(translatedPackage).then(function () {
                    return $q.resolve();
                }, function (error) {
                    return $q.reject(error);
                });*/
            }
        }
    ]);