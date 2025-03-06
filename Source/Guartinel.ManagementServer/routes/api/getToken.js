/**
 * Created by DTAP on 2017.08.07..
 */

var authenticator = include('guartinel/security/authenticator.js');
var securityTool = include("guartinel/security/tool.js");
var moment = require('moment');

exports.URL = safeGet(managementServerUrls.API_GET_TOKEN);
exports.route = function (req, res) {
   var parameters = {
      email: req.body[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)],
      password: req.body[safeGet(commonConstants.ALL_PARAMETERS.PASSWORD)]
   };
   myRequestValidation(parameters, req, res, function (err) {
      if (!err) {
         processRequest(parameters, function (result) {
            res.send(result);
         });
      }
   });
}

function processRequest(parameters, callback) {
   authenticator.authenticateAPIAndGetAccount(parameters.email, parameters.password, function (err, account) {
      if (err) {
         return callback(new ErrorResponse(err));
      }

      if (!account.getLicenseAggregate().canUseAPI) {
         var error = new MSError(commonConstants.ALL_ERROR_VALUES.MISSING_API_ACCESS_RIGHT)
            .logMessage("Account is missing API access right account:" + account.email)
            .innerError(err)
            .severe()
            .logNow();
         return callback(new ErrorResponse(error));
      }


      var rawToken = securityTool.generateToken();
      var hashedToken = securityTool.generatePasswordHash(rawToken, rawToken);
      account.api = {
         token: hashedToken,
         tokenTimeStamp: moment()
      };
      account.save(function (err) {
         if (err) {
            var error = new MSInternalServerError()
               .logMessage(exports.URL + ": Cannot save account")
               .severe()
               .innerError(err)
               .logNow();
            return callback(new ErrorResponse(error));
         }

         var response = new SuccessResponse();
         response[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)] = rawToken;
         return callback(response);
      });
   });
}