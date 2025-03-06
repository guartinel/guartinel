app.controller(DIALOGS.account.activateAccount.controller, [
   '$scope', '$controller','accountService', '$q', '$state',
   function ($scope, $controller, accountService, $q, $state) {

       angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

       $scope.email = accountService.currentUser.email;
       $scope.isResendLimitActive = false;
       $scope.activationState = "";
       $scope.isError = false;
      $scope.error = "";
       $scope.resendActivationCode = function () {
          $scope.activationState = "Sending...";
          return accountService.resendActivationCode().then(function() {
             $scope.activationState = "Sent!";
             return $q.resolve();
          }, function (error) {
             if (error.error === "ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND") {
                $scope.isResendLimitActive = true;
                error = "You have to wait 1 hour before sending the activation email again.";
             } else {
                error = "Cannot send activation mail currently. Try again later. Error code:"+ error.error_uuid;
             }
             $scope.error = error;
             $scope.isError = true;
             return $q.reject(error);
          });
       }

       $scope.activateAccount = function (activationCode) {
                return accountService.activate($scope.email, activationCode).then(function () {
                  $state.go(STATES.homepage.login);
                   $scope.answer();
                   return $q.resolve();
               }, function (error) {
                   return $q.reject(error);
               });
       }
   }
]);