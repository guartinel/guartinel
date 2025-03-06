/**
 * Created by DTAP on 2017.08.18..
 */
import * as security from "../../../security/tool";

export function getSchema  (mongoose) {
   var controlledEmailSchema = mongoose.Schema({
      email: { type: String, get: security.decryptText, set: security.encryptText},
      createdOn :{ type: Date, default: Date.now },
      blackListToken : String,
      blackListedOn: Date,
      isBlackListed : { type: Boolean, default: false }
    });
return controlledEmailSchema;
};