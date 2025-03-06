app.controller(STATES.userpage.packages.controller,
    [
        '$scope', 'packageService', 'pluginService', 'toastService', 'accountService', 'dialogService', '$interval',
        '$rootScope', '$mdSidenav', '$mdMedia','$window',
        function($scope,
            packageService,
            pluginService,
            toastService,
            accountService,
            dialogService,
            $interval,
            $rootScope,
            $mdSidenav,
            $mdMedia,
            $window) {

            $scope.availablePackages = pluginService.getAvailablePackages();
            $scope.isLoading = false;

            $scope.startTutorial = function(pack) {
                return dialogService.showDialog(DIALOGS.packages.tutorialDialog, pack.tutorialItems).then(function() {
                });
            }
            $scope.getPackages = function() {
                return packageService.packages;
            }
            $rootScope.freshGetavailable = function () {
                packageService.packages = [];
                $scope.pacages = [];
                $scope.isLoading = true;
                packageService.getAvailable().then(function(packages) {
                        if ($scope.packages == null) {
                            $scope.packages = [];
                        }
                        $scope.packages = packages;

                        $rootScope.$broadcast('packageUpdated', $scope.packages);
                        $scope.isLoading = false;
                    },
                    function(error) {
                        //add error card
                        $scope.isLoading = false;
                    });
            };

            $scope.getAvailable = function(isUpdate) {
                if (!isUpdate) {
                    $scope.isLoading = true;
                }
                packageService.getAvailable().then(function(packages) {
                        if ($scope.packages == null) {
                            $scope.packages = [];
                        }
                        $scope.packages = packages;

                        $rootScope.$broadcast('packageUpdated', $scope.packages);
                        $scope.isLoading = false;
                    },
                    function(error) {
                        //add error card
                        $scope.isLoading = false;
                    });
            };


            $scope.showPackageStatisticsDialog = function(parameterPackage) {
                dialogService.showDialog(DIALOGS.packages.statistics, parameterPackage).then(function() {
                });
            };

            $scope.showDeleteDialogFromPackageController = function(parameterPackage) {
                dialogService.showDialog(DIALOGS.packages.deletePackage, parameterPackage).then(function() {
                    $scope.getAvailable();
                });
            };
            $scope.showRemoveAccessDialog = function(parameterPackage) {
                dialogService.showDialog(DIALOGS.packages.removeAccess, parameterPackage).then(function() {
                    $scope.getAvailable();
                });
            };
            $scope.isAndroidApp = function () {
                return $window.android;
            };


            $scope.getEnabledPackagesCount = function() {
                var count = 0;
                $scope.packages.forEach(function(item, index) {
                    if (item.isEnabled) {
                        count++;
                    }
                });
                return count;
            };

            $scope.showCreateDialog = function(availablePackage) {
                if (packageService.getPackageCount() >= accountService.currentUser.licenseAggregate.maximumPackages) {
                    toastService.showToast("You cannot have more than " +
                        accountService.currentUser.licenseAggregate.maximumPackages +
                        " active packages with your current licenses!",
                        "OK");
                    return;
                }
                $mdSidenav(sideBarIds.RIGHT).close();

                return pluginService.showCreateDialog(availablePackage).then(function() {
                    $scope.getAvailable();
                });
            };

            $scope.getAvailable();

            var refreshTick = 0;
            var refreshPromise = $interval(function() {
                    refreshTick++;
                    if (!isTabVisible() && refreshTick < 30) {
                        /// if tab is invisible we are only refresh it with 30 sec interval
                        return;
                    }
                    if (isTabVisible() && refreshTick < 3) { // if tab is visible then we only refresh every 3sec
                        return;
                    }
                    refreshTick = 0;
                    $scope.getAvailable(true);
                },
                10000);

            $scope.$on('$destroy',
                function() {
                    if (refreshPromise)
                        $interval.cancel(refreshPromise);
                });

            /* $scope.showBottomSheet = function () {
                $scope.alert = '';
                $mdBottomSheet.show({
                   templateUrl: 'templates/userpage/packages/packagesBottomSheet.html',
                   controller: 'packagesBottomSheetController',
                   clickOutsideToClose: true
                }).then(function(clickedItem) {
                   console.log("clicked:" + clickedItem);
                });
             };*/

        }
    ]);