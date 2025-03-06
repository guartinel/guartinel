app.controller(DIALOGS.watcherServer.add.controller, [
   '$scope', '$controller', '$q', 'watcherServerService', function ($scope, $controller, $q, watcherServerService) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
        $scope.categories = [];
      $scope.addWatcherServer = function(name, url, port, username, password ,categories) {
         
          return watcherServerService.register(name, url, port, username, password,  categories).then(function () {
            $scope.answer();
            return $q.resolve();
         }, function(error) {
            return $q.reject(error);
         });
      }
   }
]);