package sysment.android.guartinel.core.connection.managementServer.device.android.register;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.connection.managementServer.device._login._LoginResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;

/**
 * Created by sysment_dev on 02/24/2016.
 */
public class DeviceAndroidRegisterResponse extends HttpResponse {
    private String token;

    public static class Keys extends _LoginResponse.Keys {
        public static final String DEVICE_UUID = "device_uuid";
    }

    public static class ErrorValues extends HttpResponse.ErrorValues {
        public static final String INVALID_USER_NAME_OR_PASSWORD = "INVALID_USER_NAME_OR_PASSWORD";
        public static final String DEVICE_ALREADY_REGISTERED = "DEVICE_ALREADY_REGISTERED";
        public static final String ACCOUNT_EXPIRED = "ACCOUNT_EXPIRED";
        public static final String MAXIMUM_DEVICE_COUNT_REACHED = "MAXIMUM_DEVICE_COUNT_REACHED";
    }


    public DeviceAndroidRegisterResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
        super(responseJSON);
        if (!isSuccess()) {
            return;
        }
        this.token = responseJSON.getString(Keys.TOKEN);
      }

    public String getToken() {
        return token;
    }

}
