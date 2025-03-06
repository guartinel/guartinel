app.controller(DIALOGS.managementServer.remove.controller, [
   '$scope', '$controller', '$q', 'managementServerService', 'data', function ($scope, $controller, $q, managementServerService, data) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.managementServer = data;

      $scope.removeManagementServer = function (id) {

         return managementServerService.remove(id).then(function () {
            $scope.answer();
            return $q.resolve();
         }, function (error) {
            return $q.reject(error);
         });
      }

   }
]);