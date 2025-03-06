package sysment.guartinelandroidsystemwatcher.connection;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.guartinelandroidsystemwatcher.util.LogWrapper;

/**
 * Created by sysment_dev on 11/17/2016.
 */
public class _Server {
    public static class KEYS {
        public static String TOKEN = "token";
        public static String ADDRESS = "address";
        public static String IS_AVAILABLE = "is_available";
        public static String LAST_UPDATED = "last_updated";
        public static String STATUS_MESSAGE = "status_message";
    }

    public _Server fromJSON(JSONObject json) {
        try {
            this.lastUpdated = json.getString(KEYS.LAST_UPDATED);
            this.statusMessage = json.getString(KEYS.STATUS_MESSAGE);
            this.token = json.getString(KEYS.TOKEN);
            this.address = json.getString(KEYS.ADDRESS);
            this.isAvailable = json.getBoolean(KEYS.IS_AVAILABLE);
        } catch (JSONException e) {
            LogWrapper.Err("Cannot parse server from JSON :" + json.toString(), e);
        }
        return this;
    }

    public _Server fromValues(String address, String token, boolean isAvailable) {
        this.token = token;
        this.address = address;
        this.isAvailable = isAvailable;
        this.lastUpdated = "Updated: Not Yet Received";
        this.statusMessage = "Not Yet Received.";
        return this;
    }

    public JSONObject toJSON() {
        JSONObject resultJSON = new JSONObject();
        try {
            resultJSON.put(KEYS.LAST_UPDATED, this.lastUpdated);
            resultJSON.put(KEYS.STATUS_MESSAGE, this.statusMessage);
            resultJSON.put(KEYS.TOKEN, this.token);
            resultJSON.put(KEYS.ADDRESS, this.address);
            resultJSON.put(KEYS.IS_AVAILABLE, this.isAvailable);
        } catch (JSONException e) {
            LogWrapper.Err("Cannot serialize server to json . Error: ", e);

        }
        return resultJSON;
    }

    public String getLastUpdateTime() {
        return lastUpdated;
    }

    private String token, address, lastUpdated, statusMessage ="Not Yes Received";
    private boolean isAvailable = false;

    public String getToken() {
        return token;
    }

    public String getAddress() {
        return address;
    }

    public String getStatusMessage() {
        return  statusMessage;
    }

    public boolean getIsAvailable() {
        return isAvailable;
    }

    public void setIsAvailable(boolean value) {
        this.isAvailable = value;
    }

    public void setLastUpdated(String value) {
        this.lastUpdated = "Last Updated: " + value;
    }
    public void setStatus(String value) {
        this.statusMessage = value;
    }


}
