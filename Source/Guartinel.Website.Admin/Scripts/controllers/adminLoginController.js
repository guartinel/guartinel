'use strict';
app.controller(STATES.adminLogin.controller, ['$q', '$scope', 'adminService', '$state',
   function($q, $scope, adminService, $state) {

      $scope.adminLogin = function(username, password) {

         return adminService.login(username, password).then(function() {

            $state.go(STATES.adminpage.dashboard);

         }, function(error) {
            $scope.password = '';
            return $q.reject(error);
         });
      }
   }
]);