'use strict';
app.service('apiService', [
   '$http', '$q', 'dataService', 'dialogService', function ($http, $q, dataService, dialogService) {
      

       this.sendAdminRequest = function (url, data) {
           if (data == null) {
               data = new Object();
           }
           data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = dataService.currentAdmin().token;
           return this.sendRequest(url, data);
       }

       var kickAdmin = function () {
           dataService.clearAdmin();
       }
      
       this.sendRequest = function (url, data) {
           console.log("Sending request to backend url :" + url + " request " + JSON.stringify(data));

           return $http.post(url, data).then(function (response) {
               console.log("Response from backend " + JSON.stringify(response));
               
               if (response.data[safeGet(commonConstants.ALL_PARAMETERS.SUCCESS)] === safeGet(commonConstants.ALL_SUCCESS_VALUES.SUCCESS)) {
                   return $q.resolve(response.data);
               }
               
               switch (response.data[safeGet(commonConstants.ALL_PARAMETERS.ERROR)]) {
                   case safeGet(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN):
                       console.error("The token is INVALID!");
                       kickAdmin();
                       return $q.reject("The token is invalid.");

                   case "INVALID_MODEL_STATE":
                       return $q.reject(response.data.error);

                   case safeGet(commonConstants.ALL_ERROR_VALUES.INTERNAL_SYSTEM_ERROR):
                       dialogService.showDialog(DIALOGS.connectionError, response.data[safeGet(commonConstants.ALL_PARAMETERS.ERROR_UUID)]);
                       return $q.reject("Internal system error! Error ID:" + response.data[safeGet(commonConstants.ALL_PARAMETERS.ERROR_UUID)]);

                   default:
                       console.log("Default called in sendRequest response handling part. For error " + response.data.error);
                         return $q.reject(response.data);
               }
           }, function (error) {
               console.error('Api call failed!'+ "Error:"+ JSON.stringify(error));
               return $q.reject("Request failed!"  + error);
           });
       }
      
   }
]);