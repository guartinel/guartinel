app.controller(DIALOGS.devices.disconnectHardwareSensor.controller, [
    '$scope', '$controller', 'deviceService', 'data', '$q',
    function ($scope, $controller, deviceService, data, $q) {

        angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

        $scope.device = data;


        $scope.confirmDisconnect = function () {
            return deviceService.disconnectHardwareSensor($scope.device.instance_id).then(function () {
                deviceService.getAvailable();
                $scope.answer();
                return $q.resolve();
            }, function (error) {
                return $q.reject(error);
            });
        }


    }
]);