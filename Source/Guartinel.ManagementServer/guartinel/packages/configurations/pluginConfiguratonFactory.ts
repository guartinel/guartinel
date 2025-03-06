import { ApplicationSupervisorConfiguration } from "./applicationSupervisorConfiguration";
import { WebsiteSupervisorConfiguration } from "./websiteSupervisorConfiguration";
import { HostSupervisorConfiguration } from "./hostSupervisorConfiguration";
import { EmailSupervisorConfiguration } from "./EmailSupervisorConfiguration";
import { HardwareSupervisorConfiguration } from "./hardwareSupervisorConfiguration";
import { Const } from "../../../common/constants";
import {PluginPackageConfigurationBase} from "./pluginPackageConfiguration";

export default class PluginConfigurationFactory {
   static create(packageType: string): PluginPackageConfigurationBase {
      switch (packageType) {
      case Const.plugins.ALL_PACKAGE_TYPE_VALUES.APPLICATION_SUPERVISOR:
         return new ApplicationSupervisorConfiguration();
      case Const.plugins.ALL_PACKAGE_TYPE_VALUES.WEBSITE_SUPERVISOR:
         return new WebsiteSupervisorConfiguration();
      case Const.plugins.ALL_PACKAGE_TYPE_VALUES.HOST_SUPERVISOR:
         return new HostSupervisorConfiguration();    
      case Const.plugins.ALL_PACKAGE_TYPE_VALUES.EMAIL_SUPERVISOR:
         return new EmailSupervisorConfiguration();
      case Const.plugins.ALL_PACKAGE_TYPE_VALUES.HARDWARE_SUPERVISOR:
         return new HardwareSupervisorConfiguration();
      default:
         throw new Error("Cannot find package configuration for package type: " + packageType);
      }
   }
}