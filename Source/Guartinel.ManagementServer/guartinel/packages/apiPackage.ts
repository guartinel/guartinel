import { Package } from "./package";
import { IConfigurable } from "./package";
import { MSError } from "../../error/Errors";
import { Const } from "../../common/constants";

import commonConstants = Const.commonConstants;
import plugins = Const.plugins;

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;
export class APIPackage extends Package implements IConfigurable {
   constructor(onlyIsEnabledChangeable: boolean, account) {
      super(onlyIsEnabledChangeable, account);
   }

   createFromJSON(json): MSError {
      let error = super.createFromJSON(json);
      if (error != null) {
         return error;
      }
      if (!utils.object.isNull(json[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_DEVICES)])) {
         this.alertDeviceIds = this.getDeviceIdsFromNames(json.pack[traceIfNull(commonConstants.ALL_PARAMETERS.ALERT_DEVICES)], this.account);
      }
      return null;
   }

   updateFromJSON(json): MSError {
      let error = super.updateFromJSON(json);
      if (error != null) {
         return error;
      }

      if (!utils.object.isNull(json[traceIfNull(Const.commonConstants.ALL_PARAMETERS.ALERT_DEVICES)])) {
         this.alertDeviceIds = this.getDeviceIdsFromNames(json.pack[traceIfNull(commonConstants.ALL_PARAMETERS.ALERT_DEVICES)], this.account);
      }

      let parameterCount = Object.keys(json).length;
      if (!utils.object.isNull(json[traceIfNull(commonConstants.ALL_PARAMETERS.IS_ENABLED)])) {
         parameterCount--; // remove isenabled change
      }

      if (!utils.object.isNull(json[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)])) {
         parameterCount--; // remove package name parameter
      }

      if (parameterCount > 0) {
         this.version++; // increase version now 
      }
      return null;
   };

   toJSON() {
      let packageJSON = super.toJSON();
      packageJSON[traceIfNull(commonConstants.ALL_PARAMETERS.VERSION)] = this.version;
      packageJSON[traceIfNull(commonConstants.ALL_PARAMETERS.ALERT_DEVICES)] = this.getDevicesNamesFromIds(this.alertDeviceIds, this.account);

      delete packageJSON[Const.commonConstants.ALL_PARAMETERS.ALERT_DEVICE_IDS];
      delete packageJSON[Const.commonConstants.ALL_PARAMETERS.ID];

      return packageJSON;
   }

   getDevicesNamesFromIds(ids, account) {
      var result = [];
      for (var index = 0; index < ids.length; index++) {
         var device = account.devices.id(ids[index]);
         result.push(device.name);
      }
      return result;
   }

   getDeviceIdsFromNames(deviceNames, account) {
      var resultArray = [];
      for (var index = 0; index < deviceNames.length; index++) {
         var currentDeviceName = deviceNames[index];
         for (var index2 = 0; index2 < account.devices.length; index2++) {
            var currentDevice = account.devices[index2];
            if (currentDeviceName === currentDevice.name) {
               resultArray.push(currentDevice._id.toString());
               break;
            }
         }
      }
      return resultArray;
   }

   initFromObject(object): MSError {
      super.initFromObject(object);
      return null;
   };
}