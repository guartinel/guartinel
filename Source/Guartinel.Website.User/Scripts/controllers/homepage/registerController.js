'use strict';
app.controller(STATES.homepage.register.controller, [
   'accountService', '$scope', '$state', 'dialogService', '$q',
      function (accountService, $scope, $state, dialogService, $q) {

          $scope.register = function (email, password, firstName, lastName) {
             return accountService.register(email, password, firstName, lastName).then(function () {
                  $state.go(STATES.homepage.login);
                  dialogService.showDialog(DIALOGS.account.created, email);
                  return $q.resolve("Account registered.");
              }, function (error) {
                  var emailAlreadyRegisteredMessage = safeGet(commonConstants.ALL_ERROR_VALUES.EMAIL_ALREADY_REGISTERED);
                  switch (error.error) {
                      case emailAlreadyRegisteredMessage:
                          error = "Email address already registered!";
                          break;
                  }
                  return $q.reject(error);
              });
          }  
      }
]);