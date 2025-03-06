'use strict';
app.service('applicationSupervisorPackageService',
    [
        'dialogService', '$q', function(dialogService, $q) {

            var createDialog = {
                title: 'Create Application Supervisor',
                controller: 'applicationSupervisorPackageSaveDialogController',
                template: 'plugins/applicationSupervisor/templates/saveDialog/dialog.html',
                needConfirmToClose: true
            }
            this.showCreateDialog = function(packageData) {
                return dialogService.showDialog(createDialog, packageData); //1313this.createAndFillPackageTemplate());
            }
            var editDialog = {
                title: 'Edit Application Supervisor',
                controller: 'applicationSupervisorPackageSaveDialogController',
                template: 'plugins/applicationSupervisor/templates/saveDialog/dialog.html',
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
                    packageTemplate.lastMeasurement.success = rawData.lastMeasurement.success;
                }
            }

            function addConfigurationDataToPackageTemplate(packageTemplate, rawData) {
                if (rawData.configuration == null) {
                    rawData.configuration = {};
                }
                if (isNull(rawData.configuration)) {
                    rawData.configuration = {};
                }
                packageTemplate.configuration = rawData.configuration;

                if (isNull(rawData.configuration.instances)) {
                    rawData.configuration.instances = [];
                }
                var alertedIds = packageTemplate.getAlertedPackagePartIds();
                var unknownIds = packageTemplate.getUnknownPackagePartIds();

                var alertedInstances = [];
                var unknownInstances = [];
                var okInstances = [];

                rawData.configuration.instances.forEach(function(instance) {
                    if (alertedIds.indexOf(instance.id) != -1) {
                        alertedInstances.push(instance);
                        return;
                    }
                    if (unknownIds.indexOf(instance.id) != -1) {
                        unknownInstances.push(instance);
                        return;
                    }
                    okInstances.push(instance);
                });

                packageTemplate.configuration.instances = alertedInstances.concat(unknownInstances).concat(okInstances);

                if (isNull(rawData.configuration.application_token)) {
                    rawData.configuration.application_token = [];
                }
                packageTemplate.configuration.application_token = rawData.configuration.application_token;

                packageTemplate.updatePluginRelatedUI = function () {
                    packageTemplate.ui.packageParts = {
                        data: [],
                        style: packageTemplate.getPackagePartContainerStyle(packageTemplate.configuration.instances
                            .length)
                    };

                    for (var packagePartIndex = 0;
                        packagePartIndex < packageTemplate.configuration.instances.length;
                        packagePartIndex++) {
                        var currentPackagePart = packageTemplate.configuration.instances[packagePartIndex];
                        var packagePart = {
                            data: packageTemplate.configuration.instances[packagePartIndex]
                        };
                        packageTemplate.addPackagePartStateAndStyle(packagePart, currentPackagePart.id);
                        packageTemplate.ui.packageParts.data.push(packagePart);
                    }
                };
            }


            this.createAndFillPackageTemplate = function(rawData) {
                if (rawData == null) {
                    rawData = {};
                }
                //1313 var baseTemplate = new pluginPackageService.BasePackageTemplate(rawData);
                //1313 baseTemplate.packageType = safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.APPLICATION_SUPERVISOR);

                addConfigurationDataToPackageTemplate(rawData, rawData);
                addLastMeasuremntsToPackageTemplate(rawData, rawData);

                rawData.getPackagePartCount = function() { return rawData.configuration.instences.length; }
                rawData.isDeviceUsed = function() { return false; };
                return rawData;
            };

            this.translateForSave = function(paramPackage) { // first translate the package keys
                var translatedPackage = paramPackage;
                /*1313  return packageService.save(translatedPackage).then(function () {
                      return $q.resolve();
                  }, function (error) {
                      return $q.reject(error);
                  });*/
            }
        }
    ]);