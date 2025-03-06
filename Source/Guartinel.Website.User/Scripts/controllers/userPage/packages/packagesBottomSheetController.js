app.controller('packagesBottomSheetController', [
   '$scope', 'packageService', 'pluginService', 'toastService', 'deviceService', 'accountService', 'dialogService', '$interval', '$rootScope', '$mdBottomSheet',
   function ($scope, packageService, pluginService, toastService, deviceService, accountService, dialogService, $interval, $rootScope, $mdBottomSheet) {

      $scope.items = [
     { name: 'Hangout', icon: 'hangout' },
     { name: 'Mail', icon: 'mail' },
     { name: 'Message', icon: 'message' },
     { name: 'Copy', icon: 'copy2' },
     { name: 'Facebook', icon: 'facebook' },
     { name: 'Twitter', icon: 'twitter' }
      ];

      $scope.listItemClick = function ($index) {
         var clickedItem = $scope.items[$index];
         $mdBottomSheet.hide(clickedItem);
      };

   }
]);