'use strict';
app.controller('hardwareSupervisorInstancesTabController', [
    '$scope', 'accountService', '$mdDialog', '$window', 'hardwareSupervisorPackageService', 'deviceService', function ($scope, accountService, $mdDialog, $window, hardwareSupervisorPackageService, deviceService) {
        $scope.types = [
            pluginConstants.HARDWARE_TYPE_CURRENT_LEVEL_MAX_30A,
            pluginConstants.HARDWARE_TYPE_CURRENT_LEVEL_MAX_100A,
            pluginConstants.HARDWARE_TYPE_VOLTAGE_LEVEL_MAX_230V_ONE_CHANNEL,
            pluginConstants.HARDWARE_TYPE_VOLTAGE_LEVEL_MAX_230V_THREE_CHANNEL];// the order must be the same as typeLabels
        $scope.typeLabels = [
            "Current Meter (30A)",
            "Current Meter (100A)",
            "Voltage Meter (230V/1CH)",
            "Voltage Meter (230V/3CH)"];
        $scope.pluginConstants = $window.pluginConstants;

        $scope.selectedInstance = null;
        $scope.getInstanceName = function (instance_id) {
            for (var i = 0; i < $scope.availableInstances.length; i++) {
                if ($scope.availableInstances[i].instance_id == instance_id) {
                    return $scope.availableInstances[i].name;
                }
            }
            return instance_id;
        }

        $scope.addInstance = function (instance) {
            instance = JSON.parse(instance);
            for (var i = 0; i < $scope.package.configuration.instances.length; i++){
                if ($scope.package.configuration.instances[i].instance_id == instance.instance_id) {
                    return;
                }
            }
            delete instance.id;
            delete instance.categories;
            delete instance.is_disconnected;
            delete instance.created_on;
            delete instance.device_type;

            $scope.package.configuration.instances.push(instance);
            hardwareSupervisorPackageService.addInstanceSummary(instance);
        }
        $scope.onInstanceSelected = function (instance) {
            if (!isNull($scope.selectedInstance)) {//update the previous instance config only if it is not null
                hardwareSupervisorPackageService.addInstanceSummary($scope.selectedInstance);
            }
            $scope.selectedInstance = instance;
            hardwareSupervisorPackageService.addInstanceSummary($scope.selectedInstance);
        }

        function init() {
            $scope.availableInstances = [];
            //package is not saved yet so we cannot rely on MS to gather devices
            if ($scope.package.configuration.instances.length == 0) {
                $scope.availableInstances = deviceService.getHardwareSensors();
            } else {
                //package is already saved 
                for (var i = 0; i < $scope.package.devices.length; i++){
                    var dev = $scope.package.devices[i];
                    if (isNull(dev.hardware_type)){
                        continue;
                    }
                    $scope.availableInstances.push(dev);
                }
            }
        }


        $scope.nope = function () { }
        init();
    }
]);