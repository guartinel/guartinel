'use strict';
app.controller("hostSupervisorPackageCardController", [
    'deviceService', '$scope', 'dialogService', 'hostSupervisorPackageService', '$controller', 'accountService', 'packageService', 'toastService',
   function (deviceService, $scope, dialogService, hostSupervisorPackageService, $controller, accountService, packageService, toastService) {
      $controller('basePackageCardController', { $scope: $scope });
      $scope.currentUserEmail = accountService.currentUser.email;
      function initController() {
         $scope.checkIntervalLabels = checkIntervalLabels;
         $scope.checkIntervals = checkIntervals;
       //1313  $scope.package = new hostSupervisorPackageService.createAndFillPackageTemplate($scope.package);
      }

      $scope.getCardStyle = function () {
         if (!$scope.package.isEnabled) {
            return $scope.getDisabledStyle();
         }
         if ($scope.isStateAlerting()) {
            return $scope.getAlertedStyle();
         }

         return { "background-color": "#5a9696" }// this is the normal color of the card
      }
      $scope.getPackagePartNameFromID = function (id) {
         return id;
      }
      function setCardStyle() {
         if ($scope.package.isEnabled) {
            $scope.cardStyle = {
               "background-color": "#5a9696"
            }
            if ($scope.package.lastMeasurement != null && $scope.package.state.name == "alerting") {
               $scope.cardStyle = { "background-color": "red" }
            }
         } else {
            $scope.cardStyle = {
               "background-color": "gray"
            }
         }
      }

      initController();
      setCardStyle();

       $scope.savePackage = function() {
           if ($scope.package.isEnabled &&
               (packageService.getPackageCount() >= accountService.currentUser.licenseAggregate.maximumPackages)) {
               toastService.showToast("You cannot have more than " +
                   accountService.currentUser.licenseAggregate.maximumPackages +
                   " active packages with your current licenses!",
                   "OK");
               $scope.package.isEnabled = false;
               return;
           }
           if ($scope.package.isEnabled &&
           (packageService.getPackagePartCount() + $scope.package.configuration.detailed_hosts.length >
               accountService.currentUser.licenseAggregate.maximumPackagePartCount)) {
               $scope.package.isEnabled = false;
               toastService.showToast("You cannot have more than " +
                   accountService.currentUser.licenseAggregate.maximumPackagePartCount +
                   " active package parts with your current licenses!",
                   "OK");
               return;
           }

           for (var i = 0; i < packageService.packages.length; i++) {
               if (packageService.packages[i].id === $scope.package.id) {
                   packageService.packages[i].isEnabled = $scope.package.isEnabled;
                   break;
               }
           }

           packageService.save($scope.package).then(function() {
               $scope.package.lastModificationTimestamp = moment(new Date()).toISOString();
                $scope.package.updateUI();
               $scope.package.updatePluginRelatedUI();
           });
       };

      $scope.showEditDialog = function (parameterPackage) {
         if (!$scope.canEditPackage()) {
            return;
         }
         hostSupervisorPackageService.showEditDialog(parameterPackage).then(function () {
            $scope.package.lastModificationTimestamp = new Date();
            $scope.getAvailable();
         });
      };


      $scope.$on('packageUpdated', function (event, packages) {
         for (var i = 0; i < packages.length; i++) {
            if (packages[i].id === $scope.package.id) {
               //1313$scope.package = hostSupervisorPackageService.createAndFillPackageTemplate(packages[i]);
               break;
            }
         }
         setCardStyle();
         if (isNull($scope.package.lastMeasurement)) {
            return;
         }
      });
   }
]);