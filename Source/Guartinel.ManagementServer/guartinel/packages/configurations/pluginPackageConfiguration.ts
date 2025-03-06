import { MSError } from "../../../error/Errors";
import { IConfigurable } from "../package";

export abstract class PluginPackageConfigurationBase implements IConfigurable {
   abstract initFromObject(object: any): MSError;
   abstract createFromJSON(json: any): MSError;
   abstract updateFromJSON(json: any): MSError;
   abstract getPackagePartCount(): number;
   abstract maskSensitiveInfo();
   toJSON() { return this; }

   static SENSITIVE_INFO_MASK = "SENSITIVE_INFO_MASK";
   timeout_in_seconds: number;//TODO do not forget to set!
   getTimeoutInterval(checkIntervalSeconds) {
      var timeoutInterval = checkIntervalSeconds * 1.2;
      const ONE_MINUTE = 60;
      if (timeoutInterval < ONE_MINUTE) {
         timeoutInterval = ONE_MINUTE;
      }
      return timeoutInterval;
   }
}
