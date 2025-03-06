import { LOG } from "../diagnostics/LoggerFactory";
import { ErrorResponse } from "../guartinel/response/Response";
import { MSInternalServerError } from "../error/Errors";
var requestStat = global.include('guartinel/admin/requestStat.js');
var express = require('express');
var connect = require('connect'); // express module to connect middleware
var bodyParser = require('body-parser');
var app = express();
var adminRoutesEntryLastIndex;
var inMemoryDB = global.include('guartinel/database/inMemory/databaseConnector.js');

export function getApp() {
    return app;
}
export function initAdminRoutes(finishCallback) {
    LOG.info("Admin routes initialization..");

    var adminSendMaintenanceEmail = global.include('routes/admin/sendMaintenanceEmail.js');
    app.post(adminSendMaintenanceEmail.URL, adminSendMaintenanceEmail.route);

    var adminRunJob = global.include('routes/admin/runJob.js');
    app.post(adminRunJob.URL, adminRunJob.route);

    var adminUpdate = global.include('routes/admin/update.js');
    app.post(adminUpdate.URL, adminUpdate.route);

    var adminLogin = global.include('routes/admin/login.js');
    app.post(adminLogin.URL, adminLogin.route);

    var adminLogout = global.include('routes/admin/logout.js');
    app.post(adminLogout.URL, adminLogout.route);

    var adminSetWebsiteAddress = global.include('routes/admin/setWebsiteAddress.js');
    app.post(adminSetWebsiteAddress.URL, adminSetWebsiteAddress.route);

    var adminDatabaseGetStatus = global.include('routes/admin/database/getStatus.js');
    app.post(adminDatabaseGetStatus.URL, adminDatabaseGetStatus.route);

    var adminDatabaseUpdate = global.include('routes/admin/database/update.js');
    app.post(adminDatabaseUpdate.URL, adminDatabaseUpdate.route);

    var adminStatusGetLog = global.include('routes/admin/status/getLog.js');
    app.post(adminStatusGetLog.URL, adminStatusGetLog.route);

    var adminStatusGetEvents = global.include('routes/admin/status/getEvents.js');
    app.post(adminStatusGetEvents.URL, adminStatusGetEvents.route);

    var adminStatusGetStatus = global.include('routes/admin/status/getStatus.js');
    app.post(adminStatusGetStatus.URL, adminStatusGetStatus.route);

    var adminStatusGetInvalidReqs = global.include('routes/admin/status/getInvalidRequests.js');
    app.post(adminStatusGetInvalidReqs.URL, adminStatusGetInvalidReqs.route);

    var adminWatcherServerGetAvailable = global.include('routes/admin/watcherserver/getAvailable.js');
    app.post(adminWatcherServerGetAvailable.URL, adminWatcherServerGetAvailable.route);

    var adminGetSuperVisorStatus = global.include('routes/admin/getSupervisorStatus.js');
    app.post(adminGetSuperVisorStatus.URL, adminGetSuperVisorStatus.route);

    var adminWatcherServerGetEvents = global.include('routes/admin/watcherserver/getEvents.js');
    app.post(adminWatcherServerGetEvents.URL, adminWatcherServerGetEvents.route);

    var adminWatcherServerRegister = global.include('routes/admin/watcherserver/register.js');
    app.post(adminWatcherServerRegister.URL, adminWatcherServerRegister.route);

    var adminWatcherServerRemove = global.include('routes/admin/watcherserver/remove.js');
    app.post(adminWatcherServerRemove.URL, adminWatcherServerRemove.route);

    var adminWatcherServerUpdate = global.include('routes/admin/watcherserver/update.js');
    app.post(adminWatcherServerUpdate.URL, adminWatcherServerUpdate.route);

    var adminWatcherServerGetStatus = global.include('routes/admin/watcherserver/getStatus.js');
    app.post(adminWatcherServerGetStatus.URL, adminWatcherServerGetStatus.route);


    adminRoutesEntryLastIndex = app._router.stack.length;
    for (var i in app._router.stack) {
        if (app._router.stack[i].hasOwnProperty('route')) {
            LOG.info(app._router.stack[i].route.path);
        }
    }
    LOG.info("Admin routes initialized.");
    return finishCallback();
}

export function initRoutes () {
    LOG.info("Public routes initialization.");

    app.get('/', function (req, res) {
        var ip = req.headers['x-forwarded-for'] || req.connection.remoteAddress;
        res.send({ message: "Welcome to Guartinel service! " + ip });
    });

    //ROOT ROUTE
    var getVersion = global.include('routes/getVersion.js');
    app.get(getVersion.URL, getVersion.route);
    //!ROOT ROUTE

    //PLUGIN RELATED
    var applicationSupervisorRegisterMeasurement = global.include('routes/applicationSupervisor/registerMeasurement.js');
    app.post(applicationSupervisorRegisterMeasurement.URL, applicationSupervisorRegisterMeasurement.route);

    var applicationSupervisorGetApplicationInstanceIds = global.include('routes/applicationSupervisor/getApplicationInstanceIds.js');
    app.post(applicationSupervisorGetApplicationInstanceIds.URL, applicationSupervisorGetApplicationInstanceIds.route);

    var hardwareSupervisorRegisterMeasurement = global.include('routes/hardwareSupervisor/registerMeasurement.js');
    app.post(hardwareSupervisorRegisterMeasurement.URL, hardwareSupervisorRegisterMeasurement.route);

    var hardwareSupervisorRegisterMeasuredData = global.include('routes/hardwareSupervisor/registerMeasuredData.js');
    app.post(hardwareSupervisorRegisterMeasuredData.URL, hardwareSupervisorRegisterMeasuredData.route);

    var hardwareSupervisorRegisterHardware = global.include('routes/hardwareSupervisor/registerHardware.js');
    app.post(hardwareSupervisorRegisterHardware.URL, hardwareSupervisorRegisterHardware.route);

    var hardwareSupervisorRegister = global.include('routes/hardwareSupervisor/register.js');
    app.post(hardwareSupervisorRegister.URL, hardwareSupervisorRegister.route);

    var hardwareSupervisorRemoteLog = global.include('routes/hardwareSupervisor/remoteLog.js');
    app.post(hardwareSupervisorRemoteLog.URL, hardwareSupervisorRemoteLog.route);

    var hardwareSupervisorCheckForUpdate = global.include('routes/hardwareSupervisor/checkForUpdate.js');
    app.post(hardwareSupervisorCheckForUpdate.URL, hardwareSupervisorCheckForUpdate.route);

    var hardwareSupervisorValidateHardware = global.include('routes/hardwareSupervisor/validateHardware.js');
    app.post(hardwareSupervisorValidateHardware.URL, hardwareSupervisorValidateHardware.route);
    //!PLUGIN RELATED

    //ACCOUNT
    var accountActivation = global.include('routes/account/activateAccount.js');
    app.post(accountActivation.URL, accountActivation.route);

    var sendNewPassword = global.include('routes/account/sendNewPassword.js');
    app.post(sendNewPassword.URL, sendNewPassword.route);

    var resendActivationCode = global.include('routes/account/resendActivationCode.js');
    app.post(resendActivationCode.URL, resendActivationCode.route);

    var accountCreate = global.include('routes/account/create.js');
    app.post(accountCreate.URL, accountCreate.route);

    var accountDelete = global.include('routes/account/delete.js');
    app.post(accountDelete.URL, accountDelete.route);

    var accountLogin = global.include('routes/account/login.js');
    app.post(accountLogin.URL, accountLogin.route);

    var accountLogout = global.include('routes/account/logout.js');
    app.post(accountLogout.URL, accountLogout.route);

    var accountGetStatus = global.include('routes/account/getStatus.js');
    app.post(accountGetStatus.URL, accountGetStatus.route);

    var accountFreeze = global.include('routes/account/freeze.js');
    app.post(accountFreeze.URL, accountFreeze.route);

    var accountUpdate = global.include('routes/account/update.js');
    app.post(accountUpdate.URL, accountUpdate.route);

    var accountVerifySendNewPassword = global.include('routes/account/verifySendNewPassword.js');
    app.post(accountVerifySendNewPassword.URL, accountVerifySendNewPassword.route);

    var accountValidateToken = global.include('routes/account/validateToken.js');
    app.post(accountValidateToken.URL, accountValidateToken.route);

    //!ACCOUNT

    //DEVICE
    var deviceRegister = global.include('routes/device/register.js');
    app.post(deviceRegister.URL, deviceRegister.route);

    var deviceEdit = global.include('routes/device/edit.js');
    app.post(deviceEdit.URL, deviceEdit.route);

    var deviceLogin = global.include('routes/device/login.js');
    app.post(deviceLogin.URL, deviceLogin.route);

    var deviceDelete = global.include('routes/device/delete.js');
    app.post(deviceDelete.URL, deviceDelete.route);

    var deviceGetAvailable = global.include('routes/device/getavailable.js');
    app.post(deviceGetAvailable.URL, deviceGetAvailable.route);

    var deviceDisconnect = global.include('routes/device/disconnect.js');
    app.post(deviceDisconnect.URL, deviceDisconnect.route);

    var deviceAndroidTest = global.include('routes/device/android/test.js');
    app.post(deviceAndroidTest.URL, deviceAndroidTest.route);

    var deviceAndroidLogin = global.include('routes/device/android/login.js');
    app.post(deviceAndroidLogin.URL, deviceAndroidLogin.route);

    var deviceAndroidRegister = global.include('routes/device/android/register.js');
    app.post(deviceAndroidRegister.URL, deviceAndroidRegister.route);

    //!! DEVICE

    //LICENSE
    var licenseGetAvailable = global.include('routes/license/getAvailable.js');
    app.post(licenseGetAvailable.URL, licenseGetAvailable.route);

    var licenseAddToAccount = global.include('routes/license/addToAccount.js');
    app.post(licenseAddToAccount.URL, licenseAddToAccount.route);

    var licenseActivateLicense = global.include('routes/license/activateLicense.js');
    app.post(licenseActivateLicense.URL, licenseActivateLicense.route);

    var licenseSaveLicenseOrder = global.include('routes/license/saveLicenseOrder.js');
    app.post(licenseSaveLicenseOrder.URL, licenseSaveLicenseOrder.route);

    var licenseGetLicenseOrder = global.include('routes/license/getLicenseOrder.js');
    app.post(licenseGetLicenseOrder.URL, licenseGetLicenseOrder.route);
    //!!LICENSE

    //PACKAGE
    var packageSave = global.include('routes/package/save.js');
    app.post(packageSave.URL, packageSave.route);

    var packageGetStatistics = global.include('routes/package/getStatistics.js');
    app.post(packageGetStatistics.URL, packageGetStatistics.route);

    var packageGetAvailable = global.include('routes/package/getAvailable.js');
    app.post(packageGetAvailable.URL, packageGetAvailable.route);

    var packageDelete = global.include('routes/package/delete.js');
    app.post(packageDelete.URL, packageDelete.route);

    var packageRemoveAccess = global.include('routes/package/removeAccess.js');
    app.post(packageRemoveAccess.URL, packageRemoveAccess.route);

    var packageStoreMeasurement = global.include('routes/package/storeMeasurement.js');
    app.post(packageStoreMeasurement.URL, packageStoreMeasurement.route);

    var packageStoreState = global.include('routes/package/storeState.js');
    app.post(packageStoreState.URL, packageStoreState.route);

    var packageSendTestEmail = global.include('routes/package/sendTestEmail.js');
    app.post(packageSendTestEmail.URL, packageSendTestEmail.route);


    //!!PACKAGE

    var sendDeviceAlert = global.include('routes/alert/sendDeviceAlert.js');
    app.post(sendDeviceAlert.URL, sendDeviceAlert.route);

    var alertUnsubscribeAllEmail = global.include('routes/alert/unsubscribeAllEmail.js');
    app.post(alertUnsubscribeAllEmail.URL, alertUnsubscribeAllEmail.route);

    var alertUnsubscribeFromPackageEmail = global.include('routes/alert/unsubscribeFromPackageEmail.js');
    app.post(alertUnsubscribeFromPackageEmail.URL, alertUnsubscribeFromPackageEmail.route);

    var alertEmail = global.include("routes/alert/sendEmail.js");
    app.post(alertEmail.URL, alertEmail.route);

    var confirmDeviceAlert = global.include("routes/alert/confirmDeviceAlert.js");
    app.post(confirmDeviceAlert.URL, confirmDeviceAlert.route);

    var watcherServerLogin = global.include("routes/watcherServer/login.js");
    app.post(watcherServerLogin.URL, watcherServerLogin.route);

    var watcherServerRegister = global.include("routes/watcherServer/register.js");
    app.post(watcherServerRegister.URL, watcherServerRegister.route);

    var watcherServerRequestSynchronization = global.include('routes/watcherServer/requestSynchronization.js');
    app.post(watcherServerRequestSynchronization.URL, watcherServerRequestSynchronization.route);

    //API
    var apiGetToken = global.include("routes/api/getToken.js");
    app.post(apiGetToken.URL, apiGetToken.route);

    var apiPackageGetAll = global.include("routes/api/package/getAll.js");
    app.post(apiPackageGetAll.URL, apiPackageGetAll.route);

    var apiPackageDelete = global.include("routes/api/package/delete.js");
    app.post(apiPackageDelete.URL, apiPackageDelete.route);

    var apiPackageGetPackage = global.include("routes/api/package/getPackage.js");
    app.post(apiPackageGetPackage.URL, apiPackageGetPackage.route);

    var apiPackageGetVersion = global.include("routes/api/package/getVersion.js");
    app.post(apiPackageGetVersion.URL, apiPackageGetVersion.route);

    var apiPackageSave = global.include("routes/api/package/save.js");
    app.post(apiPackageSave.URL, apiPackageSave.route);
    //!API

    for (var i = adminRoutesEntryLastIndex; i < app._router.stack.length; i++) {
        if (app._router.stack[i].hasOwnProperty('route')) {
            LOG.info(app._router.stack[i].route.path);
        }
    }
    LOG.info("Public Routes initialized.");
    // ERROR HANDLER !!MUST!! BE INITIALIZED AFTER ROUTES ADDED
    app.use(function (err, req, res, next) {
        if (err) {
            if (err.stack.indexOf("at IncomingMessage.onAborted") != -1) { // do not send email about this kind of error.
                return res.send(new ErrorResponse(new MSInternalServerError().logMessage("OnAborted catch.").logNow()));
            }
            LOG.critical(err,"Error inside route.",function(errorUUID:string){
                return res.send(new ErrorResponse(new MSInternalServerError().details(errorUUID).logNow()));
            });    
        }
    });
}

export function setupExpressApp (finishCallback) {
    app.use(bodyParser.urlencoded({ extended: true }));
    app.use(bodyParser.json());
    app.use(connect.logger('dev')); //if not works after update, it was renamed to morgan
    return finishCallback();
}

global.myRequestValidation = function (parameters, req, res, callback) {
    var clonedRequestBody = JSON.parse(JSON.stringify(req.body));
    global.utils.object.sanitizePropertyValuesByKey(["password"], clonedRequestBody);

    LOG.info("Incoming request. URL:" + req.url + " .Body :" + JSON.stringify(clonedRequestBody));
    // LOG.info("Incoming request. URL:" + req.url + " .Body :" + JSON.stringify(req.body));
    for (var property in parameters) {
        if (property.indexOf('OPT') > -1) { // skip optional parameters
            continue;
        }
        if (property.indexOf('restriction') != -1) { // skip route restrictions
            continue;
        }
        if (global.utils.object.isNull(parameters[property])) {
            requestStat.addInvalidRequest(req.url, req.body, function (err) {
                if (err) {
                    LOG.error("Cannot save invalid request due to" + err);
                }
            });
            var errorUUID = global.utils.string.generateUUID();
            LOG.error("Bad format request. Missing parameters: " + property + " Url: " + req.url + " " + JSON.stringify(req.body) + errorUUID);

            res.send(400);
            return callback("ERROR", errorUUID);
        }
    }
    if (global.utils.object.isNull(parameters.restriction)) { // there is no restriction on this api route simply answer
        return callback();
    }
    inMemoryDB.isKeyExists(parameters.restriction.UUID, function (isExists) {
        if (isExists) {
            LOG.error("Too much request from : " + req.connection.remoteAddress);
            res.send(429);
            return callback("TOO_FREQUENT_REQUEST");
        }

        inMemoryDB.addKey(parameters.restriction.UUID, parameters.restriction.coolDownSec, function () {
            return callback();
        });
    });


}
