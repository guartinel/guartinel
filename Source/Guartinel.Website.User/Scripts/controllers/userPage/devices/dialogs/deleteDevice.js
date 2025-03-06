app.controller(DIALOGS.devices.deleteDevice.controller, [
   '$scope', '$controller', 'deviceService', 'data', '$q',
   function ($scope, $controller, deviceService, data, $q) {

       angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

       $scope.device = data;


       $scope.confirmDelete = function (id) {
           return deviceService.delete(id).then(function () {
               deviceService.getAvailable();
               $scope.answer();
               return $q.resolve();
           }, function (error) {
               return $q.reject(error);
           });
       }


   }
]);