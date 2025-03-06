'use strict';
app.service('dialogService', [
   '$mdDialog', '$q', '$rootScope', function ($mdDialog, $q, $rootScope) {
      var self = this;
      self.isOpen = function () {
         if (angular.element(document).find('md-dialog').length > 0) { true; }
         return false;
      }
      this.showDialog = function (dialog, data) {
         if (self.isOpen()) { return; }
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
               data: data
             },
            fullscreen:true
         })
            .then(function (answer) {
               deferred.resolve(answer);
            }, function () {
               deferred.reject();
            });

         return deferred.promise;
      }

      this.showConfirmDialog = function (ev, title, text, actionIfConfirmed, actionCancelled) {
         if (self.isOpen()) { return; }

         // Appending dialog to document.body to cover sidenav in docs app
         var confirm = $mdDialog.confirm()
            .title(title)
            .textContent(text)
            .ariaLabel('Lucky day')
            //  .targetEvent(ev)
            .ok('Confirm')
            .cancel('Cancel');

         $mdDialog.show(confirm).then(actionIfConfirmed, actionCancelled);
      };

      this.showErrorDialog = function (title, errorText) {
         if (self.isOpen()) { return; }

         $mdDialog.show(
            $mdDialog.alert()
               .parent(angular.element(document.querySelector('#guartinelDialogContainer')))
               .clickOutsideToClose(true)
               .title(title)               
               .textContent(errorText)
               .ariaLabel(title)
               .ok('OK'));
      };
   }
]);