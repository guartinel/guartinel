'use strict';
app.service('deviceService', [
    'apiService', '$q', 'accountService', function (apiService, $q, accountService) {

        var self = this;
        var devices = [];

        self.devices = function () {
            return devices;
        }
        self.getHardwareSensors = function () {
            var result = [];
            for (var i = 0; i < devices.length; i++)
                if (devices[i].device_type === commonConstants.ALL_DEVICE_TYPE_VALUES.HARDWARE_SENSOR) {
                    result.push(devices[i]);
                }
            return result;
        }
        self.getDeviceNameFromId = function (deviceId) {
            for (var i = 0; i < devices.length; i++)
                if (devices[i].id === deviceId)
                    return devices[i].name;
            return "unknown device";
        }
        self.getDeviceFromId = function (deviceId) {
            for (var i = 0; i < devices.length; i++)
                if (devices[i].id === deviceId)
                    return devices[i];
        }

        self.getDeviceTypeFromId = function (deviceId) {
            for (var i = 0; i < devices.length; i++)
                if (devices[i].id === deviceId)
                    return devices[i].device_type;
            return "unknown device";
        }
        self.getAllDeviceForCategory = function (category) {
            var found = [];
            if (isEmptyOrNull(devices)) {
                return found;
            }
            devices.forEach(function (device) {
                if (device.categories.indexOf(category) != -1) {
                    found.push(device);
                }
            });
            return found;
        }

        self.getAvailable = function () {
            var url = safeGet(backendUserApiUrls.DEVICE_GET_EXISTING);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
            return apiService.sendRequest(url, data).then(function (response) {
                devices = response.devices;
                return $q.resolve(response.devices);
            }, function (error) {
                console.error("Getting existing devices failed! Error: " + error);
                return $q.reject(error);
            });
        }

        self.disconnectHardwareSensor = function (deviceid) {
            var url = safeGet(backendUserApiUrls.DEVICE_DISCONNECT);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
            data[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_UUID)] = deviceid;

            return apiService.sendRequest(url, data).then(function (response) {
                devices = response.devices;
                return $q.resolve(response.devices);
            }, function (error) {
                console.error("Disconnecting device failed! Error: " + error);
                return $q.reject(error);
            });
        }

        self.delete = function (deviceId) {
            var url = safeGet(backendUserApiUrls.DEVICE_DELETE);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_UUID)] = deviceId;
            data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
            return apiService.sendRequest(url, data).then(function (response) {
                return $q.resolve(response.devices);
            }, function (error) {
                console.error("Device delete failed ! Error: " + error);
                return $q.reject(error);
            });
        }

        self.test = function (deviceId) {
            var url = safeGet(backendUserApiUrls.DEVICE_TEST);

            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_UUID)] = deviceId;
            data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
            return apiService.sendRequest(url, data).then(function (response) {
                return $q.resolve(response.devices);
            }, function (error) {
                console.error("Device testing failed ! Error: " + error);
                return $q.reject("Cannot test device. Device is unavailable.");
            });
        }
        self.getAllDeviceCategories = function () {
            var devices = self.devices();
            var allCategories = [];

            for (var i = 0; i < devices.length; i++) {
                var device = devices[i];
                if (!isNull(device.categories) && device.categories.length !== 0) {
                    for (var j = 0; j < device.categories.length; j++) {
                        var category = device.categories[j];
                        var categoryAlreadyStored = false;
                        for (var k = 0; k < allCategories.length; k++) {
                            var allCategoryItem = allCategories[k];
                            if (allCategoryItem === category) {
                                categoryAlreadyStored = true;
                            }
                        }
                        if (categoryAlreadyStored) { continue; }
                        allCategories.push(category);
                    }
                }
            }
            return allCategories;
        }

        self.editDevice = function (deviceId, newDeviceName, categories) {
            var url = safeGet(backendUserApiUrls.DEVICE_EDIT);

            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_UUID)] = deviceId;
            data[safeGet(commonConstants.ALL_PARAMETERS.DEVICE_NAME)] = newDeviceName;
            data[safeGet(commonConstants.ALL_PARAMETERS.CATEGORIES)] = categories;

            data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
            return apiService.sendRequest(url, data).then(function (response) {
                return $q.resolve(response.devices);
            }, function (error) {
                console.error("Device rename failed ! Error: " + error);
                return $q.reject("Cannot rename device");
            });
        }

        self.initService = function () {
            devices = self.getAvailable();
        }
    }
]);