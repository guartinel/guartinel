/**
 * Created by DTAP on 2017.08.18..
 */


exports.getSchema = function (mongoose) {
    var adminAccount = mongoose.Schema({
        userName: String,
        salt: String,
        passwordHash: String,
        tokens: [mongoose.Schema.Types.Mixed]
    });
    return adminAccount;
};