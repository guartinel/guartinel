/**
 * Created by DTAP on 03/18/2015.
 */
var moment = require('moment');
var database = include('guartinel/database/public/databaseConnector.js');
var securityTool = include('guartinel/security/tool.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var timeTool = include('guartinel/utils/timeTool.js');
var watcherServerController = include("guartinel/admin/configuration/watcherServerController.js");

exports.validateDeviceTokenAndGetAccount = function (token, callback) {
   database.getAccountBySubDocumentProperty('devices', 'token', token, function (err, account) {
      if (err) {
         return callback(err);
      }
      if (utils.object.isNull(account)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN).logMessage("Invalid token trying. Token: " + token);
         LOG.logMSError(error);
         return callback(error);
      }
      if (utils.object.isNull(account.devices)) {
         var error = new MSInternalServerError()
            .logMessage("Account don't have devices...");
         LOG.logMSError(error);
         return callback(error);
      }

      var device;
      for (var i = 0; i < account.devices.length; i++) {
         if (account.devices[i].token === (token)) {
            device = account.devices[i];
            break;
         }
      }
      if (utils.object.isNull(device)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN).logMessage("DeviceToken is invalid " + token);
         LOG.logMSError(error);
         return callback(error);
      }

      if (!timeTool.isTokenTimeStampValid(device.tokenTimeStamp)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN).logMessage("DeviceToken expired : " + device.token + " token time stamp:" + device.tokenTimeStamp);
         LOG.logMSError(error);
         return callback(error);
      } else {
         return callback(null, account);
      }
   });
}

exports.validateBrowserTokenAndGetAccount = function (token, callback) {
   database.getAccountBySubDocumentProperty('browserSessions', 'token', token, function (err, account) {
      if (err) {
         return callback(err);
      }
      if (utils.object.isNull(account)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN).logMessage("Invalid token trying. Token: " + token);
         LOG.logMSError(error);
         return callback(error);
      }
      if (utils.object.isNull(account.browserSessions)) {
         var error = new MSInternalServerError()
            .logMessage(" Account don't have browserSessions " + account.email);
         LOG.logMSError(error);
         return callback(error);
      }
      var browserSessionId = null;
      for (var i = 0; i < account.browserSessions.length; i++) {
         if (account.browserSessions[i].token === token) {
            browserSessionId = account.browserSessions[i]._id;
            break;
         }
      }

      if (!timeTool.isTokenTimeStampValid(account.browserSessions.id(browserSessionId).tokenTimeStamp)) {
         LOG.error("Timestamp is invalid for token: " + account.browserSessions.id(browserSessionId).token + "\n BrowserSessionId:" + JSON.stringify(browserSessionId));
         var xtoken = account.browserSessions.id(browserSessionId).token;
         var timeStamp = account.browserSessions.id(browserSessionId).tokenTimeStamp;
         // account.browserSessions.id(browserSessionId).remove();

         database.getAccountModel().update({}, { $pull: { 'browserSessions': { "token": token } } }, function (err, success) {
            if (err) {
               new MSError("SessionManager error")
                  .logMessage("Cannot save account after token pull")
                  .innerError(err)
                  .logNow();
            }
            var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN).logMessage("BrowserToken expired : " + xtoken + " token time stamp:" + timeStamp).logNow();

            return callback(error);
         });
      } else {
         account.browserSessions.id(browserSessionId).tokenTimeStamp = moment().toISOString();
         return callback(null, account);

      }
   });
}

exports.validateMSTokenAndGetWatcherServer = function (token, callback) {
   watcherServerController.getWatcherServerByToken(token, function (server) {
      if (utils.object.isNull(server)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN).logMessage("Invalid token of watcher server.Token:" + token);
         LOG.logMSError(error);
         return callback(error);
      }
      if (utils.object.isNull(!timeTool.isTokenTimeStampValid(server.getMSTokenTimeStamp()))) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.TOKEN_EXPIRED).logMessage("The token of watcher server expired.Token:" + token);
         LOG.logMSError(error);
         return callback(error);
      }

      server.setMSTokenTimeStamp(moment().toISOString());
      server.save(function (err) {
         if (err) {
            new MSError("SessionManager error")
               .logMessage("Cannot save server document")
               .innerError(err)
               .severe()
               .logNow();
         }
         return callback(null, server);
      });
   });
}

exports.createBrowserSession = function (account, callback) {
   var token = securityTool.generateToken();
   var browserSession = {
      token: token,
      tokenTimeStamp: moment().toISOString()
   }
   account.browserSessions.push(browserSession);
   account.save(function (err) {
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("sessionManager.createBrowserSession: Cannot save account document")
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, browserSession.token);
   });
}

exports.validateAPITokenAndGetAccount = function (token, callback) {
   var hashedToken = securityTool.generatePasswordHash(token, token);
   database.getAccountByProperty('api.token', hashedToken, function (err, account) {
      if (err) {
         return callback(err);
      }
      if (utils.object.isNull(account)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN).logMessage("Invalid token trying. Token: " + token);
         LOG.logMSError(error);
         return callback(error);
      }
      return callback(null, account);
   });
};

exports.validateAdminToken = function (token, callback) {
   config.getAdminAccount(function (err, adminAccount) {
      var isTokenInvalidated = false;
      var isTokenFound = false;
      for (var i = 0; i < adminAccount.tokens.length; i++) {
         if (adminAccount.tokens[i].tokenString === token) {
            if (!timeTool.isTokenTimeStampValid(adminAccount.tokens[i].tokenTimeStamp)) {
               adminAccount.tokens.splice(i, 1);
               isTokenInvalidated = true;
               continue;
            }
            isTokenFound = true;
         }
      }
      if (isTokenInvalidated) {
         adminAccount.save(function (err) {
            if (err) {
               new MSError("SessionManager error")
                  .logMessage("Cannot save admin account document")
                  .innerError(err)
                  .logNow();
            }
            sendResponseAccordingToIsTokenFound();
         });
      } else {
         sendResponseAccordingToIsTokenFound();
      }

      function sendResponseAccordingToIsTokenFound() {
         if (isTokenFound) {
            return callback();
         }
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_ADMINISTRATION_TOKEN).logMessage("Invalid admin token trying.");
         LOG.logMSError(error);
         return callback(error);
      }
   });
}

