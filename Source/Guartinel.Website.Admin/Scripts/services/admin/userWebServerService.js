'use strict';
app.service('userWebServerService', [
   '$q', 'apiService', function ($q, apiService) {

       this.getStatus = function () {
           var url = safeGet(backendAdminApiUrls.USER_WEB_SERVER_GET_STATUS);

           return apiService.sendAdminRequest(url).then(function (response) {
               return $q.resolve(response);
           }, function (response) {
               if (response.error === safeGet(commonConstants.ALL_ERROR_VALUES.MISSING_USER_WEB_SITE)) {
                   return $q.resolve(null);
               }
               console.error("Failed to get User Web Server status! Error: " + response.error);
               return $q.reject(response.error);
           });
       }

       this.getAvailable = function () {
           var url = safeGet(backendAdminApiUrls.USER_WEB_SERVER_GET_AVAILABLE);
           return apiService.sendAdminRequest(url).then(function (response) {
               return $q.resolve(response);
           }, function (response) {
               console.error("Failed to get available User Web Server! Error: " + response.error);
               return $q.reject(response.error);
           });
       }


       this.remove = function () {
           var url = safeGet(backendAdminApiUrls.USER_WEB_SERVER_REMOVE);
           return apiService.sendAdminRequest(url).then(function (response) {
               return $q.resolve(response);
           }, function (response) {
               console.error("Failed to remove User Web Server! Error: " + response.error);
               return $q.reject(response.error);
           });
       }


       this.register = function (data) {
           var url = safeGet(backendAdminApiUrls.USER_WEB_SERVER_REGISTER);
           data.password = CryptoJS.SHA512(data.password + data.userName).toString().toUpperCase();
           var parameters = {};
           parameters[safeGet(commonConstants.ALL_PARAMETERS.NAME)] = safeGet(data.name);
           parameters[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)] = safeGet(data.userName);
           parameters[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = safeGet(data.password);
           parameters[safeGet(commonConstants.ALL_PARAMETERS.USER_WEB_SERVER_ADDRESS)] = safeGet(data.address);
          
           return apiService.sendAdminRequest(url, parameters).then(function (response) {
               return $q.resolve(response);
           }, function (response) {
               console.error("Failed to register User Web Server! Error: " + response.error);
               return $q.reject(response.error);
           });
       }


       this.update = function (data) {
           var url = safeGet(backendAdminApiUrls.USER_WEB_SERVER_UPDATE);

           var parameters = {};
           parameters[safeGet(commonConstants.ALL_PARAMETERS.NAME)] = safeGet(data.name);
           parameters[safeGet(commonConstants.ALL_PARAMETERS.USER_WEB_SERVER_ADDRESS)] = safeGet(data.address);

           return apiService.sendAdminRequest(url, parameters).then(function (response) {
               return $q.resolve(response);
           }, function (response) {
               console.error("Failed to update User Web Server! Error: " + response.error);
               return $q.reject(response.error);
           });
       }
   }
]);
