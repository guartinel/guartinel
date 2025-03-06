'use strict';
app.controller('websiteSupervisorCheckTabController', [
   '$scope', '$controller', 'accountService', 'packageService',
   function ($scope, $controller, accountService, packageService) {
      angular.extend(this, $controller('baseCheckTabController', { $scope: $scope }));
      $scope.currentUserEmail = accountService.currentUser.email;

      function initController() {
         $scope.newWebsiteAddress = "";
         if ($scope.package != null && $scope.package.configuration.retryCount == null) {
            $scope.package.configuration.retryCount = 2;
         }
      }
      $scope.deleteWebsite = function (index) {
         $scope.package.configuration.detailed_websites.splice(index, 1);
         $scope.checkPackagePartCountValid();
      }

      $scope.urlChanged = function () {     
         /*if (!$scope.checkPackagePartCountValid()) {
            $scope.savePackageForm.newWebsiteAddress.$setValidity('invalidURL', false);
         }
         $scope.savePackageForm.newWebsiteAddress.$setValidity('invalidURL', true);*/
      }

      $scope.checkPackagePartCountValid = function () {
         var packagePartCount = packageService.getPackagePartCount($scope.package.id);
         packagePartCount += $scope.package.configuration.detailed_websites.length;
        
         if (packagePartCount >= accountService.currentUser.licenseAggregate.maximumPackagePartCount) {
            $scope.savePackageForm.newWebsiteAddress.$setValidity('maximumPackagePartCountReached', false);
            return false;
         } else {
            $scope.savePackageForm.newWebsiteAddress.$setValidity('maximumPackagePartCountReached', true);
            return true;
         }
      }

      $scope.pushWebsite = function (address,caption) {
         if (isNull($scope.package.configuration.detailed_websites)) {
            $scope.package.configuration.detailed_websites = [];
         }
         //prevent adding the same website
         for (var index = 0; index < $scope.package.configuration.detailed_websites.length; index++) {
            if ($scope.package.configuration.detailed_websites[index].address == address) {
               return
            }
         }

         

         if(!$scope.checkPackagePartCountValid()){
            return;
         }

         $scope.package.configuration.detailed_websites.push({ address:address, caption:caption });
         $scope.newWebsiteAddress = "";
         $scope.newWebsiteCaption = "";
      }

      initController();
   }
]);