'use strict';
app.controller("loginHelperController", [
   'accountService', '$scope', '$state', '$q', 'dialogService', '$location', '$mdDialog', 'toastService','$window',
   function (accountService, $scope, $state, $q, dialogService, $location, $mdDialog, toastService, $window) {
   
      $scope.login = function (email, password) {
         return accountService.login(email, password).then(function () {
            $state.go(STATES.userpage.packages);
         }, function (error) {

            var invalidUserNameOrPasswordMessage = safeGet(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD);
            var accountExpiredMessage = safeGet(commonConstants.ALL_ERROR_VALUES.ACCOUNT_EXPIRED);

            switch (error.error) {
               case invalidUserNameOrPasswordMessage:
                  error = "Invalid user name or password!";
                  break;

               case accountExpiredMessage:
                  error = "Your account is expired!";
                  dialogService.showDialog(DIALOGS.account.expired, $scope.email);
                  break;
            }
         });
      };
   
   }
]);