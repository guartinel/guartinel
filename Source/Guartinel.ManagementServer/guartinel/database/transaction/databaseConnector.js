/**
 * Created by DTAP on 2017.07.25..
 */
var configController = include('guartinel/admin/configuration/configurationController.js');
var connection;
var moment = require('moment');
var lastInit;
var securityTool = include("guartinel/security/tool.js");
var isRouteDebugEnabled = true;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug("Transactional DB:"+ " " + message);
}

exports.getConnectionState = function(){
    function result(success,state){
        this.success = success;
        this.state = state;
    }
    if(isConnected){
        return new result(true,"Connected");
    }
    return new result(false,"Disconnected");
}

function initIfCoolDownElapsedSinceLast() {
    if (utils.object.isNull(lastInit)) {
        module.exports.init(function () {});
        return; // dont wait here to finish init
    }
    var isElapsed = include('guartinel/utils/timeTool.js').isAmountSecElapsedFromDate('30',lastInit);
        if(isElapsed){
            module.exports.init(function () {});
        }else{
            LOG.info("Transactional databse init cooldown doesnt elapsed. Waiting with the init..");
        }

    return; // don't wait here to finish init
}
var isConnected = false;
exports.init = function (callback) {
    LOG.info("Initiating Mysql connection");
    lastInit = moment();
    var mysql = require('mysql');
    var config = configController.getTransactionDatabaseConfig();
    connection = mysql.createConnection({
        host: config.host,
        user: config.user,
        password: securityTool.decryptText(config.password),
        port: config.port
    });

    connection.connect(function (err) {
        if (err) {
            LOG.error("Cannot connect to transactional database instance ERR:" + JSON.stringify(err));
            return callback();
        }
        isConnected = true;
        // CREATE DATABASE IF NOT EXISTS
        var createDatabase = "CREATE DATABASE IF NOT EXISTS " + config.database;
        connection.query(createDatabase, afterDatabaseCreated);

        function afterDatabaseCreated(err, result) {
            if (err) {
                LOG.error("Cannot create transactional database ERR:" + JSON.stringify(err));
                return callback();
            }
            //USE THE DATABASE
            connection.query("use " + config.database, afterDatabaseSelected);
        }

        function afterDatabaseSelected(err, result) {
            if (err) {
                LOG.error("Cannot USE transactional database ERR:" + JSON.stringify(err));
                return callback();
            }
            //CREATE THE MEASUREMENTS TABLE IF NOT EXISTS
            var createMeasurementsTable = " CREATE TABLE IF NOT EXISTS  measurements (ID INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Timestamp TIMESTAMP NOT NULL, Measurement VARCHAR(2048) NOT NULL, PackageID VARCHAR(50) NOT NULL)";
            connection.query(createMeasurementsTable, afterMeasurementsTableCreated);
        }

        function afterMeasurementsTableCreated(err, result) {
            if (err) {
                LOG.error("Cannot create measurements table ERR:" + JSON.stringify(err));
                return callback();
            }
            //CREATE THE STATES TABLE IF NOT EXISTS
            var createStatesTable = "CREATE TABLE IF NOT EXISTS  states (ID INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Timestamp TIMESTAMP NOT NULL, State VARCHAR(512) NOT NULL,Message VARCHAR(512) NOT NULL, PackageID VARCHAR(50) NOT NULL)";
            connection.query(createStatesTable, afterStatesTableCreated);
        }

        function afterStatesTableCreated(err, result) {
            if (err) {
                LOG.error("Cannot create states table ERR:" + JSON.stringify(err));
                return callback();
            }
            return callback();
        }

        connection.on('error', function onError(err) {
            isConnected = false;
            LOG.error("Transactional Database connection is lost :" + JSON.stringify(err));
            if (err.code == 'PROTOCOL_CONNECTION_LOST') {
                module.exports.init(function () {
                });
            }
        });
    });
}

exports.storeMeasurement = function (packageId, timeStamp, measurement,onFinished) {
    if (utils.object.isNull(connection)) {
        LOG.error("Transactional Database connection is not initialized.");
        return onFinished();
    }
    connection.ping(function (err) {
        if (err) {
            isConnected = false;
            LOG.error("Cannot store measurement in the transactional DB. It doesnt respond to ping : " + JSON.stringify(err));
            initIfCoolDownElapsedSinceLast();
            return onFinished();
        }
        executeStore();
    });
    function executeStore() {
        var insertSQL = "INSERT INTO measurements (Timestamp,Measurement,PackageID) VALUES ("
            + connection.escape(timeStamp)
            + ","
           + connection.escape(JSON.stringify(measurement))
            + ","
            + connection.escape(packageId)
            + ")";
        debugRoute("StoreMeasurement SQL query: " + insertSQL);
        connection.query(insertSQL, function (err, result) {
            if (err) {
                LOG.error("Cannot store measurement in the transactional DB ERR:" + JSON.stringify(err));
            }
            return onFinished();
        });
    }

};

exports.storeState = function (packageId, timeStamp, stateName,stateMessage,stateMessageDetails,onFinished) {
    if (utils.object.isNull(connection)) {
        LOG.error("Transactional Database SQL connection is not initialized.");
        return onFinished();
    }
    connection.ping(function (err) {
        if (err) {
            isConnected = false;
            LOG.error("Cannot store state in the transactional DB. It doesnt respond to ping : " + JSON.stringify(err));
            initIfCoolDownElapsedSinceLast();
            return onFinished();
        }
        executeStore();
    });

    function executeStore() {
        var insertSQL = "INSERT INTO states (Timestamp,State,Message,MessageDetails,PackageID) VALUES ("
            + connection.escape(timeStamp)
            + ","
           + connection.escape(stateName)
            + ","
           + connection.escape(stateMessage)
           + ","
           + connection.escape(stateMessageDetails)
           + ","
            + connection.escape(packageId)
            + ")";

        debugRoute("StoreState SQL query: " + insertSQL);
        connection.query(insertSQL, function (err, result) {
            if (err) {
                LOG.error("Cannot store state in the transactional DB ERR:" + JSON.stringify(err));
            }
            return onFinished();
        });
    }
};

exports.getPackageStatistics = function(packageId,onFinished){
    if (utils.object.isNull(connection)) {
        LOG.error("Transactional Database SQL connection is not initialized.");
        return onFinished();
    }
    connection.ping(function (err) {
        if (err) {
            isConnected = false;
            LOG.error("Cannot get statistics in the transactional DB. It doesnt respond to ping : " + JSON.stringify(err));
            initIfCoolDownElapsedSinceLast();
            return onFinished();
        }
        executeGet();
    });

    function executeGet() {
        var selectSQL = "SELECT * FROM package_statistics_v WHERE `PackageId` = " + connection.escape(packageId) + " ;";
        debugRoute("GetStatistics SQL query: " + selectSQL);
        connection.query(selectSQL, function (err, result,fields) {
            if (err) {
                LOG.error("Cannot get statistics from the transactional DB ERR:" + JSON.stringify(err));
                return onFinished(err);
            }
            debugRoute("GetStatistics result: "+ JSON.stringify(result));
            return onFinished(null,result);
        });
    }
}