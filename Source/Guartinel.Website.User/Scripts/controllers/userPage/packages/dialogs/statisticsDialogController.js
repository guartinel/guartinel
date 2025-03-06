app.controller(DIALOGS.packages.statistics.controller, [
   '$scope', '$controller', 'data', '$q', 'packageService', '$window',
   function ($scope, $controller, actualPackage, $q, packageService, $window) {
      angular.extend(this, $controller(DIALOGS.base.controller, { $scope: $scope }));
      $scope.statistics = "Not yet";
      $scope.statisticsCSV = "";
      $scope.isLoading = true;

      function getStatistics() {
         return packageService.getStatistics(actualPackage.id).then(function (statistics) {
            $scope.statistics = statistics;
            if ($scope.statistics == null || $scope.statistics.length == 0) {
               $scope.notFound = true;
               $scope.isLoading = false;
               return;
            }
            $scope.statisticsCSV = convertToCSV($scope.statistics);
           $scope.isLoading = false;
         }, function (error) {
            //  return $q.reject(error);
         });
      }

      getStatistics();

      function convertToCSV(objArray) {
         var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
         var result = 'sep=;\r\n';
         var separator = ';';
         var newLine = '\r\n';
         if (isNull(array) || array.length === 0) {
            return result;
         }
         //add the header
         Object.keys(array[0]).forEach(function (key, index) {
            result += key + separator;
         });


         for (var i = 0; i < array.length; i++) {
            result += newLine;
            var currentObject = array[i];
            var currentObjectValues = Object.values(currentObject);

            for (var valueIndex = 0; valueIndex < currentObjectValues.length; valueIndex++) {
               result += currentObjectValues[valueIndex] + separator;
            }
         }
         return result;
      }

      $scope.saveToPc = function () {
         var data = $scope.statisticsCSV;
         if (!data) {
            console.error('No data');
            return;
         }

         filename = actualPackage.packageName +'-statistics.csv';

         if (typeof data === 'object') {
            data = JSON.stringify(data, undefined, 2);
         }
         var blob = new Blob([data], { type: 'text/json' });

         // FOR IE:
         if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(blob, filename);
         }
         else {
            var e = document.createEvent('MouseEvents'),
                a = document.createElement('a');

            a.download = filename;
            a.href = window.URL.createObjectURL(blob);
            a.dataset.downloadurl = ['text/json', a.download, a.href].join(':');
            e.initEvent('click', true, false, window,
                0, 0, 0, 0, 0, false, false, false, false, 0, null);
            a.dispatchEvent(e);
         }
      };    

   }
]);
