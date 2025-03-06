app.controller(DIALOGS.managementServer.invalidRequests.controller, [
   '$scope', '$controller', 'managementServerStatusService',
   function($scope, $controller, managementServerStatusService) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.requestManagementServerInvalidRequests = function() {

         managementServerStatusService.invalidRequests().then(function(response) {
            $scope.invalidRequests = response.invalid_requests;
         }, function(error) {

         });
      }

      $scope.requestManagementServerInvalidRequests();

   }
]);