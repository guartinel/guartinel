app.controller(DIALOGS.account.created.controller, [
   '$scope', '$controller', '$q', 'accountService', 'data', '$state', 'toastService', function ($scope, $controller, $q, accountService, data, $state, toastService) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.email = data;

      $scope.activateAccount = function (activationCode) {
         return accountService.activate($scope.email, activationCode).then(function () {
            toastService.showToast("Account is activated!", "OK", null);
            $state.go(STATES.userpage.packages);
            $scope.answer();
            return $q.resolve();
         }, function (error) {
            $state.go(STATES.homepage.login);
            return $q.reject(error);
         });
      }
   }
]);