
app.controller(DIALOGS.packages.removeAccess.controller, [
   '$scope', '$controller', 'data', 'packageService', '$q',
   function ($scope, $controller, data, packageService, $q) {
      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
      $scope.package = data;


      $scope.confirmRemove = function (id) {
         return packageService.removeAccess(id).then(function () {
            packageService.getAvailable();
            $scope.answer();
            return $q.resolve();
         }, function (error) {
            return $q.reject(error);
         });
      }
      }   
]);

