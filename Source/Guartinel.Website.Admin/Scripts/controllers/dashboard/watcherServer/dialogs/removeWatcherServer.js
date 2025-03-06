app.controller(DIALOGS.watcherServer.remove.controller, [
   '$scope', '$controller', 'data', 'watcherServerService', '$q',
   function ($scope, $controller, data, watcherServerService, $q) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.watcherServer = data;

      $scope.removeWatcherServer = function(id) {
         return watcherServerService.remove(id).then(function() {
            $scope.answer();
            return $q.resolve();
         }, function(error) {
            return $q.reject(error);
         });
      }

   }
]);