app.controller(DIALOGS.userWebServer.add.controller, [
   '$scope', '$controller', '$q', 'userWebServerService', function ($scope, $controller, $q, userWebServerService) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.addUserWebServer = function (name, address, userName, password) {

          var data  = {
              userName : userName,
              name : name,
              address : address,
              password: password
          }
          return userWebServerService.register(data).then(function () {
            $scope.answer();
            return $q.resolve();
         }, function(error) {
            return $q.reject(error);
         });
      }
   }
]);