'use strict';
app.controller('logoutController', ['adminService',
   function (adminService) {
      adminService.logout();
   }]);