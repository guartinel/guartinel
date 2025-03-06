app.controller(DIALOGS.managementServer.add.controller, [
   '$scope', '$controller', '$q', 'managementServerService', function ($scope, $controller, $q, managementServerService) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      return $scope.addManagementServer = function (name, address,  description, userName, password, emailProvider, emailUserName, emailPassword) {
          var data = {}
          data.name = name;
          data.address = address;
          data.password = password;
          data.description = description;
          data.userName = userName;
          data.emailProvider = emailProvider;
          data.emailUserName = emailUserName;
          data.emailPassword = emailPassword;
          return managementServerService.add(data).then(function () {
            $scope.answer();
            return $q.resolve();
         }, function (error) {
            return $q.reject(error);
         });
      }
   }
]);