'use strict';
app.controller("basePackageCardController", [
   '$scope', 'accountService', 'packageService', 'toastService', 'pluginService','$window',
   function ($scope, accountService, packageService, toastService, pluginService, $window) {
      $scope.commonConstants = $window.commonConstants;

      function init() {
         $scope.packageDefinition = pluginService.getPackageDefinition($scope.package.packageType);
      }

      init();
      $scope.getLabelForScheduleIntervalSecond = function (seconds) {

      }

      $scope.getLocalTimeFromDate = function (utcTime) {
          return new Date(moment.utc(utcTime, "HH:mm"));
      }

      $scope.getPackageNameStyle = function () {//TODO eliminate
         if ($scope.package.packageName.length <= 9) {
            return 'md-headline white_text no_margin';
         }
         if ($scope.package.packageName.length > 9 && $scope.package.packageName.length < 25) {
            return 'md-headline-smaller white_text no_margin';
         }
         return 'md-headline-smaller white_text no_margin';
       }

      $scope.getPackageName = function () {
         if ($scope.package.packageName.length > 60) {
            return $scope.package.packageName.substring(0, 60) + "...";
         }
         return $scope.package.packageName;
      }
      $scope.shouldShowPackageNameToolTip = function () {
         if ($scope.package.packageName.length > 25) {
            return true;
         }
         return false;
      }

      $scope.getAlertIconStyle = function () {
         if ($scope.package.isEnabled) {
            return { fill: "red" }
         }
         return { fill: "gray" };
      }

      // PACKAGE PART
      $scope.getPackagePartIconStyle = function (identifier) {//eliminate
         var OK_STYLE = { "fill": "green" ,"margin-left":"8px","margin-right":"8px"};
          var UNKNOWN_STLYE = { "fill": "gray", "margin-left": "8px", "margin-right": "8px"};
          var ALERTED_STYLE = { "fill": "red", "margin-left": "8px", "margin-right": "8px"};


         if (!$scope.package.isEnabled) {
            return UNKNOWN_STLYE;
         }

         // if package is newer then state time stamp = > gray
         if ($scope.package.lastModificationTimestamp > $scope.package.state.timeStamp) {
            return UNKNOWN_STLYE;
         }
         //if overall status is unknown then package part is so
         /* if ($scope.package.state.name === safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_STATE_UNKNOWN)) {
             return UNKNOWN_STLYE;
          }*/
         if (!isNull($scope.package.state) && !isNull($scope.package.state.states)) {
            for (var index = 0; index < $scope.package.state.states.length; index++) {
               var stateItem = $scope.package.state.states[index];
               if (stateItem.package_part_identifier === identifier) {
                  if (stateItem.package_part_state === "ok") {
                     return OK_STYLE;
                  }
                  if (stateItem.package_part_state === "alerting") {
                     return ALERTED_STYLE;
                  }
                  return UNKNOWN_STLYE;
               };
            }
         }
         return UNKNOWN_STLYE;
      }

      $scope.getPackagePartStateMessage = function (identifier) {
         if ($scope.package.lastModificationTimestamp > $scope.package.state.timeStamp) {
            return "Check is running.";
         };
         if (!isNull($scope.package.state) && !isNull($scope.package.state.states)) {

            for (var index = 0; index < $scope.package.state.states.length; index++) {
               var stateItem = $scope.package.state.states[index];
               if (stateItem.package_part_identifier == identifier) {
                  return stateItem.package_part_message_built + " " +stateItem.package_part_details_built ;
               }
            }
         }
         return "Check is running.";
      }

      $scope.canEditPackage = function () {
         if (packageService.getPackageCount() > accountService.currentUser.licenseAggregate.maximumPackages) {
            toastService.showToast("You cannot have more than " + accountService.currentUser.licenseAggregate.maximumPackages + " active packages with your current licenses!", "OK");
            return false;
         }
         return true;
      }

       $scope.getPackagePartContainerStyle = function(partArray) {//eliminate
           var baseStyle = { "max-height": "155px", "overflow-x": "hidden", "background-color": "white" };

           if (partArray == null) {
               return baseStyle;
           }
           var partArrayLength = partArray.length;
           if (partArrayLength == 1) {
               baseStyle["min-height"] = "65px";
           }

           if (partArrayLength == 2) {
               baseStyle["min-height"] = "110px";
           }
           if (partArrayLength == 3) {
               baseStyle["min-height"] = "130px";
           }
           if (partArrayLength >= 4) {
               baseStyle["min-height"] = "150px";
           }
           return baseStyle;
       };

      //!! PACKAGE PART
       $scope.getNotCheckedIconStyle = function() {
           if ($scope.package.isEnabled) {
               return { fill: "gray" };
           }
           return { fill: "gray" };
       };

       $scope.getOkIconStyle = function() {
           if ($scope.package.isEnabled) {
               return { fill: "green" };
           }
           return { fill: "gray" };
       };

       $scope.isStateOK = function() {
           var isPropertiesNull = (isNull($scope.package.lastModificationTimestamp) ||
               isNull($scope.package.state.timeStamp));

           if (!isPropertiesNull && ($scope.package.lastModificationTimestamp > $scope.package.state.timeStamp)) {
               return false;
           }

           if ($scope.package.state.name == null) {
               return false;
           }
           return $scope.package.state.name == 'ok';
       };
       $scope.getAlertedStyle = function() {
           return { "background-color": "red" };
       };
       $scope.getDisabledStyle = function() {
           return { "background-color": "gray" };
       };

       $scope.isStateAlerting = function() {
           var isPropertiesNull = (isNull($scope.package.lastModificationTimestamp) ||
               isNull($scope.package.state.timeStamp));

           if (!isPropertiesNull && ($scope.package.lastModificationTimestamp > $scope.package.state.timeStamp)) {
               return false;
           }
           if ($scope.package.state == null) {
               return false;
           }
           return $scope.package.state.name == 'alerting';
       };

       $scope.isStateNotChecked = function() {
           if (isNull($scope.package.lastModificationTimestamp) || isNull($scope.package.state.timeStamp)) {
               return true;
           }
           if ($scope.package.lastModificationTimestamp > $scope.package.state.timeStamp) {
               return true;
           }
           return $scope.package.state.name == null || $scope.package.state.name == 'unknown';
       };

       $scope.getDeviceNameFromId = function(deviceId) {
           var result = "";
           $scope.package.devices.forEach(function(item, index) {
               if (item.id === deviceId) {
                   result = item.name;
               }
           });
           return result;
       };

       $scope.getDeviceOwnerFromId = function(deviceId) {
           var result = "";
           $scope.package.devices.forEach(function(item, index) {
               if (item.id === deviceId) {
                   result = item.owner;
               }
           });
           return result;
       };

       $scope.getDeviceTypeFromId = function(deviceId) {
           var result = "";
           $scope.package.devices.forEach(function(item, index) {
               if (item.id === deviceId) {
                   result = item.device_type;
               }
           });
           return result;
       };
   }
]);

