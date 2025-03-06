app.directive('dialogcontent', [ function () {

      return {
         restrict: 'E',
         replace: true,
         template: '<ng-include src="template"/>',
         link: function (scope, element, attrs) {
            scope.template = attrs.template;
         },
      };
   }
]);