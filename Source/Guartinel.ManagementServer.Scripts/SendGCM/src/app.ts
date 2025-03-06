import * as gcm from "node-gcm";

var sender:gcm.Sender = new gcm.Sender("AIzaSyBMsUFWc2k-_lsoZ8axBii50f3LnmwsWv4");
var gcmMessage:gcm.Message = new  gcm.Message();

gcmMessage.addData('title', 'Guartinel');
gcmMessage.addData('alert_message', "Test");
gcmMessage.addData('alert_details', "TestElek");
gcmMessage.addData('alert_id', "123456");
gcmMessage.addData('package_name', "Test");
gcmMessage.addData('is_package_alerted', "true");
gcmMessage.addData('forced_device_alert', "true");
gcmMessage.addData("is_recovery", "false");


sender.send(gcmMessage,"cH7fePl9n_Y:APA91bHsQvO4LnaBNrhwlt2j_zCMEgsR_GZLSepHH98DSxr3U7m0EMlZsubwt4IcFmjxc4eU9_95QPLgN2lGy-7O3wZWENIrJbwYd3JTFmY4OGxcaJy5IkV-xPoHz31j34ZygIYPbtaM",function onResult(err:any,responseJson:gcm.IResponseBody){
    console.log(JSON.stringify(responseJson));
});