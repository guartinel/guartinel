'use strict';
app.service('rootService', [
   'apiService', '$q', function (apiService, $q) {

      this.getVersion = function (license) {
         var url = "home/getversion";//safeGet(backendUserApiUrls.GET_VERSION);
         var data = {};
         return apiService.sendRequest(url, data).then(function (response) {
            return $q.resolve(response);
         }, function (error) {
            console.error("getVersion failed! Error: " + error);
            return $q.reject(error);
         });
      }
   }
]);