'use strict';
app.service('toastService', [
   '$mdToast', function ($mdToast) {

        this.showToast = function(text, buttonText, actionIfButtonClicked) {
            var toast = $mdToast.simple()
                .content(text)
                .action(buttonText)
                .hideDelay(0)
                .highlightAction(false);
            $mdToast.show(toast).then(function(response) {
                if (response == 'ok') {
                    actionIfButtonClicked();
                }
            });
       };

        this.showCookieToast = function() {
            $mdToast.show({
                hideDelay: 30000,
                position: 'bottom left',
                controller: 'cookieToastController',
                templateUrl: 'templates/homepage/cookieToast.html'
            });
       };
    }
]);