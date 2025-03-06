/**
 * Created by DTAP on 2017.02.24..
 */
var helper = require('../../helper.js'); // must be required first to used as globals
var routeResponse = require('./routes/routeResponse.js');
import  "chai";
import 'mocha';
var assert = require("assert");

describe('Testing routes/routeResponse',function(){
       it('should return a success response',function(){
        var success //= new RouteResponse.Success();
       assert.notEqual(success["success"],"success");
    });
})