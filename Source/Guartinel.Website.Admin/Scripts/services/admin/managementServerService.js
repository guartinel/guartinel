'use strict';
app.service('managementServerService', ['$q', 'apiService', function ($q, apiService) {

    this.add = function (data) {
        data.password = CryptoJS.SHA512(data.password + data.userName).toString().toUpperCase();
        var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_ADD);
        var parameters = {};
        parameters[safeGet(commonConstants.ALL_PARAMETERS.NAME)] = data.name;
        parameters[safeGet(commonConstants.ALL_PARAMETERS.ADDRESS)] = data.address;
        parameters[safeGet(commonConstants.ALL_PARAMETERS.PORT)] = data.port;
        parameters[safeGet(commonConstants.ALL_PARAMETERS.DESCRIPTION)] = data.description;
        parameters[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)] = data.userName;
        parameters[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = data.password;
        parameters[safeGet(commonConstants.ALL_PARAMETERS.EMAIL_PROVIDER)] = data.emailProvider;
        parameters[safeGet(commonConstants.ALL_PARAMETERS.EMAIL_USER_NAME)] = data.emailUserName;
        parameters[safeGet(commonConstants.ALL_PARAMETERS.EMAIL_PASSWORD)] = data.emailPassword;

        return apiService.sendAdminRequest(url, parameters).then(function (response) {
            console.log("Management server added");
            return $q.resolve(response);
        }, function (response) {
            console.error("Adding management server failed! Error: " + response.error);
            return $q.reject(response.error);
        });
    }

    this.getAvailable = function () {
        var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_GET_EXISTING);

        return apiService.sendAdminRequest(url).then(function (response) {
           var server = safeGet(response[safeGet(commonConstants.ALL_PARAMETERS.MANAGEMENT_SERVER)]);
           var parsed = {
              name: server.name,
              port: server.port,
              address: server.address,
              description: server.description
           }
           return $q.resolve(parsed);
        }, function (response) {
            if (response.error === safeGet(commonConstants.ALL_ERROR_VALUES.MISSING_MANAGEMENT_SERVER)) {
                return $q.resolve(null);
            }
            console.error("Getting existing management servers failed! Error: " + response.error);
            return $q.reject(response.error);
        });
    }

    this.update = function (id, name, address, port, description, userName, password) {
        var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_UPDATE);
        password = CryptoJS.SHA512(password + userName).toString().toUpperCase();
        var data = {};
        data[safeGet(commonConstants.ALL_PARAMETERS.ID)] = id;
        data[safeGet(commonConstants.ALL_PARAMETERS.NAME)] = name;
        data[safeGet(commonConstants.ALL_PARAMETERS.ADDRESS)] = address;
        data[safeGet(commonConstants.ALL_PARAMETERS.PORT)] = port;
        data[safeGet(commonConstants.ALL_PARAMETERS.DESCRIPTION)] = description;
        data[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)] = userName;
        data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = password;

        return apiService.sendAdminRequest(url, data).then(function (response) {
            console.log("Management server updated: " + id);
            return $q.resolve(response);
        }, function (response) {
            console.error("Updating management server failed! Error: " + response.error);
            return $q.reject(response.error);
        });
    }

    this.remove = function (id) {
        var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_REMOVE);

        var data = {};
        data[safeGet(commonConstants.ALL_PARAMETERS.ID)] = id;

        return apiService.sendAdminRequest(url, data).then(function (response) {
            console.log("Management server deleted: " + id);
            return $q.resolve(response);
        }, function (response) {
            console.error("Removing management server failed! Error: " + response.error);
            return $q.reject(response.error);
        });
    }
}
]);
