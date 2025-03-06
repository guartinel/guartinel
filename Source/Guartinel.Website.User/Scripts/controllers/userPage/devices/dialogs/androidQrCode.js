app.controller(DIALOGS.devices.createAndroidDevice.androidQrCode.controller, [
   '$scope', '$controller', 'data',
   function($scope, $controller, data) {

      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));


      $scope.qrCodeString = data;
   }
]);