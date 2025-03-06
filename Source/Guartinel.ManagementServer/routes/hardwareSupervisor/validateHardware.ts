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
var database = global.include("guartinel/database/public/databaseConnector.js");
import * as securityTool from "../../guartinel/security/tool";
import * as config from "../../guartinel/admin/configuration/configurationController"
import { HardwareSupervisorConfiguration } from
   "../../guartinel/packages/configurations/hardwareSupervisorConfiguration";
import { Package } from "../../guartinel/packages/package";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(managementServerUrls.HARDWARE_SUPERVISOR_VALIDATE_HARDWARE);

var isRouteDebugEnabled = true;
function debugRoute(message) {
   if (!isRouteDebugEnabled) {
      return;
   }
   LOG.debug(exports.URL + " " + message);
}

exports.route = function (req, res) {
   var parameters = {
      hardwareToken: req.body[traceIfNull(pluginConstants.HARDWARE_TOKEN)]
   }
   global.myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         validateHardware(parameters, function (result) {
            return res.send(result);
         });
      }
   });
}

function validateHardware(parameters, callback) {
       var response = new SuccessResponse();
      return callback(response); 
}

