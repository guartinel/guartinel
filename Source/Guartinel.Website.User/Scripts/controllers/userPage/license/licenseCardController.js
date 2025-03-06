'use strict';
app.controller('licenseCardController', ['$rootScope', '$scope', '$state', 'accountService', 'licenseService', '$location', '$filter', 'dialogService',
   function ($rootScope, $scope, $state, accountService, licenseService, $location, $filter, dialogService) {


      $scope.getCardStyle = function () {
         var result = {
            /*'background-image': 'url(Content/Images/licenses/'+$scope.licenseItem.license.name +'.png)',
            '-webkit-background-size': 'cover',
            '-moz-background-size': 'cover',
            '-o-background-size': 'cover',
            'background-size': 'cover'*/
         };
         if ($scope.licenseItem.expiryDate < new Date()) {
            result['background-color'] = 'gray';           
         } else {
            result['background-color'] = '#0277BD';
         }
         return result;
      }

      $scope.getTitleText = function () {
         if ($scope.licenseItem.expiryDate < new Date()) {
            return $scope.licenseItem.license.caption + " (expired)"
         }
         return $scope.licenseItem.license.caption 
      }
      
      }]);