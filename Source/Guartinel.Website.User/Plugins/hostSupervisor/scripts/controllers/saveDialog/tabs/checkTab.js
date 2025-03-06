'use strict';
app.controller('hostSupervisorCheckTabController', [
   '$scope', '$controller', 'accountService','packageService',
   function ($scope, $controller, accountService, packageService) {
      angular.extend(this,$controller('baseCheckTabController', { $scope: $scope }));


      function initController() {
         $scope.newHostAddress = "";
         if ($scope.package != null && $scope.package.configuration.retryCount == null) {
            $scope.package.configuration.retryCount = 2;
         }
      }

       $scope.deleteHost = function (index) {
          $scope.package.configuration.detailed_hosts.splice(index, 1);
          $scope.checkPackagePartCountValid();
       }

       $scope.ipChanged = function () {         
          if ( $scope.newHostAddress.length  <5 ) {
             $scope.savePackageForm.newHostAddress.$setValidity('invalidIP', false);
             return;
          } 
          $scope.savePackageForm.newHostAddress.$setValidity('invalidIP', true);
       }

       $scope.checkPackagePartCountValid = function () {
          var packagePartCount = packageService.getPackagePartCount($scope.package.id);
          packagePartCount += $scope.package.configuration.detailed_hosts.length;
        
          if (packagePartCount  >= accountService.currentUser.licenseAggregate.maximumPackagePartCount) {
             $scope.savePackageForm.newHostAddress.$setValidity('maximumPackagePartCountReached', false);
             return false;
          } else {
             $scope.savePackageForm.newHostAddress.$setValidity('maximumPackagePartCountReached', true);
             return true;
          }
       }

       $scope.pushHost = function (address,caption) {
          if (isNull($scope.package.configuration.detailed_hosts)) {
             $scope.package.configuration.detailed_hosts = [];
          }

          //prevent adding the same host twice
          for (var index = 0; index < $scope.package.configuration.detailed_hosts.length;index++) {
             if ($scope.package.configuration.detailed_hosts[index].address == address){
                return;
             }
          }
        
          if (!$scope.checkPackagePartCountValid()) {
             return;
          }

          $scope.package.configuration.detailed_hosts.push({ address: address, caption: caption });
          $scope.newHostAddress = "";
          $scope.newHostCaption = "";
       }

      initController();
    }
]);