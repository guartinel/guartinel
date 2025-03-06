/**
 * Created by DTAP on 2017.04.28..
 */
import { Const } from "../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import { LOG } from "../../diagnostics/LoggerFactory";
import pluginConstants = Const.pluginConstants;
import commonConstants = Const.commonConstants;
import { ErrorResponse, SuccessResponse } from "../../guartinel/response/Response";
import { MSError, MSInternalServerError } from "../../error/Errors";


let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(managementServerUrls.HARDWARE_SUPERVISOR_REMOTE_LOG);

var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
   var parameters = {
      caption: req.body[traceIfNull(pluginConstants.CAPTION)],
      message: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.MESSAGE)],
      instanceId: req.body[traceIfNull(pluginConstants.INSTANCE_ID)],
      firmware: req.body[traceIfNull(pluginConstants.FIRMWARE)]
   }
   global.myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         doRemoteLog(parameters, function (result) {
            return res.send(result);
         });
      }
   });
}

function doRemoteLog(parameters, callback) {
   debugRoute("Remote log done");
   var response = new SuccessResponse(); 
   return callback(response);
}

