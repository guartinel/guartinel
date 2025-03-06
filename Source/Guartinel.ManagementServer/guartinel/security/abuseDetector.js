/**
 * Created by DTAP on 2016.05.29..
 */
var emailer = include('guartinel/connection/emailer.js');
var moment = require('moment');
var abuseLEVELS = {
    FATAL_ERROR: {
        LOW: 2,
        MEDIUM: 5,
        FATAL: 10
    }
}
var fatalErrors = [];

function FatalError() {
    this.timeStamp = moment()
}
exports.onFatalError = function () {
    fatalErrors.push(new FatalError());
    for(var err in fatalErrors){
       // if(err.isBefore(moment().substract(1,'m'))){
            //emailer.sendProblemEmailToAdmin("moqs001@gmail.com")
        //    LOG.info("Abuse detecter would send an email now.");
      //  }
    }
}