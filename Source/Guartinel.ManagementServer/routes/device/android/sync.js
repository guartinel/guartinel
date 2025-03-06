/*var sessionManager = include('guartinel/security/sessionManager.js');
var configurationManager = include('guartinel/configuration/configurationManager.js');
var constants = include('guartinel/constants.js');
var database = include("guartinel/database/databaseConnector.js");

exports.URL = "/device/android/sync";
exports.route = function (req, res) {
    var parameters = {
        token : req.body.token,
        timeStamp : req.body.configuration_time_stamp,
        gcmID : req.body.gcm_id
    }
    
    myRequestValidation(parameters, req, res, function (requestErr) {
        if (!requestErr) {
            doSync(parameters, function (result) {
                return res.send(result);
            });
        }
    });
}

function doSync(parameters, callback) {
    sessionManager.validateDeviceToken(parameters.token, function (sessionErr) {
        if (sessionErr) {
            return callback({ content: sessionErr });
        }
        database.saveGCMID(parameters.token, parameters.gcmID, function (dbErr) {
            if (dbErr) {
                return callback({ content: dbErr });
            }
            return callback({ content: constants.RESULT.SUCCESS });
        });
    });
}
*/