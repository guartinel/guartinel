app.controller(DIALOGS.managementServer.events.controller, [
   '$scope', '$controller', 'managementServerStatusService',
   function($scope, $controller, managementServerStatusService) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.requestManagementServerEvents = function() {

         managementServerStatusService.events().then(function(response) {
            $scope.events = response.events;
         }, function(error) {

         });
      }

      $scope.requestManagementServerEvents();


   }
]);