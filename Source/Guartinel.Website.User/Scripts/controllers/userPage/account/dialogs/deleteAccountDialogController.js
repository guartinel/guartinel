app.controller(DIALOGS.account.deleteAccount.controller, [
   '$scope', '$controller', 'accountService','$q','$state',
   function ($scope, $controller,  accountService,$q,$state) {

       angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

       $scope.deleteAccount = function (email,password) {

           return accountService.deleteAccount(email, password).then(function () {
               $state.go(STATES.homepage.login);
               $scope.answer();
               return $q.resolve();
           }, function (error) {
               return $q.reject(error);
           });
       }
   }
]);