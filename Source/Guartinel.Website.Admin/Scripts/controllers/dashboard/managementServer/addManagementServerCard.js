'use strict';
app.controller("addManagementServerCardController", [
   '$scope', 'dialogService', 
   function ($scope, dialogService) {
       
      $scope.showAddDialog = function () {
         dialogService.showDialog(DIALOGS.managementServer.add).then(function () {
             $scope.init();
         });
      }
   }
]);