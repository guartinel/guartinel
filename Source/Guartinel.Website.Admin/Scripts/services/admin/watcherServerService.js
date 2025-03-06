'use strict';
app.service('watcherServerService', [
      '$q', 'apiService', function ($q, apiService) {

          this.existing = function () {
              return existingRequest().then(function (response) {
                 console.log("Existing watcher servers arrived!");
                 return $q.resolve(response.servers);
              }, function (response) {
                  console.error("Getting existing watcher servers failed! Error: " + response.error);
                  return $q.reject(response.error);
              });
          }

          this.events = function (watcherServerId) {
              return eventsRequest(watcherServerId).then(function (response) {
                  console.log("Events of wachter server: " + watcherServerId);
                  return $q.resolve(response);
              }, function (response) {
                  console.error("Getting events for watcher server: " + watcherServerId + " failed! Err: " + response.error);
                  return $q.reject(response.error);
              });
          }

          this.getStatus = function (watcherServerId) {
              return getStatusRequest(watcherServerId).then(function (response) {
                  return $q.resolve(response);
              }, function (response) {
                  console.error("Getting status for watcher server: " + watcherServerId + " failed!Err:" + response.error);
                  return $q.reject(response.error);
              });
          }

          this.register = function (name, address, port, userName, password, categories) {
              password = CryptoJS.SHA512(password + userName).toString().toUpperCase();
              return registerRequest(name, address, port, userName, password, categories).then(function (response) {
                  console.log("Watcher server created: " + name);
                  return $q.resolve(response);
              }, function (response) {
                  console.error("Creating watcher server: " + name + " failed! Err:" + response.error);
                  return $q.reject(response.error);
              });
          }

          this.update = function (watcherServerId, name, address, port, categories) {
              return updateRequest(watcherServerId, name, address, port, categories).then(function (response) {
                  console.log("Updated watcher server: " + watcherServerId);
                  return $q.resolve(response);
              }, function (response) {
                  console.error("Updating watcher server: " + watcherServerId + " failed!Err:" + response.error);
                  return $q.reject(response.error);
              });
          }

          this.remove = function (watcherServerId) {
              return removeRequest(watcherServerId).then(function (response) {
                  console.log("Removed watcher server: " + watcherServerId);
                  return $q.resolve(response);
              }, function (response) {
                  console.error("Removing watcher server: " + watcherServerId + " failed!Err:" + response.error);
                  return $q.reject(response.error);
              });
          }

          var existingRequest = function () {
              var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_WATCHER_SERVER_GET_EXISTING);

              return apiService.sendAdminRequest(url);
          }

          var eventsRequest = function (watcherServerId) {
              var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_WATCHER_SERVER_GET_EVENTS);
              var data = {};
              data[safeGet(commonConstants.ALL_PARAMETERS.WATCHER_SERVER_ID)] = watcherServerId;

              return apiService.sendAdminRequest(url, data);
          }

          var getStatusRequest = function (watcherServerId) {
              var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_WATCHER_SERVER_GET_STATUS);
              var data = { watcher_server_id: watcherServerId }
              return apiService.sendAdminRequest(url, data);
          }

          var registerRequest = function (name, address, port, userName, password, categories) {
              var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_WATCHER_SERVER_REGISTER);
              password = CryptoJS.SHA512(password + userName).toString().toUpperCase();
              var data = {};
              data[safeGet(commonConstants.ALL_PARAMETERS.NAME)] = name;
              data[safeGet(commonConstants.ALL_PARAMETERS.ADDRESS)] = address;
              data[safeGet(commonConstants.ALL_PARAMETERS.PORT)] = port;
              data[safeGet(commonConstants.ALL_PARAMETERS.USER_NAME)] = userName;
              data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = password;
              data[safeGet(commonConstants.ALL_PARAMETERS.CATEGORIES)] = categories;
              return apiService.sendAdminRequest(url, data);
          }

          var updateRequest = function (watcherServerId, name, address, port, categories) { // TODO add userName and password?
              var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_WATCHER_SERVER_UPDATE);

              var data = {};
              data[safeGet(commonConstants.ALL_PARAMETERS.NAME)] = name;
              data[safeGet(commonConstants.ALL_PARAMETERS.ADDRESS)] = address;
              data[safeGet(commonConstants.ALL_PARAMETERS.PORT)] = port;
              data[safeGet(commonConstants.ALL_PARAMETERS.WATCHER_SERVER_ID)] = watcherServerId;
              data[safeGet(commonConstants.ALL_PARAMETERS.CATEGORIES)] = categories;

              return apiService.sendAdminRequest(url, data);
          }

          var removeRequest = function (watcherServerId) {
              var url = safeGet(backendAdminApiUrls.MANAGEMENT_SERVER_WATCHER_SERVER_REMOVE);

              var data = {};
              data[safeGet(commonConstants.ALL_PARAMETERS.WATCHER_SERVER_ID)] = watcherServerId;
              return apiService.sendAdminRequest(url, data);
          }
      }
]);