'use strict';
app.service('adminService', [
    '$q', 'apiService', 'dataService', function ($q, apiService, dataService) {

        this.login = function (userName, password) {
            password = CryptoJS.SHA512(password + userName).toString().toUpperCase();

            var url = safeGet(backendAdminApiUrls.ADMINISTRATOR_LOGIN);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)] = userName;
            data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = password;

            return apiService.sendRequest(url, data).then(function (response) {
                dataService.storeAdminToken(response.token);
                return $q.resolve(response);
            }, function (error) {
               console.error("Admin login failed! Error: " + error.error);
                return $q.reject(error.error);
            });
        }

        this.info = function () {
            var url = safeGet(backendAdminApiUrls.ADMINISTRATOR_GET_STATUS);

            return apiService.sendAdminRequest(url).then(function (response) {
                return $q.resolve(response);
            }, function (response) {
                console.error("Admin info failed! Error: " + response.error);
                return $q.reject(response.error);
            });
        }

        this.update = function (userName, password) {
            password = CryptoJS.SHA512(password + userName).toString().toUpperCase();
          
            var url = safeGet(backendAdminApiUrls.ADMINISTRATOR_UPDATE);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)] = userName;
            data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = password;
            

            return apiService.sendAdminRequest(url, data).then(function (response) {
                return $q.resolve(response);
            }, function (response) {
                console.error("Admin update failed! Error: " + response.error);
                return $q.reject(response.error);
            });
        }

        this.logout = function () {
            var url = safeGet(backendAdminApiUrls.ADMINISTRATOR_LOGOUT);
            return apiService.sendAdminRequest(url).then(function (response) {
                dataService.clearAdmin();
                return $q.resolve(response);
            }, function (response) {
                console.error("Admin logout failed! Error: " + response.error);
                return $q.reject(response.error);
            });
        }
 
    }
]);
