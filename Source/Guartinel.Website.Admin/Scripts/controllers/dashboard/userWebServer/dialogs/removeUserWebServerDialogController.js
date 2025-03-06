app.controller(DIALOGS.userWebServer.remove.controller, [
   '$scope', '$controller', '$q', 'userWebServerService', 'data', function ($scope, $controller, $q, userWebServerService, data) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.userWebServerService = data;

      $scope.removeUserWebServer = function () {
          return userWebServerService.remove().then(function () {
            $scope.answer();
            return $q.resolve();
         }, function (error) {
            return $q.reject(error);
         });
      }

   }
]);