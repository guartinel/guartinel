'use strict';
app.controller("managementServerCardController", [
   '$scope', 'dialogService', 'managementServerService', 'managementServerStatusService','databaseService',
   function ($scope, dialogService, managementService, managementStatusService, databaseService) {
   /*
      $scope.managementServer = $scope.dashboardCard.data;
      $scope.status = "loading";
      

      $scope.loadData = function () {
         managementStatusService.getStatus().then(function(statusResponse) {
             $scope.managementServerStatus = statusResponse[safeGet(commonConstants.ALL_PARAMETERS.STATUS)];

             $scope.managementServerStatus.current_cpu_usage =
               parseInt($scope.managementServerStatus.current_cpu_usage * 100);
            $scope.managementServerStatus.current_memory_usage =
               parseInt($scope.managementServerStatus.current_memory_usage * 100);
            $scope.managementServerStatus.current_storage_usage =
               parseInt($scope.managementServerStatus.current_storage_usage * 100);
            $scope.status = "connected";

             databaseService.getStatus().then(function(databaseStatus) {
                 $scope.database = databaseStatus;
             }, function (error) {
                 console.log("Cannot get db status: " + JSON.stringify(error));
                 $scope.database = {};
                 $scope.database.isConnected = false;
             });
         }, function(error) {
            $scope.status = "disconnected";
         });
      }
      $scope.loadData();*/
       
      // --- DIALOGS ---

      $scope.showUpdateDatabaseDialog = function () {
         dialogService.showDialog(DIALOGS.database.update, $scope.managementServerStatus.database).then(function () {
            $scope.init();
          });
      }


      $scope.showEditDialog = function() {
         dialogService.showDialog(DIALOGS.managementServer.edit, $scope.managementServerStatus).then(function () {
            $scope.init();
         });
      }

      $scope.showRemoveDialog = function() {
         dialogService.showDialog(DIALOGS.managementServer.remove, $scope.managementServerStatus).then(function () {
            $scope.init();
         });
      }


      $scope.showLogDialog = function() {
         dialogService.showDialog(DIALOGS.managementServer.log).then(function () {
            $scope.init();
         });
      }

      $scope.showEventsDialog = function() {
         dialogService.showDialog(DIALOGS.managementServer.events).then(function () {
            $scope.init();
         });
      }

      $scope.showInvalidRequestsDialog = function() {
         dialogService.showDialog(DIALOGS.managementServer.invalidRequests).then(function () {
            $scope.init();
         });
      }


   }
]);