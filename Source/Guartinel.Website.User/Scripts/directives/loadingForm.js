app.directive('loadingForm', ['$compile', function ($compile) {

   return {
      restrict: 'A',
      scope: {
         submitMethod: '&'
      },
      transclude: false,
      link: function (scope, element, attrs) {

         var loadingElement = '<div class="loading_element_container">' +
            '<md-progress-circular class="loading_element" md-mode="indeterminate"></md-progress-circular></div>';
         var newElement = $compile(loadingElement)(scope);

         //save current element
         var formActionsElement = element.find('.form_actions');
         var oldElement = formActionsElement[0];


         // on submit, replace element with progress loading element
         // revert back, when async method returns with result

         element.bind('submit', function () {

            formActionsElement.replaceWith(newElement);
            element.find('.loading_form_error_text').remove();
            element.find('.loading_form_response_text').remove();

            scope.submitMethod().then(function (response) {
               if (isNull(response)) {
                  response = "Success";
               }
               element.find('.loading_element_container').replaceWith(oldElement);
               element.append("<div class='loading_form_response_text'>" + response + "</div>");
            }, function (error) {
               if (isNull(error)) {
                  element.find('.loading_element_container').replaceWith(oldElement);
                  return;
               }
               if (!isNull(error.error)) {
                  error = error.error;
               }
               element.find('.loading_element_container').replaceWith(oldElement);
               element.append("<div class='loading_form_error_text'>" + error + "</div>");
            });

         });

      },
   };
}
]);