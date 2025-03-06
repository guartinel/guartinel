'use strict';
app.controller(STATES.homepage.unsubscribeFromPackageEmail.controller, [
   'accountService', '$scope', '$state', '$q', '$stateParams','$location',
   function (accountService, $scope, $state, $q, $stateParams,$location) {      
      var blackListToken = $location.search().blackListToken;
      var packageId = $location.search().packageID;

      $scope.unsubscribe = function () {

         accountService.unsubscribeFromPackageEmail(blackListToken, packageId).then(function (message) {
            $scope.state = "success";
            $scope.message = message
         }, function (error) {
            $scope.state = "fail";
            $scope.message = error;
         });
      }
   }
]);