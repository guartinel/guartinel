/**
 * Created by DTAP on 2016.08.09..
 */
var sessionManager = include('guartinel/security/sessionManager.js');
var config = include('guartinel/admin/configuration/configurationController.js');
var securityTool = include('guartinel/security/tool.js');
var database = include('guartinel/database/public/databaseConnector.js');

exports.URL = safeGet(managementServerUrls.ADMIN_DATABASE_GET_STATUS);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doGetStatus(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doGetStatus(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        if (!database.isConnectedToDB()) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.NO_DATABASE_CONNECTED)
              .logMessage("No database is connected to management server!" + parameters.verificationCode).logNow();
           return callback(new ErrorResponse(error));   
         }
        database.disconnect(function(err){
            if(err){
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Cannot disconnect DB ")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
         //   config.USE().setDatabaseConfiguration("","","");
         //   config.save();
        });
    });
}