app.controller(DIALOGS.managementServer.edit.controller, [
   '$scope', '$controller', '$q', 'managementServerService','data', function ($scope, $controller, $q, managementServerService, data) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.managementServer = Object.create(data);

      $scope.updateManagementServer = function (id, name, url,port, description, username, password) {

         return managementServerService.update(id, name, url,port, description, username, password).then(function () {
            $scope.answer();
            return $q.resolve();
         }, function (error) {
            return $q.reject(error);
         });
      }

   }
]);