var sessionManager = include("guartinel/security/sessionManager.js");
var constants = include("guartinel/constants.js");
var database = include("guartinel/database/public/databaseConnector.js");
var licenseController = include("guartinel/license/licenseController.js");

exports.URL = safeGet(managementServerUrls.ACCOUNT_GET_STATUS);

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

    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
           return callback(new ErrorResponse(err));
        }


        var deviceCount = 0;
        if (!utils.object.isNull(account.devices)) {
            deviceCount = account.devices.length;
        }

        var DTOAccount = {};
        DTOAccount[safeGet(commonConstants.ALL_PARAMETERS.ID)] = account._id.toString();
        DTOAccount[safeGet(commonConstants.ALL_PARAMETERS.CREATED_ON)] = account.createdOn;
        DTOAccount[safeGet(commonConstants.ALL_PARAMETERS.FIRST_NAME)] = account.firstName;
        DTOAccount[safeGet(commonConstants.ALL_PARAMETERS.LAST_NAME)] = account.lastName;
        DTOAccount[safeGet(commonConstants.ALL_PARAMETERS.EMAIL)] = account.email;
        DTOAccount[safeGet(commonConstants.ALL_PARAMETERS.IS_ACTIVATED)] = account.activationInfo.isActivated;
        DTOAccount[safeGet(commonConstants.ALL_PARAMETERS.LICENSE_AGGREGATE)] = account.getLicenseAggregate();

        var DTOAccountLicenses = [];
        for (var i = 0; i < account.licenses.length; i++) {
            var licenseDTO = {};
            licenseDTO["startDate"] = account.licenses[i].startDate;
            licenseDTO["expiryDate"] = account.licenses[i].expiryDate;
            licenseDTO["license"] = account.licenses[i].license;
            licenseDTO["id"] = account.licenses[i]._id;
            DTOAccountLicenses.push(licenseDTO);
        }


        DTOAccount[safeGet(commonConstants.ALL_PARAMETERS.LICENSES)] = DTOAccountLicenses;

        var response = new SuccessResponse();
        response[safeGet(commonConstants.ALL_PARAMETERS.ACCOUNT)] = DTOAccount;
        account.save(function(err){
            if (err) {
               var error = new MSInternalServerError()
                  .logMessage(exports.URL + ": Cannot save account")
                  .severe()
                  .innerError(err)
                  .logNow();
               return callback(new ErrorResponse(error));
            }
            return callback(response);
        });
     });
}