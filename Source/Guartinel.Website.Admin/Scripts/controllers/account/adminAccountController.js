'use strict';
app.controller(STATES.adminpage.adminAccount.controller, ['$rootScope','$scope', 'adminService', 'dataService','$q', function ($rootScope,$scope, adminService, dataService,$q) {
    $rootScope.screen = "Administrator Account";
    $scope.update = function (userName, currentPassword,newPassword) {
        // These fields are optional, sending them is necessary only if they are changed

        var newUserName = userName === dataService.currentAdmin().userName ? undefined : userName;
       
        return adminService.update(newUserName,currentPassword,newPassword).then(function () {
            $scope.getAdminInfo();
            $scope.message = "Administrator account information updated successfully";
            $scope.password = '';
            $scope.confirmPassword = '';
            $scope.changed = false;
            $scope.adminAccountForm.$dirty = false;
        },
           function (error) {
               return $q.reject(error);
           });
    }
    $scope.getAdminInfo = function () {
        adminService.info().then(function () {
            $scope.currentAdmin = Object.create(dataService.currentAdmin());
        },
           function (error) {
               $scope.error = error;
           });
    }
    $scope.getAdminInfo();
}]);

/*
'use strict';
app.controller(STATES.userpage.account.controller, [
   '$rootScope', 'accountService', '$scope', 'dataService', '$q', function ($rootScope, accountService, $scope, dataService, $q) {

      
]);*/