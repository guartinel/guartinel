app.controller('baseDialogController', [
   '$mdDialog', '$scope', function($mdDialog, $scope) {

      // functions required by every dialog controller
      $scope.hide = function() {
         $mdDialog.hide();
      };
      $scope.cancel = function() {
         $mdDialog.cancel();
      };
      $scope.answer = function(answer) {
         $mdDialog.hide(answer);
      };

   }
]);