var helper = require('../../../helper.js'); // must be required first to used as globals
var commons = include('common/constants.js'); // must be stay to here to be used as global variable later in the app
var timeTool = include('guartinel/utils/timeTool.js');
var moment = require('moment');
var assert = require("chai").assert;

describe('testing isTimeAmountElapsedSinceTimeStamp', function () {
    it('check the execution paths for this function', function () {
        var timeStamp = moment().add(100,'d');
        var result = timeTool.isTimeAmountElapsedSinceTimeStamp(10,'d',timeStamp);
        assert.notOk(result);

        timeStamp = moment().subtract(100,'d');
        result = timeTool.isTimeAmountElapsedSinceTimeStamp(10,'d',timeStamp);
        assert.ok(result);
    });
});
