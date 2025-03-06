'use strict';
app.service('dialogService', [
   '$mdDialog', '$q', '$rootScope', function ($mdDialog, $q, $rootScope) {

      this.showDialog = function (dialog, data) {

         var newScope = $rootScope.$new();
         newScope.dialog = dialog;

         var deferred = $q.defer();

         // if the the dialog has no specific controller then the base controller must be used
         if (dialog.controller == undefined || dialog.controller == null)
            dialog.controller = DIALOGS.base.controller;

         $mdDialog.show({
            controller: dialog.controller,
            templateUrl: DIALOGS.base.template,
            scope: newScope,
            parent: angular.element(document.body),

            // passing data to controller
            locals: {
               data: data,
            },
         })
         .then(function (answer) {
            deferred.resolve(answer);
         }, function () {
            deferred.reject();
         });

         return deferred.promise;
      }

   }
]);