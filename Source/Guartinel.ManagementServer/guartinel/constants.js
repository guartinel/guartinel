/**
 * Created by DAVT on 2015.01.30..
 */
/*
 var colors = require('colors');
 exports.makeRed = function (constant) {
 return constant.red;
 }
 exports.makeBold = function (constants) {
 return constants.bold;
 }*/
exports.NOTIFY_WATCHER_SERVER_CHANGED = "NOTIFY_WATCHER_SERVER_CHANGED";

exports.INTERNAL = {
    DB: {
        OPENED: "DATABASE_OPENED".bold,
        ERROR: "DATABASE_ERROR".red,
        CLOSED: "DATABASE_CONNECTION_CLOSED",       
    },
    HTTP: {
        STARTED: "HTTP_SERVER_STARTED_SUCCESSFULLY".green,
    },
    HTTPS: {
        STARTED: "HTTPS_SERVER_STARTED_SUCCESSFULLY".green,
    }

}
exports.RESULT = {
    SUCCESS: "SUCCESS",
    INTERNAL_SYSTEM_ERROR: "INTERNAL_SYSTEM_ERROR",
    FAILED: "FAILED",
}
exports.DEVICE = {
    NAME_TAKEN: "DEVICE_NAME_TAKEN",
    INVALID_DEVICE_UUID: "INVALID_DEVICE_UUID",
    MISSING_GCM_ID: "MISSING_GCM_ID",
    NOT_USED_IN_PACKAGE: "NOT_USED_IN_PACKAGE"
}
exports.ACCOUNT = {
    EMAIL_ALREADY_REGISTERED: "EMAIL_ALREADY_REGISTERED",
    CANNOT_FIND_EMAIL: "CANNOT_FIND_EMAIL",
    INVALID_PASSWORD: "INVALID_PASSWORD",
    INVALID_ACCOUNT_ID: "INVALID_ACCOUNT_ID",
    INVALID_ACTIVATION_CODE: "INVALID_ACTIVATION_CODE",
    ACCOUNT_EXPIRED: "ACCOUNT_EXPIRED"
}
exports.TOKEN = {
    VALID: "VALID_TOKEN",
    INVALID: "INVALID_TOKEN",
    REVOKED: "TOKEN_REVOKED"
}
exports.ADMINISTRATION_TOKEN = {
    INVALID: "INVALID_ADMINISTRATION_TOKEN"
}
exports.ADMIN = {
    INVALID_USERNAME: "INVALID_USERNAME",
    INVALID_PASSWORD: "INVALID_PASSWORD"

}

exports.PACKAGE = {
    ALREADY_EXIST: "PACKAGE_ALREADY_EXIST",
    NOT_EXISTS: "PACKAGE_NOT_EXISTS",
    TYPE: {
        COMPUTER_SUPERVISOR: "COMPUTER_SUPERVISOR"
    }
}
exports.DATE = {
    FORMAT: 'DD/MM/YYYY HH:mm:ss'
}



