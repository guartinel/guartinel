var moment = require("moment");
var fs = require('fs');
var os = require('os-utils');
var disk = require('diskspace');
var requestStat = include('guartinel/admin/requestStat.js');

exports.getProgramVersion = function (callback) {
    fs.readFile(getGuartinelHome()+'package.json', 'utf8', function (err, data) {
        if (err) {
            LOG.error("Error while getting program version : " + err);
            return callback("ERROR");
       }
       var packageParsedJson = "unknown";
        try {
           packageParsedJson  = JSON.parse(data);
        } catch (err) {
            if (err) {
                LOG.error("Error while parsing program version : " + err);
                return callback("ERROR");
            }
        }
        return callback(packageParsedJson.version);
    });
}

function getMemoryUsage(callback) {
    var totalMem = os.totalmem();
    var currentMem = os.freemem();
    var result = 1-(currentMem / totalMem);
    return callback(result);
}

var cpuID ;
exports.getCpuId = function(callback){
    if(!utils.object.isNull(cpuID)){return callback(cpuID);}

    var exec = require('child_process').exec;
    exec("wmic CPU get ProcessorId", function puts(error, stdout, stderr) {
        var resultArray =  stdout.split("\r\r\n");
        var cpuIDRAW = resultArray[1].replaceAll(" ","");
        return callback(cpuIDRAW);
    });
}

exports.getServerInfos = function (callback) {
    os.cpuFree(function (cpuUsage) {
        disk.check('C', function (err, total, free, status) {
            requestStat.getRequestCount(function (reqCount) {
                module.exports.getProgramVersion(function (programVersion) {
                    getMemoryUsage(function (memoryUsage) {
                        var serverStatus = {
                            program_version: programVersion,
                            nodejs_version: process.version,
                            uptime_ms: process.uptime() * 1000,
                            current_cpu_usage: 1-cpuUsage,
                            current_memory_usage: memoryUsage ,
                            current_storage_usage: 1-(free/total),
                            gcms_sent: 1000,
                            emails_sent: 100,
                            alerts_sent: 1000,
                            requests: reqCount
                        }
                        return callback( serverStatus);
                    });
                });
            });
        });
    });
}

exports.getAvailableCheckerServers = function (callback) {
    var result = [
        {
            id: "foo",
            name: "bar",
            uptime : 10,
            current_cpu_usage: 10,
            current_memory_usage: 10,
            current_storage_usage: 10,
            alerts_sent: 10,
            date_registered: moment().toISOString(),
        }
    ];
    return callback(null, result);
}

exports.getCheckerEvents = function (callback) {
    var result = [
        {
            time_stamp: moment().toISOString(),
            short_description: "foo(abstract is reserved for js :( .",
            description: "foobarbaz"
        }
    ];
    return callback(null, result);
}