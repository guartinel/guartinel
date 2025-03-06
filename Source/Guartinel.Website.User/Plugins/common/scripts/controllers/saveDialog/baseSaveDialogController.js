
'use strict';
app.controller('baseSaveDialogController', [
   '$scope', function ($scope) {
      $scope.showErrorsIfErrorsPresent = function (deviceId) {
         if (!isNull($scope.package.disableAlerts) && !isNull($scope.package.disableAlerts.schedules)) {
            for (var index = 0; index < $scope.package.disableAlerts.schedules.length; index++) {
               var schedule = $scope.package.disableAlerts.schedules[index];
               if (schedule.type === commonConstants.ALL_PARAMETERS.PACKAGE_SCHEDULE_DAILY) {
                  delete schedule.date_time;
                  delete schedule.days;
                  delete schedule.date;
               }
               if (schedule.type === commonConstants.ALL_PARAMETERS.PACKAGE_SCHEDULE_WEEKLY) {
                  delete schedule.date_time;
                  delete schedule.date;
               }
               if (schedule.type === commonConstants.ALL_PARAMETERS.PACKAGE_SCHEDULE_ONCE) {
                  delete schedule.days;
                  delete schedule.time;
                  schedule.date_time = schedule.date_time.toISOString();
               }
            }
         }

         if (!$scope.package.hasAnyAlertMethodSet()) {
            $scope.formError = "You must select at least one alert method (alert email or device)";
            $scope.$apply();
            return true;
         }

         if ($scope.package.hasEmptyAccessRule()) {
            $scope.formError = "Cannot save with empty access email address. Check your access preferences in the access tab.";
            $scope.$apply();
            return true;
         }
         if ($scope.package.hasMultipliedOwnerAccout()) {
            $scope.formError = "Cannot add your own access to a package. Check your access preferences in the access tab and remove your email from there.";
            $scope.$apply();
            return true;
         }
         try {
            if ($scope.$$childTail.savePackageForm.accessUserEmailForm.$invalid) {
               $scope.formError = "There is something wrong with your access settings.";
               $scope.$apply();
               return true;
            }
         } catch (err) {
            console.log("Cannot get accessuseremailForm validity status");
         }
         return false;
      }
   }
]);

