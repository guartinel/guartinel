var database = include("guartinel/database/public/databaseConnector.js");

var DEFAULT_CATEGORY_NAME = "free";
var TRIAL_LICENSE_NAME = "trial";

exports.createDefaultLicenseIfNeeded = function (callback) {
    database.getLicenseByProperty('name', DEFAULT_CATEGORY_NAME, function (err, license) {
        if (utils.object.isNull(license)) {
            var defaultLicense = new database.getLicenseModel()({
                name: DEFAULT_CATEGORY_NAME,
                caption: "This is the free license",
                categories: ["free", "default"],
                maximumPackages: 2,
                minimumCheckIntervalSec: 1800,
                maximumPackagePartCount :2,
                prices:[{price:0,interval:100}]
            });
            defaultLicense.save(function (err) {
                callback(err);
            });
        } else {
            return callback();
        }
    });
}

exports.getDefaultLicense = function (callback) {
    database.getLicenseByProperty('name', DEFAULT_CATEGORY_NAME, function (err, license) {
        return callback(err, license);
    });
};

exports.getTrialLicense = function(callback){
    database.getLicenseByProperty('name', TRIAL_LICENSE_NAME, function (err, license) {
        return callback(err, license);
    });
}
;

exports.getDTOFromDBO = function (licenses) {//needed to avoid valuable data leak
    var licensesDTO = [];
    for (var i = 0; i < licenses.length; i++) {
        var licenseDTO = {
            id: licenses[i].license._id.toString(),
            name: licenses[i].license.name,
            caption: licenses[i].license.caption,
            categories: licenses[i].categories,
            maximumPackages: licenses[i].maximumPackages,
            packageConstraints: licenses[i].packageConstraints,
            maximumDevices: licenses[i].maximumDevices,
            minimumCheckIntervalSec: licenses[i].minimumCheckIntervalSec,
            maximumAlertsPerHour: licenses[i].maximumAlertsPerHour,
            prices: licenses[i].prices
        }
        licensesDTO.push(licenseDTO);
    }
    return licensesDTO;
}



