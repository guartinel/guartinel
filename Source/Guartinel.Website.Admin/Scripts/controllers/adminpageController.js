'use strict';
app.controller(STATES.adminpage.controller, [
   '$rootScope', '$scope', 'adminService', '$state', '$mdSidenav', 'dialogService',
   function ($rootScope, $scope, adminService, $state, $mdSidenav, dialogService) {

      $scope.screen = $state.$current.displayName;
      $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
         $scope.screen = toState.displayName;
      });

      $scope.logout = function () {
         adminService.logout().then(function () {
            $state.go(STATES.adminLogin);
         }, function (error) {

         });
      }

      $scope.menu = [
         {
            link: 'adminpage.dashboard',
            title: 'Dashboard',
            image: "Content/Images/dashboard.png"
         },
         {
            link: 'adminpage.adminAccount',
            title: 'Account',
            image: "Content/Images/myaccount.png"
         },
          {
             link: 'adminpage.logout',
             title: 'Exit',
             image: "Content/Images/exit.png",
             isExit: true
          }
      ];

      $scope.toggleSidenav = function (menuId) {
         $mdSidenav(menuId).toggle();
      };

      $scope.verifyAccountConfigured = function () {
         adminService.info().then(function (response) {
            if (!response.configured) {
               dialogService.showDialog(DIALOGS.admin.configure);
            }
         }, function (error) {

         });
      }

      // call when loaded
      $scope.verifyAccountConfigured();
   }
]);