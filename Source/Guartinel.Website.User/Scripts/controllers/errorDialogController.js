app.controller('errorDialogController', [
   '$scope', '$controller', 'apiService', 'data', '$q',
   function ($scope, $controller, apiService, data, $q) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.errorMessage = data;

      $scope.close = function () {
         $scope.answer();
         return $q.resolve();
      }
   }
]);