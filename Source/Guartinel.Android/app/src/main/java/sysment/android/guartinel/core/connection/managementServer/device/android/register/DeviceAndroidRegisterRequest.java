package sysment.android.guartinel.core.connection.managementServer.device.android.register;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.RegisterDeviceResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks._RegisterResultCallback;

/**
 * Created by sysment_dev on 02/23/2016.
 */
public class DeviceAndroidRegisterRequest extends HttpRequest {
    protected static class Constants {

        public static String URL = "/device/android/register";

        public static class RequestKeys {
            public static final String EMAIL = "email";
            public static final String PASSWORD = "password";
            public static final String DEVICE_NAME = "device_name";
            public static final String GCM_ID = "gcm_id";
            public static final String DEVICE_UUID = "device_uuid";
            public static final String PROPERTIES = "properties";
        }
    }

    public DeviceAndroidRegisterRequest(String deviceName, String email, String passwordHash, String androidIdHash, String gcmToken, PresenterResultCallback listener) {
        super(listener);
        try {
            _requestParametersJSON.put(Constants.RequestKeys.EMAIL, email);
            _requestParametersJSON.put(Constants.RequestKeys.PASSWORD, passwordHash);
            _requestParametersJSON.put(Constants.RequestKeys.DEVICE_NAME, deviceName);
            _requestParametersJSON.put(Constants.RequestKeys.GCM_ID, gcmToken);
            _requestParametersJSON.put(Constants.RequestKeys.DEVICE_UUID, androidIdHash);
            _requestParametersJSON.put(Constants.RequestKeys.PROPERTIES, SystemInfoUtil.getSystemProperties());
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create Register Request e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        DeviceAndroidRegisterResponse response = null;

        try {
            response = new DeviceAndroidRegisterResponse(responseJSON);
        } catch (JSONException | InternalSystemErrorException e) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onSuccess(response.getToken());
            return;
        }

        String error = response.error;
        if (error.equals(DeviceAndroidRegisterResponse.ErrorValues.INVALID_USER_NAME_OR_PASSWORD)) {
            cast(_callback).onInvalidUserNameOrPassword();
            return;
        } else if (error.equals(DeviceAndroidRegisterResponse.ErrorValues.DEVICE_ALREADY_REGISTERED)) {
            cast(_callback).onDeviceNameAlreadyTaken();
            return;
        } else if (error.equals(DeviceAndroidRegisterResponse.ErrorValues.ACCOUNT_EXPIRED)) {
            cast(_callback).onAccountExpired();
            return;
        }

        LOG.I("Register device response is unknown:" + responseJSON.toString());
        cast(_callback).onConnectionError();
    }

    private RegisterDeviceResultCallback cast(PresenterResultCallback callback) {
        return ((RegisterDeviceResultCallback) callback);
    }

    @Override
    public String getRequestJSONString() {
        return _requestParametersJSON.toString();
    }

    @Override
    public String getURL() {
        return Constants.URL;
    }
}
