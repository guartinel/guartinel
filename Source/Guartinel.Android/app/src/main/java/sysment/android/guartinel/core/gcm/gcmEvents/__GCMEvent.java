package sysment.android.guartinel.core.gcm.GCMEvents;

/**
 * Created by sysment_dev on 02/26/2016.

public class __GCMEvent {
    private String timeStamp, message, messageID;
    private boolean isResolved = false;

    public class Keys {
        public static final String ALERT_MESSAGE = "message";
        public static final String MESSAGE_ID = "message_id";
    }

    public __GCMEvent(String timeStamp, String messageJSONString, boolean isResolved) {
        this.timeStamp = timeStamp;
        this.isResolved = isResolved;

        try {
            JSONObject messageJSON = new JSONObject(messageJSONString);

            message = messageJSON.getString(Keys.ALERT_MESSAGE);
            messageID = messageJSON.getString(Keys.MESSAGE_ID);

        } catch (JSONException e) {
            e.printStackTrace();
            LogWrapper.Log("Cannot parse GCM message: " + e.getMessage());
        }
    }

    public String getTimeStamp() {
        return timeStamp;
    }

    public String getMessage() {
        return message;
    }

    public String getMessageID() {
        return messageID;
    }


    public boolean isResolved() {
        return isResolved;
    }
}
 */