/**
 * Created by DTAP on 2017.03.21..
 */
var helper = require('../../../helper.js'); // must be required first to used as globals
var commons = include('common/constants.js'); // must be stay to here to be used as global variable later in the app
var commonUtils = include('guartinel/utils/commonUtils.js');

var result =   utils.object.ensureAsObject("{teve\":\"pata\"}");




var assert = require("chai").assert;

describe('testing common utils.string.tryParse', function () {
    it('check the error handling of this function', function () {

        assert.equal(result.teve,"pata");
    });
});
