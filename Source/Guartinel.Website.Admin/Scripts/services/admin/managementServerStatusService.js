'use strict';
app.service('managementServerStatusService', [
   '$q', 'apiService', function ($q, apiService) {

       this.events = function () {
           var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_STATUS_GET_EVENTS);
           return apiService.sendAdminRequest(url).then(function (response) {
            console.log("Management events arrived");
            return $q.resolve(response);
           }, function (response) {
               console.error("Management events request failed! Error: " + response.error);
               return $q.reject(response.error);
         });
      }

      this.getStatus = function() {
          var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_STATUS_GET_STATUS);

         return apiService.sendAdminRequest(url).then(function(response) {
            console.log("Management status info arrived");
            var result = {
               cpu: parseInt(response.status.current_cpu_usage * 100),
               memory: parseInt(response.status.current_memory_usage * 100),
               storage: parseInt(response.status.current_storage_usage * 100),
               programVersion: response.status.program_version,
               nodejsVersion: response.status.nodejs_version,
               uptimeMs: response.status.uptime_ms,
               gcmsSent: response.status.gcms_sent,
               emailsSent: response.status.emails_sent,
               alertsSent: response.status.alerts_sent,
               requests: response.status.requests
            };
   
            return $q.resolve(result);
         }, function (response) {
             console.error("Management status info failed! Error: " + response.error);
             return $q.reject(response.error);
         });
      }

      this.invalidRequests = function() {
          var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_STATUS_GET_INVALID_REQUESTS);

         return apiService.sendAdminRequest(url).then(function (response) {
            console.log("Management invalidrequests arrived");
            return $q.resolve(response);
         }, function (response) {
             console.error("Management invalidrequests failed! Error: " + response.error);
             return $q.reject(response.error);
         });
      }

      this.log = function() {
          var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_STATUS_GET_LOG);

         return apiService.sendAdminRequest(url).then(function (response) {
            console.log("Management log arrived");
            return $q.resolve(response);
         }, function (response) {
             console.error("Management log failed! Error: " + response.error);
             return $q.reject(response.error);
         });
      }
   }
]);
