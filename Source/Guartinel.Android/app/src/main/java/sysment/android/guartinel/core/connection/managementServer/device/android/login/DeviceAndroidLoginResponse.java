package sysment.android.guartinel.core.connection.managementServer.device.android.login;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;

/**
 * Created by sysment_dev on 02/24/2016.
 */
public class DeviceAndroidLoginResponse extends HttpResponse {
    public static class Keys {
        public static final String TOKEN = "token";
        public static final String DEVICE_NAME = "device_name";
    }

    public static class ErrorValues extends HttpResponse.ErrorValues {
        public static final String INVALID_USER_NAME_OR_PASSWORD = "INVALID_USER_NAME_OR_PASSWORD";
        public static final String DEVICE_NOT_REGISTERED = "DEVICE_NOT_REGISTERED";
        public static final String UPDATE_NOW = "UPDATE_NOW";
    }

    private String token;
    private String deviceName;

    public DeviceAndroidLoginResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
        super(responseJSON);
        if (!isSuccess()) {
            return;
        }
        this.token = responseJSON.getString(Keys.TOKEN);
        this.deviceName = responseJSON.getString(Keys.DEVICE_NAME);
    }

    public String getToken() {
        return token;
    }

    public String getDeviceName() {
        return deviceName;
    }
}
