import { LOG } from "../../diagnostics/LoggerFactory";
import * as config from "../admin/configuration/configurationController"
import * as os from "os";
import { Const } from "../../common/constants";
import * as postmark from "postmark";
import { Templates } from "./email/templates";
import * as controlledMailManager from "./email/controlledMailManager";
import { MSInternalServerError, MSError } from "../../error/Errors";
import { TemplateProperties } from "./email/templateProperties";

let TAG = "EmailDebug: ";
let isEmailDebugEnabled = true;
function debugEmailing(msg: string) {
   if (!isEmailDebugEnabled) {
      return;
   }
   LOG.debug(TAG + msg);
}

export function configure(emailConfiguration) {
   let provider = emailConfiguration.provider.toLowerCase();
   LOG.info("Creating new mailsender.");
   Templates.configure(emailConfiguration);
   if (provider === 'postmark') {
      mailSender = new PostMarkMSMailer(emailConfiguration);
   } else {
      new MSInternalServerError().logMessage("Missing provider from email configuration!").severe().logNow();
   }
}

let mailSender: IMSMailer;
interface IMSMailer {
   sendMail(destinationAddress: string, templateId: number, templateModel: any, callback: (error: MSError, info: any) => void);
}

const GUARTINEL_ADMIN_ALERT_EMAIL = 'alert@sysment.hu';
const DEFAULT_EMAIL_SENDER = 'service@guartinel.com';
class PostMarkMSMailer implements IMSMailer {
   client;
   configuration;
   constructor(configuration) {
      this.configuration = configuration;
      this.client = new postmark.Client(configuration.userName, {});
   }
   
   sendMail(destinationAddress: string, templateId: number, templateModel: any, onFinishedCallback: (error: MSError, info: any) => void) {
      if (!config.getBaseConfig().isEmailingEnabled) {
         LOG.info("Email is not sent because it is supressed from config");
         return onFinishedCallback(null, "Emailing supressed by config to address: " + destinationAddress);
      }

      this.client.sendEmailWithTemplate({
         "From": DEFAULT_EMAIL_SENDER,
         "To": destinationAddress,
         "TemplateId": templateId,
         "TemplateModel": templateModel
      }, function (err, inf) {
         debugEmailing("JUST AFTER EMAIL SEND");
         return onFinishedCallback(err, inf);
      });
   }
}

function sendTemplateEmail(destinationEmail: string, templateModel: Templates.BaseTemplate, onSendTemplateEmailFinished) {
   debugEmailing("sendTemplateEmail start");
   if (global.utils.object.isNull(mailSender)) {
      LOG.error("Mail sender client is null. Aborting email send.");
      return onSendTemplateEmailFinished();
   }
   controlledMailManager.isEmailBlackListed(destinationEmail, afterIsEmailBlackListed);
   function afterIsEmailBlackListed(err: MSError, isBlackListed: boolean, blackListToken: string) {
      if (isBlackListed) {
         LOG.error("Won't send email because email " + destinationEmail + " is blacklisted");
         return onSendTemplateEmailFinished();
      } else {
         debugEmailing("sendTemplateEmail email not blacklisted");
         addUnSubscribeEmailToTemplate(blackListToken, templateModel, afterAddUnSubscribeEmailToTemplate);
      }
   }

   function addUnSubscribeEmailToTemplate(blackListToken, templateModel, onFinished) {
      debugEmailing("addUnSubscribeEmailToTemplate start");
      config.getLocalConfiguration(function (err, config) {
         var urlUnsubscribeAll = createLinkWithParameters(config.webpageURL, "/unsubscribeAllEmail", [{
            key: "blackListToken",
            value: blackListToken
         }]);

         var urlUnsubscribeFromPackage = createLinkWithParameters(config.webpageURL, "/unsubscribeFromPackageEmail", [{
            key: "blackListToken",
            value: blackListToken
         }, {
            key: "packageID",
            value: templateModel.packageId
         }
         ]);
         templateModel["unsubscribeAllEmailLink"] = urlUnsubscribeAll;
         templateModel["unsubscribeFromPackageEmailLink"] = urlUnsubscribeFromPackage;
         debugEmailing("addUnSubscribeEmailToTemplate finished");
         return onFinished(templateModel);
      });
   }

   function afterAddUnSubscribeEmailToTemplate(templateModel: Templates.BaseTemplate) {
      debugEmailing("afterAddUnSubscribeEmailToTemplate start");
      mailSender.sendMail(destinationEmail, templateModel.getTemplateId(), templateModel, afterSendEmailWithTemplate);
   }

   function afterSendEmailWithTemplate(err, inf) {
      debugEmailing("afterSendEmailWithTemplate start");
      if (err) {
         return onSendTemplateEmailFinished(
            new MSError(Const.commonConstants.ALL_ERROR_VALUES.CANNOT_SEND_EMAIL)
               .logMessage("Cannot send email to address: " + destinationEmail)
               .innerError(err)
               .logNow()
         );
      } else {
         if (inf) {
            LOG.info("Email  sending information: " + JSON.stringify(inf));
         }

         debugEmailing("afterSendEmailWithTemplate finished");
         return onSendTemplateEmailFinished();
      }
   }
}

export function sendActivationEmail(account, callback) {
   config.getLocalConfiguration(function (err, config) {
      var activateActionURL = createLinkWithParameters(config.webpageURL, "/", [{
         key: "activationCode",
         value: account.activationInfo.activationCode
      },
      {
         key: "email",
         value: account.email
      }]);

      var templateModel = new Templates.InfoEmailTemplate("Welcome aboard " + account.getUserName() + "!",
         "You are almost done. All you need to do is to click on the button below to start using your new account! We also gift you with a free 10 day trial license for the activation!",
         "Or you can manually enter the following code: <br>" + account.activationInfo.activationCode,
         activateActionURL,
         "Activate My Account!",
         "If you are not activating it in 7 days your account will be suspended!",
         null
      );

      sendTemplateEmail(account.email, templateModel, callback);
   });
};

export function sendVerifyPasswordSendEmail(email, verificationCode, addressInfo, callback) {
   config.getLocalConfiguration(function (err, config) {
      var confirmPasswordRenewAction = createLinkWithParameters(config.webpageURL, "/", [{
         key: "verificationCode",
         value: verificationCode
      }, {
         key: "email",
         value: email
      }
      ]);

      var templateModel = new Templates.InfoEmailTemplate("Guartinel password reset request",
         "Somebody has requested a new password for your account.",
         "If it was you please click on the button below to confirm it. Otherwise ignore this email.<br>" + addressInfo,
         confirmPasswordRenewAction,
         "Reset my password!");
      sendTemplateEmail(email, templateModel, callback);
   });
};
export function sendNewPasswordToEmail(email, password, callback) {
   var templateModel = new Templates.InfoEmailTemplate("Guartinel password change is done.",
      "As you request we have crated a new password for you.",
      "New password: " + password,
      "https://manage.guartinel.com",
      "Login",
      "It's strongly suggested to change it at next login!");
   sendTemplateEmail(email, templateModel, callback);
}

export function sendMaintenanceEmail(email, userName, message, callback) {
   var templateModel = new Templates.InfoEmailTemplate("Guartinel planned maintenance", message, "We apologize for any inconvenience and hope to welcome you back once our service is live again.");
   sendTemplateEmail(email, templateModel, callback);
};

export function sendTestEmail(email, callback) {
   var templateModel = new Templates.InfoEmailTemplate("Guartinel Test Email", "This is just a test email email to verify your email address validity", "Please ignore and delete it.")
   sendTemplateEmail(email, templateModel, callback);
};

export function sendProblemEmailToAdmin(errorCaption, errorMessage, callback) {
   debugEmailing("sendProblemEmailToAdmin start");
   var templateModel = new Templates.InfoEmailTemplate(`Guartinel Error: ${errorCaption} on: ${os.hostname()}`, errorMessage);
   sendTemplateEmail(GUARTINEL_ADMIN_ALERT_EMAIL, templateModel, callback);
};

export function sendPackageStatusChangeEmail(email, packageName, usePlainAlertEmail, isRecovery, isPackageStillAlerting, packageStatusMessage, packageStatusMessageDetail, packageStatusExtraInfo, callback) {
   let packageLink =
      `<a href="https://manage.guartinel.com/packages" class="white_link font_m font_face"><span class="white_link font_m font_face" > <font color="#FFFFFF"><u>package</u></font></span></a>`;

   let status;
   let statusCaption;
   let statusColor;

    let packageStatusDetailsColor;
    
   if (isRecovery && isPackageStillAlerting) {
      status = TemplateProperties.Status.partiallyRecovered;
      statusColor = TemplateProperties.StatusColor.recovered;
      packageStatusDetailsColor = TemplateProperties.StatusColor.alerting;
      statusCaption = TemplateProperties.StatusCaption.partiallyRecovered;
      packageStatusExtraInfo = "There are other parts of the package which are still alerting!\n" + packageStatusExtraInfo;
   }
   if (isRecovery && !isPackageStillAlerting) {
      status = TemplateProperties.Status.recovered;
      statusColor = TemplateProperties.StatusColor.recovered;
      packageStatusDetailsColor = TemplateProperties.StatusColor.recovered;
      statusCaption = TemplateProperties.StatusCaption.recovered;
   }
   if ((!isRecovery && isPackageStillAlerting)) {
      status = TemplateProperties.Status.alerting;
      statusCaption = TemplateProperties.StatusCaption.alerting;
      statusColor = TemplateProperties.StatusColor.alerting;
      packageStatusDetailsColor = TemplateProperties.StatusColor.alerting;
   }

   let templateModel = null;
   if (usePlainAlertEmail) {
       templateModel = new Templates.PlainPackageStatusEmailTemplate(packageName, statusCaption, packageStatusMessage, packageStatusMessageDetail, packageStatusExtraInfo);
   } else {
      packageStatusMessage = packageStatusMessage.replace("\\", "#$@$#");
      packageStatusMessage = packageStatusMessage.replace("\\n", "<br>");
      packageStatusMessage = packageStatusMessage.replace("#$@$#", "\\");

      // packageStatusMessage = packageStatusMessage.replace("\\n", "<br>");
      templateModel = new Templates.PackageStatusEmailTemplate(packageName,
         status,
         statusCaption,
         statusColor,
         packageStatusDetailsColor,
         packageStatusMessage,
         packageStatusMessageDetail,
         packageStatusExtraInfo,
         null,
         null);
   }
   sendTemplateEmail(email, templateModel, callback);
};
export function sendDailyAlertedPackageSummary(email, alertedPackageNames, callback) {
   debugEmailing("sendDailyAlertedPackageSummary started with email " + email);
   let infoSubject = "Your package is still alerted.";
   if (alertedPackageNames.length > 1) {
      infoSubject = "Your packages are still alerted.";
   }
   let warningMessage = "Try to recover the problem or disable the packege!";
   if (alertedPackageNames.length > 1) {
      warningMessage = "Try to recover the problems or disable the packages!";
   }
   let infoMessage = "Affected package:<br>";
   if (alertedPackageNames.length > 1) {
      infoMessage = "Affected packages:<br>";
   }
   infoMessage += alertedPackageNames.join();
   var templateModel = new Templates.InfoEmailTemplate(infoSubject, infoMessage, null, null, null, warningMessage, null);
   sendTemplateEmail(email, templateModel, callback);

}

export function sendEmailAboutExpiredLicense(email, messageForTheUser, callback) {
   debugEmailing("sendEmailAboutExpiredLicense started with email " + email);
   var templateModel = new Templates.InfoEmailTemplate("Guartinel license expired", messageForTheUser);
   sendTemplateEmail(email, templateModel, callback);
};
export function sendEmailAboutTrialActivation(email, callback) {
   debugEmailing("sendEmailAboutExpiredLicense started with email " + email);
   var templateModel = new Templates.InfoEmailTemplate("Trial license activated", "Your trial license is activated for 10 days.");
   sendTemplateEmail(email, templateModel, callback);
};
export function sendWarningAboutLicenseExpiration(email, expiringLicenses, callback) {
   var templateModel = new Templates.InfoEmailTemplate("Guartinel license will be expired soon",
      "Your " + expiringLicenses[0].license.caption + " will be expired in " + expiringLicenses[0].daysRemaining + " days.",
      "Please renew it, otherwise your packages will be disabled.");
   sendTemplateEmail(email, templateModel, callback);
};
function createLinkWithParameters(url, path, parameters) {
   var HTTPS_PREFIX = "https://";
   var HTTP_PREFIX = "http://";

   var parsedURL = "";
   if (url.indexOf(HTTPS_PREFIX) == -1 && url.indexOf(HTTP_PREFIX) == -1) {
      parsedURL += HTTPS_PREFIX;
   }

   parsedURL += url;
   parsedURL += path;
   if (parameters.length == 0) {
      return parsedURL;
   }

   parsedURL += "?";

   for (var index = 0; index < parameters.length; index++) {
      if (parsedURL[parsedURL.length - 1] != "?") {
         parsedURL += "&";
      }
      parsedURL += parameters[index].key;
      parsedURL += "=";
      parsedURL += parameters[index].value;
   }
   return parsedURL;
}
