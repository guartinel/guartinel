app.controller(DIALOGS.devices.createAndroidDevice.controller, [
   '$scope', '$controller', 'accountService', 'dialogService',
   function ($scope, $controller, accountService, dialogService) {

       angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

       $scope.showQrCode = function (deviceName, password) {
           var keyValueSeparator = "!&!"; //Warning IF changed must be changed in android app
           var keyEqualsSeparator = "&!&";
           var qrCode =
               "email" + keyEqualsSeparator + accountService.currentUser.email
               + keyValueSeparator +
               "password" + keyEqualsSeparator + password
               + keyValueSeparator +
               "device_name" + keyEqualsSeparator + deviceName;

           dialogService.showDialog(DIALOGS.devices.createAndroidDevice.androidQrCode, qrCode);
       }
   }
]);