/**
 * Created by moqs_the_one on 2017.07.25..
 */
var mysql = require('mysql');

var connection = mysql.createConnection({
    host: "localhost",
    user: "root",
    password: "qw12qw"//,
    // database: "guartinel"
});

connection.connect(function (err) {
    if (err) {
        console.log(JSON.stringify(err));
    }
    function storeMeasurement(packageId, timeStamp, measurement) {
        var createDatabase = "CREATE DATABASE IF NOT EXISTS guartinel";

        connection.query(createDatabase, function (err, result) {
            if (err) throw err;
            console.log("Result: " + JSON.stringify(result));

            connection.query("use guartinel", function (err, result) {
                if (err) throw err;
                console.log("Result: " + JSON.stringify(result));
                      var createMeasurementsTable = " CREATE TABLE IF NOT EXISTS  measurements (ID INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Timestamp TIMESTAMP NOT NULL, Measurement VARCHAR(2048) NOT NULL, PackageID VARCHAR(50) NOT NULL)";
                    connection.query(createMeasurementsTable,function(err,result){
                        if (err) throw err;
                        console.log("Result: " + JSON.stringify(result));
                        connection.query("select * from measurements", function (err, result) {
                            if (err) throw err;
                            console.log("Result: " + JSON.stringify(result));

                        });
                });
            });
        });
    }

    storeMeasurement("csomagajdi", "2022-10-22 12:12:12", "mezsorment");
});