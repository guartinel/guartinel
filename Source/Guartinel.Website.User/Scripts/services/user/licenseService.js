'use strict';
app.service('licenseService', [
   'apiService', '$q','accountService', function (apiService, $q,accountService) {
       
       this.startBuyingLicense = function (license) {
           var url = safeGet(backendUserApiUrls.LICENSE_START_BUYING_LICENSE);
           var data = {};
           data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
           data[safeGet(commonConstants.ALL_PARAMETERS.LICENSE)] = license;
           data["account"] = accountService.currentUser;
           return apiService.sendRequest(url, data).then(function (response) {
              var redirectURL = response[safeGet(commonConstants.ALL_PARAMETERS.REDIRECT_URL)];
              if (isNull(redirectURL)) {
                 return $q.reject(response.error);
              }
              return $q.resolve(redirectURL);
           }, function (error) {
               console.error("startBuyingLicense failed! Error: " + error);
               return $q.reject(error);
           });
       }

       this.finalizeBuyingLicense = function (payerID,token) {
           var url = safeGet(backendUserApiUrls.LICENSE_FINALIZE_BUYING_LICENSE);
           var data = {};
           data[safeGet(commonConstants.ALL_PARAMETERS.PAYER_ID)] = payerID;
           data[safeGet(commonConstants.ALL_PARAMETERS.PAYPAL_TOKEN)] = token;
           data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
           return apiService.sendRequest(url, data).then(function (response) {
               return $q.resolve(response);
           }, function (error) {
               console.error("finalizeBuyingLicense failed ! Error: " + error);
               return $q.reject(error);
           });
       }

       this.getAvailable = function () {
           var url = safeGet(backendUserApiUrls.LICENSE_GET_AVAILABLE);
           var data = {};
           data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
           return apiService.sendRequest(url, data).then(function (response) {
               var licenses = response[safeGet(commonConstants.ALL_PARAMETERS.LICENSES)];
               return $q.resolve(licenses);
           }, function (error) {
               console.error("Getting existing licenses failed! Error: " + error);
               return $q.reject(error);
           });
       }

       this.activateLicense = function (license) {
          var url = safeGet(backendUserApiUrls.LICENSE_ACTIVATE_LICENSE);
          var data = {};
          data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = accountService.currentUser.token;
          data[safeGet(commonConstants.ALL_PARAMETERS.LICENSE_ID)] = license.id;

          return apiService.sendRequest(url, data).then(function (response) {
            return $q.resolve(response);
          }, function (error) {
             console.error("Activating license failed! Error: " + error);
             return $q.reject(error);
          });
       }
   }
]);