'use strict';
app.controller(STATES.homepage.login.controller,
    [
        'accountService', '$scope', '$state', '$q', 'dialogService', '$location', '$mdDialog', 'toastService',
        '$cookieStore',
        function(accountService, $scope, $state, $q, dialogService, $location, $mdDialog, toastService, $cookieStore) {
            var forgotButtonTexts = {
                FORGOT: "Forgot your password?",
                ARE_YOU_SURE: "Click to request a new password to : ",
                SENDING: "Sending verification email...",
                SENT: "Verification email is sent to : ",
                TYPE_YOUR_EMAIL_IN: "Please type your email address in the field above. And click here again.",
                NOT_REGISTERED: "The email address is not registered. Click to try again."
            }

            function initController() {
                var email = $location.search().email;
                var activationCode = $location.search().activationCode;

                if (activationCode != null) {
                    $scope.forgotButtonText = "Activating account";
                    accountService.activate(email, activationCode).then(function() {
                            $scope.forgotButtonText = "Account activated";
                            toastService.showToast("Account is activated.", "OK", null);
                            $state.go(STATES.userpage.packages);
                        },
                        function(error) {
                            $scope.forgotButtonText = "Invalid activation code";
                        });
                    return;
                }

                var verificationCode = $location.search().verificationCode;
                if (verificationCode != null) {
                    $scope.forgotButtonText = "Sending the new password.";
                    accountService.verifySendNewPassword(email, verificationCode).then(function() {
                            $scope.forgotButtonText = "New password sent.";
                        },
                        function(error) {
                            $scope.forgotButtonText = "Invalid password renew verification code";
                            return $q.reject(error);
                        });
                    return;
                }
                $scope.forgotButtonText = forgotButtonTexts.FORGOT;
                if (isEmptyOrNull($cookieStore.get('cookiesAccepted'))) {
                    /* toastService.showCookieToast("This page is using cookies to provide you a better customer experience.", "OK", function () {
                        $cookieStore.put('cookiesAccepted', "YES");
                     });*/
                    toastService.showCookieToast();
                }
            }

            initController();
            $scope.login = function(email, password) {
                return accountService.login(email, password).then(function(isActivated) {
                        $state.go(STATES.userpage.packages);
                        return $q.resolve("Successfull login");
                    },
                    function(error) {

                        var invalidUserNameOrPasswordMessage =
                            safeGet(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD);
                        var accountExpiredMessage = safeGet(commonConstants.ALL_ERROR_VALUES.ACCOUNT_EXPIRED);

                        switch (error.error) {
                        case invalidUserNameOrPasswordMessage:
                            error = "Invalid user name or password!";
                            break;

                        case accountExpiredMessage:
                            error = "Your account is expired!";
                            dialogService.showDialog(DIALOGS.account.expired, $scope.email);
                            break;
                        }
                        $scope.password = '';
                        return $q.reject(error);
                    });
            };

            $scope.forgotYourPasswordClicked = function(email) {

                if ($scope.forgotButtonText.indexOf(forgotButtonTexts.SENDING) > -1) {
                    return;
                }

                if ($scope.email == null) {
                    $scope.forgotButtonText = forgotButtonTexts.TYPE_YOUR_EMAIL_IN;
                    return;
                }

                if ($scope.forgotButtonText == forgotButtonTexts.TYPE_YOUR_EMAIL_IN) {
                    $scope.forgotButtonText = forgotButtonTexts.ARE_YOU_SURE + email;
                    return;
                }

                if ($scope.forgotButtonText == forgotButtonTexts.FORGOT) {
                    $scope.forgotButtonText = forgotButtonTexts.ARE_YOU_SURE + email;
                    return;
                }
                if ($scope.forgotButtonText.indexOf(forgotButtonTexts.SENT) != -1) {
                    $scope.forgotButtonText = forgotButtonTexts.FORGOT;
                    return;
                }
                if ($scope.forgotButtonText.indexOf(forgotButtonTexts.ARE_YOU_SURE) > -1) {
                    $scope.forgotButtonText = forgotButtonTexts.SENDING;
                    $.getJSON("https://freegeoip.net/json/",
                        function(data) {
                            data.user_agent = navigator.userAgent;
                            return accountService.sendNewPassword(email, data).then(function() {
                                    $scope.forgotButtonText = forgotButtonTexts.SENT + email;
                                    return $q.resolve();
                                },
                                function(error) {
                                    var errorMessage = error.error;
                                    var oneHourNotElapsed = safeGet(commonConstants.ALL_ERROR_VALUES
                                        .ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND);
                                    var invalidUserNameOrPassword =
                                        safeGet(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD);

                                    if (errorMessage === oneHourNotElapsed) {
                                        errorMessage = "You must wait 1 hour between two password resets.";
                                    }

                                    if (errorMessage === invalidUserNameOrPassword) {
                                        errorMessage = "The email address is not registered. Click to try again.";
                                    }

                                    $scope.forgotButtonText = errorMessage;
                                    return $q.reject(errorMessage);
                                });
                        }).fail(function() {
                        var data = {};
                        data.user_agent = navigator.userAgent;
                        data.address = {};
                        data.address.ip = "unknown";
                        data.address.country_name = "unknown";
                        data.address.region_name = "unknown";
                        return accountService.sendNewPassword(email, data).then(function() {
                                $scope.forgotButtonText = forgotButtonTexts.SENT + email;
                                return $q.resolve();
                            },
                            function(error) {
                                var errorMessage = error.error;
                                var oneHourNotElapsed =
                                    safeGet(commonConstants.ALL_ERROR_VALUES
                                        .ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND);
                                var invalidUserNameOrPassword =
                                    safeGet(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD);

                                if (errorMessage === oneHourNotElapsed) {
                                    errorMessage = "You must wait 1 hour between two password resets.";
                                }

                                if (errorMessage === invalidUserNameOrPassword) {
                                    errorMessage = "The email address is not registered. Click to try again.";
                                }

                                $scope.forgotButtonText = errorMessage;
                                return $q.reject(errorMessage);
                            });
                    });

                }
            };

            function showInvalidTokenAlert() {
                $mdDialog.show(
                    $mdDialog.alert()
                    .parent(angular.element(document.querySelector('#popupContainer')))
                    .clickOutsideToClose(true)
                    .title('User session expired.')
                    .textContent('You were inactive with user session. To create a new please login.')
                    .ariaLabel('Expires user session')
                    .ok('OK')
                    .targetEvent(ev));
            }
        }
    ]);