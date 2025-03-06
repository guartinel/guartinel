'use strict';
app.controller('cookieToastController', [
    '$mdToast', '$scope', '$window','$cookieStore' ,
    function ($mdToast, $scope, $window, $cookieStore) {

        $scope.accept = function () {
            $mdToast
                .hide()
                .then(function () {
                    $cookieStore.put('cookiesAccepted', "YES");
                });
        };
       
        $scope.openMoreInfo = function (e) {
            $window.open('https://manage.guartinel.com:8888/GuartinelGDPR.pdf', '_blank');
        };
    }
]);