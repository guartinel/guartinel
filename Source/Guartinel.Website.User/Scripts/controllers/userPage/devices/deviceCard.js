'use strict';
app.controller("deviceCardController", [
    '$scope', 'dialogService', 'packageService',
    function ($scope, dialogService, packageService) {
        $scope.showTestDialog = function (device) {
            dialogService.showDialog(DIALOGS.devices.test, device);
        }

        $scope.showDeleteDialog = function(device) {
            var isDeviceUsed = false;
            var usedPackages = [];

            packageService.packages.forEach(function(pack, index) {
                if (isNull(pack.devices)) {
                    return;
                }
                if (pack.isDeviceUsed(device)) {
                    isDeviceUsed = true;
                    usedPackages.push(pack.packageName);
                    return;
                }
                pack.alertDeviceIds.forEach(function(alertDeviceID) {
                    if (alertDeviceID === device.id ) {
                        isDeviceUsed = true;
                        usedPackages.push(pack.packageName);
                    }
                });
            });
            if (isDeviceUsed) {
                dialogService.showDialog(DIALOGS.errorDialog, "Device is used in package : " + usedPackages.join(','));
                return;
            }
            dialogService.showDialog(DIALOGS.devices.deleteDevice, device).then(function() {
                    $scope.getAvailable();
                },
                function(error) {
                });
        };

        $scope.showEditDeviceDialog = function (device) {
            $scope.isDialogOpen = true;
            dialogService.showDialog(DIALOGS.devices.editDevice, device).then(function () {
                $scope.isDialogOpen = false;
                $scope.getAvailable(true);
            }, function (error) {
                console.log("Cannot rename device");
            });
        }
        $scope.showDisconnectHardwareSensorDialog = function (device) {
            $scope.isDialogOpen = true;
            dialogService.showDialog(DIALOGS.devices.disconnectHardwareSensor, device).then(function () {
                $scope.isDialogOpen = false;
                $scope.getAvailable(true);
            }, function (error) {
                console.log("Cannot rename device");
            });
        }
        //ui related methods 
        $scope.getCardTitleStyle = function (device) {
            var androidDeviceType = safeGet(commonConstants.ALL_DEVICE_TYPE_VALUES.ANDROID_DEVICE);
            var hardwareSensorType = safeGet(commonConstants.ALL_DEVICE_TYPE_VALUES.HARDWARE_SENSOR);

            switch (device.device_type) {
                case androidDeviceType:
                    return { backgroundColor: "yellowgreen", color: "white", fill: "yellowgreen" }
                case hardwareSensorType:
                    return { backgroundColor: "lightblue", color: "white", fill: "lightblue" }

                default:
                    console.error('Unknown device type!');
                    return { backgroundColor: "black" }
            }
        }

        $scope.getCardIconFillStyle = function (device) {
            var androidDeviceType = safeGet(commonConstants.ALL_DEVICE_TYPE_VALUES.ANDROID_DEVICE);
            var hardwareSensorType = safeGet(commonConstants.ALL_DEVICE_TYPE_VALUES.HARDWARE_SENSOR);
            switch (device.device_type) {
                case androidDeviceType:
                    return { fill: "yellowgreen" }
                case hardwareSensorType:
                    return { fill:"yellowgreen"}
                default:
                    console.error('Unknown device type!');
                    return { backgroundColor: "black" }
            }
        }

        $scope.isAndroidDevice = function (device) {
            if (device.device_type === safeGet(commonConstants.ALL_DEVICE_TYPE_VALUES.ANDROID_DEVICE)) {
                return true;
            };
            return false;
        }

        $scope.isHardwareSensor = function (device) {
            if (device.device_type === safeGet(commonConstants.ALL_DEVICE_TYPE_VALUES.HARDWARE_SENSOR)) {
                return true;
            };
            return false;
        }
        $scope.getDeviceTypeName = function (device) {
            var androidDeviceType = safeGet(commonConstants.ALL_DEVICE_TYPE_VALUES.ANDROID_DEVICE);

            switch (device.device_type) {
                case androidDeviceType:
                    return "android";

                default:
                    console.error('Unknown device type!');
                    return "unknown";
            }
        }
    }
]);