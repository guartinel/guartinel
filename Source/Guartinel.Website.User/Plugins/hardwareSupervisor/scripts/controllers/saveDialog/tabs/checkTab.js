'use strict';
app.controller('hardwareSupervisorCheckTabController', [
   '$scope', '$controller', function ($scope, $controller) {
      angular.extend(this, $controller('baseCheckTabController', { $scope: $scope }));
  

      function generateRandomToken() {
         var RESULT_LENGTH = 25;
         var result = "";
         var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

         for (var i = 0; i < RESULT_LENGTH; i++) {
            result += possible.charAt(Math.floor(Math.random() * possible.length));
         }

         return result;
      }

      /*
      $scope.renewToken = function () {
         $scope.package.configuration[safeGet(pluginConstants.HARDWARE_TOKEN)] = generateRandomToken();//TODO APPLICATION_SUPERVISOR_CONSTANTS
      }
      */

      function initController() {
         if (isEmptyOrNull($scope.package.configuration[safeGet(pluginConstants.HARDWARE_TOKEN)])) {
           // $scope.renewToken();
            $scope.package.configuration[safeGet(pluginConstants.HARDWARE_TOKEN)] = generateRandomToken();
         }
      }

      initController();
   }
]);