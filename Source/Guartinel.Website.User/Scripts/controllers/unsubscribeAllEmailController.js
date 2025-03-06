'use strict';
app.controller(STATES.homepage.unsubscribeAllEmail.controller, [
   'accountService', '$scope', '$state', '$q', '$stateParams','$location',
   function (accountService, $scope, $state, $q, $stateParams,$location) {
      var blackListToken = $location.search().blackListToken;

  
      $scope.unsubscribe = function () {  
         accountService.unsubscribeAllEmail(blackListToken).then(function (message) {
            $scope.state = "success";
            $scope.message = message
         }, function (error) {
            $scope.state = "fail";
            $scope.message = error;
         });
      }
   }
]);