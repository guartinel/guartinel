'use strict';
app.controller("userpageMenuController", [
   '$mdMedia', '$rootScope', 'accountService', '$scope', '$state', '$mdSidenav',
   function ($mdMedia, $rootScope, accountService, $scope, $state, $mdSidenav) {

       $scope.logout = function () {
           accountService.logout();
       };

       $scope.screen = $state.$current.displayName;

       $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {// event when page loaded
           $scope.screen = toState.displayName;
       });

       $scope.toggleRightSideNav = function() {
           $mdSidenav(sideBarIds.RIGHT).toggle().then(function() {});
       };

       $scope.toggleLeftSideNav = function() {
           $mdSidenav(sideBarIds.LEFT).toggle().then(function() {});
       };

       $scope.onMenuItemClicked = function(menuitem) {
           $mdSidenav(sideBarIds.LEFT).toggle().then(function() {
               $state.go(STATES.userpage[menuitem]);
           });
       };
       $scope.onBackButtonPressed = function() {
           $mdSidenav(sideBarIds.LEFT).close();
           $mdSidenav(sideBarIds.RIGHT).close();
       };
       $scope.setViewForAndroidApp = function() {
           isViewedFromAndroidApp = true;
       };
       $scope.onMenuButtonPressed = function() {
           $mdSidenav(sideBarIds.LEFT).open().then(function() {});
           $mdSidenav(sideBarIds.RIGHT).close().then(function() {});
       };
       $scope.menu = [
          {
              link: 'userpage.devices',
              title: 'DEVICES',
              icon: 'devices',
              image: "Content/Images/menu_icons/devices.png"

          },
          {
              link: 'userpage.packages',
              title: 'PACKAGES',
              icon: 'layers',
              image: "Content/Images/menu_icons/packages.png"
          },
          {
              link: 'userpage.account',
              title: 'ACCOUNT',
              icon: 'person_outline',
              image: "Content/Images/menu_icons/myaccount.png"
          },
          {
              link: 'userpage.license',
              title: 'LICENSE',
              icon: 'attach_money',
              image: "Content/Images/menu_icons/mylicenses.png"
          },
          {
              link: 'userpage.logout',
              title: 'EXIT',
              icon: 'logout',
              image: "Content/Images/menu_icons/exit.png",
              isExit: true
          }
       ];      
   }
]);




