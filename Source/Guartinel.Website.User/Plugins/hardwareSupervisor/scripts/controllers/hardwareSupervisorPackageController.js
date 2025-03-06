'use strict';
app.controller("hardwareSupervisorPackageCardController", [
    'deviceService', '$scope', 'dialogService', 'hardwareSupervisorPackageService', '$controller', 'accountService', 'packageService', 'toastService',
    function (deviceService, $scope, dialogService, hardwareSupervisorPackageService, $controller, accountService, packageService, toastService) {
        $controller('basePackageCardController', { $scope: $scope });
        $scope.currentUserEmail = accountService.currentUser.email;

        function initController() {
            $scope.checkIntervalLabels = checkIntervalLabels;
            $scope.checkIntervals = checkIntervals;

            //1313$scope.package = new hardwareSupervisorPackageService.createAndFillPackageTemplate($scope.package);


            $scope.availableInstances = [];
            for (var i = 0; i < $scope.package.devices.length; i++) {
                var dev = $scope.package.devices[i];
                if (isNull(dev.hardware_type)) {
                    continue;
                }
                $scope.availableInstances.push(dev);
            }
        }

        $scope.getStyleForInstanceContainer = function () {
            var baseStyle = { "max-height": "155px", "overflow-x": "hidden", "background-color": "white" }
            if ($scope.package.configuration.instances.length == 1) {
                baseStyle["min-height"] = "50px";
            }

            if ($scope.package.configuration.instances.length == 2) {
                baseStyle["min-height"] = "90px";
            }
            if ($scope.package.configuration.instances.length == 3) {
                baseStyle["min-height"] = "135px";
            }
            if ($scope.package.configuration.instances.length >= 4) {
                baseStyle["min-height"] = "165px";
            }
            return baseStyle;
        }

        $scope.getInstanceName = function (instance_id) {
            for (var i = 0; i < $scope.availableInstances.length; i++) {
                if ($scope.availableInstances[i].instance_id == instance_id) {
                    return $scope.availableInstances[i].name;
                }
            }
            return instance_id;
        }

        $scope.getInstanceStateMessage = function (instance) {
            if (!isNull($scope.package.state) && !isNull($scope.package.state.states)) {
                for (var index = 0; index < $scope.package.state.states.length; index++) {
                    var stateItem = $scope.package.state.states[index];
                    if (stateItem.package_part_identifier == instance.id) {
                        return stateItem.package_part_message_built;
                    }
                }
            }
            return "Check is running.";
        }

        $scope.getInstanceIconStyle = function (instance) {
            if (!$scope.package.isEnabled) {
                return { "fill": "gray" };
            }
            for (var index = 0; index < $scope.package.state.states.length; index++) {
                var stateItem = $scope.package.state.states[index];
                if (stateItem.package_part_identifier == instance.id) {
                    if (stateItem.package_part_state == "ok") {
                        return { "fill": "green" };
                    }
                    return { "fill": "red" };
                }
            }
            return { "fill": "gray" };
        }

        $scope.getCardStyle = function () {
            if (!$scope.package.isEnabled) {
                return $scope.getDisabledStyle();
            }
            if ($scope.isStateAlerting()) {
                return $scope.getAlertedStyle();
            }

            return { "background-color": "#5ca6bc" }// this is the normal color of the card

        }


        function setCardStyle() {
            if ($scope.package.isEnabled) {
                $scope.cardStyle = {
                    "background-color": "#5ca6bc"
                }
                if ($scope.package.lastMeasurement != null && $scope.package.state.name == "alerting") {
                    $scope.cardStyle = { "background-color": "red" }
                }
            } else {
                $scope.cardStyle = {
                    "background-color": "gray"
                }
            }
        }

        initController();
        setCardStyle();

        $scope.savePackage = function() {
            if ($scope.package.isEnabled &&
                (packageService.getPackageCount() >= accountService.currentUser.licenseAggregate.maximumPackages)) {
                toastService.showToast("You cannot have more than " +
                    accountService.currentUser.licenseAggregate.maximumPackages +
                    " active packages with your current licenses!",
                    "OK");
                $scope.package.isEnabled = false;
                return;
            }
            if ($scope.package.isEnabled &&
            (packageService.getPackagePartCount() + $scope.package.configuration.instances.length >
                accountService.currentUser.licenseAggregate.maximumPackagePartCount)) {
                $scope.package.isEnabled = false;
                toastService.showToast("You cannot have more than " +
                    accountService.currentUser.licenseAggregate.maximumPackagePartCount +
                    " active package parts with your current licenses!",
                    "OK");
                return;
            }
            for (var i = 0; i < packageService.packages.length; i++) {
                if (packageService.packages[i].id === $scope.package.id) {
                    packageService.packages[i].isEnabled = $scope.package.isEnabled;
                    break;
                }
            }
            packageService.save($scope.package).then(function() {
                $scope.package.lastModificationTimestamp = moment(new Date()).toISOString();
                 $scope.package.updateUI();
                $scope.package.updatePluginRelatedUI();
            });
        };

        $scope.showEditDialog = function (parameterPackage) {
            if (!$scope.canEditPackage()) {
                return;
            }
            hardwareSupervisorPackageService.showEditDialog(parameterPackage).then(function () {
                $scope.getAvailable();
            });
        };


        $scope.$on('packageUpdated', function (event, packages) {
            for (var i = 0; i < packages.length; i++) {
                if (packages[i].id === $scope.package.id) {
                  //1313  $scope.package = hardwareSupervisorPackageService.createAndFillPackageTemplate(packages[i]);
                    break;
                }
            }
            setCardStyle();
            if (isNull($scope.package.lastMeasurement)) {
                return;
            }
        });
    }
]);