app.controller(DIALOGS.account.update, [
   '$scope', '$controller','data','accountService', '$q',
   function ($scope, $controller, data,accountService, $q) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

     

   }
]);