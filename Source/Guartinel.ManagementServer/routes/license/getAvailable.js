/**
 * Created by DTAP on 2016.06.28..
 */
var sessionManager = include('guartinel/security/sessionManager.js');
var database = include('guartinel/database/public/databaseConnector.js');
var licenseController = include('guartinel/license/licenseController.js');

exports.URL = safeGet(managementServerUrls.LICENSE_GET_AVAILABLE);
exports.route = function (req, res) {
    var parameters = {
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    }

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doGetAvailable(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doGetAvailable(parameters, callback) {
    sessionManager.validateBrowserTokenAndGetAccount(parameters.token, function (err, account) {
        if (err) {
            return callback(new ErrorResponse(err));
        }
        database.getMultipleLicenses(null, null, function (err, licenses) {
            var licensesDTO = [];
            for (var i = 0; i < licenses.length; i++) {
                if (licenses[i].name === "free") {
                    continue;
                }
                if ((licenses[i].name === "trial ") && !account.isTrialAvailable()) {
                    continue;
                }
                var pricesDTO = [];
                for (var j = 0; j < licenses[i].prices.length; j++) {
                    var priceDTO = {
                        price: licenses[i].prices[j].price,
                        interval: licenses[i].prices[j].interval
                    };
                    pricesDTO.push(priceDTO);
                }

                var licenseDTO = {
                    id: licenses[i]._id.toString(),
                    name: licenses[i].name,
                    caption: licenses[i].caption,
                    categories: licenses[i].categories,
                    maximumPackages: licenses[i].maximumPackages,
                    packageConstraints: licenses[i].packageConstraints,
                    minimumCheckIntervalSec: licenses[i].minimumCheckIntervalSec,
                    maximumPackagePartCount: licenses[i].maximumPackagePartCount,
                    prices: pricesDTO
                }
                licensesDTO.push(licenseDTO);
            }
            var response = new SuccessResponse();
            response[commonConstants.ALL_PARAMETERS.LICENSES] = licensesDTO;
            account.save(function (err) {
                if(err){
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
    });
}
