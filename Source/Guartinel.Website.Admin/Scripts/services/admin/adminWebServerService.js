'use strict';
app.service('adminWebServerService', [
   '$q', 'apiService', function($q, apiService) {

      this.getStatus = function() {
          var url = safeGet(backendAdminApiUrls.WEB_SITE_STATUS_GET_STATUS);
          return apiService.sendAdminRequest(url).then(function (response) {
            return $q.resolve(response);
          }, function (response) {
              console.error("Status request failed! Error: " + response.error);
              return $q.reject(response.error);
         });
      }
   }
]);
