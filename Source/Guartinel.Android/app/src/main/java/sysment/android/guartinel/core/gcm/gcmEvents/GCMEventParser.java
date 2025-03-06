package sysment.android.guartinel.core.gcm.GCMEvents;

/**
 * Created by sysment_dev on 02/26/2016.

public class GCMEventParser {

    protected static class Constants {
        protected static class Keys {
            protected static final String ALERT_MESSAGE = "ALERT_MESSAGE";
            protected static final String MESSAGE_ID = "MESSAGE_ID";
            protected static final String TIMESTAMP = "TIMESTAMP";
            protected static final String IS_RESOLVED = "IS_RESOLVED";
        }
    }

    public static String serializeEvents(ArrayList<_GCMEvent> events) {
        JSONArray array = new JSONArray();
        for (_GCMEvent event : events) {
            JSONObject jsonObject = getJSONObjectFromEvent(event);
            array.put(jsonObject);
        }
        return array.toString();
    }

    public static JSONObject getJSONObjectFromEvent(_GCMEvent event) {
        JSONObject result = new JSONObject();
        try {
            result.put(Constants.Keys.ALERT_MESSAGE, event.getMessage());
            result.put(Constants.Keys.MESSAGE_ID, event.getMessageID());
            result.put(Constants.Keys.TIMESTAMP, event.getTimeStamp());
            result.put(Constants.Keys.IS_RESOLVED, event.isResolved());
        } catch (JSONException e) {
            LogWrapper.Log("JSON exception :" + e.getMessage());
        }
        return result;
    }

    public static ArrayList<_GCMEvent> deserializeEvents(String eventsSerialized) {
        ArrayList<_GCMEvent> events = new ArrayList<_GCMEvent>();
        if (eventsSerialized == null) {
            return events;
        }
        try {
            JSONArray array = new JSONArray(eventsSerialized);
            for (int i = 0; i < array.length(); i++) {
                JSONObject eventJSON = array.getJSONObject(i);
                events.add(getEventFromJSON(eventJSON));
            }
        } catch (JSONException e) {
            LogWrapper.Log("JSON exception :" + e.getMessage());
        }
        return events;
    }

    public static _GCMEvent getEventFromJSON(JSONObject eventJSON) throws JSONException {
        String message = eventJSON.getString(Constants.Keys.ALERT_MESSAGE);
        String timeStamp = eventJSON.getString(Constants.Keys.TIMESTAMP);
        String messageID = eventJSON.getString(Constants.Keys.MESSAGE_ID);
        boolean isResolved = eventJSON.getBoolean(Constants.Keys.IS_RESOLVED);
        return new _GCMEvent(timeStamp, message, isResolved);
    }
}
 */