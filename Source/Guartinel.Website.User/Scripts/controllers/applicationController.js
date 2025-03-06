'use strict';
app.controller('applicationController', ['$rootScope', '$scope', '$state', 'accountService', 'packageService', 'deviceService', '$window',
   function ($rootScope, $scope, $state, accountService, packageService, deviceService, $window) {
      $window.onblur = function () {

      };

      $window.onfocus = function () {
         accountService.validateUserToken().then(function () {
            packageService.getAvailable().then(function (packages) {
               if ($scope.packages == null) {
                  $scope.packages = [];
               }
               $scope.packages = packages;

               $rootScope.$broadcast('packageUpdated', $scope.packages);
               $scope.isLoading = false;
            }, function (error) {
               //add error card
               $scope.isLoading = false;
            });
         }, function () {

         });
      };


       $rootScope.getString = function(identifier) {
           var language = "ENGLISH";
           if (!isNull(accountService.currentUser) && isNull(!accountService.currentUser.language)) {
               language = accountService.currentUser.language;
           }
           if (isNull(languageTable.LANGUAGES[language][identifier])) {
               return identifier;
           }
           return languageTable.LANGUAGES[language][identifier];
       };

       $rootScope.trimTextShort = function (text) {
           if (isEmptyOrNull(text)) {
               return text;
           }
           if (text.length > 43) {
               return text.substr(0, 44) + "...";
           }
           return text;
       };
      $rootScope.insertLineBreak = function (text) {
         if (isNull(text)) {
            return "";
         }
         var chunkSizeBeforeLineBreak = 50;
         var textWords = text.split(" ");

         var lines = [];
         var currentLine = ""
         for (var wordIndex = 0; wordIndex < textWords.length; wordIndex++) {
            var word = textWords[wordIndex];
            currentLine += word;
            if (currentLine.length < chunkSizeBeforeLineBreak) {
               currentLine += " ";
            } else {
               currentLine += " ";
               lines.push(currentLine);
               currentLine = "";
            }
         }
         if (currentLine.length != 0) {
            lines.push(currentLine);
         }
         return lines.join("<br>");
      }
       angular.element(document).ready(function () {
           if (isNull($window.android)) {
               console.log("$window.android is null");
           } else {
               "$window.android is  present calling interface"
               $window.android.onWebAppLoaded();
           }
       });
      $rootScope.$on('$stateChangeStart',
         //   $rootScope.$on('$viewContentLoaded',
         function (event, toState, toParams, fromState, fromParams, options) {
            // check if page need user token
            var isUserTokenNeeded = false;
            if (toState.data != null) {
               isUserTokenNeeded = toState.data.requireUserLogin;
            }
            // if token not needed just return
            //  if (!isUserTokenNeeded) {
            //     return;
            // }

            //if needed check if user token is present 
            var isThereSavedToken = accountService.isThereSavedToken();

            //if token not present than redirect user to index
            if (!isThereSavedToken && isUserTokenNeeded) {
               event.preventDefault();
               return $state.go(STATES.homepage.login);
            }

            //in this point user token is needed and user has it so init the application if its not already done
            var isCurrentUserEmpty = true;

            if (accountService.currentUser != null) {
               isCurrentUserEmpty = accountService.currentUser.id == null;
            }

            if (isCurrentUserEmpty) {
               accountService.initService().then(function () {
                  //  packageService.initService();
                  deviceService.initService();
                  if (toState.url == "/") {
                     return $state.go(STATES.userpage.packages);
                  }
                  return;
               },
                  function (error) {
                     return;
                  });
            }



            /*   // check if page need user token
               var isUserTokenNeeded = false;
               if (toState.data != null) {
                   isUserTokenNeeded = toState.data.requireUserLogin;
               }
               // if token not needed just return
               if (!isUserTokenNeeded) {
                   return;
               }
    
               //if needed check if user token is present 
               var isThereSavedToken = accountService.isThereSavedToken();
    
               //if token not present than redirect user to index
               if (!isThereSavedToken) {
                   event.preventDefault();
                   return $state.go(STATES.homepage.login);
               }
    
               //in this point user token is needed and user has it so init the application if its not already done
               var isCurrentUserEmpty = true;
    
               if (accountService.currentUser != null) {
                   isCurrentUserEmpty = accountService.currentUser.id == null;
               }
    
              if (isCurrentUserEmpty) {
                   accountService.initService().then(function () {
                       packageService.initService();
                       deviceService.initService();
                       return;
                   },
                  function (error) {
                      return;
                  });
              }*/
         });

   }]);

