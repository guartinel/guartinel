'use strict';
app.controller('emailSupervisorCheckTabController', [
   '$scope', '$controller', 'accountService', 'packageService',
   function ($scope, $controller, accountService, packageService) {
      angular.extend(this, $controller('baseCheckTabController', { $scope: $scope }));
     
      function init() {
         if (isNull($scope.package.configuration.imap.useSSL)) {
            $scope.package.configuration.imap.useSSL = false;
         }
         if (isNull($scope.package.configuration.smtp.useSSL)) {
            $scope.package.configuration.smtp.useSSL = false;
         }
      }
      init();

   }
]);