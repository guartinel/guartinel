/**
 * Created by DTAP on 2017.08.18..
 */
exports.getSchema = function (mongoose) {
    var price = {
        price: Number,
        interval: Number
    };
    var licenseSchema = mongoose.Schema({
        createdOn: {type: Date, default: Date.now},
        name: String,
        caption: String,
        categories: [String],
        maximumPackages: Number,
        packageConstraints: mongoose.Schema.Types.Mixed,
        maximumDevices: Number,
        minimumCheckIntervalSec: Number,
        maximumAlertsPerHour: Number,
        maximumPackagePartCount: {type:Number, default :3},
        canUseAPI:{type:Boolean,default:false},
        prices: [price]
    });
    return licenseSchema;
};