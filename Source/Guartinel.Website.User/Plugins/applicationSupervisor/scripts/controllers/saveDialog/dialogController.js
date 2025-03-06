app.controller('applicationSupervisorPackageSaveDialogController',
    [
        '$scope', '$controller', 'packageService', '$q', 'data', 'accountService',
        function($scope, $controller, packageService, $q, parameterPackage, accountService) {
            angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
            $controller('baseSaveDialogController', { $scope: $scope });
            //  deviceService.getAvailable();

            $scope.currentUserEmail = accountService.currentUser.email;

            function initController() {
                $scope.package = parameterPackage;
            }

            initController();

            $scope.savePackage = function() {
                if ($scope.showErrorsIfErrorsPresent()) {
                    return $q.reject();
                }

                return packageService.save($scope.package).then(function() {
                        $scope.answer();
                        return $q.resolve();
                    },
                    function(error) {
                        return $q.reject(error.error);
                    });
            }
        }
    ]);