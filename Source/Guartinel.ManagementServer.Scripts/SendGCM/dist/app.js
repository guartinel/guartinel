"use strict";
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const gcm = __importStar(require("node-gcm"));
var sender = new gcm.Sender("AIzaSyBMsUFWc2k-_lsoZ8axBii50f3LnmwsWv4");
var gcmMessage = new gcm.Message();
gcmMessage.addData('title', 'Guartinel');
gcmMessage.addData('alert_message', "Test");
gcmMessage.addData('alert_details', "TestElek");
gcmMessage.addData('alert_id', "123456");
gcmMessage.addData('package_name', "Test");
gcmMessage.addData('is_package_alerted', "true");
gcmMessage.addData('forced_device_alert', "true");
gcmMessage.addData("is_recovery", "false");
sender.send(gcmMessage, "cH7fePl9n_Y:APA91bHsQvO4LnaBNrhwlt2j_zCMEgsR_GZLSepHH98DSxr3U7m0EMlZsubwt4IcFmjxc4eU9_95QPLgN2lGy-7O3wZWENIrJbwYd3JTFmY4OGxcaJy5IkV-xPoHz31j34ZygIYPbtaM", function onResult(err, responseJson) {
    console.log(JSON.stringify(responseJson));
});
//# sourceMappingURL=app.js.map