
'use strict';
app.controller('baseCheckTabController', [
    '$scope', 'accountService', function ($scope, accountService) {
        $scope.useDefaultTimeoutValue = true;

        function getCurrentCheckIntervalIndex() {
            var checkInterval = parseInt($scope.package.checkIntervalSeconds);
            return $scope.checkIntervals.indexOf(checkInterval);
        }
        function getCurrentTimeoutIntervalIndex() {
            if (isNull($scope.package.timeoutIntervalSeconds)) {
                return getCurrentCheckIntervalIndex(); // return check interval index if timeout is null 
            }
            var timeoutInterval = parseInt($scope.package.timeoutIntervalSeconds);
            return $scope.timeoutIntervals.indexOf(timeoutInterval);
        }


        $scope.nope = function () { }
        function initController() {
            var licenseAggregate;

            if (!isNull($scope.package.owner) && $scope.package.owner != accountService.currentUser.email) {
                licenseAggregate = $scope.package.license;
            } else {
                licenseAggregate = accountService.currentUser.licenseAggregate;
            }

            var minInterval = licenseAggregate.minimumCheckIntervalSec;
            $scope.checkIntervals = checkIntervals;
            $scope.checkIntervalLabels = checkIntervalLabels;
            $scope.timeoutIntervals = timeoutIntervals;
            $scope.timeoutIntervalLabels = timeoutIntervalLabels;

            for (var i = 0; i < $scope.checkIntervals.length; i++) {
                if ($scope.checkIntervals[i] >= minInterval) {
                    $scope.checkIntervals.splice(0, i);
                    $scope.checkIntervalLabels.splice(0, i);
                    $scope.timeoutIntervals.splice(0, i);
                    $scope.timeoutIntervalLabels.splice(0, i);
                    break;
                }
            }
            if (isNull($scope.package.timeoutIntervalSeconds) || $scope.package.timeoutIntervalSeconds == $scope.package.checkIntervalSeconds) {
                $scope.useDefaultTimeoutValue = true;

            } else {
                $scope.useDefaultTimeoutValue = false;
                $scope.timeoutIntervalIndex = getCurrentTimeoutIntervalIndex();

            }

            $scope.checkIntervalIndex = getCurrentCheckIntervalIndex();
          
            $scope.$watch('checkIntervalIndex', function (newValue) {
                $scope.package.checkIntervalSeconds = $scope.checkIntervals[newValue];
            });
            $scope.$watch('timeoutIntervalIndex', function (newValue) {
                $scope.package.timeoutIntervalSeconds = $scope.timeoutIntervals[newValue];
            });            
        }

        $scope.onDefaultTimeoutSelected = function (value) {
            if (!value) {
               // $scope.package.timeoutIntervalSeconds = $scope.checkIntervals[value]; 
                return;
            }
            //$scope.timeoutIntervalIndex = $scope.checkIntervalIndex;
            $scope.package.timeoutIntervalSeconds = null;
        }
        initController();
    }
]);