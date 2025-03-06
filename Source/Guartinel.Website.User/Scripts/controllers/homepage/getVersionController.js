'use strict';
app.controller(STATES.homepage.getVersion.controller, [
   'rootService', '$scope', '$state', '$q', '$location',
   function (rootService, $scope, $state, $q, $locatio) {
      $scope.result;


      function getVersion() {
         return rootService.getVersion().then(function (versionData) {
            $scope.result = versionData;
            return $q.resolve();
         }, function (error) {
            $scope.result = error;

            return $q.reject(error);
         });
      };
      getVersion();

   }
]);