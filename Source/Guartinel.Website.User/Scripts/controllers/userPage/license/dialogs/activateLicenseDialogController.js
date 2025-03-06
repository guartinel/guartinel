app.controller(DIALOGS.license.activateLicenseDialog.controller, [
   '$scope', '$controller', 'accountService', 'data', '$q', 'licenseService', '$window',
   function ($scope, $controller, accountService, data, $q, licenseService, $window) {
      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.license = data;

      $scope.doActivateLicense = function () {
         licenseService.activateLicense($scope.license).then(function () {
            $scope.cancel();
         }, function (error) {

         });
      }

      $scope.isActivationAvailable = function () {
         for (var i = 0; i < accountService.currentUser.licenses.length; i++) {
            if (accountService.currentUser.licenses[i].license._id === $scope.license.id) {
               return false;
            }
         }
         return true;
      }

   }
]);