app.controller(DIALOGS.watcherServer.edit.controller, [
   '$scope', '$controller', 'data', 'watcherServerService', '$q',
   function ($scope, $controller, data, watcherServerService, $q) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.watcherServer = Object.create(data);

      $scope.editWatcherServer = function(id, name, address, port,categories) { // TODO can be used as function(watcherServer) ???????

          return watcherServerService.update(id, name, address, port, categories).then(function () {
            $scope.answer();
            return $q.resolve();
         }, function(error) {
            return $q.reject(error);
         });

      }

   }
]);