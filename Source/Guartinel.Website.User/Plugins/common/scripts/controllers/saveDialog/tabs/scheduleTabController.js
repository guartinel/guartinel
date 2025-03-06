'use strict';
app.controller('scheduleTabController', [
   '$scope', 'accountService', '$window', function ($scope, accountService, $window) {
      //bring schedule constans to local scope from global to be accesed from directives
      $scope.commonConstants = $window.commonConstants;

      $scope.intervalLabels = ["1 min", "5 min", "15 min", "30 min ", "1 hour", "2 hour", "3 hour", "6 hour", "12 hour"];
      $scope.intervals = [60, 300, 900, 1800, 3600, 7200, 10800, 21600, 43200];

      $scope.typeLabels = ["Weekly", "Daily", "Once"];  //the order is important must be the same as types
      $scope.types = [commonConstants.ALL_PARAMETERS.PACKAGE_SCHEDULE_WEEKLY, commonConstants.ALL_PARAMETERS.PACKAGE_SCHEDULE_DAILY, commonConstants.ALL_PARAMETERS.PACKAGE_SCHEDULE_ONCE];

      $scope.today = new Date();

      $scope.getTypeLabel = function (type) {
         return $scope.typeLabels[$scope.types.indexOf(type)];
      }

      function initController() {
         if (isNull($scope.package.disableAlerts)) {
            $scope.package.disableAlerts = {};
         }
         if (isNull($scope.package.disableAlerts.schedules)) {
            $scope.package.schedules.disableAlerts = [];
         }

         if (isNull($scope.package.disableAlerts.schedules[0])) {
            $scope.package.disableAlerts.schedules.push({
               is_enabled: false,
               type: $scope.types[0],
               time: new Date(),
               interval_in_seconds: 300,
               days: {
                  monday: false,
                  tuesday: false,
                  wednesday: false,
                  thursday: false,
                  friday: false,
                  saturday: false,
                  sunday: false
               }

            });
         }
         if (!isNull($scope.package.disableAlerts.schedules[0].date)) {
            $scope.selectedDate = new Date($scope.package.disableAlerts.schedules[0].date);
         }
         if (!isNull($scope.package.disableAlerts.schedules[0].time)) {
            $scope.selectedTime = new Date(moment.utc($scope.package.disableAlerts.schedules[0].time, "HH:mm"));
         }
         if (!isNull($scope.package.disableAlerts.schedules[0].date_time)) {
            $scope.package.disableAlerts.schedules[0].date_time = new Date($scope.package.disableAlerts.schedules[0].date_time);
         }
      }

      $scope.onTimeChanged = function () {
         var stringDate = $scope.selectedTime.toISOString();
         var onlyTime = stringDate.substring(stringDate.indexOf('T') + 1, stringDate.length);
         $scope.package.disableAlerts.schedules[0].time = onlyTime;
      }

      initController();
   }
]);



