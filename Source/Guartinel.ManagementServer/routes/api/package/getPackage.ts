import { Const } from "../../../common/constants";
import managementServerUrls = Const.managementServerUrls;
import commonConstants = Const.commonConstants;
import { ErrorResponse, SuccessResponse } from "../../../guartinel/response/Response";
import { MSError } from "../../../error/Errors";
import { APIPackage } from "../../../guartinel/packages/apiPackage";
var sessionManager = global.include("guartinel/security/sessionManager.js");
let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

export let URL = traceIfNull(managementServerUrls.API_PACKAGE_GET_PACKAGE);
export function route(req, res) {
   var parameters = {
      token: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.TOKEN)],
      packageName: req.body[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE_NAME)]
   }
   global.myRequestValidation(parameters, req, res, function (err) {
      if (!err) {
         processRequest(parameters, function (result) {
            res.send(result);
         });
      }
   });
}


function processRequest(parameters, callback) {
   sessionManager.validateAPITokenAndGetAccount(parameters.token, function (sessionErr, account) {
      if (sessionErr) {
         return callback(new ErrorResponse(sessionErr));
      }

      var foundPack;

      for (var index = 0; index < account.packages.length; index++) {
         if (account.packages[index].isDeleted) {
            continue;
         }
         if (account.packages[index].packageName === parameters.packageName) {
            foundPack = account.packages[index];
            break;
         }
      }

      if (utils.object.isNull(foundPack)) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.INVALID_PACKAGE_NAME)
            .logMessage("Cannot get package by name: " + parameters.packageName)
            .logNow();
         return callback(new ErrorResponse(error));
      }

      let packageObject = new APIPackage(true, account);
      packageObject.initFromObject(foundPack);
      var result = new SuccessResponse();
      result[traceIfNull(commonConstants.ALL_PARAMETERS.PACKAGE)] = packageObject.toJSON();//DTOBuilder.getPackageDTO(foundPack,account);
      return callback(result);
   }
   );
}