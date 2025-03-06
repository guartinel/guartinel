app.controller(DIALOGS.userWebServer.edit.controller, [
   '$scope', '$controller', '$q', 'userWebServerService', 'data', function ($scope, $controller, $q, userWebServerService, data) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.userWebServer = Object.create(data);

      $scope.updateUserWebServer = function (address, name) {
          var data = {
              address: address,
              name: name
          };
          return userWebServerService.update(data).then(function () {
            $scope.answer();
            return $q.resolve();
         }, function (error) {
            return $q.reject(error);
         });
      }

   }
]);