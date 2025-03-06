import { PluginPackageConfigurationBase } from "./pluginPackageConfiguration";
import { MSError } from "../../../error/Errors";
import { Const } from "../../../common/constants";
import * as securityTool from "../../security/tool";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;


export class ApplicationSupervisorConfiguration extends PluginPackageConfigurationBase {
   application_token: string;
   instances = [];
   constructor() {
      super();
   }

   initFromObject(object: any): MSError {
      this.instances = object.instances;
      this.application_token = object.application_token;
      return null;
   }

   createFromJSON(json: any): MSError {
      // instances are not obligatory so we wont raise error if it is null
      if (!utils.object.isNull(json[traceIfNull(Const.pluginConstants.INSTANCES)])) {
         this.instances = json[Const.pluginConstants.INSTANCES];
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.APPLICATION_TOKEN)])) {
         this.application_token = json[Const.pluginConstants.APPLICATION_TOKEN];
      } else {
         this.application_token = securityTool.generateToken();
      }
       return null; // everything is OK!
   }

   updateFromJSON(json: any): MSError {
      if (!utils.object.isNull(json[traceIfNull(Const.pluginConstants.INSTANCES)])) {
         this.instances = json[Const.pluginConstants.INSTANCES];
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.APPLICATION_TOKEN)])) {
         this.application_token = json[Const.pluginConstants.APPLICATION_TOKEN];
      }
      return null; // everything is OK!
   }

   hasInstance(instanceId) {
      for (let instanceIndex = 0; instanceIndex < this.instances.length; instanceIndex++) {
         if (this.instances[instanceIndex].id === instanceId) {
            return true;
         }
      }
      return false;
   }
   addInstance(instanceId, instanceName, isHeartBeat) {
      this.instances.push({
         name: instanceName,
         id: instanceId,
         is_heartbeat: isHeartBeat
      });
   }
   updateInstance(instanceId, instanceName, isHeartBeat) {
      for (let instanceIndex = 0; instanceIndex < this.instances.length; instanceIndex++) {
         if (this.instances[instanceIndex].id === instanceId) {
            this.instances[instanceIndex].name = instanceName;
            this.instances[instanceIndex].is_heartbeat = isHeartBeat;
            break;
         }
      }
   }

   getPackagePartCount(): number {
      if (utils.object.isNull(this.instances)) {
         return 0;
      }
      return this.instances.length;
   }
   maskSensitiveInfo() { }

}