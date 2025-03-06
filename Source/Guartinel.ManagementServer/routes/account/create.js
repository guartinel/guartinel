var emailer = include("guartinel/connection/emailer.js");
var securityTool = include("guartinel/security/tool.js");
var database = include("guartinel/database/public/databaseConnector.js");
var licenseController = include("guartinel/license/licenseController.js");
var moment = require('moment');

exports.URL = safeGet(managementServerUrls.ACCOUNT_CREATE);
exports.route = function(req, res) {
    var parameters = {
        email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)],
        password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)],
        lastNameOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.FIRST_NAME)],
        firstNameOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.LAST_NAME)],
        tokenOPT: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    };

    myRequestValidation(parameters,
        req,
        res,
        function(err) {
            if (!err) {
                doCreate(parameters,
                    function(result) {
                        res.send(result);
                    });
            }
        });
};

function doCreate(parameters, callback) {
   database.getAccountByMultiplePropertyValues('email', [securityTool.encryptText(parameters.email), parameters.email], function (err, account) {
        if (err) {
           return callback(new ErrorResponse(err));
        }

        if (!utils.object.isNull(account)) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.EMAIL_ALREADY_REGISTERED)
              .logMessage("Email:" + parameters.email).logNow();
           return callback(new ErrorResponse(error));
         }
         
        var currentTime = moment();
        var expiryDate = currentTime.add(7, 'd').toISOString();
        var activationInfo = {
            activationCode: securityTool.generateRandomString(),
            expiryDate: expiryDate,
            isActivated: false
        }

        licenseController.getDefaultLicense(function (err, license) {
            var toDate = moment().add(100, 'y');

            var newLicense = {
                license: license,
                startDate: moment().toISOString(),
                expiryDate: toDate.toISOString()
            };

            var newAccount = new database.getAccountModel()({
                firstName: parameters.firstNameOPT,
                lastName: parameters.lastNameOPT,

                email: parameters.email,
                passwordHash: securityTool.generatePasswordHash(parameters.email, parameters.password),
                maxPackages: 10,
                maxDevices: 10,
                activationInfo: activationInfo,
                browserSessions: [],
                packages: [],
                devices: [],
                licenses: []
            });
            newAccount.licenses.push(newLicense);
           LOG.info("New account registered: " + newAccount.email);
           var isTesterAccount = false;
            if (parameters.tokenOPT == "hxAIJWAvzmx07U7tuwI9"){//this is a testing token which is created during creation from unit test
               LOG.info("This is a tester account creation.");
               convertToTesterAccount(newAccount);
               isTesterAccount = true;
            }
            newAccount.save(function (err) {
               if (err) {
                  var error = new MSError()
                     .logMessage("Failed to save new account" + newAccount.email)
                     .innerError(err)
                     .severe()
                     .logNow();
                  return callback(new ErrorResponse(error));
               }
               if (isTesterAccount) {
                  return callback(new SuccessResponse());
               } else {
                  emailer.sendActivationEmail(newAccount, function (emailErr) {
                     var error = new MSError()
                        .logMessage("Cannot send activation mail to account: " + newAccount.email)
                        .innerError(emailErr)
                        .severe()
                        .logNow();
                     return callback(new SuccessResponse());
                  });
               }    
            });
        });
        function convertToTesterAccount(account){
            account.activationInfo.isActivated = true;
            account.licenses[0].license.name = "test";
            account.licenses[0].license.canUseAPI = true;
            account.licenses[0].license.maximumPackages = 999;
            account.licenses[0].license.maximumPackagePartCount = 999;
            account.licenses[0].license.minimumCheckIntervalSec = 10;
        }
    });
}