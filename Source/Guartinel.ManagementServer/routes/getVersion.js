/**
 * Created by DTAP on 2017.08.08..
 */
var statusInformer = include('guartinel/admin/statusInformer.js');
var moment = require('moment');
var timeTool = include('guartinel/utils/timeTool.js');
var watcherServerController = include('guartinel/admin/configuration/watcherServerController.js');
var httpRequester = include('guartinel/connection/httpRequester.js');

exports.URL = safeGet(managementServerUrls.GET_VERSION);
exports.route = function (req, res) {
    var parameters = {};
    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            processRequest(parameters, function (result) {
                res.send(result);
            });
        }
    });
};
var lastRequestTimestamp;
function processRequest(parameters, callback) {
    if (!utils.object.isNull(lastRequestTimestamp)) {
        var isElapsed = timeTool.isTimeAmountElapsedSinceTimeStamp(30, 'm', lastRequestTimestamp);
        if (!isElapsed) {
           var error = new MSError(commonConstants.ALL_ERROR_VALUES.TIMEOUT_IS_NOT_ELAPSED_SINCE_LAST_QUERY)
              .logMessage(exports.URL + ": Timeout violation")
              .logNow();
           return callback(new ErrorResponse(error));
         }
    }
    statusInformer.getProgramVersion(afterMSVersionReturned);
    function afterMSVersionReturned(msVersion) {
        lastRequestTimestamp = moment();
        var result = new SuccessResponse();
        var managementServer = {
            version: msVersion
        };
        result.managementServer = managementServer;
        watcherServerController.getAllWatcherServers(afterWatcherServersRetrieval);

        function afterWatcherServersRetrieval(servers){
            httpRequester.getVersion(servers[0],afterWatcherServerGetVersion);  //TODO change it to multiple servers
        }

        function afterWatcherServerGetVersion(err,version){
            result.watcherServer = {
                version:version
            };
            return callback(result);
        }
    }
}