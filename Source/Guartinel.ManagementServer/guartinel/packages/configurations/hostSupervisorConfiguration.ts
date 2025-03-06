import { PluginPackageConfigurationBase } from "./pluginPackageConfiguration";
import { MSError } from "../../../error/Errors";
import { Const } from "../../../common/constants";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export class Host {
   address: string;
   caption: string;
}

export class HostSupervisorConfiguration extends PluginPackageConfigurationBase {
   hosts: Array<string> = new Array<string>();
   detailed_hosts: Array<Host> = new Array<Host>();
   retry_count;

   initFromObject(object: any): MSError {
      if (!utils.object.isNull(object.hosts)) {
         this.hosts = object.hosts;
      }
      if (!utils.object.isNull(object.detailed_hosts)) {
         this.detailed_hosts = object.detailed_hosts;
      }
    
      //lets copy all hosts to detailed hosts
      for (var hostsIndex = 0; hostsIndex < this.hosts.length; hostsIndex++) {
         var found = false;
         for (var detailedHostsIndex = 0; detailedHostsIndex < this.detailed_hosts.length; detailedHostsIndex++) {
            if (this.detailed_hosts[detailedHostsIndex].address == this.hosts[hostsIndex]) {
               found = true;
               break;
            }
         }
         if (!found) {
            this.detailed_hosts.push({
               address: this.hosts[hostsIndex],
               caption: this.hosts[hostsIndex]
            });
         }
      }
      this.retry_count = object.retry_count;

      return null;
   }


   createFromJSON(json: any): MSError {
  if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.RETRY_COUNT)])) {
         this.retry_count = json[Const.pluginConstants.RETRY_COUNT];
      }
      if (utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.DETAILED_HOSTS)]) && utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.HOSTS)])) {
         return new MSError("You must provide either hosts or detailed_hosts");
      }    

      //hosts driven only possible from the API
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.HOSTS)])) {
         this.hosts = json[Const.pluginConstants.HOSTS];
         for (var hostsIndex = 0; hostsIndex < this.hosts.length; hostsIndex++) {
            this.detailed_hosts.push({
               address: this.hosts[hostsIndex],
               caption: this.hosts[hostsIndex]
            });
         }
         return null;
      }

      //basic usecase when website builds the JSON
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.DETAILED_HOSTS)])) {
         this.detailed_hosts = json[Const.pluginConstants.DETAILED_HOSTS];

         for (var detailedHostsIndex = 0; detailedHostsIndex < this.detailed_hosts.length; detailedHostsIndex++) {
            this.hosts.push(this.detailed_hosts[detailedHostsIndex].address);
         }
         return null;
      }  
      return null; // everything is OK!
   }

   updateFromJSON(json: any): MSError {
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.RETRY_COUNT)])) {
         this.retry_count = json[Const.pluginConstants.RETRY_COUNT];
      }

    //basic usecase when website builds the JSON
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.DETAILED_HOSTS)])) {
         this.detailed_hosts = json[Const.pluginConstants.DETAILED_HOSTS];
         this.hosts = [];
         for (var detailedHostsIndex = 0; detailedHostsIndex < this.detailed_hosts.length; detailedHostsIndex++) {
            this.hosts.push(this.detailed_hosts[detailedHostsIndex].address);
         }
         return null;
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.HOSTS)])) {
         this.hosts = json[Const.pluginConstants.HOSTS];
         this.detailed_hosts = [];
         for (var hostsIndex = 0; hostsIndex < this.hosts.length; hostsIndex++) {
            this.detailed_hosts.push({
               address: this.hosts[hostsIndex],
               caption: this.hosts[hostsIndex]
            });
         }
         return null;
      }     
      return null; // everything is OK!
   }

   getPackagePartCount(): number {
      return this.detailed_hosts.length;
   }
   maskSensitiveInfo() { }

}