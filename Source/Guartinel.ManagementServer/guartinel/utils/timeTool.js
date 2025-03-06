/**
 * Created by DTAP on 2016.05.29..
 */
var moment = require('moment');
exports.isTokenTimeStampValid =  function (tokenTimeStamp) {
    var tokenTimeStampMoment = moment(tokenTimeStamp);
    var tokenMaxDate = tokenTimeStampMoment.add(1, ('h'));
    var currentMoment = moment();

    if (currentMoment.isBefore(tokenMaxDate)) {
        return true;
    }
    return false;
}

exports.isAccountExpired = function (expiryDate) {
    var currentMoment = moment();
    var expiryDateMoment = moment(expiryDate);

    if (currentMoment.isBefore(expiryDateMoment)) {
        return false;
    }
    return true;
}

exports.isAmountSecElapsedFromDate = function(amount,fromDate){
    var fromTimeStamp = moment(fromDate);
    var timeStampMinimumDate = fromTimeStamp.add(amount, ('s'));
    var currentMoment = moment();
    if(currentMoment.isAfter(timeStampMinimumDate)){
        return true;
    }
    return false;
}

exports.isTimeAmountElapsedSinceTimeStamp = function(amount,unit,timeStamp){
    var fromTimeStamp = moment(timeStamp);
    var maxTimeStamp = fromTimeStamp.add(amount, unit);
    var currentMoment = moment();

    if(currentMoment.isAfter(maxTimeStamp)){
        return true;
    }
    return false;
}

exports.isOneHourElapsedFromThisTimeStamp =  function (lastSendTimeStamp) {
    if(utils.object.isNull(lastSendTimeStamp)){
        lastSendTimeStamp = moment();
        lastSendTimeStamp.subtract(2,'h');
    }

    var timeStampMoment = moment(lastSendTimeStamp);
    var timeStampMinimumDate = timeStampMoment.add(1, ('h'));
    var currentMoment = moment();

    if (currentMoment.isAfter(timeStampMinimumDate)) {
        return true;
    }
    return false;
}