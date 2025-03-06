var moment = require('moment');
var events = [];

exports.addEvent = function (event, callback) {
    if(events.length >= 500){
        events.splice(400,100);
    }
    events.unshift({
        timeStamp: moment().toISOString(),
        event: event
    });
    return callback();
}

exports.getEvents = function (callback) {
    return callback(events);
}

exports.clearEvents = function () {
    events = [];
    return ;
}