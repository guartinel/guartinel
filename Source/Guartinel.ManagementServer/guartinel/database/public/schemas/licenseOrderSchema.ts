/**
 * Created by DTAP on 2017.10.05..
 */
import * as security from "../../../security/tool";
export function getSchema(mongoose){
    var licenseOrderSchema = mongoose.Schema({
       data: { type: mongoose.Schema.Types.Mixed, get: security.decryptObject, set: security.encryptObject },
    });
    return licenseOrderSchema;
};