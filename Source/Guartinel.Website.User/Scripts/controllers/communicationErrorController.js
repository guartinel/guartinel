app.controller('communicationErrorController', [
   '$scope', '$controller', 'apiService', 'data', '$q',
   function ($scope, $controller, apiService, data, $q) {

       angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
       
       $scope.kickUser = function () {
           //apiService.kickUser();
           $scope.hide();
           return $q.resolve();
       }
       $scope.errorUUID = data;

       $scope.onSuccess = function (e) {
           console.info('Action:', e.action);
           console.info('Text:', e.text);
           console.info('Trigger:', e.trigger);
           e.clearSelection();
       };
       $scope.onError = function (e) {
           console.error('Action:', e.action);
           console.error('Trigger:', e.trigger);
       }
   }
  
]);