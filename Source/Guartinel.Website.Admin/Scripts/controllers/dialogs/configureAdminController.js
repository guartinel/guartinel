app.controller(DIALOGS.admin.configure.controller, [
   '$scope', '$controller', 'adminService', '$q', 'data',
   function ($scope, $controller, adminService, $q, data) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.changePassword = function (userName, currentPassword,newPassword) {
          return adminService.update(userName, currentPassword, newPassword).then(function () {
            $scope.answer();
            return $q.resolve();
         }, function(error) {
            return $q.reject(error);
         });
      }

   }
]);