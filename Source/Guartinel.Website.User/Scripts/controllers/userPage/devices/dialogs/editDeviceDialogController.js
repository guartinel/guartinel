app.controller(DIALOGS.devices.editDevice.controller, [
   '$scope', '$controller', 'deviceService',"data",'$q',
   function ($scope, $controller, deviceService, data, $q) {
      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));

      $scope.allCategories = deviceService.getAllDeviceCategories();

      $scope.device = data;
      $scope.chipCategoriesForDevice = [];

      $scope.transformChip = function(chip) {
         // If it is an object, it's already a known chip
         if (angular.isObject(chip)) {
            return chip;
         }
         // Otherwise, create a new one
         return { name: chip, type: 'new' }
      }
      for (var i = 0; i < $scope.device.categories.length; i++) {
         $scope.chipCategoriesForDevice.push($scope.transformChip($scope.device.categories[i]));
      }

      $scope.submitEdit = function() {
         var resultCategories = [];
         for (var i = 0; i < $scope.chipCategoriesForDevice.length; i++) {
            resultCategories.push($scope.chipCategoriesForDevice[i].name);
         }
         deviceService.editDevice($scope.device.id, $scope.device.name, resultCategories).then(function() {
            deviceService.getAvailable();
            $scope.answer();
         }, function(err) {
            console.log("Cannot edit device:" + JSON.stringify(err));
            return $q.reject(error);
         });
      }


      $scope.querySearch = function(query) {
         if (query.length === 0 || query === '') {
            return $scope.allCategories;
         }
         var result = [];
         for (var i = 0; i < $scope.allCategories.length; i++) {
            if ($scope.allCategories[i].indexOf(query) != -1) {
               result.push($scope.allCategories[i]);
            }
         }
         return result;
      }

   }
]);