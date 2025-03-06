/**
 * Created by DTAP on 2017.08.03..
 */
var assert = require("chai").assert;

describe('testing common utils.string.tryParse', function () {
    it('check the error handling of this function', function () {
        var myarray = [{name: "name1"}, {name: "name2"}]
        var item = myarray;
        item[0].name = "changedName";
    });
});
