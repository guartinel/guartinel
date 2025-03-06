'use strict';
app.controller(STATES.homepage.activate.controller, [
   'accountService', '$scope', '$state', '$q', '$stateParams',
   function (accountService, $scope, $state, $q, $stateParams) {

      $scope.state = "loading";

      $scope.activateAccount = function() {
         
         var activationCode = $stateParams.activationCode;

         accountService.activate(activationCode).then(function() {
            $scope.state = "success";
         }, function(error) {
            $scope.state = "fail";
            $scope.error = error;
         });
      }

      $scope.activateAccount();


      $scope.resendActivationCode = function(email) {
         
         accountService.resendActivationCode(email).then(function () {
            $scope.state = "success";
         }, function () {
            $scope.state = "fail";
         });
      }

   }
]);