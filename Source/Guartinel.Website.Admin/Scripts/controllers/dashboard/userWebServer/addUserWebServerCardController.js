'use strict';
app.controller("addUserWebServerCardController", [
    '$scope', 'dialogService', function ($scope, dialogService) {

        $scope.showAddDialog = function () {
            dialogService.showDialog(DIALOGS.userWebServer.add).then(function () {
                $scope.init();
            });
        }
    }
]);