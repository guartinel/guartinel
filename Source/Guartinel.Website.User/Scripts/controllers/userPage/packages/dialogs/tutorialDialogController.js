app.controller(DIALOGS.packages.tutorialDialog.controller, [
   '$scope', '$controller', 'data', '$q',
   function ($scope, $controller, data, $q) {
      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
     
      var tutorialItems = data;
      var currentIndex = 0;
      var currentItem = tutorialItems[currentIndex];

      $scope.setNext = function () {
         if (currentIndex == tutorialItems.length-1) {
            return;
         }
         currentIndex++;
      }

      $scope.setPrevious = function () {
         if (currentIndex == 0) {
            return;
         }
         currentIndex--;
      }

      $scope.getStyle = function () {
         return { 'background-image': 'url('+tutorialItems[currentIndex]+')','height':'300px' };
      }
   }
]);
