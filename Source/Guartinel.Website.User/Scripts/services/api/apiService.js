'use strict';
app.service('apiService', [
   '$http', '$q', 'dialogService', '$state', function ($http, $q, dialogService, $state) {

      this.sendRequest = function (url, data) {
         LOG.debug("Sending request to backend url :" + url + " request " + JSON.stringify(data));
        // console.error("Sending request to backend url :" + url + " request " + JSON.stringify(data));
         return $http.post(url, data).then(function (response) {
           LOG.debug("Response from backend " + JSON.stringify(response));
            var successMessage = safeGet(commonConstants.ALL_SUCCESS_VALUES.SUCCESS);

            if (response.data[safeGet(commonConstants.ALL_PARAMETERS.SUCCESS)] === successMessage) {
               return $q.resolve(response.data);
            }

            var invalidTokenMessage = safeGet(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN);
            if (isNull(response.data.error)) {
               return $q.reject("There was an error in our system. Sorry for the inconvenience.");
            }
            switch (response.data.error) {
               case invalidTokenMessage:
                  if ($state.$current === STATES.homepage.login) {
                     return $q.reject("INVALID TOKEN");
                  }

                  var accountService = angular.element('*[ng-app]').injector().get("accountService");
                  console.error("The token is INVALID!");
                  if (accountService.isLoggedIn) {
                     dialogService.showErrorDialog("Session expired", "Please login again.");
                     accountService.isLoggedIn = false;
                  }
           
                  accountService.clearUser();
                  $state.go(STATES.homepage.login, { isInvalidToken: true });                  
                  return $q.reject("INVALID TOKEN");

               case "INVALID_MODEL_STATE":
                  return $q.reject(response.data.error);
               case 'MISSING_MANAGEMENT_SERVER':
                  console.error("Missing Management Server from User Website settings. Please connect this site to your Guartinel System from the Administration Website");
                  return $q.reject(safeGet(commonConstants.ALL_ERROR_VALUES.INTERNAL_SYSTEM_ERROR));

               case safeGet(commonConstants.ALL_ERROR_VALUES.INTERNAL_SYSTEM_ERROR):
                  dialogService.showDialog(DIALOGS.connectionError, response.data[safeGet(commonConstants.ALL_PARAMETERS.ERROR_UUID)]);
                  return $q.reject(response.data);//$q.reject("Internal system error! Error ID:" + response.data[safeGet(commonConstants.ALL_PARAMETERS.ERROR_UUID)]);

               case safeGet(commonConstants.ALL_ERROR_VALUES.LICENSE_ERROR):
                  dialogService.showDialog(DIALOGS.errorDialog, response.data[safeGet(commonConstants.ALL_PARAMETERS.ERROR_DETAILS)]);
                  return $q.reject(response.data);
               default:
                  console.log("Default called in sendRequest response handling part. For error" + JSON.stringify(response.data));
                  //return $q.reject("There was an error in our system. Sorry for the inconvenience.");
                  return $q.reject(response.data);
            }
         }, function (error) {
            console.error('Api call failed!' + "Error:" + JSON.stringify(error));
            //return $q.reject(error.data.message);
            return $q.reject("Request failed! Check your internet connection settings.");
         });
      }


   }
]);



/*
'use strict';
app.service('apiService', [
   '$http', '$q', 'dialogService' , function ($http, $q, dialogService) {

      this.sendRequest = function (url, data) {
           console.error("Sending request to backend url :" + url + " request " + JSON.stringify(data));
           return $http.post(url, data).then(function (response) {
               console.log("Response from backend " + JSON.stringify(response));
               var successMessage = safeGet(commonConstants.ALL_SUCCESS_VALUES.SUCCESS);

               if (response.data[safeGet(commonConstants.ALL_PARAMETERS.SUCCESS)] === successMessage) {
                   return $q.resolve(response.data);
               }



               var invalidTokenMessage = safeGet(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN);

               switch (response.data.error) {
                   case invalidTokenMessage:
                       console.error("The token is INVALID!");
                       accountService.clearUser();
                       return $q.reject("INVALID TOKEN");

                   case "INVALID_MODEL_STATE":
                       return $q.reject(response.data.error);

                   case safeGet(commonConstants.ALL_ERROR_VALUES.INTERNAL_SYSTEM_ERROR):
                       dialogService.showDialog(DIALOGS.connectionError, response.data[safeGet(commonConstants.ALL_PARAMETERS.ERROR_UUID)]);
                       return $q.reject("Internal system error! Error ID:"+ response.data[safeGet(commonConstants.ALL_PARAMETERS.ERROR_UUID)]);

                   default:
                       console.log("Default called in sendRequest response handling part. For error" + response.data.error);
                       return $q.reject(response.data.error);
               }

           }, function (error) {
               console.error('Api call failed!' + "Error:" + JSON.stringify(error));
               //return $q.reject(error.data.message);
               return $q.reject("Request failed!");
           });
       }

   }
]);
*/