app.controller(DIALOGS.account.expired.controller, [
   '$scope', '$controller', '$q', 'accountService', 'data', function ($scope, $controller, $q, accountService, data) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.email = data;

      return $scope.resendActivationCode = function (email) {
         return accountService.resendActivationCode(email).then(function () {
            return $q.resolve("Email sent!");
         }, function (error) {
            return $q.reject(error);
         });
      }

   }
]);