'use strict';
app.controller('accessTabController', [
   '$scope', 'accountService', function ($scope, accountService) {
      $scope.currentUserEmail = accountService.currentUser.email;
      $scope.nope = function () { }
      $scope.newAccessItem = {};

      function initController() {
         if (!isNull($scope.package.access)) {
            for (var accessIndex = 0; accessIndex < $scope.package.access.length; accessIndex++) {
               if ($scope.package.access[accessIndex].packageUserEmail == $scope.currentUserEmail) {
                  $scope.package.access[accessIndex].shouldHide = true;
               }
            }

         }
      }

      $scope.onAccessRuleChanged = function (accessRule) {
         if (accessRule.canEdit || accessRule.canDisable || accessRule.canDelete) {
            accessRule.canRead = true;
         }
      }

      $scope.validateAccessEmail = function (emailForm) {
         if (emailForm.email.$viewValue == $scope.currentUserEmail) {
            emailForm.email.$setValidity('ownerEmail', false);
            return;
         } else {
            emailForm.email.$setValidity('ownerEmail', true);
         }

         if (emailForm.email.$viewValue.length == 0) {
            emailForm.email.$setValidity('required', false);
            return;
         } else {
            emailForm.email.$setValidity('required', true);
         }
      }

      initController();
      /*  $scope.shouldHideAccessItem = function(accessItem) {
           var isOwnerAccess = accessItem.packageUserEmail === $scope.package.owner;
           var isUserSelfAccess = accessItem.packageUserEmail === $scope.currentUserEmail;
           var isNewPackageCreation = $scope.package.owner == null;
  
           if (isNewPackageCreation) {
              return false;
           }
  
           if (isOwnerAccess || isUserSelfAccess) {
              return true;
           }
  
           return false;
        }*/

      $scope.checkEnteredEmail = function (email) {
         if (email === $scope.package.owner) {
            $scope.isOwnerEmailUsed = new Object();
         } else {
            $scope.isOwnerEmailUsed = null;

         }
      }

      $scope.addNewAccessRule = function (packageUserEmail, canRead, canEdit, canDisable, canDelete) {
         var newRule = {}
         newRule.packageUserEmail = packageUserEmail;
         newRule.canRead = canRead;
         newRule.canEdit = canEdit;
         newRule.canDisable = canDisable;
         newRule.canDelete = canDelete;
         $scope.package.access.push(newRule);
      }

      $scope.addNewEmptyRule = function () {
         var newRule = {}
         newRule.packageUserEmail = "";
         newRule.canRead = false;
         newRule.canEdit = false;
         newRule.canDisable = false;
         newRule.canDelete = false;
         $scope.package.access.push(newRule);
      }
   }
]);



