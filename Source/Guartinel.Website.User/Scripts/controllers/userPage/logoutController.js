'use strict';
app.controller('logoutController', ['accountService',
   function (accountService) {
       accountService.logout();
   }]);