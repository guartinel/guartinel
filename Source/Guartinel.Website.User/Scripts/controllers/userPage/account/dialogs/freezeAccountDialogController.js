app.controller(DIALOGS.account.freezeAccount.controller, [
   '$scope', '$controller',  'accountService', '$q',
   function ($scope, $controller, accountService, $q) {

       angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

       $scope.freezeAccount = function (email, password) {
           return accountService.freezeAccount(email, password).then(function () {
               $scope.answer();
               return $q.resolve();
           }, function (error) {
               return $q.reject(error);
           });
       }
   }
]);