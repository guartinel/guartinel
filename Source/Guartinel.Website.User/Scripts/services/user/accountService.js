'use strict';
app.service('accountService', [
      '$q', 'apiService', '$cookieStore', '$state', 'toastService', 'dialogService', '$filter', function ($q, apiService, $cookieStore, $state, toastService, dialogService, $filter) {
         var self = this;

         self.clearUser = function () {
            $cookieStore.remove('userToken');
            self.currentUser = {};
         }
         self.isLoggedIn = false;

         self.unsubscribeAllEmail = function (blackListToken) {
            var url = safeGet(backendUserApiUrls.UNSUBSCRIBE_ALL_EMAIL);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.BLACK_LIST_TOKEN)] = blackListToken;

            return apiService.sendRequest(url, data).then(function (response) {
               return $q.resolve("Your email adress won't receive any Guartinel email in the future.");
            }, function (error) {
               console.error("unsubscribeAllEmail failed! Error: " + error.error);
               if (error.error === "INVALID_TOKEN") {
                  resultError = "Cannot remove email. Your link is invalid.";
               }
               return $q.reject(error);
            });
         };

         self.validateUserToken = function () {
            var url = safeGet(backendUserApiUrls.ACCOUNT_VALIDATE_TOKEN);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = self.currentUser.token;

            return apiService.sendRequest(url, data).then(function (response) {
               return $q.resolve("Session OK");
            }, function (error) {
               console.error("Session validation failed : " + error.error);
               var resultError = error.error;
               if (error.error === "INVALID_TOKEN") {
                  resultError = "Cannot remove email. Your link is invalid.";
               }
               return $q.reject(resultError);
            });
         }

         self.unsubscribeFromPackageEmail = function (blackListToken, packageID) {
            var url = safeGet(backendUserApiUrls.UNSUBSCRIBE_FROM_PACKAGE_EMAIL);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.BLACK_LIST_TOKEN)] = blackListToken;
            data[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)] = packageID;

            return apiService.sendRequest(url, data).then(function (response) {
               return $q.resolve("Your email adress won't receive any alert from this package until the package owner add this address again.");
            }, function (error) {
               console.error("unsubscribeFromPackageEmails failed! Error: " + error.error);
               if (error.error === "INVALID_TOKEN") {
                  resultError = "Cannot remove email. Your link is invalid.";
               }
               return $q.reject(error);
            });
         }

         self.login = function (email, password) {
            var url = safeGet(backendUserApiUrls.ACCOUNT_LOGIN);
            var passwordHash = CryptoJS.SHA512(password + email).toString().toUpperCase();
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;
            data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = passwordHash;

            return apiService.sendRequest(url, data).then(function (response) {
               var token = response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)];             
               self.currentUser = new self.UserAccount();
               self.isLoggedIn = true;
               self.currentUser.saveToken(token);
               return $q.resolve();
            }, function (error) {
               console.error("Login failed! Error: " + error.error);
               return $q.reject(error);
            });
         }
         self.register = function (email, password, firstName, lastName) { // TODO rename to create
            var url = safeGet(backendUserApiUrls.ACCOUNT_CREATE);
            var passwordHash = CryptoJS.SHA512(password + email).toString().toUpperCase();

            var data = {}
            data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;
            data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = passwordHash;
            data[safeGet(commonConstants.ALL_PARAMETERS.FIRST_NAME)] = firstName;
            data[safeGet(commonConstants.ALL_PARAMETERS.LAST_NAME)] = lastName;

            return apiService.sendRequest(url, data).then(function (response) {
               console.log("Account register successful! Firstname: " + firstName + ", lastName: " + lastName);
               return $q.resolve(response.account);
            }, function (error) {
               console.error("Account register failed! Error: " + error);
               return $q.reject(error);
            });
         }

         self.logout = function () {
            var url = safeGet(backendUserApiUrls.ACCOUNT_LOGOUT);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = self.currentUser.token;
            return apiService.sendRequest(url, data).then(function (response) {
               self.clearUser();
               self.isLoggedIn = false;
               $state.go(STATES.homepage.login);
               return $q.resolve();
            }, function (error) {
               self.clearUser();
               console.error("Logout failed! Error: " + error);
               return $q.reject(error);
            });
         }

         self.getStatus = function () {
            var url = safeGet(backendUserApiUrls.ACCOUNT_GET_ACCOUNT_INFO);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = self.currentUser.token;
            return apiService.sendRequest(url, data).then(function (response) {
               var rawData = {
                  id: response.account[safeGet(commonConstants.ALL_PARAMETERS.ID)],
                  createdOn: response.account[safeGet(commonConstants.ALL_PARAMETERS.CREATED_ON)],
                  firstName: response.account[safeGet(commonConstants.ALL_PARAMETERS.FIRST_NAME)],
                  lastName: response.account[safeGet(commonConstants.ALL_PARAMETERS.LAST_NAME)],
                  email: response.account[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)],
                  isActivated: response.account[safeGet(commonConstants.ALL_PARAMETERS.IS_ACTIVATED)],
                  licenseAggregate: response.account[safeGet(commonConstants.ALL_PARAMETERS.LICENSE_AGGREGATE)],                 
                  licenses: response.account[safeGet(commonConstants.ALL_PARAMETERS.LICENSES)],
               }

               self.currentUser = new self.UserAccount(rawData);

               if (!self.currentUser.isActivated) {
                  toastService.showToast("Your account is not activated yet.", "Activate!",
                      function () { dialogService.showDialog(DIALOGS.account.activateAccount, self.currentUser.email); });
               }

               return $q.resolve();
            }, function (error) {
               console.error("Retrieving account info failed! Error: " + error);
               return $q.reject(error);
            });
         }

         self.update = function (id, email, currentPassword, newPassword, firstName, lastName) {
            var url = safeGet(backendUserApiUrls.ACCOUNT_UPDATE);
            var currentPasswordHash = CryptoJS.SHA512(currentPassword + email).toString().toUpperCase();

            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.ID)] = id;
            data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;
            data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = currentPasswordHash;
            data[safeGet(commonConstants.ALL_PARAMETERS.FIRST_NAME)] = firstName;
            data[safeGet(commonConstants.ALL_PARAMETERS.LAST_NAME)] = lastName;
            data[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = self.currentUser.token;

            if (!isNull(newPassword)) {
               var newPasswordHash = CryptoJS.SHA512(newPassword + email).toString().toUpperCase();
               data[safeGet(commonConstants.ALL_PARAMETERS.NEW_PASSWORD)] = newPasswordHash;
            }

            return apiService.sendRequest(url, data).then(function (response) {
               console.log("Account updated successfully!");
               return $q.resolve("Account updated.");
            }, function (error) {
               console.error("Account update failed! Error: " + JSON.stringify(error));
               var resultError = "Cannot udate account. Error:" + error.error_uuid;
               if (error.error === "INVALID_USER_NAME_OR_PASSWORD") {
                  resultError = "Cannot update. Invalid user name or password.";
               }
               return $q.reject(resultError);
            });
         }
         
         self.activate = function (email, activationCode) {
            var url = safeGet(backendUserApiUrls.ACCOUNT_ACTIVATE);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.ACTIVATION_CODE)] = activationCode;
            data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;

            return apiService.sendRequest(url, data).then(function (response) {
               var token = response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)];
               if (isNull(token)) {
                  return $q.reject("Cannot activate account.");
               }
               self.currentUser = new self.UserAccount();
               self.isLoggedIn = true;
               self.currentUser.saveToken(token);
               return $q.resolve();
            }, function (error) {
               if (error.error === (safeGet(commonConstants.ALL_ERROR_VALUES.INVALID_ACTIVATION_CODE))) {
                  error = "The activation code is invalid. Please try again.";
               }
               console.error("Account activation failed! Error: " + error);
               return $q.reject(error);
            });
         }


         self.deleteAccount = function (email, password) {
            var url = safeGet(backendUserApiUrls.ACCOUNT_DELETE);
            password = CryptoJS.SHA512(password + email).toString().toUpperCase();

            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;
            data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = password;

            return apiService.sendRequest(url, data).then(function (response) {
               return $q.resolve();
            }, function (error) {
               console.error("Account delete failed! Error: " + JSON.stringify(error));
               var resultError = "Cannot delete account. Error:" + error.error_uuid;
               if (error.error === "INVALID_USER_NAME_OR_PASSWORD") {
                  resultError = "Cannot delete. Invalid user name or password.";
               }
               return $q.reject(resultError);
            });
         }

         self.freezeAccount = function (email, password) {
            var url = safeGet(backendUserApiUrls.ACCOUNT_FREEZE);
            password = CryptoJS.SHA512(password + email).toString().toUpperCase();
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;
            data[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)] = password;

            return apiService.sendRequest(url, data).then(function (response) {
               return $q.resolve();
            }, function (error) {
               console.error("Account freeze failed! Error: " + JSON.stringify(error));
               var resultError = "Cannot freeze account. Error:" + error.error_uuid;
               if (error.error === "INVALID_USER_NAME_OR_PASSWORD") {
                  resultError = "Cannot freeze. Invalid user name or password.";
               }
               return $q.reject(resultError);
            });
         }

         self.resendActivationCode = function (email) {
            var url = safeGet(backendUserApiUrls.ACCOUNT_RESEND_ACTIVATION_CODE);
            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;
            return apiService.sendRequest(url, data).then(function (response) {

               return $q.resolve();
            }, function (error) {
               console.error("Resending account activation code failed! Error: " + error);


               return $q.reject(error);
            });
         }

         self.sendNewPassword = function (email, requesterAddress) {
            var url = safeGet(backendUserApiUrls.ACCOUNT_SEND_NEW_PASSWORD);

            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;
            data[safeGet(commonConstants.ALL_PARAMETERS.ADDRESS)] = requesterAddress;

            return apiService.sendRequest(url, data).then(function (response) {
               return $q.resolve();
            }, function (error) {
               console.error("Sending new password failed! Error: " + error);
               return $q.reject(error);
            });
         }

         self.verifySendNewPassword = function (email, verificationCode) {
            var url = safeGet(backendUserApiUrls.ACCOUNT_VERIFY_SEND_NEW_PASSWORD);

            var data = {};
            data[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = email;
            data[safeGet(commonConstants.ALL_PARAMETERS.VERIFICATION_CODE)] = verificationCode;

            return apiService.sendRequest(url, data).then(function (response) {
               return $q.resolve();
            }, function (error) {
               console.error("Sending new password verification failed! Error: " + error);
               return $q.reject(error);
            });
         }



         self.UserAccount = function (rawData) {
            if (rawData == null) {
               rawData = {};
            }

            this.isEmpty = this.token == null;
            this.id = rawData.id;
            this.createdOn = rawData.createdOn;
            this.firstName = rawData.firstName;
            this.lastName = rawData.lastName;
            this.email = rawData.email;
            this.isActivated = rawData.isActivated;
            this.licenseAggregate = rawData.licenseAggregate;
            this.licenses = rawData.licenses;
            try {
               this.token = $cookieStore.get('userToken');
            } catch (err) {
               console.log("Cannot get token from cookiestore" + err);
            }
            if (!isNull(this.licenses)) {
               for (var i = 0; i < this.licenses.length; i++) {
                  this.licenses[i].startDate = new Date($filter('date')(this.licenses[i].startDate, 'yyyy-MM-ddTHH:mm:ssZ'));
                  this.licenses[i].expiryDate = new Date($filter('date')(this.licenses[i].expiryDate, 'yyyy-MM-ddTHH:mm:ssZ'));

               }
            }    

          /*  this.getMaxPackageCount = function () {
               var maxCount = 0;
               for (var i = 0; i < this.licenses.length; i++) {
                  if (this.licenses[i].license.maximumPackages > maxCount) {
                     maxCount = this.licenses[i].license.maximumPackages;
                  }
               }
               return maxCount;
            }*/

            this.saveToken = function (token) {
               this.token = token;
               $cookieStore.put('userToken', token);
            }

            this.getMinimumCheckInterval = function () {
               var min = this.licenses[0].license.minimumCheckIntervalSec;
               for (var i = 0; i < this.licenses; i++) {
                  if (this.licenses[i].license.minimumCheckInterval < min) {
                     min = this.licenses[i].license.minimumCheckIntervalSec;
                  }
               }
               return min;
            }
         }

         self.initService = function () {
            if (self.currentUser == null || self.currentUser.id == null) {
               self.currentUser = new self.UserAccount();
            }
            return self.getStatus();
         }

         self.isThereSavedToken = function () {
            return $cookieStore.get('userToken') != null;
         }
      }
]);
