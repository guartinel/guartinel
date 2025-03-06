'use strict';
app.controller('applicationController', ['$rootScope', '$scope', '$state', 'dataService',
   function ($rootScope, $scope, $state, dataService, accountService) {

   $rootScope.$on('$stateChangeStart',
      function (event, toState, toParams, fromState, fromParams) {

         // check if an admin session is present
         if (dataService.currentAdmin().token) {

            // if reaching state other than adminpage, redirect to dashboard
            if (toState.name.indexOf(STATES.adminpage.name) == -1) {
               $state.go(STATES.adminpage.dashboard);
               event.preventDefault();
            }
         } else {
            // no token, but user wants to access adminpage
            if (toState.name.indexOf(STATES.adminpage.name) != -1) {
                $state.go(STATES.adminLogin);
               event.preventDefault();
            }
         }
      });
}]);