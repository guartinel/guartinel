import * as moment from "moment";



var expiryMoment = moment("2018-02-24T08:37:30.694Z");
var currentMoment = moment();
var diffInDays = expiryMoment.diff(currentMoment, 'days');
if (diffInDays > 0) {
   console.log("OK");
} else {
   console.log("BAD");
}
console.log("Diff:" + diffInDays);
