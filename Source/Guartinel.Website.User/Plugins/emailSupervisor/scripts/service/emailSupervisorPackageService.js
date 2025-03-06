'use strict';
app.service('emailSupervisorPackageService',
    [
        'dialogService', '$q', function(dialogService, $q) {

            var createDialog = {
                title: 'Create Email Supervisor Package',
                controller: 'emailSupervisorPackageSaveDialogController',
                template: 'plugins/emailSupervisor/templates/saveDialog/dialog.html',
                needConfirmToClose: true
            }
            this.showCreateDialog = function(packageData) {
                return dialogService.showDialog(createDialog, packageData); //1313this.createAndFillPackageTemplate());
            }
            var editDialog = {
                title: 'Edit Email Supervisor Package',
                controller: 'emailSupervisorPackageSaveDialogController',
                template: 'plugins/emailSupervisor/templates/saveDialog/dialog.html',
                needConfirmToClose: true
            }
            this.showEditDialog = function(paramPackage) {
                editDialog.title = "Edit " + paramPackage.packageName;
                return dialogService.showDialog(editDialog, paramPackage);
            }

            function addLastMeasuremntsToPackageTemplate(packageTemplate, rawData) {
                if (rawData.lastMeasurementTimeStamp == null || rawData.lastMeasurement == null) {
                    return;
                }
                packageTemplate.lastMeasurementTimeStamp = rawData.lastMeasurementTimeStamp;

                packageTemplate.lastMeasurement = {};
                if (!isNull(rawData.lastMeasurement)) {
                    packageTemplate.lastMeasurement.message = rawData.lastMeasurement.message;
                    packageTemplate.lastMeasurement.success = rawData.lastMeasurement.success;
                }
            }

            function addConfigurationDataToPackageTemplate(packageTemplate, rawData) {
                if (rawData.configuration == null) {
                    rawData.configuration = {};
                }
                if (packageTemplate.configuration == null) {
                    packageTemplate.configuration = {};
                }

                if (packageTemplate.configuration.imap == null) {
                    packageTemplate.configuration.imap = {};
                }
               if (rawData.configuration.imap != null) {
                    packageTemplate.configuration.imap.serverAddress = rawData.configuration.imap.server_address;
                    packageTemplate.configuration.imap.serverPort = rawData.configuration.imap.server_port;
                    packageTemplate.configuration.imap.useSSL = rawData.configuration.imap.use_ssl;
                    packageTemplate.configuration.imap.user = rawData.configuration.imap.user;
                    packageTemplate.configuration.imap.password = rawData.configuration.imap.password;
                }

                if (packageTemplate.configuration.smtp == null) {
                    packageTemplate.configuration.smtp = {};
                }
                if (rawData.configuration.smtp != null) {

                    packageTemplate.configuration.smtp.serverAddress = rawData.configuration.smtp.server_address;
                    packageTemplate.configuration.smtp.serverPort = rawData.configuration.smtp.server_port;
                    packageTemplate.configuration.smtp.useSSL = rawData.configuration.smtp.use_ssl;
                    packageTemplate.configuration.smtp.user = rawData.configuration.smtp.user;
                    packageTemplate.configuration.smtp.password = rawData.configuration.smtp.password;
                }
                packageTemplate.configuration.testEmailAddress = rawData.configuration.test_email_address;

                packageTemplate.updatePluginRelatedUI = function () {
                 
                };
            }

            this.createAndFillPackageTemplate = function(rawData) {
                if (rawData == null) {
                    rawData = {};
                }
                //1313 var baseTemplate = new pluginPackageService.BasePackageTemplate(rawData);
                //1313baseTemplate.packageType = safeGet(plugins.ALL_PACKAGE_TYPE_VALUES.EMAIL_SUPERVISOR);

                addConfigurationDataToPackageTemplate(rawData, rawData);
                addLastMeasuremntsToPackageTemplate(rawData, rawData);

                rawData.getPackagePartCount = function () { return 1; } // this package is count as one
                rawData.isDeviceUsed = function () { return false; };

                return rawData;
            }

            this.translateForSave = function(paramPackage) { // first translate the package keys
                var translatedPackage = paramPackage;
                if (isNull(translatedPackage.configuration)) {
                    translatedPackage.configuration = {
                        imap: {},
                        smtp: {}
                    }
                }

                translatedPackage.configuration.smtp.server_address = paramPackage.configuration.smtp.serverAddress;
                translatedPackage.configuration.smtp.server_port = paramPackage.configuration.smtp.serverPort;
                translatedPackage.configuration.smtp.use_ssl = paramPackage.configuration.smtp.useSSL;
                translatedPackage.configuration.smtp.user = paramPackage.configuration.smtp.user;
                translatedPackage.configuration.smtp.password = paramPackage.configuration.smtp.password;

                translatedPackage.configuration.imap.server_address = paramPackage.configuration.imap.serverAddress;
                translatedPackage.configuration.imap.server_port = paramPackage.configuration.imap.serverPort;
                translatedPackage.configuration.imap.use_ssl = paramPackage.configuration.imap.useSSL;
                translatedPackage.configuration.imap.user = paramPackage.configuration.imap.user;
                translatedPackage.configuration.imap.password = paramPackage.configuration.imap.password;

                translatedPackage.configuration.test_email_address = paramPackage.configuration.testEmailAddress;

                //translatedPackage.configuration = configuration;
                /*1313return packageService.save(translatedPackage).then(function () {
                    return $q.resolve();
                }, function (error) {
                    return $q.reject(error);
                });*/
            }
        }
    ]);