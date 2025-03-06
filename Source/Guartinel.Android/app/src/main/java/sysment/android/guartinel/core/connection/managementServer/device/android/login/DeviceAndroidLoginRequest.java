package sysment.android.guartinel.core.connection.managementServer.device.android.login;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.connection.managementServer.device._register._RegisterResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.ui.presenterCallbacks.LoginDeviceResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public class DeviceAndroidLoginRequest extends HttpRequest {
    public static String URL = "/device/android/login";

    public static class Keys {
        public static final String EMAIL = "email";
        public static final String PASSWORD = "password";
        public static final String DEVICE_UUID = "device_uuid";
        public static final String GCM_ID = "gcm_id";
        public static final String VERSION_CODE = "version_code";
    }

    public DeviceAndroidLoginRequest(String email, String passwordHash, String androidIdHash, String gcmId, LoginDeviceResultCallback callback) {
        super(callback);
        try {
            _requestParametersJSON.put(Keys.EMAIL, email);
            _requestParametersJSON.put(Keys.PASSWORD, passwordHash);
            _requestParametersJSON.put(Keys.DEVICE_UUID, androidIdHash);
            _requestParametersJSON.put(Keys.GCM_ID, gcmId);
            _requestParametersJSON.put(Keys.VERSION_CODE, SystemInfoUtil.getVersionCode());
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create _deviceLogin request e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        DeviceAndroidLoginResponse response = null;
        try {
            response = new DeviceAndroidLoginResponse(responseJSON);
        } catch (JSONException | InternalSystemErrorException e) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onLoginSuccess(response.getToken(), response.getDeviceName());
            return;
        }

        String error = response.error;
        if (error.equals(DeviceAndroidLoginResponse.ErrorValues.INVALID_USER_NAME_OR_PASSWORD)) {
            cast(_callback).onInvalidUserNameOrPassword();
            return;
        } else if (error.equals(DeviceAndroidLoginResponse.ErrorValues.DEVICE_NOT_REGISTERED)) {
            cast(_callback).onDeviceNotRegistered();
            return;
        } else if (error.equals(DeviceAndroidLoginResponse.ErrorValues.ACCOUNT_EXPIRED)) {
            cast(_callback).onAccountExpired();
            return;
        } else if (error.equals(DeviceAndroidLoginResponse.ErrorValues.UPDATE_NOW)) {
            cast(_callback).onUpdateNow();
            return;
        }
        cast(_callback).onConnectionError();
        LOG.I("Login Response is unknown:" + responseJSON.toString());
    }

    private LoginDeviceResultCallback cast(PresenterResultCallback callback) {
        return ((LoginDeviceResultCallback) callback);
    }

    @Override
    public String getRequestJSONString() {
        return _requestParametersJSON.toString();
    }

    @Override
    public String getURL() {
        return URL;
    }
}
