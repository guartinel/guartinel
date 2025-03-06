import { Const } from "../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import commonConstants = Const.commonConstants;
import pluginConstants = Const.pluginConstants;

import { ErrorResponse, SuccessResponse } from "../../guartinel/response/Response";
import { MSInternalServerError } from "../../error/Errors";
import { LOG } from "../../diagnostics/LoggerFactory";
import { Package } from "../../guartinel/packages/package";
import * as securityTool from "../../guartinel/security/tool";
import { isNullOrUndefined, isNull, debug, debuglog } from "util";

var sessionManager = global.include("guartinel/security/sessionManager.js");
var database = global.include("guartinel/database/public/databaseConnector.js");
var alertMessageBuilder = global.include("guartinel/utils/alertMessageBuilder.js");


let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

var isRouteDebugEnabled = false;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(URL + " " + message);
}


export let URL = traceIfNull(managementServerUrls.PACKAGE_GET_AVAILABLE);
export function route(req, res) {
    var parameters = {
        token: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.TOKEN)]
    };

    global.myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doGetAvailable(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}

function doGetAvailable(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (sessionErr, account) {
        if (sessionErr) {
            return callback(new ErrorResponse(sessionErr));
        }

        if (account.isDebug) {
            isRouteDebugEnabled = true;
        } else {
            isRouteDebugEnabled = false;
        }

        debugRoute(account.email + "=> starting");
        var DTOPackages = [];
        var interestedAccounts = [];

        addAccessiblePackagesToDTOList();

        function addAccessiblePackagesToDTOList() {
            debugRoute(account.email + "ADDING ACCESSIBLE PACKAGES");
            debugRoute(account.email + "=>Getting device from the packages shared with me where this account is a guest");
            var ownerDevices = account.devices;
            database.updateTokenForBrowserSession(parameters.token, function (err) {
                if (err) {
                    var error = new MSInternalServerError()
                        .logMessage(URL + ": Cannot updateTokenForBrowserSession")
                        .severe()
                        .innerError(err)
                        .logNow();
                    return callback(new ErrorResponse(error));
                }
                var packageIdsAsObjectIds = [];
                account.accessiblePackageIds.forEach(function (item, index) {
                    packageIdsAsObjectIds.push(database.toObjectId(item));
                });
                debugRoute(account.email + "=>Getting other accounts packages from accessible packages list: " +
                    JSON.stringify(account.accessiblePackageIds));
                database.getAccountModel().find({ 'packages._id': { $in: packageIdsAsObjectIds } }, afterDatabaseQuery);


                function afterDatabaseQuery(err, accounts) {
                    if (err) {
                        var error = new MSInternalServerError()
                            .logMessage(URL + ": Cannot get connected packages")
                            .severe()
                            .innerError(err)
                            .logNow();
                        return callback(new ErrorResponse(error));
                    }
                    debugRoute(account.email + "=> List of accounts : " + JSON.stringify(accounts));
                    //filter out the packages that are not accessible by the original account 
                    for (var i = 0; i < accounts.length; i++) {
                        var accountItem = accounts[i];
                        for (var j = 0; j < accountItem.packages.length; j++) {
                            var packageItem = accountItem.packages[j];
                            if (account.accessiblePackageIds.indexOf(packageItem._id) === -1 || packageItem.isDeleted) {
                                continue;
                            }
                            debugRoute(account.email + "=>Adding devices from account : +" + accountItem.email);
                            var packageDTO = createPackageDTO(accountItem, accountItem.packages[j]);
                            packageDTO.license = accountItem.getLicenseAggregate();

                            if (!isNull(ownerDevices) && !isNull(accountItem.devices)) {
                                for (var m = 0; m < ownerDevices.length; m++) {
                                    // accountItem.devices.push(ownerDevices[m]);
                                    packageDTO.devices.push(createDTOFromDeviceDBO(account.email, ownerDevices[m]));
                                }
                            }
                            DTOPackages.push(packageDTO);
                        }
                    }
                    addOwnerPackagesToDTO();
                }
            });
        }

        function addOwnerPackagesToDTO() {
            debugRoute(account.email + "ADDING OWNED PACKAGES");
            for (var i = 0; i < account.packages.length; i++) {
                if (account.packages[i].isDeleted) {
                    continue;
                }
                var currentPackage = account.packages[i];
                //add every interested account only ONCE to a list
                for (var accessIndex = 0; accessIndex < currentPackage.access.length; accessIndex++) {
                    var found = false;
                    for (var interestedAccountIndex = 0; interestedAccountIndex < interestedAccountIndex; interestedAccountIndex++) {
                        if (account.access[accessIndex] == interestedAccounts[interestedAccountIndex]) {
                            found = true;
                            break;
                        }
                    }
                    if (!found && currentPackage.access[accessIndex].packageUserEmail != account.email) { // do not add owner to the list..
                        interestedAccounts.push(currentPackage.access[accessIndex].packageUserEmail);
                    }
                }
                DTOPackages.push(createPackageDTO(account, currentPackage));
            }
            debugRoute(account.email + "=>DTOPackages of the owner:  " + JSON.stringify(DTOPackages));
            debugRoute(account.email + "=>Guest access account list : " + interestedAccounts.join(','));

            var encryptedAccounts = [];
            //encrypt the interested accounts for the DB query
            for (var interestedAccountIndex = 0; interestedAccountIndex < interestedAccounts.length; interestedAccountIndex++) {
                encryptedAccounts.push(securityTool.encryptText(interestedAccounts[interestedAccountIndex]));
            }
            debugRoute(account.email + "=>Guest access encrypted list : " + encryptedAccounts.join(','));

            //get guest access users all device 
            database.getNativeAccountsConnection().find({ "email": { $in: encryptedAccounts } }, { "devices": 1, "email": 1 }).toArray(addGuestAccessDevicesToDTO);
        }

        //add them to the related packages after retrieved
        function addGuestAccessDevicesToDTO(err, accounts, other) {
            debugRoute(account.email + "ADDING GUEST DEVICES TO PACKAGES");
            debugRoute(account.email + "=>Retrieved all account which are included in the owner packages access list");
            debugRoute(account.email + "=>Error: " + err);

            //iterate the currently adedd packages
            for (var dtoPackageIndex = 0; dtoPackageIndex < DTOPackages.length; dtoPackageIndex++) {
                var currentPackage = DTOPackages[dtoPackageIndex];
                debugRoute(account.email + "=> DTO package: " + currentPackage.package_name);

                //iterate the access list of the current item of the DTO list
                for (var accessIndex = 0; accessIndex < currentPackage.access.length; accessIndex++) {
                    var currentAccess = currentPackage.access[accessIndex];
                    debugRoute(account.email + "=> DTO package: " + currentPackage.package_name + " access: " + currentAccess.packageUserEmail);

                    //finding the account which is matching with the access item email
                    for (var accountIndex = 0; accountIndex < accounts.length; accountIndex++) {
                        var currentAccount = accounts[accountIndex];
                        var currentAcountEmail = securityTool.decryptText(currentAccount.email);
                        debugRoute(account.email + "=>package access user : " + currentAcountEmail);
                        if (currentAccess.packageUserEmail != currentAcountEmail) {
                            //  debugRoute(account.email + "=>Skipping access user because it is not matching");
                            continue;
                        }

                        debugRoute(account.email + "=>Checking devices from access user");
                        for (var deviceIndex = 0; deviceIndex < currentAccount.devices.length; deviceIndex++) {
                            var currentDevice = currentAccount.devices[deviceIndex];
                            if (isNull(currentPackage.devices)) {
                                currentPackage.devices = [];
                            }
                            var isAlreadyAdded = false;
                            //check if device is already added
                            for (var addedDeviceIndex = 0; addedDeviceIndex < currentPackage.devices.length; addedDeviceIndex++) {
                                if (currentPackage.devices[addedDeviceIndex].id.toString() == currentDevice._id.toString()) {
                                    isAlreadyAdded = true;
                                    break;
                                }
                            }
                            if (isAlreadyAdded) {
                                debugRoute("Device already added" + currentDevice.name);
                                continue;
                            }

                            var deviceDTO = createDTOFromDeviceDBO(securityTool.decryptText(currentAccount.email), currentDevice);
                            currentPackage.devices.push(deviceDTO);
                        }
                    }
                }
            }
            sendResponse(); //TODO
        }
        function sendResponse() {
            debugRoute("Final list of packages: " + JSON.stringify((DTOPackages)));
            var response = new SuccessResponse();
            response[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGES)] = DTOPackages;
            return callback(response);
        }

    });
}

function createPackageDTO(accountP, packageDBO) {
    let packageObject = new Package(true, accountP);
    packageObject.initFromObject(packageDBO);
    debugRoute("Creating package DTO for account: " + accountP.email + " Package name: " + packageObject.packageName);

    let resultJSONObject = packageObject.toJSON();
    resultJSONObject[traceIfNull(Const.commonConstants.ALL_PARAMETERS.DEVICES)] = getDevicesForPackage(accountP, packageDBO);
    debugRoute("Devices for account  " + accountP.email + " package : " + packageDBO.packageName + " devices: " + JSON.stringify(resultJSONObject.devices));
    var state = packageDBO.state;
    if (!utils.object.isNullOrEmpty(packageDBO.state)) {
        state = migratePackageStates(accountP, packageDBO);
    }
    resultJSONObject[traceIfNull(Const.commonConstants.ALL_PARAMETERS.STATE)] = state;

    if (!utils.object.isNullOrEmpty(packageDBO.measurements)) {
        resultJSONObject[traceIfNull(commonConstants.ALL_PARAMETERS.LAST_MEASUREMENT)] = packageDBO.measurements[0].data;
        resultJSONObject[traceIfNull(commonConstants.ALL_PARAMETERS.LAST_MEASUREMENT_TIMESTAMP)] = packageDBO.measurements[0].timeStamp;
    }
    
    resultJSONObject[traceIfNull(commonConstants.ALL_PARAMETERS.CHECKSUM)] = securityTool.getCheckSum(JSON.stringify(resultJSONObject));
    return resultJSONObject;
}

function migratePackageStates(account, packageDBO) { // TODO MIGRATION.
    var migratedState = {
        states: [],
        message_details_built: "",
        message_built: "",
        name: "",
        timeStamp: ""
    };
    for (var index = 0; index < packageDBO.state.states.length; index++) {
        if (utils.string.isNullOrEmpty(packageDBO.state.states[index].package_part_message_built)) {
            migratedState.states.push({
                package_part_message_built: alertMessageBuilder.build(account, packageDBO.state.states[index].package_part_message),
                package_part_details_built: alertMessageBuilder.build(account, packageDBO.state.states[index].package_part_details),
                package_part_extract_built: alertMessageBuilder.build(account, packageDBO.state.states[index].package_part_extract)
            });
        } else {
            migratedState.states.push({
                package_part_message_built: packageDBO.state.states[index].package_part_message_built,
                package_part_details_built: packageDBO.state.states[index].package_part_details_built,
                package_part_extract_built: packageDBO.state.states[index].package_part_extract_built,
                package_part_state: packageDBO.state.states[index].package_part_state,
                package_part_identifier: packageDBO.state.states[index].package_part_identifier
            });
        }
    }

    if (utils.string.isNullOrEmpty(packageDBO.state.message_details_built)) {
        migratedState.message_details_built = alertMessageBuilder.build(account, packageDBO.state.message_details);
    } else {
        migratedState.message_details_built = packageDBO.state.message_details_built;
    }

    if (utils.string.isNullOrEmpty(packageDBO.state.message_built)) {
        migratedState.message_built = alertMessageBuilder.build(account, packageDBO.state.message);
    } else {
        migratedState.message_built = alertMessageBuilder.build(account, packageDBO.state.message_built);
    }
    migratedState.name = packageDBO.state.name;
    migratedState.timeStamp = packageDBO.state.timeStamp;
    //  debugRoute("Package state after migration: " + JSON.stringify(migratedState));
    return migratedState;
}

function getDevicesForPackage(accountD, packageDBO) {
    var result = [];
    var addedDeviceIds = [];
    /*  packageDBO.alertDeviceIds.forEach(function (id, index) {
         var deviceDBO;
         if (isNullOrUndefined(accountD.devices)) {
            LOG.error("Account.devices is null.");
            return;
         }
         accountD.devices.forEach(function (device, index) {
            if (isNullOrUndefined(device)) {
               LOG.error("Device is null.");
               return;
            }       
            if (device._id.toString() == id) {
               deviceDBO = device;
            }
         });
         if (utils.object.isNull(deviceDBO)) {
            return;
         }
         addedDeviceIds.push(deviceDBO._id.toString());
         result.push(createDTOFromDeviceDBO(accountD.email, deviceDBO));
      });*/
    if (isNullOrUndefined(accountD.devices)) {
        LOG.error("Account.devices is null.");
        return;
    }
    accountD.devices.forEach(function (device, index) {
        if (isNullOrUndefined(device)) {
            LOG.error("Device is null.");
            return;
        }
        result.push(createDTOFromDeviceDBO(accountD.email, device));
    });
    debugRoute("Devices from account : " + accountD.email + " => " + JSON.stringify(result));
    return result;
}
function createDTOFromDeviceDBO(ownerEmail, deviceDBO) {
    debugRoute("Creating device DBO . Owner email: " + ownerEmail + " device name : " + deviceDBO.name);
    var deviceDTO = {};
    deviceDTO[traceIfNull(commonConstants.ALL_PARAMETERS.DEVICE_TYPE)] = deviceDBO.deviceType;
    deviceDTO[traceIfNull(commonConstants.ALL_PARAMETERS.NAME)] = deviceDBO.name;
    deviceDTO[traceIfNull(commonConstants.ALL_PARAMETERS.ID)] = deviceDBO._id.toString();
    deviceDTO[traceIfNull(commonConstants.ALL_PARAMETERS.OWNER)] = ownerEmail;
    deviceDTO[traceIfNull(commonConstants.ALL_PARAMETERS.INSTANCE_ID)] = deviceDBO.instanceId;
    deviceDTO[traceIfNull(pluginConstants.HARDWARE_TYPE)] = deviceDBO.hardwareType;
    return deviceDTO;
}
