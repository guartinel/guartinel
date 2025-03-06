import { PluginPackageConfigurationBase } from "./pluginPackageConfiguration";
import { MSError } from "../../../error/Errors";
import { Const } from "../../../common/constants";
import { LOG } from "../../../diagnostics/LoggerFactory";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export class Website {
   address: string;
   caption: string;
}
export class WebsiteSupervisorConfiguration extends PluginPackageConfigurationBase {
   websites: Array<string> = new Array<string>(); // legacy  variable
   detailed_websites: Array<Website> = new Array<Website>();
   check_certificate_minimum_days = null;
   check_load_time_seconds = null;
   check_text_pattern = null;
   retry_count = null;
   initFromObject(configObject: any): MSError {
      //legacy code changed to detailed_websites   
      if (!utils.object.isNull(configObject.websites)) {
         this.websites = configObject.websites;
      }
      if (!utils.object.isNull(configObject.detailed_websites)) {
         this.detailed_websites = configObject.detailed_websites;
      }

      //lets copy all websites to detailed websites
      for (var websiteIndex = 0; websiteIndex < this.websites.length; websiteIndex++) {
         var found = false;
         for (var detailedWebsitesIndex = 0; detailedWebsitesIndex < this.detailed_websites.length; detailedWebsitesIndex++) {
            if (this.detailed_websites[detailedWebsitesIndex].address == this.websites[websiteIndex]) {
               found = true;
               break;
            }
         }
         if (!found) {
            this.detailed_websites.push({
               address: this.websites[websiteIndex],
               caption: this.websites[websiteIndex]
            });
         }
      }


      this.check_load_time_seconds = configObject.check_load_time_seconds;
      this.check_certificate_minimum_days = configObject.check_certificate_minimum_days;
      this.check_text_pattern = configObject.check_text_pattern;
      this.retry_count = configObject.retry_count;
      return null;
   }

   createFromJSON(json: any): MSError {
      if (utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.DETAILED_WEBSITES)]) && utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.WEBSITES)])) {
         return new MSError("You must provide either websites or detailed_websites");
      }  
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.CHECK_CERTIFICATE_MINIMUM_DAYS)])) {
         this.check_certificate_minimum_days = json[traceIfNull(Const.pluginConstants.CHECK_CERTIFICATE_MINIMUM_DAYS)];
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.CHECK_LOAD_TIME_SECONDS)])) {
         this.check_load_time_seconds = json[traceIfNull(Const.pluginConstants.CHECK_LOAD_TIME_SECONDS)];
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.CHECK_TEXT_PATTERN)])) {
         this.check_text_pattern = json[traceIfNull(Const.pluginConstants.CHECK_TEXT_PATTERN)];
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.RETRY_COUNT)])) {
         this.retry_count = json[Const.pluginConstants.RETRY_COUNT];
      }

       //websites driven only possible from the API
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.WEBSITES)])) {
         this.websites = json[Const.pluginConstants.WEBSITES];
         for (var websiteIndex = 0; websiteIndex < this.websites.length; websiteIndex++) {
            this.detailed_websites.push({
               address: this.websites[websiteIndex],
               caption: this.websites[websiteIndex]
            });
         }
         return null;
      }

      //basic usecase when website builds the JSON
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.DETAILED_WEBSITES)])) {
         this.detailed_websites = json[Const.pluginConstants.DETAILED_WEBSITES];

         for (var detailedWebsitesIndex = 0; detailedWebsitesIndex < this.detailed_websites.length; detailedWebsitesIndex++) {
            this.websites.push(this.detailed_websites[detailedWebsitesIndex].address);
         }
         return null;
      } 
      return null; // everything is OK!
   }

   updateFromJSON(json: any): MSError {    
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.CHECK_CERTIFICATE_MINIMUM_DAYS)])) {
         this.check_certificate_minimum_days = json[traceIfNull(Const.pluginConstants.CHECK_CERTIFICATE_MINIMUM_DAYS)];
      } else {
         this.check_certificate_minimum_days = null;
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.CHECK_LOAD_TIME_SECONDS)])) {
         this.check_load_time_seconds = json[traceIfNull(Const.pluginConstants.CHECK_LOAD_TIME_SECONDS)];
      } else {
         this.check_load_time_seconds = null;
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.CHECK_TEXT_PATTERN)])) {
         this.check_text_pattern = json[traceIfNull(Const.pluginConstants.CHECK_TEXT_PATTERN)];
      } else {
         this.check_text_pattern = null;
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.RETRY_COUNT)])) {
         this.retry_count = json[traceIfNull(Const.pluginConstants.RETRY_COUNT)];
      } else {
         this.retry_count = null;
      } 

      //basic usecase when website builds the JSON
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.DETAILED_WEBSITES)])) {
         this.detailed_websites = json[Const.pluginConstants.DETAILED_WEBSITES];
         this.websites = [];
         for (var detailedWebsitesIndex = 0; detailedWebsitesIndex < this.detailed_websites.length; detailedWebsitesIndex++) {
            this.websites.push(this.detailed_websites[detailedWebsitesIndex].address);
         }
         return null;
      }
      //websites driven only possible from the API
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.WEBSITES)])) {
         this.websites = json[Const.pluginConstants.WEBSITES];
         this.detailed_websites = [];
         for (var websiteIndex = 0; websiteIndex < this.websites.length; websiteIndex++) {
            this.detailed_websites.push({
               address: this.websites[websiteIndex],
               caption: this.websites[websiteIndex]
            });
         }
         return null;
      }
      return null; // everything is OK!
   }

   getPackagePartCount(): number {
      return this.detailed_websites.length;
   }
   maskSensitiveInfo() { }

}