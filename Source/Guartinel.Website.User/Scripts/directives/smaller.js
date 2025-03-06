'use strict';

app.directive('smaller', smaller);

function smaller($parse) {
   return {
      require: '?ngModel',
      restrict: 'A',
      link: function (scope, elem, attrs, ctrl) {
         if (!ctrl) {
            if (console && console.warn) {
               console.warn('Smaller validation requires ngModel to be on the element');
            }
            return;
         }

         var smallerGetter = $parse(attrs.smaller);
         var caselessGetter = $parse(attrs.smallerCaseless);

         scope.$watch(getsmallerValue, function () {
            ctrl.$$parseAndValidate();
         });

         ctrl.$validators.smaller = function () {
            var smaller = getsmallerValue();
            if (isNull(smaller) || isNull(ctrl.$viewValue)) {
               return true;
            }
            return ctrl.$viewValue < smaller;
         };

         function getsmallerValue() {
            var smaller = smallerGetter(scope);
            if (angular.isObject(smaller) && smaller.hasOwnProperty('$viewValue')) {
               smaller = smaller.$viewValue;
            }
            return smaller;
         }
      }
   };
}