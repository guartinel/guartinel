app.directive('property', [ '$window', function () {

   return {
      restrict: 'E',
      scope: {
         current: "=current"
      },
      templateUrl: 'templates/dashboard/directives/property.html',
      link: function(scope, element, attrs) {
         
         scope.name = attrs.name;
         scope.color = '69CA69';

         scope.$watch('current', function (newValue) {

            if (newValue > 75 && newValue < 90)
               scope.color = 'f1cb00';

            if (newValue > 90)
               scope.color = 'ee2200';
         });
      }
   }
}]);