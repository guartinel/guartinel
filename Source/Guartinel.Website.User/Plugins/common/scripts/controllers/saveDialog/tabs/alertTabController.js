"use strict";
app.controller("alertTabController", [
   "$scope", "deviceService", "packageService", "accountService",
   function ($scope, deviceService, packageService, accountService) {
      $scope.currentUser = accountService.currentUser.email;

      function initController() {
         $scope.availableAlertDevices = [];
         var allDevice = deviceService.devices();
         for (var deviceIndex = 0; deviceIndex < allDevice.length; deviceIndex++) {
            allDevice[deviceIndex].owner = accountService.currentUser.email;
         }
         //copy all device from device service
         for (var deviceIndex = 0; deviceIndex < allDevice.length; deviceIndex++) {
            //add only alert devices
            if (allDevice[deviceIndex].device_type === "android_device") {
               $scope.availableAlertDevices.push(allDevice[deviceIndex]);
            }
         }
         if (!isNull($scope.package.devices)) {
            var newAlertDevicesFromPackage = [];
            for (var packageDeviceIndex = 0; packageDeviceIndex < $scope.package.devices.length; packageDeviceIndex++) {
               var packageDevice = $scope.package.devices[packageDeviceIndex];
               //check if already added
               var found = false;
               for (var availableAlertDeviceIndex = 0; availableAlertDeviceIndex < $scope.availableAlertDevices.length; availableAlertDeviceIndex++) {
                  var addedDevice = $scope.availableAlertDevices[availableAlertDeviceIndex];
                  if (addedDevice.id === packageDevice.id) {
                     found = true;
                     break;
                  }
               }
               if (found) { //already added lets continue
                  continue;
               }
               if (packageDevice.device_type !== "android_device") { //it is not an alert device continue...
                  continue;
               }
               newAlertDevicesFromPackage.push(packageDevice);
            }
            //copy newly found devices to the main array
            newAlertDevicesFromPackage.forEach(function (newAlertDeviceFromPackage) {
               $scope.availableAlertDevices.push(newAlertDeviceFromPackage);
            });
         }

         if (!$scope.package.hasAnyAlertMethodSet()) {
            $scope.package.addAlertEmail(accountService.currentUser.email);
         }
      }
      
      $scope.hasAvailableAlertDevice = function () {
         if ($scope.availableAlertDevices != null && $scope.availableAlertDevices.length > 0) {
            return true;
         }
         return false;
      }

      initController();
      $scope.nop = function () { };

      $scope.isEmailValidationInProgress = false;
      $scope.testEmail = function (email) {
         if (email === accountService.currentUser.email) { // assume that the user account email is valid
            addToValidatedEmails(email, true);
            return;
         }

         $scope.isEmailValidationInProgress = true;
         return packageService.testEmail(email).then(function () {
            addToValidatedEmails(email, true);
            $scope.isEmailValidationInProgress = false;
         }, function () {
            addToValidatedEmails(email, false);
            $scope.isEmailValidationInProgress = false;
         });
      };

      $scope.onAddNewAlertEmail = function () {
         $scope.package.addAlertEmail($scope.newMail);
         $scope.testEmail($scope.newMail);
      };

      $scope.getDeviceOwnerFromId = function (deviceId) {
         var result = "";
         if (isNull($scope.package.devices)) {
            return accountService.currentUser.email;;
         }
         $scope.package.devices.forEach(function (item, index) {
            if (item.id === deviceId) {
               result = item.owner;
            }
         });
         return result;
      }
      $scope.getDeviceNameFromId = function (deviceId) {
         var result = "";
         $scope.availableAlertDevices.forEach(function (item, index) {
            if (item.id === deviceId) {
               result = item.name;
            }
         });
         return result;
      };

      var validatedMails = [];

      function addToValidatedEmails(email, isValid) {
         for (var i = 0; i < validatedMails.length; i++) {
            if (validatedMails[i].email === email) {
               validatedMails[i].isValid = isValid;
               return;
            }
         }
         validatedMails.push({ email: email, isValid: isValid });
      }

      $scope.isValidated = function (email) {
         for (var i = 0; i < validatedMails.length; i++) {
            if (validatedMails[i].email === email) {
               return true;
            }
         }
         return false;
      };
      $scope.getValidationResult = function (email) {
         for (var i = 0; i < validatedMails.length; i++) {
            if (validatedMails[i].email === email) {
               return validatedMails[i].isValid;
            }
         }
         return "";
      };
   }
]);