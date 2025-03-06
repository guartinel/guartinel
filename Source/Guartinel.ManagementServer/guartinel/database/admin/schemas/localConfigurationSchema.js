exports.getSchema = function (mongoose) {
    var emailConfiguration = {
        provider: String,
        userName: String,
        passwordEncrypted: String,
        packageStatusChangeTemplateId: Number,
        plainPackageStatusChangeTemplateId: Number,
        infoMailTemplateId: Number
    };

    var databaseConfiguration = {
        url: String,
        userName: String,
        passwordEncrypted: String
    };

    var localConfiguration = mongoose.Schema({
        address: {
            HTTPPort: String,
            HTTPSPort: String,
            fullHTTP: String,
            fullHTTPS: String,
            ip: String
        },
        gcm: mongoose.Schema.Types.Mixed,
        slack: {
            token: String
        },
        updateServer: {
            host: String,
            port: String,
            protocolPrefix: String
        },
        webpageURL: String,
        databaseConfiguration: databaseConfiguration,
        emailConfiguration: emailConfiguration,
        updateServerAddress: String,
        adminDeviceIds: [String]
    });
    return localConfiguration;
};
