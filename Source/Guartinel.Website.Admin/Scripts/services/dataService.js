'use strict';
app.service('dataService', [
   '$cookieStore', '$state', function($cookieStore, $state) {

      var currentAdmin = {
         token: null,
         userName: null
      }

      this.currentAdmin = function() {
         return currentAdmin;
      }

      this.storeAdminToken = function(token) {
         currentAdmin.token = token;
         $cookieStore.put('adminToken', token);
      }

      this.clearAdmin = function () {

         currentAdmin.email = null;
         currentAdmin.token = null;

         $cookieStore.remove('adminToken');
         $state.go(STATES.adminLogin);
      }

      // load values from cookies, when this class is loaded
      this.loadData = function () {
         // admin
         currentAdmin.token = $cookieStore.get('adminToken');
      }
      this.loadData();
   }
]);