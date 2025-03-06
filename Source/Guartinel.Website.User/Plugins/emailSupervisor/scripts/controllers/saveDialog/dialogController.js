app.controller('emailSupervisorPackageSaveDialogController',
    [
        '$scope', '$controller', '$q', 'data', 'accountService', 'packageService',
        function($scope, $controller, $q, parameterPackage, accountService, packageService) {
            angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
            $controller('baseSaveDialogController', { $scope: $scope });
            //deviceService.getAvailable();
            $scope.currentUserEmail = accountService.currentUser.email;

            function initController() {
                $scope.package = parameterPackage;
            }

            initController();

            $scope.savePackage = function() {
                if ($scope.showErrorsIfErrorsPresent()) {
                    return $q.reject();
                }

                if (isNull($scope.package.configuration.testEmailAddress)) {
                    $scope.formError = "You must add an email address!";
                    $scope.$apply();
                    return $q.reject();
                }
                if (isNull($scope.package.configuration.smtp.serverAddress)) {
                    $scope.formError = "You must add the SMTP server address!";
                    $scope.$apply();
                    return $q.reject();
                }
                if (isNull($scope.package.configuration.smtp.serverPort)) {
                    $scope.formError = "You must add the SMTP server port!";
                    $scope.$apply();
                    return $q.reject();
                }
                if (isNull($scope.package.configuration.smtp.user)) {
                    $scope.formError = "You must add the SMTP server user!";
                    $scope.$apply();
                    return $q.reject();
                }
                if (isNull($scope.package.configuration.imap.serverAddress)) {
                    $scope.formError = "You must add the IMAP server address!";
                    $scope.$apply();
                    return $q.reject();
                }
                if (isNull($scope.package.configuration.imap.serverPort)) {
                    $scope.formError = "You must add the IMAP server port!";
                    $scope.$apply();
                    return $q.reject();
                }
                if (isNull($scope.package.configuration.imap.user)) {
                    $scope.formError = "You must add the IMAP server user!";
                    $scope.$apply();
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