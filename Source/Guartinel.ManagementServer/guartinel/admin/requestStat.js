var moment = require('moment');

var reqNum = 0;
var invalidReqs = [];

exports.getRequestCount = function (callback) {
    return callback(reqNum);
}

exports.increaseRequestCount = function (callback) {
    reqNum++;
    return callback();
}

exports.addInvalidRequest = function (url, reqBodyString, callback) {
    var invalidReq = {
        timeStamp: moment().toISOString(),
        url: url,
        body: reqBodyString
    };
    invalidReqs.push(invalidReq);
    callback();
}

exports.getInvalidRequests = function (callback) {
    callback(invalidReqs);
}

