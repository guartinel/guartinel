app.controller(DIALOGS.packages.deletePackage.controller, [
    '$scope', '$controller', 'data', 'packageService', '$q',
   function ($scope, $controller, data, packageService, $q) {

       angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

       $scope.package = data;

       $scope.confirmDelete = function (id) {
           return packageService.delete(id).then(function () {
               packageService.getAvailable();
               $scope.answer();
               return $q.resolve();
           }, function (error) {
               return $q.reject(error);
           });
       }

   }
]);