app.controller('invalidTokenDialogController', [
   '$scope', '$controller', 'apiService', 'data', '$q',
   function ($scope, $controller, apiService, data, $q) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
      $scope.close = function () {
         $scope.cancel();
         $scope.hide();
         $scope.destroy();
         return $q.resolve();
      }

     /* invalidToken: {
            headerStyle: 'md-warn',
            title: 'Session error',
            controller: 'invalidTokenDialogController',
            template: 'templates/dialogs/invalidTokenDialog.html'
      }*/
   }
]);