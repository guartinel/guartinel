'use strict';
app.controller(STATES.adminpage.dashboard.controller, [
   '$rootScope', '$scope', 'managementServerService', 'watcherServerService', 'userWebServerService', '$q', 'adminWebServerService', 'managementServerStatusService', 'databaseService',
      function ($rootScope, $scope, managementServerService, watcherServerService, userWebServerService, $q, adminWebServerService, managementServerStatusService, databaseService) {
         $scope.watcherServers = [];

         function init() {
            function loadAdminWebServerStatus() {
               if ($scope.adminWebServerStatus == null) {
                  $scope.adminWebServerStatus = new Object();
               }
               $scope.adminWebServerStatus.isLoading = true;
               adminWebServerService.getStatus().then(function (response) {
                  $scope.adminWebServerStatus.cpu = parseInt(response.cpu);
                  $scope.adminWebServerStatus.memory = parseInt(response.memory);
                  $scope.adminWebServerStatus.storage = parseInt(response.storage);
                  $scope.adminWebServerStatus.isLoading = false;
                  $scope.adminWebServerStatus.isPresent = true;
                  $scope.adminWebServerStatus.isError = true;
               }, function (error) {
                  errorLog("Cannot get Admin webserver information.", error);
                  $scope.adminWebServerStatus.isPresent = false;
                  $scope.adminWebServerStatus.isLoading = false;
                  $scope.adminWebServerStatus.isError = true;
                  return $q.defer().promise;
               });
            }
            function loadUserWebServerStatus() {
               if ($scope.userWebServerStatus == null) {
                  $scope.userWebServerStatus = new Object();
               }
               $scope.userWebServerStatus.isLoading = true;
               userWebServerService.getAvailable().then(function (response) {
               if (response == null || response.address== null) {
                  $scope.userWebServerStatus.isPresent = false;
                  $scope.userWebServerStatus.isLoading = false;
                  return;
               }
                  $scope.userWebServerStatus.name = response.name;
                  $scope.userWebServerStatus.address = response.address;

                  userWebServerService.getStatus().then(function (response) {
                     $scope.userWebServerStatus.cpu = parseInt(response.cpu);
                     $scope.userWebServerStatus.memory = parseInt(response.memory);
                     $scope.userWebServerStatus.storage = parseInt(response.storage);
                     $scope.userWebServerStatus.isLoading = false;
                     $scope.userWebServerStatus.isPresent = true;
                  }, function (error) {
                     $scope.userWebServerStatus.isLoading = false;
                     $scope.userWebServerStatus.isPresent = false;
                     $scope.userWebServerStatus.isError = true;
                     errorLog("Cannot get user webserver information.", error);
                     return $q.defer().promise;
                  });
               }, function (error) {
                  $scope.userWebServerStatus.isLoading = false;
                  $scope.userWebServerStatus.isPresent = false;
                  $scope.userWebServerStatus.isError = true;
                  errorLog("Cannot get user webserver information.", error);
                  return $q.defer().promise;
               });
            }

            function loadDatabaseStatus() {
               databaseService.getStatus().then(function (databaseStatus) {
                  $scope.managementServerStatus.database = databaseStatus;
                  $scope.managementServerStatus.isLoading = false;
                  loadWatcherServers();
               }, function (error) {
                  errorLog("Cannot get MS DB status: ", error);

                  $scope.managementServerStatus.database.isConnected = false;
                  $scope.managementServerStatus.isLoading = false;
               });
            }
            function loadManagementServerStatus() {
               managementServerStatusService.getStatus().then(function (managementServerStatus) {
                  $scope.managementServerStatus.status = managementServerStatus;
                  $scope.managementServerStatus.database = {};
                  loadDatabaseStatus();
               }, function (error) {
                  $scope.managementServerStatus.isLoading = false;
                  $scope.managementServerStatus.isError = true;
               });
            }

            function loadManagementServer() {
               if ($scope.managementServerStatus == null) {
                  $scope.managementServerStatus = new Object();
               }
               $scope.managementServerStatus.isLoading = true;
               managementServerService.getAvailable().then(function (managementServerProperties) {
                  if (managementServerProperties == null) {
                     $scope.managementServerStatus.isPresent = false;
                     $scope.managementServerStatus.isLoading = false;
                     return;
                  }
                  $scope.managementServerStatus.properties = managementServerProperties;

                  $scope.managementServerStatus.isPresent = true;
                  loadManagementServerStatus();
               }, function (error) {
                  $scope.managementServerStatus.isLoading = false;
                  $scope.managementServerStatus.isPresent = false;
                  $scope.managementServerStatus.isError = true;
                  errorLog("Cannot get  management server information.", error);
                  return $q.defer().promise;
               });
            }

            function loadWatcherServers() {

               if (!$scope.managementServerStatus.isPresent) {
                  return;
               }
               watcherServerService.existing().then(function (watcherServers) {
                   for (var i = 0 ; i < watcherServers.length; i++) {
                     $scope.watcherServers.push(watcherServers[i]);
                  }
               }, function (err) {
                  console.log(err);
                  return $rootScope.refreshUserWebServer();
               });
            }

            loadAdminWebServerStatus();
            loadUserWebServerStatus();
            loadManagementServer();
         }

         init();
         $scope.init = init;
         /*
         $scope.dashboardCards = [];

         function removeSpecificTypeCards(type) {
            for (var i = 0; i < $scope.dashboardCards.length ; i++) {
               if ($scope.dashboardCards[i].type === type) {
                  $scope.dashboardCards.splice(i, 1);
               }
            }
         }

        /* $rootScope.refreshWebServer = function () {
            removeSpecificTypeCards(safeGet(CARD_TYPES.WEB_SERVER_CARD));
            $scope.webServerCards.push({ type: safeGet(CARD_TYPES.WEB_SERVER_CARD) });
            return $q.defer().promise;
         }

         $rootScope.refreshManagementServer = function () {
            removeSpecificTypeCards(safeGet(CARD_TYPES.ADD_MANAGEMENT_SERVER_CARD));
            removeSpecificTypeCards(safeGet(CARD_TYPES.MANAGEMENT_SERVER_CARD));

            managementServerService.getAvailable().then(function (managementServer) {
               if (managementServer === null) {
                  $scope.isManagementServerPresent = false;
                  $scope.dashboardCards.push({ type: safeGet(CARD_TYPES.ADD_MANAGEMENT_SERVER_CARD) });
                  return $q.defer().promise;
               }
               $scope.isManagementServerPresent = true;
               $scope.dashboardCards.push({
                  data: managementServer,
                  type: safeGet(CARD_TYPES.MANAGEMENT_SERVER_CARD)
               });
               return $rootScope.refreshWatcherServers();
            });
         }
         $rootScope.refreshUserWebServer = function () {
            if (!$scope.isManagementServerPresent) {
               return $q.defer().promise;
            }
            removeSpecificTypeCards(safeGet(CARD_TYPES.ADD_USER_WEB_SERVER_CARD));
            removeSpecificTypeCards(safeGet(CARD_TYPES.USER_WEB_SERVER_CARD));

            userWebServerService.getAvailable().then(function (userWebServer) {
               if (userWebServer == null) {
                  $scope.dashboardCards.push({ type: safeGet(CARD_TYPES.ADD_USER_WEB_SERVER_CARD) });
                  return $q.defer().promise;
               }
               $scope.isUserWebServerPresent = true;
               $scope.dashboardCards.push({
                  data: userWebServer,
                  type: safeGet(CARD_TYPES.USER_WEB_SERVER_CARD)
               });
               return $q.defer().promise;
            }, function (err) {
               $scope.dashboardCards.push({ type: safeGet(CARD_TYPES.ADD_USER_WEB_SERVER_CARD) });
               return $q.defer().promise;
            });
         }

         $rootScope.refreshWatcherServers = function () {
            if (!$scope.isManagementServerPresent) {
               return $rootScope.refreshUserWebServer();
            }

            removeSpecificTypeCards(safeGet(CARD_TYPES.WATCHER_SERVER_CARD));
            removeSpecificTypeCards(safeGet(CARD_TYPES.ADD_WATCHER_SERVER_CARD));

            watcherServerService.existing().then(function (watcherServersResponse) {
               if (watcherServersResponse.userWebServerStatus != null) {
                  for (var i = 0; i < watcherServersResponse.servers.length; i++) {
                     $scope.dashboardCards.push({ data: watcherServersResponse.servers[i], type: safeGet(CARD_TYPES.WATCHER_SERVER_CARD) });
                  }
                  $scope.dashboardCards.push({ type: safeGet(CARD_TYPES.ADD_WATCHER_SERVER_CARD) });
               }
               return $rootScope.refreshUserWebServer();
            }, function (err) {
               console.log(err);
               return $rootScope.refreshUserWebServer();
            });
         }

         /*$rootScope.refreshDashboard = function () {
            // $rootScope.refreshWebServer().then($rootScope.refreshManagementServer()).then($rootScope.refreshUserWebServer()).then($rootScope.refreshWatcherServers());
            $rootScope.refreshWebServer();
            $rootScope.refreshManagementServer();
         }

         $scope.refreshDashboard();*/
      }
]);