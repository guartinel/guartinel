app.controller(DIALOGS.license.buyLicense.controller, [
   '$scope', '$controller', 'deviceService', 'data', '$q', 'licenseService', '$window','accountService',
   function ($scope, $controller, deviceService, data, $q, licenseService, $window, accountService) {
      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.license = data;
      $scope.startDate = moment().toDate();
      $scope.selectedInterval = null;
      $scope.startBuyingLicenseErrorId = null;

      function setDates() {
         $scope.expiryDate = moment($scope.startDate);
         $scope.expiryDate.add($scope.selectedInterval, 'months');
         $scope.expiryDate = $scope.expiryDate.toDate();
      }

      function setPrices() {
         for (var i = 0; i < $scope.license.prices.length; i++) {
            if ($scope.license.prices[i].interval === $scope.selectedInterval) {
               $scope.currentPrice = $scope.license.prices[i].price;
               return;
            }
         }
         $scope.currentPrice = null;
      }
      $scope.getSetPaymentParameters = function () {
         var result =  {
            license : {
               id: $scope.license.id,
               startDate: moment($scope.formatDate).toISOString(),
               selectedInterval: $scope.selectedInterval
            },
            account:  {},
            token: accountService.currentUser.token
         }
         result.license = JSON.stringify(result.license);
         return result;       
      }

      $scope.onSelectedIntervalChanged = function (interval) {
         $scope.selectedInterval = interval;
         setDates();
         setPrices();
      }

      $scope.onStartDateChange = function (startDate) {
         $scope.startDate = startDate;
         setDates();
      }
    
      $scope.startBuyingLicense = function () {
         $scope.startBuyingLicenseError = false;
         var license = {
            id: $scope.license.id,
            startDate: moment($scope.formatDate).toISOString(),
            selectedInterval: $scope.selectedInterval
         }
         licenseService.startBuyingLicense(license).then(function (redirectURL) {
            $scope.startBuyingLicenseErrorId = null;
            // redirectToPayPal(token);
            $window.location.href = redirectURL;
         }, function (error) {
            $scope.startBuyingLicenseErrorId = error.error_uuid;
            console.log(error);
         });
      }
      $scope.getSelectedLicense = function () {
         return     {
            id: $scope.license.id,
            startDate: moment($scope.formatDate).toISOString(),
            selectedInterval: $scope.selectedInterval
         }
      }
      $scope.getLicenseService = function () {
         return licenseService;
      }
   }
]);