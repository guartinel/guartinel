// create angularjs module, initialize application
var app = angular.module('guartinelApp', ['ui.router', 'ngMaterial', 'ngMessages', 'ngMdIcons', 'ngCookies', 'angular-svg-round-progress', 'ngclipboard', 'ngSanitize', 'chart.js', 'infinite-scroll','ngMaterialDatePicker']);


function replaceAll(string, whatToChange, toWhat) {
   string = string.replace(whatToChange, toWhat);

   while (string.indexOf(whatToChange) !== -1) {
      string = string.replace(whatToChange, toWhat);
   }
   return string;
}

function safeGet(value) {
    if (value === null || value === 'undefined' || value === undefined) {
        console.error("Cannot get value for :" + key);
        return "";
    }
    return value;
}
function sg(value){
   return safeGet(value);
}



var LOG = new Logger();
var shouldLogDebug = false;
function Logger() {
   this.debug = function(message) {
       if (shouldLogDebug || document.location.host.indexOf("backend2") != -1 || document.location.host.indexOf("test") != -1 || document.location.host.indexOf("dev") != -1) {
         console.info(message);
      }
   }
   this.info = function(message) {
      console.info(message);
   }
}

function isEmptyOrNull(param) {
   if (isNull(param)) {
      return true;
   }

   if (param === "") {
      return true;
   }

   if (param.length == 0) {
      return true;
   }

   return false;
}

function isNull(param) {
    if (typeof (param) == 'undefined' ||  param == undefined || param == null) {
        return true;
    }
    return false;
}


var isErrorLogEnabled = true;
function errorLog(message, cause) {
    if (!isErrorLogEnabled) {
        return;
    }
    console.error(message + " : " + JSON.stringify(cause));
}

var isTabVisible = (function () {
   var stateKey, eventKey, keys = {
      hidden: "visibilitychange",
      webkitHidden: "webkitvisibilitychange",
      mozHidden: "mozvisibilitychange",
      msHidden: "msvisibilitychange"
   };
   for (stateKey in keys) {
      if (stateKey in document) {
         eventKey = keys[stateKey];
         break;
      }
   }
   return function (c) {
      if (c) document.addEventListener(eventKey, c);
      return !document[stateKey];
   }
})();