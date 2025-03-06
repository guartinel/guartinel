package sysment.android.guartinel.core.persistance;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.utils.GeneralUtil;
import sysment.android.guartinel.core.utils.LOG;

/**
 * Created by sysment_dev on 11/10/2016.
 */
public class Alert {
    public class Keys {
        public static final String ALERT_MESSAGE = "alert_message";
        public static final String ALERT_DETAILS = "alert_details";
        public static final String TIME_STAMP = "time_stamp";
        public static final String ALERT_ID = "alert_id";
        public static final String UUID = "uuid";
        public static final String IS_RECOVERY = "is_recovery";
        public static final String PACKAGE_NAME = "PACKAGE_NAME";
        public static final String IS_PACKAGE_ALERTED = "is_package_alerted";
    }

    public String timeStamp;
    public String alertID;
    public String alertMessage;
    public String UUID;
    public boolean isRecovery;
    public boolean isPackageAlerted;
    public String packageName;
    public String alertDetails;

    public Alert BuildFromGCMMessage(String alertMessage,String alertDetails, String alertID, String packageName, boolean isRecovery,boolean isPackageAlerted) {
        this.timeStamp = GeneralUtil.getCurrentTimeString();
        this.alertMessage = alertMessage;
        this.alertID = alertID;
        this.UUID = java.util.UUID.randomUUID().toString();
        this.isRecovery = isRecovery;
        this.packageName = packageName;
        this.isPackageAlerted = isPackageAlerted;
        this.alertDetails = alertDetails;
        return this;
    }

    public Alert BuildFromJSON(JSONObject jsonObject) {
        try {
            timeStamp = jsonObject.getString(Keys.TIME_STAMP);
            alertMessage = jsonObject.getString(Keys.ALERT_MESSAGE);
            alertID = jsonObject.getString(Keys.ALERT_ID);
            UUID = jsonObject.getString(Keys.UUID);
            isRecovery = jsonObject.getBoolean(Keys.IS_RECOVERY);
            packageName = jsonObject.getString(Keys.PACKAGE_NAME);
            isPackageAlerted = jsonObject.getBoolean(Keys.IS_PACKAGE_ALERTED);
            alertDetails = jsonObject.getString(Keys.ALERT_DETAILS);
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot parse Alert. E: " + e.getMessage());
        }
        return this;
    }

    public JSONObject getJSON() {
        JSONObject alertJSON = new JSONObject();
        try {
            alertJSON.put(Keys.ALERT_MESSAGE, alertMessage);
            alertJSON.put(Keys.ALERT_ID, alertID);
            alertJSON.put(Keys.TIME_STAMP, timeStamp);
            alertJSON.put(Keys.UUID, UUID);
            alertJSON.put(Keys.IS_RECOVERY, isRecovery);
            alertJSON.put(Keys.PACKAGE_NAME, packageName);
            alertJSON.put(Keys.IS_PACKAGE_ALERTED, isPackageAlerted);
            alertJSON.put(Keys.ALERT_DETAILS, alertDetails);
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot parse AlertMessage. E: " + e.getMessage());
        }
        return alertJSON;
    }
}
