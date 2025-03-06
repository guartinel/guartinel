app.controller(DIALOGS.devices.test.controller, [
   '$scope', '$controller', 'deviceService', 'data', '$q',
   function($scope, $controller, deviceService, data, $q) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.device = data;
      
      $scope.confirmTest = function(id) {

         return deviceService.test(id).then(function() {
            $scope.answer();
            return $q.resolve();
         }, function(error) {
            return $q.reject(error);
         });
      }
   }
]);