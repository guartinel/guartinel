app.controller(DIALOGS.managementServer.log.controller, [
   '$scope', '$controller', 'managementServerStatusService',
   function($scope, $controller, managementServerStatusService) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.requestManagementServerLog = function() {

         managementServerStatusService.log().then(function(response) {
            $scope.log = response.log;
         }, function(error) {

         });
      }

      $scope.requestManagementServerLog();

   }
]);