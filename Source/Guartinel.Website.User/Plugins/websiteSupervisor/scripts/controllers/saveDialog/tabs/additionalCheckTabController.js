'use strict';
app.controller('websiteSupervisorAdditionalCheckTabController', [
   '$scope', '$controller', 'accountService', function ($scope, $controller, accountService) {
      angular.extend(this, $controller('baseCheckTabController', { $scope: $scope }));
      $scope.currentUserEmail = accountService.currentUser.email;
  
   }
]);