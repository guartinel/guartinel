'use strict';
app.controller('licenseController', ['$rootScope', '$scope', '$state', 'accountService', 'licenseService', '$location', '$filter', 'dialogService',
   function ($rootScope, $scope, $state, accountService, licenseService, $location, $filter, dialogService) {
      $scope.allLicenses = [];
      $scope.currentUserLicenses = [];
 
      var dates = [];
      $scope.convertDate = function (rawDate) {
         var convertedString = $filter('date')(rawDate, 'yyyy-MM-ddTHH:mm:ssZ');

         for (var i = 0; i < dates.length; i++) {
            if (dates[i].convertedString.indexOf(convertedString) != -1) {
               return dates[i].date;
            }
         }

         var newDate = new Date(convertedString);
         dates.push({
            convertedString: convertedString,
            date: newDate
         });

         return newDate;
      }
    

      function checkIfRedirectFromPayPal() {
         var payerID = $location.search().PayerID;
         var token = $location.search().paymentId;
         var error = $location.search().error;
         if (error == null && payerID != null && token != null) {
           licenseService.finalizeBuyingLicense(payerID, token).then(function () {
               $scope.refreshUserLicenses();
               $state.go(STATES.userpage.license);
           }, function (error) {
              $state.go(STATES.userpage.license);
               dialogService.showDialog(DIALOGS.errorDialog, "Cannot complete paypal transaction.\n Error: " + error.error_uuid);
            });
         }
         if (error) {
            $state.go(STATES.userpage.license);
            dialogService.showErrorDialog();
         }
      }


      $scope.doLicenseAction = function (license) {
         if (license.name == "trial") {
            dialogService.showDialog(DIALOGS.license.activateLicenseDialog, license).then(function() {
               $scope.refreshUserLicenses();
            }, function (err) {
               $scope.refreshUserLicenses();
            });
            return;
         }
         dialogService.showDialog(DIALOGS.license.buyLicense, license).then(function() {
            $scope.refreshUserLicenses();
         }, function (err) {
            $scope.refreshUserLicenses();
         });
      }
      $scope.selectLicenseIndex = function (index) {
         $scope.selectedLicenseIndex = index;
      }

      $scope.getLicensePriceText = function (licenseItem) {
         var result ="";

         if (licenseItem.prices[0].price === 0) {
            result += " Free ";
         } else {
            result += Math.round((licenseItem.prices[0].price / licenseItem.prices[0].interval) * 100) / 100 + " USD / Month";
         }
         return result;
      }

      $scope.getActionButtonText = function (licenseItem) {
         if (licenseItem.name === "trial") {
            return "Activate";
         }
         return "Buy";
      }

      $scope.getAvailable = function () {
         licenseService.getAvailable().then(function (licenses) {
            $scope.allLicenses = licenses;
         }, function (error) {
            //add error card
         });
      }
      $scope.refreshUserLicenses = function () {
         accountService.getStatus().then(function () {
            $scope.currentUserLicenses = accountService.currentUser.licenses;

         });
      }
      $scope.refreshUserLicenses();
      $scope.getAvailable();

      angular.element(document).ready(function () {
         checkIfRedirectFromPayPal();
      });

      /* $scope.allLicenses = [
           {
              _id: "57b301dc98fd416808b6911b",
              name: "free",
              span: {
                 col: 3,
                 row: 3
              },
              background: "tile1",
              caption: "Free",
              maximumPackages: 2,
              maximumDevices: 2,
              minimumCheckIntervalSec: 5,
              maximumAlertsPerHour: 10,
              priceForMonth: 0,
              categories: [
                  "free",
                  "default"
              ],
              createdOn: "2016-08-16T12:06:52.698Z"
           },
           {
              _id: "57b301dc98fd416808b6911c",
              name: "trial",
              span: {
                 col: 3,
                 row: 3
              },
              background: "tile2",
              caption: "Trial License",
              maximumPackages: 25,
              maximumDevices: 10,
              minimumCheckIntervalSec: 60,
              maximumAlertsPerHour: 5,
              priceForMonth: 0,
              categories: [
                  "trial"
              ],
              createdOn: "2016-08-16T12:06:52.698Z"
           },
           {
              _id: "57b301dc98fd416808b6911e",
              name: "s",
              span: {
                 col: 3,
                 row: 3
              },
              background: "tile3",
              caption: "S-Package",
              maximumPackages: 25,
              maximumDevices: 10,
              minimumCheckIntervalSec: 180,
              maximumAlertsPerHour: 5,
              priceForMonth: 0,
              categories: [
                  "s"
              ],
              createdOn: "2016-08-16T12:06:52.698Z"
           },
           {
              _id: "57b301dc98fd416808b6911f",
              name: "m",
              span: {
                 col: 3,
                 row: 3
              },
              background: "tile4",
              caption: "M-Package",
              maximumPackages: 10,
              maximumDevices: 10,
              minimumCheckIntervalSec: 120,
              maximumAlertsPerHour: 20,
              priceForMonth: 20,
              categories: [
                  "m"
              ],
              createdOn: "2016-08-16T12:06:52.698Z"
           },
          {
             _id: "57b301dc98fd416808b6912f",
             name: "l",
             span: {
                col: 3,
                row: 3
             },
             background: "tile5",
             caption: "L-Package",
             maximumPackages: 25,
             maximumDevices: 10,
             minimumCheckIntervalSec: 60,
             maximumAlertsPerHour: 40,
             priceForMonth: 80,
             price: [
                {
                   price: 10,
                   interval: 2
                },
             {
                price: 15,
                interval: 3
             }
             ],
             categories: [
                 "l"
             ],
             createdOn: "2016-08-16T12:06:52.698Z"
          },
            {
               _id: "57b301dc98fd416808b6913e",
               name: "xl",
               span: {
                  col: 3,
                  row: 3
               },
               background: "tile6",
               caption: "XL-Package",
               maximumPackages: 100,
               maximumDevices: 100,
               minimumCheckIntervalSec: 30,
               maximumAlertsPerHour: 100,
               priceForMonth: 500,
               categories: [
                   "xl"
               ],
               createdOn: "2016-08-16T12:06:52.698Z"
            }
       ];*/
   }]);