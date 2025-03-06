'use strict';
app.controller("websiteSupervisorPackageCardController", [
    'deviceService', '$scope', 'dialogService', 'websiteSupervisorPackageService', '$controller', 'accountService', 'packageService', 'toastService',
   function (deviceService, $scope, dialogService, websiteSupervisorPackageService, $controller, accountService, packageService, toastService) {
      $controller('basePackageCardController', { $scope: $scope });

      $scope.currentUserEmail = accountService.currentUser.email;

      function initController() {
         $scope.checkIntervalLabels = checkIntervalLabels;
         $scope.checkIntervals = checkIntervals;
        //1313 $scope.package = websiteSupervisorPackageService.createAndFillPackageTemplate($scope.package);
      }

      $scope.getWebsiteDomain = function (url) {
         var result = url.replace('http://', '').replace('https://', '').replace('www.', '').split(/[/?#]/)[0];

         return result;
      }

      function setCardStyle() {
         if ($scope.package.isEnabled) {
            $scope.cardStyle = {
               "background-color": "#6c7355"
            }
            if ($scope.isStateAlerting()) {
                $scope.cardStyle = { "background-color": "red" };
            }
         } else {
            $scope.cardStyle = {
               "background-color": "gray"
            }
         }
      }
      $scope.getCardStyle = function () {
         if (!$scope.package.isEnabled) {
            return $scope.getDisabledStyle();
         }
         if ($scope.isStateAlerting()) {
            return $scope.getAlertedStyle();
         }

         return { "background-color": "#6c7355" }// this is the normal color of the card
      }

      initController();
      setCardStyle();

      $scope.savePackage = function () {
         if ($scope.package.isEnabled && (packageService.getPackageCount() >= accountService.currentUser.licenseAggregate.maximumPackages)) {
            toastService.showToast("You cannot have more than " + accountService.currentUser.licenseAggregate.maximumPackages + " active packages with your current licenses!", "OK");
            $scope.package.isEnabled = false;
            return;
         }
         if ($scope.package.isEnabled && (packageService.getPackagePartCount() + $scope.package.configuration.detailed_websites.length > accountService.currentUser.licenseAggregate.maximumPackagePartCount)) {
            $scope.package.isEnabled = false;
            toastService.showToast("You cannot have more than " + accountService.currentUser.licenseAggregate.maximumPackagePartCount + " active package parts with your current licenses!", "OK");
            return;
         }

         for (var i = 0; i < packageService.packages.length; i++) {
            if (packageService.packages[i].id === $scope.package.id) {
               packageService.packages[i].isEnabled = $scope.package.isEnabled;
               break;
            }
         }
          packageService.save($scope.package).then(function () {
              $scope.package.lastModificationTimestamp = moment(new Date()).toISOString();
              $scope.package.updateUI();
              $scope.package.updatePluginRelatedUI();
          });
      }

      $scope.showEditDialog = function (parameterPackage) {
         if (!$scope.canEditPackage()) {
            return;
         }
         websiteSupervisorPackageService.showEditDialog(parameterPackage).then(function () {
            $scope.getAvailable();
         });
      };

      $scope.$on('packageUpdated', function (event, packages) {
         for (var i = 0; i < packages.length; i++) {
            if (packages[i].id === $scope.package.id) {
            //1313   $scope.package = websiteSupervisorPackageService.createAndFillPackageTemplate(packages[i]);
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