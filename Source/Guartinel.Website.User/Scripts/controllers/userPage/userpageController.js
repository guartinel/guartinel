'use strict';
app.controller(STATES.userpage.controller, [
   '$rootScope', '$scope', 'accountService', 'deviceService', 'packageService', 'dialogService', 'toastService','$mdMedia',
   function ($rootScope, $scope, accountService, deviceService, packageService, dialogService, toastService, $mdMedia) {

       $scope.logout = function () {
           accountService.logout();
       };

       $scope.getRightSideBarClass = function() {
           if ($mdMedia('gt-md')) {
               return 'md-headline-autosize';
           }
           return ['md-headline'];
       };
       $scope.getUserName = function() {
           var userName = "";
           if (isEmptyOrNull(accountService.currentUser.firstName) &&
               isEmptyOrNull(accountService.currentUser.lastName)) {
               userName = accountService.currentUser.email;
           }
           if (isEmptyOrNull(accountService.currentUser.firstName) &&
               !isEmptyOrNull(accountService.currentUser.lastName)) {
               userName = accountService.currentUser.lastName;
           }
           if (!isEmptyOrNull(accountService.currentUser.firstName) &&
               isEmptyOrNull(accountService.currentUser.lastName)) {
               userName = accountService.currentUser.firstName;
           }
           if (!isEmptyOrNull(accountService.currentUser.firstName) &&
               !isEmptyOrNull(accountService.currentUser.lastName)) {
               userName = accountService.currentUser.firstName + " " + accountService.currentUser.lastName;
           }
           return userName;
       };
   }
]);




