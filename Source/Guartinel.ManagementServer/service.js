/**
 * Created by DTAP on 2017.08.01..
 */
var helper = require('./helper.js');
var Service = require('node-windows').Service;
// Create a new service object
console.log("Script to install MangementServer Service");
console.log("Make sure you rung this script as an Administrator!");

var svc = new Service({
    name: 'GuartinelManagementServerService',
    script: require('path').join(__dirname, 'app.js'),
    description: 'ManagementServerService'
});

if (!utils.object.isNull(process.argv[2]) && !utils.object.isNull(process.argv[3]) && !utils.object.isNull(process.argv[4])) {
    svc.name = process.argv[2];
    svc.user.account = process.argv[3];
    svc.user.password = process.argv[4];
    console.log("Installing service with name: " + process.argv[2] + "  user :" + process.argv[3] + " and with password:" + process.argv[4]);
}

svc.on('install', function (err) {
    if (err) {
        console.log("install : " + JSON.stringify(err));
        return;
    }
    console.log("Service installed as : GuartinelManagementServerService")
});
svc.on('alreadyinstalled', function (err) {
    if (err) {
        console.log("alreadyinstalled  : " + JSON.stringify(err));
        return;
    }
    console.log("Service already installed ")
});
svc.on('invalidinstallation', function (err) {
    if (err) {
        console.log("invalidinstallation  : " + JSON.stringify(err));
        return;
    }
    console.log("Service installation is invalid. ")
});

svc.on('error ', function (err) {
    if (err) {
        console.log("error   : " + JSON.stringify(err));
        return;
    }
    console.log("Cannot install service.")
});

svc.install();