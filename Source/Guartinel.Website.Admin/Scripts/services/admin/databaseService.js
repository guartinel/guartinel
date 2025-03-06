'use strict';
app.service('databaseService', [
   '$q', 'apiService', function ($q, apiService) {

       this.getStatus = function () {
           var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_DATABASE_GET_STATUS);
           return apiService.sendAdminRequest(url).then(function (response) {
               console.log("Database status arrived");
               var database = {};
               response = response[safeGet(commonConstants.ALL_PARAMETERS.STATUS)];
               database.isConnected = response[safeGet(commonConstants.ALL_PARAMETERS.IS_CONNECTED)];
               database.version = response[safeGet(commonConstants.ALL_PARAMETERS.DB_VERSION)];
               database.size = response[safeGet(commonConstants.ALL_PARAMETERS.STORAGE_SIZE)]+"";
               database.url = response[safeGet(commonConstants.ALL_PARAMETERS.URL)];
               database.userName = response[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)];
               return $q.resolve(database);
           }, function (response) {
               console.error("Database status failed! Error: " + response.error);
               return $q.reject(response.error);
           });
       }

       this.update = function (data) {
           var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_DATABASE_UPDATE);
           var translatedData = {};
           data[safeGet(commonConstants.ALL_PARAMETERS.DATABASE_URL)] = data.url;
           data[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)] = data.userName;
           data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = data.password;

           return apiService.sendAdminRequest(url, translatedData).then(function (response) {
               return $q.resolve(response);
           }, function (response) {
               console.error("Database register failed! Error: " + response.error);
               return $q.reject(response.error);
           });
       }

   }
]);
