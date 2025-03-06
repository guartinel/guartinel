'use strict';
app.controller(STATES.userpage.devices.controller, [
    'packageService', 'deviceService', '$scope', 'dialogService', '$interval', '$window','$mdSidenav',
    function (packageService, deviceService, $scope, dialogService, $interval, $window, $mdSidenav) {

      $scope.isLoading = false;
        $scope.getAvailable = function(isUpdate) {
            if (!isUpdate) {
                $scope.isLoading = true;
            }
            packageService.getAvailable().then(function() {
                deviceService.getAvailable().then(function(devices) {
                        $scope.devices = devices;
                        $scope.isLoading = false;
                    },
                    function(error) {
                        $scope.isLoading = false;
                        //add error card
                    });
            });
        };
        $scope.startAddingHardwareSensor = function () {
            $mdSidenav(sideBarIds.RIGHT).close();

            $window.android.addNewHardwareSensor();
        };
        $scope.isAndroidApp = function() {
            return $window.android;
        };

        $scope.isNavBarOpen = function() {
            return $mdMedia('gt-md');
        };

        $scope.createAndroidDevice = function () {
            $mdSidenav(sideBarIds.RIGHT).close();

            dialogService.showDialog(DIALOGS.devices.createAndroidDevice, null).then(function() {
                $scope.loadDevices();
            });
        };

      $scope.getAvailable();
      $scope.isDialogOpen = false;

      var refreshPromise = $interval(function () {
         if ($scope.isDialogOpen) {
            return;
         }
         if (isTabVisible()) {
            $scope.getAvailable(true);
         }
      }, 6000);

      $scope.$on('$destroy', function () {
         if (refreshPromise)
            $interval.cancel(refreshPromise);
      });
   }
]);