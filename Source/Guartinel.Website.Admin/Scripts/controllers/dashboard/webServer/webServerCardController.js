'use strict';
app.controller("webServerCardController", [
   '$scope', 'dialogService', 'adminWebServerService', function ($scope, dialogService, adminWebServerService) {

    /*  $scope.state = "loading";

      $scope.loadData = function() {
          adminWebServerService.getStatus().then(function (response) {
            $scope.webServerStatus = new Object();
            $scope.webServerStatus.current_cpu_usage = parseInt(response.cpu);
            $scope.webServerStatus.current_memory_usage = parseInt(response.memory);
            $scope.webServerStatus.current_storage_usage = parseInt(response.storage);

            $scope.state = "connected";
         }, function(error) {
            $scope.state = "disconnected";
         });
      }

      $scope.loadData();*/
   }
]);