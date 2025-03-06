var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var statusInformer = include('guartinel/admin/statusInformer.js');
var watcherServerController = include('guartinel/admin/configuration/watcherServerController.js');
var database = include('guartinel/database/public/databaseConnector.js');
exports.URL = safeGet(managementServerUrls.ADMIN_WATCHER_SERVER_REMOVE);
exports.route = function (req, res) {
   var parameters = {
      token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)],
      watcherServerId: req.body[safeGet(commonConstants.ALL_PARAMETERS.WATCHER_SERVER_ID)]
   }

   myRequestValidation(parameters, req, res, function (err) {
      if (!err) {
         doRemove(parameters, function (result) {
            res.send(result);
         });
      }
   });
}

function doRemove(parameters, callback) {
   sessionManager.validateAdminToken(parameters.token, function (err) {
      if (err) {
         return callback(new ErrorResponse(err));
      }
      watcherServerController.removeWatcherServer(parameters.watcherServerId, function (err) {
         if (err) {
            var error = new MSInternalServerError()
               .logMessage(exports.URL + "Invalid watcherserver id: " + parameters.watcherServerId)
               .severe()
               .innerError(err)
               .logNow();
            return callback(new ErrorResponse(error));	
          }
         //find packages with this server id and deassign them
         database.getMultipleAccountBySubDocumentProperty("packages", "watcherServerId", parameters.watcherServerId, function (err, accounts) {
            if (accounts == null || accounts.length == 0) {
               return callback(new SuccessResponse());
            }
            accountIterator(0);
            function accountIterator(i) {
               if (i = accounts.length) {
                  if (err) {
                     var error = new MSInternalServerError()
                        .logMessage(exports.URL + "Invalid watcherserver id: " + parameters.watcherServerId)
                        .severe()
                        .innerError(err)
                        .logNow();
                     return callback(new ErrorResponse(error));	
                  }
                  return callback(new SuccessResponse());
               }
               var account = accounts[i];

               for (var j = 0; j < account.packages.length; j++) {
                  accounts[i].packages[j].watcherServerId = null;
               }

               account.save(function (err) {
                  if (err) {
                     var error = new MSInternalServerError()
                        .logMessage("Cannot save account after watcherserverid delete from serverAvailibityMonitor")
                        .innerError(err)
                        .logNow();
                  }
                  accountIterator(i + 1);
               });
            }
         });
      });
   });
}