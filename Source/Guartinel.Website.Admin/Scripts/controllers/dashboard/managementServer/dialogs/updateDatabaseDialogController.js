app.controller(DIALOGS.database.update.controller, [
   '$scope', '$controller', '$q', 'databaseService', 'data', function ($scope, $controller, $q, databaseService, data) {

       angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

       $scope.database = Object.create(data);

       $scope.updateDatabase = function (url, userName, password) {
           var data = {};
           data.url = url;
           data.userName = userName;
           data.password = password;
           return databaseService.update(data).then(function () {
               $scope.answer();
               return $q.resolve();
           }, function (error) {
               return $q.reject(error);
           });
       }

   }
]);