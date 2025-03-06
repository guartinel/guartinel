import { TemplateProperties } from "./templateProperties";
import * as Os from "os";

export namespace Templates {
    let infoMailTemplateId;
    let plainPackageStatusChangeTemplateId;
    let packageStatusChangeTemplateId;

    export function configure(emailConfiguration) {
        infoMailTemplateId = emailConfiguration.infoMailTemplateId;
        plainPackageStatusChangeTemplateId = emailConfiguration.plainPackageStatusChangeTemplateId;
        packageStatusChangeTemplateId = emailConfiguration.packageStatusChangeTemplateId;
    }

    export class BaseTemplate {
        environment: string;
        protected templateId;

        constructor(templateId) {
            if (Os.hostname().indexOf("test") != -1) {
                this.environment = "Test";
            } else if (Os.hostname().indexOf("DEV") != -1) {
                this.environment = "Dev";
            } else {
                this.environment = "";
            }
            this.templateId = templateId;
        }

        getTemplateId() {
            return this.templateId;
        }
    }

    export class InfoEmailTemplate extends BaseTemplate {
        infoSubject: string;
        infoMessage: string;
        hideInfoMessage2: TemplateProperties.Visibility;
        infoMessage2: string;
        infoActionHref: string;
        infoActionText: string;
        hideInfoAction: TemplateProperties.Visibility;
        warningMessage: string;
        hideWarningMessage: TemplateProperties.Visibility;
        unsubscribeAllEmailLink: string;
        hideUnsubscribeLink: TemplateProperties.Visibility;

        constructor(
            infoSubject: string,
            infoMessage: string,
            infoMessage2?: string,
            infoActionHref?: string,
            infoActionText?: string,
            warningMessage?: string,
            unsubscribeAllEmailLink?: string) {
            super(infoMailTemplateId);
            this.infoSubject = infoSubject;
            this.infoMessage = infoMessage;

            this.infoMessage2 = infoMessage2;
            this.hideInfoMessage2 = ((global.utils.string.isNullOrEmpty(this.infoMessage2))
                ? TemplateProperties.Visibility.hidden
                : TemplateProperties.Visibility.visible);

            this.infoActionHref = infoActionHref;
            this.infoActionText = infoActionText;
            this.hideInfoAction = ((global.utils.string.isNullOrEmpty(this.infoActionText))
                ? TemplateProperties.Visibility.hidden
                : TemplateProperties.Visibility.visible);

            this.warningMessage = warningMessage;
            this.hideWarningMessage = ((global.utils.object.isNullOrEmpty(this.warningMessage))
                ? TemplateProperties.Visibility.hidden
                : TemplateProperties.Visibility.visible);

            this.unsubscribeAllEmailLink = unsubscribeAllEmailLink;
            this.hideUnsubscribeLink = ((global.utils.object.isNullOrEmpty(this.unsubscribeAllEmailLink))
                ? TemplateProperties.Visibility.hidden
                : TemplateProperties.Visibility.visible);
        }
    }

    export class PackageStatusEmailTemplate extends BaseTemplate {
        packageName: string;
        packageStatus: string;
        packageStatusCaption: string;
        packageStatusColor: string;
        packageStatusDetailsColor: string;
        packageStatusMessage: string;
        packageStatusMessageDetail: string;
        packageStatusExtraInfo:string;
        unsubscribeAllEmailLink: string;
        unsubscribeFromPackageEmailLink: string;

        constructor(packageName,
            packageStatus,
            packageStatusCaption,
            packageStatusColor,
            packageStatusDetailsColor,
            packageStatusMessage,
            packageStatusMessageDetail,
            packageStatusExtraInfo,
            unsubscribeAllEmailLink,
            unsubscribeFromPackageEmailLink) {
            super(packageStatusChangeTemplateId);
            this.packageName = packageName;
            this.packageStatusCaption = packageStatusCaption;
            this.packageStatus = packageStatus;
            this.packageStatusColor = packageStatusColor;
            this.packageStatusDetailsColor = packageStatusDetailsColor;
            this.packageStatusMessage = packageStatusMessage;
            this.packageStatusMessageDetail = packageStatusMessageDetail;
            this.packageStatusExtraInfo = packageStatusExtraInfo;
            this.unsubscribeAllEmailLink = unsubscribeAllEmailLink;
            this.unsubscribeFromPackageEmailLink = unsubscribeFromPackageEmailLink;
        }
    }

    export class PlainPackageStatusEmailTemplate extends BaseTemplate {
        packageName: string;
        packageStatusCaption: string;
        packageStatusMessage: string;
        packageStatusMessageDetail: string;
        packageStatusExtraInfo:string;
        hideUnsubscribeLink = TemplateProperties.Visibility.hidden;

        constructor(packageName, packageStatusCaption, packageStatusMessage, packageStatusMessageDetail, packageStatusExtraInfo) {
            super(plainPackageStatusChangeTemplateId);
            this.packageName = packageName;
            this.packageStatusCaption = packageStatusCaption;
            this.packageStatusMessage = packageStatusMessage;
            this.packageStatusMessageDetail = packageStatusMessageDetail;
            this.packageStatusExtraInfo = packageStatusExtraInfo;
        }
    }
}