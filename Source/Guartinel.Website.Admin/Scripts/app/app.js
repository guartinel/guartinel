// create angularjs module, initialize application
var app = angular.module('watcherApp', ['ui.router', 'ngMaterial', 'ngMessages', 'ngMdIcons', 'ngCookies', 'angular-svg-round-progress','ngclipboard']);

function safeGet(value){
    if (value == null || value == 'undefined' || value == undefined) {
        console.error("Cannot get value for :" + key);
        return "";
    }
    return value;
}

function errorLog(message, err) {
   console.error(message + "ERROR: "+  JSON.stringify(err));
}

 