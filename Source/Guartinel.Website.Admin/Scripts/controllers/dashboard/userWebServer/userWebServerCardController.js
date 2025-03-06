'use strict';
app.controller("userWebServerCardController", [
   '$scope', 'dialogService', 'userWebServerService', function ($scope, dialogService, userWebServerService) {

    /*  $scope.state = "loading";
      $scope.userWebServer = $scope.dashboardCard.data;
      $scope.loadData = function() {
          userWebServerService.getStatus().then(function (response) {

            $scope.userWebServerStatus = new Object();
            $scope.userWebServerStatus.current_cpu_usage = parseInt(response.cpu);
            $scope.userWebServerStatus.current_memory_usage = parseInt(response.memory);
            $scope.userWebServerStatus.current_storage_usage = parseInt(response.storage);

            $scope.state = "connected";
          }, function (error) {
              console.log("Error while getting userWebServerInfos:" + error);
            $scope.state = "disconnected";
         });
      }
      $scope.loadData();*/

      $scope.showEditDialog = function () {
         dialogService.showDialog(DIALOGS.userWebServer.edit, $scope.userWebServerStatus).then(function () {
            $scope.init();
          });
      }

      $scope.showRemoveDialog = function () {
         dialogService.showDialog(DIALOGS.userWebServer.remove, $scope.userWebServerStatus).then(function () {
            $scope.init();
          });
      }

   }
]);