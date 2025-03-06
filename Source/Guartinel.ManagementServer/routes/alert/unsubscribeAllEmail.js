/**
 * Created by DTAP on 2017.08.08..
 */
exports.URL = safeGet(managementServerUrls.ALERT_UNSUBSCRIBE_ALL_EMAIL);

var isRouteDebugEnabled = true;
function debugRoute(message) {
    if (!isRouteDebugEnabled) {
        return;
    }
    LOG.debug(exports.URL + " " + message);
}
var moment = require('moment');
var database = include('guartinel/database/public/databaseConnector.js');
exports.route = function (req, res) {
    var parameters = {
        blackListToken: req.body[safeGet(commonConstants.ALL_PARAMETERS.BLACK_LIST_TOKEN)]
    };
    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            processRequest(parameters, function (result) {
                return res.send(result);
            });
        }
    });
};

function processRequest(parameters, callback) {
    database.getControlledEmailByProperty('blackListToken', parameters.blackListToken, function (err, controlledEmail) {
        if (err) {
           var error = new MSInternalServerError()
              .logMessage(exports.URL + ": Cannot retrieve controlled email by token :" + parameters.blackListToken)
              .severe()
              .innerError(err)
              .logNow();
           return callback(new ErrorResponse(error));	
         }

        if (utils.object.isNull(controlledEmail)) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_TOKEN)
              .logMessage("Invalid blacklist token" + parameters.blackListToken)
              .innerError(err)
              .logNow();
           return callback(new ErrorResponse(error));
         }
        controlledEmail.isBlackListed = true;
        controlledEmail.blackListedOn = moment().toISOString();

        controlledEmail.save(function (err) {
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Cannot save controlled email by token.")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
             }
            return callback(new SuccessResponse());
        });
    });
}