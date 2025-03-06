/**
 * Created by DTAP on 03/18/2015.
 */

var moment = require('moment');
var path = require('path');

exports.registerGlobals = function(){};

global.base_dir = __dirname;
global.abs_path = function(path) {
    return base_dir + path;
};
global.include = function(file) {
    return require(abs_path('/' + file));
};
global.getGuartinelHome = function() {
    return path.dirname(process.argv[1]) + '\\';
};
global.isDebug = function() {
    return process.env.develop;
};

global.safeGet = function (key) {
    if (key == null) {
        LOG.error("Cannot get value.\n" + (new Error).stack, utils.string.generateUUID());
        return "";
    }
    return key;
};
String.prototype.replaceAll = function (str1, str2, ignore) {
    return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof(str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
};