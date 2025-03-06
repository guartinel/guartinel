/**
 * Created by DTAP on 2017.08.07..
 */

var sessionManager = include("guartinel/security/sessionManager.js");
var database = include("guartinel/database/public/databaseConnector.js");

exports.URL = safeGet(managementServerUrls.API_PACKAGE_DELETE);

var isRouteDebugEnabled = false;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
   var parameters = {
      token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
      packageName: req.body[safeGet(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)]
   };
   myRequestValidation(parameters, req, res, function (err) {
      if (!err) {
         processRequest(parameters, function (result) {
            res.send(result);
         });
      }
   });
};

function processRequest(parameters, callback) {
   sessionManager.validateAPITokenAndGetAccount(parameters.token, function (sessionErr, account) {
      if (sessionErr) {
         return callback(new ErrorResponse(sessionErr));
      }

      var foundPack;
      for (var index = 0; index < account.packages.length; index++) {
         if (account.packages[index].packageName === parameters.packageName) {
            foundPack = account.packages[index];
            break;
         }
      }

      if (utils.object.isNull(foundPack)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_PACKAGE_NAME)
            .logMessage("Cannot delete account because wrong package name")
            .logNow();
         return callback(new ErrorResponse(error));
      }
      debugRoute("Pulling out found package by id: " + foundPack._id);
      // account.packages.pull({_id: foundPack._id}); TODO no deletion from DB only mark is deleted
      foundPack.isDeleted = true;
      foundPack.state = {};
      foundPack.isEnabled = false;
      foundPack.measurements = [];
      foundPack.watcherServerId = null;

      account.save(function (err) {
         if (err) {
            var error = new MSInternalServerError()
               .logMessage(exports.URL + ": Cannot save account")
               .severe()
               .innerError(err)
               .logNow();
            return callback(new ErrorResponse(error));
         }
         debugRoute("Save done after pull");
         removeAccessLinksToThisPackage();
      });

      function removeAccessLinksToThisPackage() {
         database.getNativeAccountsConnection().updateMany(
            { 'accessiblePackageIds': foundPack._id.toString() },
            { $pull: { 'accessiblePackageIds': foundPack._id.toString() } },
            afterAccessLinkRemove);
         debugRoute("removeAccessLinksToThisPackage started ");

      }

      function afterAccessLinkRemove(err) {
         debugRoute("afterAccessLinkRemove started");
         if (err) {
            var error = new MSInternalServerError()
               .logMessage(exports.URL + ": Cannot delete access links ")
               .severe()
               .innerError(err)
               .logNow();
            return callback(new ErrorResponse(error));
         }
         debugRoute("afterAccessLinkRemove finished without error");

         return callback(new SuccessResponse());
      }
   }
   );
}

