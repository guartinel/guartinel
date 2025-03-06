'use strict';
app.controller("addWatcherServerCardController", [
    '$scope', 'dialogService', function ($scope, dialogService) {

       $scope.showAddDialog = function () {
          dialogService.showDialog(DIALOGS.watcherServer.add).then(function () {
             $scope.init();
          });
       }
   }
]);