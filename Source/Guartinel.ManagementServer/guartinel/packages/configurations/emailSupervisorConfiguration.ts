import { PluginPackageConfigurationBase } from "./pluginPackageConfiguration";
import { MSError } from "../../../error/Errors";
import { Const } from "../../../common/constants";
import { LOG } from "../../../diagnostics/LoggerFactory";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

class Imap {
   server_address: string;
   server_port: number;
   user: string;
   use_ssl: boolean;
   password: string;
}
class Smtp {
   server_address: string;
   server_port: number;
   user: string;
   use_ssl: boolean;
   password: string;
}

export class EmailSupervisorConfiguration extends PluginPackageConfigurationBase {
   imap: Imap;
   smtp: Smtp;
   test_email_address: string;

   initFromObject(config: any): MSError {
      this.imap = new Imap();
      this.imap.server_address = config.imap.server_address;
      this.imap.server_port = config.imap.server_port;
      this.imap.user = config.imap.user;
      this.imap.use_ssl = config.imap.use_ssl;
      this.imap.password = config.imap.password;

      this.smtp = new Smtp();
      this.smtp.server_address = config.smtp.server_address;
      this.smtp.server_port = config.smtp.server_port;
      this.smtp.user = config.smtp.user;
      this.smtp.use_ssl = config.smtp.use_ssl;
      this.smtp.password = config.smtp.password;
      this.test_email_address = config.test_email_address;
      return null;
   }

   createFromJSON(json: any): MSError {
      this.smtp = new Smtp();
      this.imap = new Imap();
      let smptpObject;
      //SMTP
      if (utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.SMTP)])) {
         return new MSError("Smtp is missing from configuration");
      }
      smptpObject = json[Const.pluginConstants.SMTP];

      if (utils.string.isNullOrEmpty(smptpObject[traceIfNull(Const.pluginConstants.SERVER_ADDRESS)])) {
         return new MSError("Smtp.server_address is missing from configuration");
      }
      this.smtp.server_address = smptpObject[traceIfNull(Const.pluginConstants.SERVER_ADDRESS)];

      if (utils.string.isNullOrEmpty(smptpObject[traceIfNull(Const.pluginConstants.SERVER_PORT)])) {
         return new MSError("Smtp.server_port is missing from configuration");
      }
      this.smtp.server_port = smptpObject[traceIfNull(Const.pluginConstants.SERVER_PORT)];

      if (utils.string.isNullOrEmpty(smptpObject[traceIfNull(Const.pluginConstants.USER)])) {
         return new MSError("Smtp.user is missing from configuration");
      }
      this.smtp.user = smptpObject[traceIfNull(Const.pluginConstants.USER)];

      if (utils.string.isNullOrEmpty(smptpObject[traceIfNull(Const.pluginConstants.PASSWORD)])) {
         return new MSError("Smtp.password is missing from configuration");
      }
      this.smtp.password = smptpObject[traceIfNull(Const.pluginConstants.PASSWORD)];

      if (utils.string.isNullOrEmpty(smptpObject[traceIfNull(Const.pluginConstants.USE_SSL)])) {
         return new MSError("Smtp.use_ssl is missing from configuration");
      }
      this.smtp.use_ssl = smptpObject[traceIfNull(Const.pluginConstants.USE_SSL)];


      let imapObject;
      //IMAP
      if (utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.IMAP)])) {
         return new MSError("Imap is missing from configuration");
      }

      imapObject = json[Const.pluginConstants.IMAP];

      if (utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.SERVER_ADDRESS)])) {
         return new MSError("Imap.server_address is missing from configuration");
      }
      this.imap.server_address = imapObject[traceIfNull(Const.pluginConstants.SERVER_ADDRESS)];

      if (utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.SERVER_PORT)])) {
         return new MSError("Imap.server_port is missing from configuration");
      }
      this.imap.server_port = imapObject[traceIfNull(Const.pluginConstants.SERVER_PORT)];

      if (utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.USER)])) {
         return new MSError("Imap.user is missing from configuration");
      }
      this.imap.user = imapObject[traceIfNull(Const.pluginConstants.USER)];

      if (utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.PASSWORD)])) {
         return new MSError("Imap.password is missing from configuration");
      }
      this.imap.password = imapObject[traceIfNull(Const.pluginConstants.PASSWORD)];

      if (utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.USE_SSL)])) {
         return new MSError("Imap.use_ssl is missing from configuration");
      }
      this.imap.use_ssl = imapObject[traceIfNull(Const.pluginConstants.USE_SSL)];

      if (utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.TEST_EMAIL_ADDRESS)])) {
         return new MSError("test_email_address is missing from configuration");
      }
      this.test_email_address = json[Const.pluginConstants.TEST_EMAIL_ADDRESS];
      return null; // everything is OK!
   }

   updateFromJSON(json: any): MSError {
      let smtpObject;
      //SMTP
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.SMTP)])) {
         smtpObject = json[Const.pluginConstants.SMTP];
      }

      if (!utils.string.isNullOrEmpty(smtpObject[traceIfNull(Const.pluginConstants.SERVER_ADDRESS)])) {
         this.smtp.server_address = smtpObject[traceIfNull(Const.pluginConstants.SERVER_ADDRESS)];
      }

      if (!utils.string.isNullOrEmpty(smtpObject[traceIfNull(Const.pluginConstants.SERVER_PORT)])) {
         this.smtp.server_port = smtpObject[traceIfNull(Const.pluginConstants.SERVER_PORT)];
      }

      if (!utils.string.isNullOrEmpty(smtpObject[traceIfNull(Const.pluginConstants.USER)])) {
         this.smtp.user = smtpObject[traceIfNull(Const.pluginConstants.USER)];
      }

      if (!utils.string.isNullOrEmpty(smtpObject[traceIfNull(Const.pluginConstants.PASSWORD)]) && smtpObject[traceIfNull(Const.pluginConstants.PASSWORD)] !== PluginPackageConfigurationBase.SENSITIVE_INFO_MASK) {
         this.smtp.password = smtpObject[traceIfNull(Const.pluginConstants.PASSWORD)];
      }

      if (!utils.string.isNullOrEmpty(smtpObject[traceIfNull(Const.pluginConstants.USE_SSL)])) {
         this.smtp.use_ssl = smtpObject[traceIfNull(Const.pluginConstants.USE_SSL)];
      }
      let imapObject;

      //IMAP
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.IMAP)])) {

         imapObject = json[Const.pluginConstants.IMAP];

      }

      if (!utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.SERVER_ADDRESS)])) {
         this.imap.server_address = imapObject[traceIfNull(Const.pluginConstants.SERVER_ADDRESS)];
      }

      if (!utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.SERVER_PORT)])) {
         this.imap.server_port = imapObject[traceIfNull(Const.pluginConstants.SERVER_PORT)];
      }

      if (!utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.USER)])) {
         this.imap.user = imapObject[traceIfNull(Const.pluginConstants.USER)];
      }

      if (!utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.PASSWORD)]) && imapObject[traceIfNull(Const.pluginConstants.PASSWORD)] !== PluginPackageConfigurationBase.SENSITIVE_INFO_MASK) {
         this.imap.password = imapObject[traceIfNull(Const.pluginConstants.PASSWORD)];
      }

      if (!utils.string.isNullOrEmpty(imapObject[traceIfNull(Const.pluginConstants.USE_SSL)])) {
         this.imap.use_ssl = imapObject[traceIfNull(Const.pluginConstants.USE_SSL)];
      }
      if (!utils.string.isNullOrEmpty(json[traceIfNull(Const.pluginConstants.TEST_EMAIL_ADDRESS)])) {
         this.test_email_address = json[traceIfNull(Const.pluginConstants.TEST_EMAIL_ADDRESS)];
      }
      return null; // everything is OK!
   }

   getPackagePartCount(): number {
      return 1; // this package count as one package part
   }

   maskSensitiveInfo() {
      this.smtp.password = PluginPackageConfigurationBase.SENSITIVE_INFO_MASK;
      this.imap.password = PluginPackageConfigurationBase.SENSITIVE_INFO_MASK;
   }
}