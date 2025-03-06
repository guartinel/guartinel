/**
 * Created by DTAP on 2016.06.09..
 */

var jobRunner = include('guartinel/admin/jobs/jobRunner.js');
var sessionManager = include('guartinel/security/sessionManager.js');

exports.URL = safeGet(managementServerUrls.WATCHER_SERVER_REQUEST_SYNCHRONIZATION);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doRequestSynchronization(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doRequestSynchronization(parameters, callback) {
    sessionManager.validateMSTokenAndGetWatcherServer(parameters.token, function (err, server) {
        if(err){
            return callback(err);
        }
       jobRunner.runPackageTimeStampMonitoring(server);
        return callback(new SuccessResponse());
    });
}
