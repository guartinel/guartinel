app.controller('hostSupervisorPackageSaveDialogController', [
   '$scope', '$controller','$q', 'data','accountService','packageService',
    function ($scope, $controller, $q, parameterPackage, accountService, packageService) {
      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
      $controller('baseSaveDialogController', { $scope: $scope });
      //deviceService.getAvailable();
      
      $scope.currentUserEmail = accountService.currentUser.email;

      function initController() {
         $scope.package = parameterPackage;
      }

      initController();

      $scope.savePackage = function () {
         if ($scope.showErrorsIfErrorsPresent()) {
            return $q.reject();
         }
         if (isNull($scope.package.configuration.detailed_hosts) || $scope.package.configuration.detailed_hosts.length == 0) {
            $scope.formError = "You must add at least one host to the package!";
            $scope.$apply();
            return $q.reject();
         }
          return packageService.save($scope.package).then(function () {
            $scope.answer();
            return $q.resolve();
         }, function (error) {
            return $q.reject(error.error);
         });
      }
   }
]);