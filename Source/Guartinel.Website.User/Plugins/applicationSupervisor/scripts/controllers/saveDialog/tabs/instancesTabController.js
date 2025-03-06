'use strict';
app.controller('applicationSupervisorInstancesTabController', [
   '$scope', 'accountService', function ($scope) {
    
      $scope.nope = function () { }

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

      function initController() {
      }
      initController();
   }
]);