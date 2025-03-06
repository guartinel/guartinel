package sysment.android.guartinel.core.persistance;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

import sysment.android.guartinel.core.utils.LOG;

/**
 * Created by sysment_dev on 11/10/2016.
 */
public class AlertHolder {
    public class Keys {
        public static final String ALERTS = "alerts";
    }

    public List<Alert> alerts = new ArrayList<>();

    public AlertHolder(String alertHolderJSON) {
        if (alertHolderJSON == null || alertHolderJSON == "") {
            return;
        }
        try {
            JSONObject alertHolder = new JSONObject(alertHolderJSON);
            JSONArray alertsArray = alertHolder.getJSONArray(Keys.ALERTS);
           if (alertsArray == null) {
                return;
            }
            for (int i = 0; i < alertsArray.length(); i++) {
                Alert newAlert = new Alert().BuildFromJSON(alertsArray.getJSONObject(i));
               alerts.add(newAlert);
            }

        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot parse AlertHolder. E: " + e.getMessage());
        }
    }

    private int MAX_ALERT_SIZE = 100;
    public void addNewAlert(Alert newAlert) {
        alerts.add( 0,newAlert);
        if (alerts.size() >= MAX_ALERT_SIZE) {
            alerts = alerts.subList(0, MAX_ALERT_SIZE);
        }
    }

    public String getJSONString() {
        JSONObject resultJSON = new JSONObject();
        try {
            JSONArray alertsArray = new JSONArray();
            for (Alert alert : alerts) {
                alertsArray.put(alert.getJSON());
            }
            resultJSON.put(Keys.ALERTS, alertsArray);

        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot parse AlertHolder. E: " + e.getMessage());
        }
        return resultJSON.toString();
    }
}
