/**
 * Created by DTAP on 2016.12.02..
 */
var constants = include('guartinel/constants.js');
var sessionManager = include('guartinel/security/sessionManager.js');
var configController = include('guartinel/admin/configuration/configurationController.js');
var securityTool = include('guartinel/security/tool.js');

exports.URL = safeGet(managementServerUrls.ADMIN_SET_WEBSITE_ADDRESS);
exports.route = function (req, res) {
    var parameters = {
        websiteAddress: req.body[safeGet(commonConstants.ALL_PARAMETERS.WEBSITE_ADDRESS)],
        token: req.body[safeGet(commonConstants.ALL_PARAMETERS.TOKEN)]
    };

    myRequestValidation(parameters, req, res, function (err) {
        if (!err) {
            doSetWebsiteAddress(parameters, function (result) {
                res.send(result);
            });
        }
    });
}

function doSetWebsiteAddress(parameters, callback) {
    sessionManager.validateAdminToken(parameters.token, function (err) {
        if (err) {
            return callback(new ErrorResponse(err));
        }

        configController.getLocalConfiguration(function (err, config) {
            config.emailConfiguration.emailProvider = parameters.emailProvider;
            config.emailConfiguration.emailUserName = parameters.emailUserName;
            config.emailConfiguration.emailPassword = parameters.emailPassword;
            config.webpageURL = parameters.websiteAddress;

            config.save(function (err) {
                if(err){
                   var error = new MSInternalServerError()
                      .logMessage("Invalid supervisorStatus token : " + parameters.token)
                      .innerError(err)
                      .logNow();
                   return callback(new ErrorResponse(error));   
                }
                return callback(new SuccessResponse());
            });
        });
    });
}