'use strict';
app.service('hardwareSupervisorPackageService',
    [
        'dialogService',
        function(dialogService) {
            var self = this;
            var createDialog = {
                title: 'Create Hardware Supervisor Package',
                controller: 'hardwareSupervisorPackageSaveDialogController',
                template: 'plugins/hardwareSupervisor/templates/saveDialog/dialog.html',
                needConfirmToClose: true
            }
            this.showCreateDialog = function(packageData) {
                return dialogService.showDialog(createDialog, packageData); //1313this.createAndFillPackageTemplate());
            }
            var editDialog = {
                title: 'Edit Hardware Supervisor',
                controller: 'hardwareSupervisorPackageSaveDialogController',
                template: 'plugins/hardwareSupervisor/templates/saveDialog/dialog.html',
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
                    for (var i = 0; i < packageTemplate.devices.length; i++) {
                        var currentDevice = packageTemplate.devices[i];
                        if (currentDevice.instance_id == instance.instance_id) {
                            instance.name = currentDevice.name;
                        }
                    }

                    if (alertedIds.indexOf(instance.instance_id) != -1) {
                        alertedInstances.push(instance);
                        return;
                    }
                    if (unknownIds.indexOf(instance.instance_id) != -1) {
                        unknownInstances.push(instance);
                        return;
                    }
                    okInstances.push(instance);
                });

                packageTemplate.configuration.instances = alertedInstances.concat(unknownInstances).concat(okInstances);
                packageTemplate.configuration.instances.forEach(function(instance) {
                    self.addInstanceSummary(instance);
                });

                if (isNull(rawData.configuration.hardware_token)) {
                    rawData.configuration.hardware_token = [];
                }
                packageTemplate.configuration.hardware_token = rawData.configuration.hardware_token;

                packageTemplate.updatePluginRelatedUI = function() {
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
                        packageTemplate.addPackagePartStateAndStyle(packagePart, currentPackagePart.instance_id);
                        packageTemplate.ui.packageParts.data.push(packagePart);
                    }
                };
            }

            this.addInstanceSummary = function(instance) {
                instance.summary = "";

                if (instance.hardware_type == pluginConstants.HARDWARE_TYPE_CURRENT_LEVEL_MAX_30A) {
                    if (!isNull(instance.min_threshold)) {
                        instance.summary += "Min ";
                        instance.summary += instance.min_threshold + " A ";
                    }
                    if (!isNull(instance.max_threshold)) {
                        instance.summary += " Max ";
                        instance.summary += instance.max_threshold + " A ";
                    }
                    if (isNull(instance.min_threshold) && isNull(instance.max_threshold)) {
                        instance.summary += " Not configured yet ";
                    }
                }

                if (instance.hardware_type == pluginConstants.HARDWARE_TYPE_CURRENT_LEVEL_MAX_100A) {
                    if (!isNull(instance.min_threshold)) {
                        instance.summary += "Min ";
                        instance.summary += instance.min_threshold + " A ";
                    }
                    if (!isNull(instance.max_threshold)) {
                        instance.summary += " Max ";
                        instance.summary += instance.max_threshold + " A ";
                    }
                    if (isNull(instance.min_threshold) && isNull(instance.max_threshold)) {
                        instance.summary += " Not configured yet ";
                    }
                }

                if (instance.hardware_type == pluginConstants.HARDWARE_TYPE_VOLTAGE_LEVEL_MAX_230V_ONE_CHANNEL) {
                    if (!isNull(instance.min_threshold)) {
                        instance.summary += "Min ";
                        instance.summary += instance.min_threshold + " V ";
                    }
                    if (!isNull(instance.max_threshold)) {
                        instance.summary += " Max ";
                        instance.summary += instance.max_threshold + " V ";
                    }

                    if (isNull(instance.min_threshold) && isNull(instance.max_threshold)) {
                        instance.summary += " Not configured yet ";
                    }
                }

                if (instance.hardware_type == pluginConstants.HARDWARE_TYPE_VOLTAGE_LEVEL_MAX_230V_THREE_CHANNEL) {
                    if (isNull(instance.channel_1) ||
                        isNull(instance.channel_1.min_threshold) &&
                        isNull(instance.channel_1.max_threshold) &&
                        isNull(instance.channel_2.expected_state) &&
                        isNull(instance.channel_3.expected_state)) {
                        instance.summary += " Not configured yet ";
                        return;
                    }

                    if (!isNull(instance.channel_1.min_threshold) || !isNull(instance.channel_1.max_threshold)) {
                        instance.summary += "CH1";
                    }

                    if (!isNull(instance.channel_1.min_threshold)) {
                        instance.summary += " Min " + instance.channel_1.min_threshold + " V ";
                    }
                    if (!isNull(instance.channel_1.max_threshold)) {
                        instance.summary += " Max " + instance.channel_1.max_threshold + " V ";
                    }
                    if (!isNull(instance.channel_1.min_threshold) || !isNull(instance.channel_1.max_threshold)) {
                        instance.summary += "|";
                    }

                    if (!isNull(instance.channel_2.expected_state)) {
                        if (instance.channel_2.expected_state == 'any') {
                            // instance.summary += " CH2 no check |";
                        } else {
                            instance.summary += "CH2 " + instance.channel_2.expected_state + "|";
                        }
                    }

                    if (!isNull(instance.channel_3.expected_state)) {
                        if (instance.channel_3.expected_state == 'any') {
                            //  instance.summary += " CH3 no check";
                        } else {
                            instance.summary += "CH3 " + instance.channel_3.expected_state + "";
                        }
                    }
                }

                if (instance.hardware_type == pluginConstants.HARDWARE_TYPE_TEMPERATURE_DHT22 ||
                    instance.hardware_type == pluginConstants.HARDWARE_TYPE_TEMPERATURE_DHT11) {
                    if (!isNull(instance.temperature_celsius) && !isNull(instance.temperature_celsius.min_threshold)) {
                        instance.summary += "°C >" + instance.temperature_celsius.min_threshold + " | ";
                    }
                    if (!isNull(instance.temperature_celsius) && !isNull(instance.temperature_celsius.max_threshold)) {
                        instance.summary += "°C <" + instance.temperature_celsius.max_threshold + " | ";
                    }
                    if (!isNull(instance.relative_humidity_percent) &&
                        !isNull(instance.relative_humidity_percent.min_threshold)) {
                        instance.summary += "RH% >" + instance.relative_humidity_percent.min_threshold + " | ";
                    }
                    if (!isNull(instance.relative_humidity_percent) &&
                        !isNull(instance.relative_humidity_percent.max_threshold)) {
                        instance.summary += "RH% <" + instance.relative_humidity_percent.max_threshold + " | ";
                    }
                }

                if (instance.hardware_type == pluginConstants.HARDWARE_TYPE_GAS_MQ135) {
                    if (!isNull(instance.expected_state)) {
                        if (instance.expected_state == 'any') {
                            instance.summary += "No check";
                        } else {
                            instance.summary += "Expected " + instance.expected_state + ". ";

                        }
                    }
                }
                if (instance.hardware_type == pluginConstants.HARDWARE_TYPE_WATER_PRESENCE) {
                    if (!isNull(instance.expected_state)) {
                        if (instance.expected_state == 'any') {
                            instance.summary += "No check";
                        } else {
                            instance.summary += "Expected " + instance.expected_state + ". ";

                        }
                    }
                }


                if (instance.summary.length == 0) {
                    instance.summary += "Not configured yet";
                }

            }

            this.createAndFillPackageTemplate = function(rawData) {
                if (rawData == null) {
                    rawData = {};
                }
                //1313 var baseTemplate = new pluginPackageService.BasePackageTemplate(rawData);
                //1313  baseTemplate.packageType = safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.HARDWARE_SUPERVISOR);

                addConfigurationDataToPackageTemplate(rawData, rawData);
                addLastMeasuremntsToPackageTemplate(rawData, rawData);

                rawData.getPackagePartCount = function () { return rawData.configuration.instances.length; };
                rawData.isDeviceUsed = function (device) {
                    if (isEmptyOrNull(device.instance_id)) {
                        return false;
                    }
                    for (var instanceId = 0; instanceId < rawData.configuration.instances.length; instanceId++) {
                        var instance = rawData.configuration.instances[instanceId];
                        if (instance.instance_id == device.instance_id) {
                            return true;
                        }
                    }
                    return false;
                };

                return rawData;
            };

            this.translateForSave = function(paramPackage) {
                var translatedPackage = paramPackage;
                for (var i = 0; i < translatedPackage.configuration.instances.length; i++) {
                    delete translatedPackage.configuration.instances[i].summary;
                }
                /*1313  return packageService.save(translatedPackage).then(function() {
                          return $q.resolve();
                      },
                      function(error) {
                          return $q.reject(error);
                      });*/
            };
        }
    ]);