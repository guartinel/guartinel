var database = include('guartinel/database/public/databaseConnector.js');
var securityTool = include('guartinel/security/tool.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var timeTool = include('guartinel/utils/timeTool.js');
var moment = require('moment');

exports.authenticateAndGetAccount = function (untrustedEmail, untrustedPasswordHash, callback) {
   database.getAccountByMultiplePropertyValues('email', [securityTool.encryptText(untrustedEmail), untrustedEmail], function (err, trustedAccount) {// TODO remove after all account email is encrypted
      if (err) {
         return callback(err);
      }
      if (utils.object.isNull(trustedAccount)) {
         return callback(new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD).logMessage("Invalid website login attempt email: " + untrustedEmail).logNow());
      }
      // hash again the passwordHash
      var rehashedPassword = securityTool.generatePasswordHash(untrustedEmail, untrustedPasswordHash);
      if (trustedAccount.passwordHash != rehashedPassword) {
         return callback(

            new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD)
               .logMessage("Invalid website password attempt email: " + untrustedEmail)
               .logNow());
      }

      if (!trustedAccount.activationInfo.isActivated) {
         return callback(new MSError(commonConstants.ALL_ERROR_VALUES.ACCOUNT_EXPIRED)
            .logMessage("Account is expired email: " + untrustedEmail)
            .logNow());
      }
      return callback(null, trustedAccount); // password correct -> grant
   });
};

exports.authenticateAPIAndGetAccount = function (untrustedEmail, untrustedPlainPassword, callback) {
   LOG.info("Looking for email: "+ untrustedEmail);
   database.getAccountByProperty('email', [securityTool.encryptText(untrustedEmail), untrustedEmail], function (err, trustedAccount) { // TODO remove after all account email is encrypted
      if (err) {
         return callback(err);
      }
      if (utils.object.isNull(trustedAccount)) {
         return callback(
            new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD)
               .logMessage("Invalid  API login attempt email: " + untrustedEmail)
               .logNow());
      }
      var hashedPassword = securityTool.generatePasswordHash(untrustedEmail, untrustedPlainPassword);
      // hash again the passwordHash
      var rehashedPassword = securityTool.generatePasswordHash(untrustedEmail, hashedPassword);
      if (trustedAccount.passwordHash != rehashedPassword) {
         return callback(
            new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD)
               .logMessage("Invalid  API password attempt email: " + untrustedEmail)
               .logNow());
      }
      if (!trustedAccount.activationInfo.isActivated && timeTool.isAccountExpired(trustedAccount.activationInfo.expiryDate)) {
         return callback(

            new MSError(commonConstants.ALL_ERROR_VALUES.ACCOUNT_EXPIRED)
               .logMessage("Account is expired: " + untrustedEmail)
               .logNow());
      }
      return callback(null, trustedAccount); // password correct -> grant
   });
};

exports.authenticateAdministrator = function (userName, passwordHashFromClient, callback) {
   config.getAdminAccount(function (err, adminAccount) {

      if (userName != adminAccount.userName) {
         return callback(
            new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD)
               .logMessage("Invalid admin account login attempt")
               .severe()
               .logNow());
      }

      var reHashedPasswordFromClient = securityTool.generatePasswordHash(userName, passwordHashFromClient);

      if ((reHashedPasswordFromClient != adminAccount.passwordHash) && passwordHashFromClient != adminAccount.passwordHash) {
         return callback(
            new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_USER_NAME_OR_PASSWORD)
               .logMessage("Invalid admin account login attempt")
               .severe()
               .logNow());
      }

      var tokenString = securityTool.generateToken();
      var tokenTimeStamp = moment().toISOString();
      var token = {
         tokenString: tokenString,
         tokenTimeStamp: tokenTimeStamp
      }
      adminAccount.tokens.push(token);
      //config.USE().setAdminAccountWhole(adminAccount);
      adminAccount.save(function (err) {
         return callback(null, tokenString);
      });
   });
}