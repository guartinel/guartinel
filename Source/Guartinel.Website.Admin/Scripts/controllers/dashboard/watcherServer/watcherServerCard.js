'use strict';
app.controller("watcherServerCardController", [
   '$scope', 'dialogService', 'watcherServerService', function ($scope, dialogService, watcherServerService) {

      //$scope.watcherServer = $scope.dashboardCard.data;
      $scope.state = "loading";

      $scope.loadData = function() {
         watcherServerService.getStatus($scope.watcherServer.id).then(function (response) {
            $scope.watcherServerStatus = response.status;
            $scope.watcherServerStatus.cpu = parseInt($scope.watcherServerStatus.cpu);
            $scope.watcherServerStatus.hdd_free_gb = parseInt($scope.watcherServerStatus.hdd_free_gb);
            $scope.watcherServerStatus.memory = parseInt($scope.watcherServerStatus.memory);


            $scope.state = "connected";
         }, function(error) {
            $scope.state = "disconnected";
         });
      }

      $scope.loadData();

      // --- DIALOGS ---

      $scope.showEditDialog = function(watcherServer) {
         dialogService.showDialog(DIALOGS.watcherServer.edit, watcherServer).then(function () {
            $scope.init();
         });
      }

      $scope.showRemoveDialog = function (watcherServer) {
         dialogService.showDialog(DIALOGS.watcherServer.remove, watcherServer).then(function () {
            $scope.init();
         });
      }

   }
]);