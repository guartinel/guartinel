app.filter('notInArray', function ($filter) {
   return function (list, arrayFilter, element) {
      if (arrayFilter) {
         return $filter("filter")(list, function (listItem) {
            for (var i = 0; i < arrayFilter.length; i++) {
               if (arrayFilter[i] == listItem[element])
                  return false;
            }
            return true;
         });
      }
   };
});