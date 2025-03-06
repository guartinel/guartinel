var sessionManager = include("guartinel/security/sessionManager.js");
var moment = require('moment');
var database = include("guartinel/database/public/databaseConnector.js");
var httpRequester = include('guartinel/connection/httpRequester.js');
var watcherServerController = include('guartinel/admin/configuration/watcherServerController.js');

exports.URL = safeGet(managementServerUrls.PACKAGE_DELETE);
exports.route = function (req, res) {
   var parameters = {
      token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
      packageId: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_ID)]
   }
   myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         doDelete(parameters, function (result) {
            return res.send(result);
         });
      }
   });
}

function doDelete(parameters, callback) {

   sessionManager.validateBrowserTokenAndGetAccount(parameters.token, afterSessionValidation);

   function afterSessionValidation(sessionErr, accountForUser) {
      if (sessionErr) {
         return callback(new ErrorResponse(sessionErr));
      }
      var account;
      if (!database.isIdValid(parameters.packageId)) {
         var error = new MSInternalServerError()
            .logMessage(exports.URL + ": Invalid package id:" + parameters.packageId)
            .severe()
            .logNow();
         return callback(new ErrorResponse(error));
      }

      //we must get the account by packageID because the issuer of the deletion could be not the the same user as the owner
      database.getAccountBySubDocumentProperty('packages', '_id', database.toObjectId(parameters.packageId), afterPackageRetrieval);

      function afterPackageRetrieval(err, _account) {
         account = _account;
         var pack = account.packages.id(parameters.packageId);

         var accessRule = getAccessForEmail(pack, accountForUser.email);
         if (utils.object.isNull(accessRule) || (!accessRule.canDelete)) {
            var error = new MSError(commonConstants.ALL_ERROR_VALUES.PERMISSION_DENIED)
               .logMessage(exports.URL + "Account dont have permission to edit this package :" + account.email)
               .logNow();
            return callback(new ErrorResponse(error));
         }

         account.packages.id(parameters.packageId).isDeleted = true;
         account.packages.id(parameters.packageId).state = {};
         account.packages.id(parameters.packageId).isEnabled = false;
         account.packages.id(parameters.packageId).measurements = [];
         account.packages.id(parameters.packageId).lastModificationTimeStamp = moment().toISOString();
         account.packages.id(parameters.packageId).watcherServerId = null;
         watcherServerController.getWatcherServerById(pack.watcherServerId, afterWsRetrieval);
      }

      function afterWsRetrieval(server) {
         if (!utils.object.isNull(server)) {
            httpRequester.deletePackage(server, parameters.packageId, afterDeletionFromWS);
            return;
         }
         // package not yet created on WS just delete it from the DB
         //   account.packages.id(parameters.packageId).remove();  TODO this was uncommented to prevent package deletion from db
        removeAccessLinksToThisPackage();

         function afterDeletionFromWS(httpErr) {
            if (httpErr) {//tried to delete from WS but cannot be done. It will be retried from the package monitor timer
               var error = new MSError("WS_ERROR")
                  .logMessage(exports.URL + "Cannot delete the package from WS. ")
                  .severe()
                  .innerError(httpErr)
                  .logNow();
            }
            removeAccessLinksToThisPackage();
         }
      }

      function removeAccessLinksToThisPackage() {
         database.getNativeAccountsConnection().updateMany(
            { 'accessiblePackageIds': parameters.packageId },
            { $pull: { 'accessiblePackageIds': parameters.packageId } },
            afterLinkRemoval);
      }

      function afterLinkRemoval(err) {
         if (err) {
            var error = new MSInternalServerError()
               .logMessage(exports.URL + "Cannot remove link from package. ")
               .innerError(err)
               .severe()
               .logNow();
            return callback(new ErrorResponse(error));
         }
         account.save(function (err) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + "Cannot save the account after package delete.")
                  .innerError(err)
                  .severe()
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            return callback(new SuccessResponse());
         });
      }
   }
}

function getAccessForEmail(pack, email) {
   var result;
   pack.access.forEach(function (item, index) {
      if (item.packageUserEmail === email) {
         result = item;
      }
   });
   return result;
}
