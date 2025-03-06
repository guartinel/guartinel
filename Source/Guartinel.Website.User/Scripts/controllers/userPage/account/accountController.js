'use strict';
app.controller(STATES.userpage.account.controller, [
   '$rootScope', 'accountService', '$scope', '$q', 'dialogService', '$state', '$mdDialog', function($rootScope, accountService, $scope, $q, dialogService, $state, $mdDialog) {
   
      $rootScope.screen = "Account";

    
      this.FormState = function (scope, successMessage) {
         scope.successMessage = successMessage;
         scope.isLoading = false;
         scope.error = "";
         scope.isError = false;
         scope.isSuccessFull = false;

         this.onStartLoading = function () {
            scope.isLoading = true;
            scope.error = "";
            scope.isError = false;
            scope.isSuccessFull = false;
         }

         this.onError = function (error) {
            scope.isLoading = false;
            scope.error = error;
            scope.isError = true;
            scope.isSuccessFull = false;
         }

         this.onSuccess = function () {
            scope.isLoading = false;
            scope.error = "";
            scope.isError = false;
            scope.isSuccessFull = true;

            scope.accountForm.$setUntouched();
            scope.accountForm.$setPristine();

            scope.newPassword = null;
            scope.newPasswordConfirm = null;
            scope.currentPassword = null;
            scope.currentUser = {};
         }
      }

      $scope.formState = new this.FormState($scope, "Successfully updated user account information!");

      $scope.getAccountInfo = function() {
         accountService.getStatus().then(function() {
               $scope.currentUser = Object.create(accountService.currentUser);
            },
            function(error) {
               $scope.error = error;
            });
      }

      $scope.updateAccount = function () {
         var confirm = $mdDialog.confirm()
            .title("Confirm")
            .content('Are you sure to update account information?')
            .ok('Yes')
            .cancel('Cancel');

         $mdDialog.show(confirm).then(function (success) {
            $mdDialog.hide();
            var email = $scope.currentUser.email;
            var newPassword = $scope.newPassword;
            var currentPassword = $scope.currentPassword;
            var firstName = $scope.currentUser.firstName;
            var lastName = $scope.currentUser.lastName;

            var newEmail = email === accountService.currentUser.email ? accountService.currentUser.email : email;
            newPassword = newPassword == null ? undefined : newPassword;
            var newFirstName = firstName === accountService.currentUser.firstName ? undefined : firstName;
            var newLastName = lastName === accountService.currentUser.lastName ? undefined : lastName;
           $scope.formState.onStartLoading();

          return  accountService.update(accountService.currentUser.id, newEmail, currentPassword, newPassword, newFirstName, newLastName).then(function(success) {
               $scope.getAccountInfo();
                $scope.formState.onSuccess();
               return $q.resolve();
            },
              function (error) {
                 $scope.formState.onError(error);
                 return;
              });
         }, function (error) {
            $scope.isLoading = false;
             $mdDialog.cancel();
             $q.reject(error);
         });
      }

      $scope.deleteAccount = function() {
         dialogService.showDialog(DIALOGS.account.deleteAccount).then(function() {
            $state.go(STATES.homepage.login);
         });
      }

      $scope.freezeAccount = function() {
         dialogService.showDialog(DIALOGS.account.freezeAccount).then(function() {
            $scope.message = "Account is freezed.";
         });
      }

      $scope.getAccountInfo();
   }
]);