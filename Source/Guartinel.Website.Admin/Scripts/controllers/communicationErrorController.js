app.controller('communicationErrorController', [
   '$scope', '$controller', 'apiService', 'data', '$q',
   function ($scope, $controller, apiService, data, $q) {

       angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
       
       $scope.kickUser = function () {
           apiService.kickUser();
           $scope.answer();
           return $q.resolve();
       }
       $scope.errorUUID = data;
   }
  
]);