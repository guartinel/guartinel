'use strict';
app.service('packageService',
    [
        'apiService', '$q', 'pluginService', 'pluginPackageService', 'accountService',
        function(apiService, $q, pluginService, pluginPackageService, accountService) {
            var self = this;
            self.packages = [];

            this.getPackageCount = function() {
                var count = 0;
                for (var packageIndex = 0; packageIndex < self.packages.length; packageIndex++) {
                    var currentPack = self.packages[packageIndex];
                    if (currentPack.owner !== accountService.currentUser.email) {
                        continue;
                    }
                    if (currentPack.isEnabled) {
                        count++;
                    }
                }
                return count;
            };

            this.removeAccess = function(packageId) {
                var url = safeGet(backendUserApiUrls.PACKAGE_REMOVE_ACCESS);
                var data = {};
                data[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = packageId;
                data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;

                return apiService.sendRequest(url, data).then(function() {
                        return $q.resolve();
                    },
                    function(error) {
                        console.error("Removing access failed. Error: " + error);
                        return $q.reject(error);
                    });
            };

            this.getPackagePartCount = function(packageToSkip) {
                var count = 0;
                for (var packageIndex = 0; packageIndex < self.packages.length; packageIndex++) {
                    var currentPack = self.packages[packageIndex];
                    if (currentPack.owner !== accountService.currentUser.email) {
                        continue;
                    }
                    if (!currentPack.isEnabled) {
                        continue;
                    } // dont count disabled packages

                    if (!isNull(packageToSkip) && currentPack.id == packageToSkip) {
                        continue;
                    }
                    //TODO do something with this. plugin related properties handled here..
                    currentPack = self.packages[packageIndex];
                    if (!isNull(currentPack.configuration.detailed_websites)) {
                        count += currentPack.configuration.detailed_websites.length;
                    }
                    if (!isNull(currentPack.configuration.detailed_hosts)) {
                        count += currentPack.configuration.detailed_hosts.length;
                    }
                    if (!isNull(currentPack.configuration.instances)) {
                        count += currentPack.configuration.instances.length;
                    }
                }
                return count;
            };
         
            this.getAvailable = function() {
                var url = safeGet(backendUserApiUrls.PACKAGE_GET_AVAILABLE);
                var data = {};
                data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
                return apiService.sendRequest(url, data).then(function(response) {
                        var packages = response[safeGet(commonConstants.ALL_PARAMETERS.PACKAGES)];

                        var okPackages = [];
                        var alertingPackages = [];
                        var unknownPackages = [];
                        var disabledPackages = [];

                        for (var i = 0; i < packages.length; i++) {
                            var translatedPackage = {
                                id: packages[i][safeGet(commonConstants.ALL_PARAMETERS.ID)],
                                packageType: packages[i][safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_TYPE)],
                                packageName: packages[i][safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)],
                                isEnabled: packages[i][safeGet(commonConstants.ALL_PARAMETERS.IS_ENABLED)],
                                configuration: packages[i][safeGet(commonConstants.ALL_PARAMETERS.CONFIGURATION)],
                                cardTemplate: pluginService.getCardTemplate(
                                    packages[i][safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_TYPE)]),
                                lastMeasurement: packages[i][safeGet(commonConstants.ALL_PARAMETERS.LAST_MEASUREMENT)],
                                lastMeasurementTimeStamp: packages[i][safeGet(commonConstants.ALL_PARAMETERS
                                    .LAST_MEASUREMENT_TIMESTAMP)],
                                checkIntervalSeconds: packages[i][safeGet(commonConstants.ALL_PARAMETERS
                                        .CHECK_INTERVAL_SECONDS)
                                ],
                                timeoutIntervalSeconds: packages[i][safeGet(commonConstants.ALL_PARAMETERS
                                    .TIMEOUT_INTERVAL_SECONDS)],
                                alertEmails: packages[i][safeGet(commonConstants.ALL_PARAMETERS.ALERT_EMAILS)],
                                alertDeviceIds: packages[i][safeGet(commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS)],
                                access: packages[i][safeGet(commonConstants.ALL_PARAMETERS.ACCESS)],
                                devices: packages[i][safeGet(commonConstants.ALL_PARAMETERS.DEVICES)],
                                owner: packages[i][safeGet(commonConstants.ALL_PARAMETERS.OWNER)],
                                license: packages[i][safeGet(commonConstants.ALL_PARAMETERS.LICENSE)],
                                state: packages[i][safeGet(commonConstants.ALL_PARAMETERS.STATE)],
                                lastModificationTimestamp: packages[i][safeGet(commonConstants.ALL_PARAMETERS
                                    .LAST_MODIFICATION_TIMESTAMP)],
                                usePlainAlertEmail: packages[i][safeGet(commonConstants.ALL_PARAMETERS.USE_PLAIN_ALERT_EMAIL)],
                                forcedDeviceAlert: packages[i][safeGet(commonConstants.ALL_PARAMETERS.FORCED_DEVICE_ALERT)],
                                disableAlerts: packages[i][safeGet(commonConstants.ALL_PARAMETERS.DISABLE_ALERTS)],
                                checksum: packages[i][safeGet(commonConstants.ALL_PARAMETERS.CHECKSUM)]
                            };

                            var templatedPackage;
                            var isFound = false;
                            var isChanged = false;
                            for (var j = 0; j < self.packages.length; j++) {
                                var currentPack = self.packages[j];
                                if (currentPack.id != translatedPackage.id) {
                                    continue;
                                }
                                isFound = true;
                                if (currentPack.checksum !== translatedPackage.checksum) {
                                    isChanged = true;
                                    continue;
                                }
                                templatedPackage = currentPack;
                            }

                            if (!isFound || isChanged) {
                                templatedPackage = new pluginPackageService.BasePackageTemplate(translatedPackage);
                            }

                            if (isNull(templatedPackage.state)) {
                                templatedPackage.state = {};
                            }
                            if (!templatedPackage.isEnabled) {
                                disabledPackages.push(templatedPackage);
                                continue;
                            }
                            if (templatedPackage.lastModificationTimestamp > templatedPackage.state.timeStamp) {
                                templatedPackage.state.name =
                                    safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_STATE_UNKNOWN);
                                unknownPackages.push(templatedPackage);
                                continue;
                            }
                            if (templatedPackage.state.name ===
                                safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_STATE_OK)) {
                                okPackages.push(templatedPackage);
                                continue;
                            }
                            if (templatedPackage.state.name ===
                                safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_STATE_ALERTING)) {
                                alertingPackages.push(templatedPackage);
                                continue;
                            }
                            unknownPackages.push(templatedPackage);
                        }
                        var allPackages = alertingPackages.concat(unknownPackages).concat(okPackages)
                            .concat(disabledPackages);
                        for (var newPackagesIndex = 0; newPackagesIndex < allPackages.length; newPackagesIndex++) {
                            var newPackage = allPackages[newPackagesIndex];
                            var foundOldPackage = null;
                            var foundOldPackageIndex;
                            for (var oldPackagesIndex = 0;
                                oldPackagesIndex < self.packages.length;
                                oldPackagesIndex++) {
                                var oldPackage = self.packages[oldPackagesIndex];

                                //checking if package is already present
                                if (oldPackage.id === newPackage.id) {
                                    foundOldPackage = oldPackage;
                                    foundOldPackageIndex = oldPackagesIndex;
                                    break;
                                }
                            }

                            //this package didnt existed previously so we add it!
                            if (isNull(foundOldPackage)) {
                                self.packages.push(newPackage);
                                continue;
                            }

                            //checking the two package checksum and if it is differ then overwrite the old package
                            if (foundOldPackage.checksum !== newPackage.checksum) {
                                self.packages[foundOldPackageIndex] = newPackage;
                            }
                        }
                        var oldPackagesToDelete = [];
                        // at this point every new and updated package is updated or added but we need to delete the packages that are missing from the new  dataset
                        for (var oldPackIndex = 0; oldPackIndex < self.packages.length; oldPackIndex++) {
                            var oldPack = self.packages[oldPackIndex];
                            var isFound = false;
                            for (var newPackIndex = 0; newPackIndex < allPackages.length; newPackIndex++) {
                                var newPack = allPackages[newPackIndex];
                                if (oldPack.id === newPack.id) {
                                    isFound = true;
                                    break;
                                }
                            }
                            if (!isFound) {
                                oldPackagesToDelete.push(oldPack);
                            }
                        }
                        //removing the found packages
                        for (var removeIndex = 0; removeIndex < oldPackagesToDelete.length; removeIndex++) {
                            var indexToDelete = self.packages.indexOf(oldPackagesToDelete[removeIndex]);
                            self.packages.splice(indexToDelete, 1);
                        }
                        // self.packages = allPackages;
                        return $q.resolve(self.packages);
                    },
                    function(error) {
                        console.error("Getting existing packages failed! Error: " + error);
                        return $q.reject(error);
                    });
            };


            this.getStatistics = function(packageID) {
                var url = safeGet(backendUserApiUrls.PACKAGE_GET_STATISTICS);
                var data = {};
                data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
                data[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = packageID;

                return apiService.sendRequest(url, data).then(function(response) {
                        return $q.resolve(response[safeGet(commonConstants.ALL_PARAMETERS.STATISTICS)]);
                    },
                    function(error) {
                        console.error("Getting package statistics failed! Error: " + error);
                        return $q.reject(error);
                    });
            }

            this.save = function(_package) {
                pluginPackageService.translateForSave(_package);

                var url = safeGet(backendUserApiUrls.PACKAGE_SAVE);
                var data = {};
                data[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = _package.id;
                data[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_TYPE)] = _package.packageType;
                data[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)] = _package.packageName;
                data[safeGet(commonConstants.ALL_PARAMETERS.CONFIGURATION)] = _package.configuration;
                data[safeGet(commonConstants.ALL_PARAMETERS.CHECK_INTERVAL_SECONDS)] = _package.checkIntervalSeconds;
                data[safeGet(commonConstants.ALL_PARAMETERS.TIMEOUT_INTERVAL_SECONDS)] =
                    _package.timeoutIntervalSeconds;
                data[safeGet(commonConstants.ALL_PARAMETERS.ALERT_EMAILS)] = _package.alertEmails;
                data[safeGet(commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS)] = _package.alertDeviceIds;
                data[safeGet(commonConstants.ALL_PARAMETERS.IS_ENABLED)] = _package.isEnabled;
                data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
                data[safeGet(commonConstants.ALL_PARAMETERS.ACCESS)] = _package.access;
                data[safeGet(commonConstants.ALL_PARAMETERS.USE_PLAIN_ALERT_EMAIL)] = _package.usePlainAlertEmail;
                data[safeGet(commonConstants.ALL_PARAMETERS.FORCED_DEVICE_ALERT)] = _package.forcedDeviceAlert;
                data[safeGet(commonConstants.ALL_PARAMETERS.DISABLE_ALERTS)] = _package.disableAlerts;

                return apiService.sendRequest(url, data).then(function() {
                        return $q.resolve();
                    },
                    function(error) {
                        console.error("Saving package failed! Error: " + error);
                        return $q.reject(error);
                    });
            }

            this.delete = function(packageId) {
                var url = safeGet(backendUserApiUrls.PACKAGE_DELETE);
                var data = {};
                data[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = packageId;
                data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
                return apiService.sendRequest(url, data).then(function(response) {
                        console.log("Package DELETED successfully!");
                        return $q.resolve(response.packages);
                    },
                    function(error) {
                        console.error("Package delete failed! Error: " + error);
                        return $q.reject(error);
                    });
            }

            this.testEmail = function(email) {
                var url = safeGet(backendUserApiUrls.PACKAGE_TEST_EMAIL);
                var data = {};
                data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;
                data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
                return apiService.sendRequest(url, data).then(function(response) {
                        console.log("Package email tested successfully!");
                        if (response.error) {
                            return $q.reject(response.error);
                        }
                        return $q.resolve(response.packages);
                    },
                    function(error) {
                        console.error("Package testing email failed! Error: " + error);
                        return $q.reject(error);
                    });
            }

            self.initService = function() {
                self.getAvailable();
            }
        }
    ]);