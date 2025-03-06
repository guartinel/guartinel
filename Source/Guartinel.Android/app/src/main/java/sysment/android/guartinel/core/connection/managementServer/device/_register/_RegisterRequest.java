package sysment.android.guartinel.core.connection.managementServer.device._register;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks._RegisterResultCallback;

/**
 * Created by sysment_dev on 02/23/2016.
 */
public class _RegisterRequest extends HttpRequest {
    protected static class Constants {

        public static String URL = "/device/register";
        public static String ANDROID_DEVICE_TYPE = "android_device";

        public static class RequestKeys {
            public static final String EMAIL = "email";
            public static final String PASSWORD = "password";
            public static final String DEVICE_TYPE = "device_type";
            public static final String DEVICE_NAME = "device_name";
            public static final String GCM_ID = "gcm_id";
            public static final String UID = "uid";
            public static final String OVER_WRITE = "over_write";
            public static final String PROPERTIES = "properties";
        }
    }

    public _RegisterRequest(String email, String password, String deviceName, String gcmId, String androidId, boolean overWrite, PresenterResultCallback listener) {
        super(listener);
        try {
            _requestParametersJSON.put(Constants.RequestKeys.EMAIL, email);
            _requestParametersJSON.put(Constants.RequestKeys.PASSWORD, password);
            _requestParametersJSON.put(Constants.RequestKeys.DEVICE_TYPE, Constants.ANDROID_DEVICE_TYPE);
            _requestParametersJSON.put(Constants.RequestKeys.DEVICE_NAME, deviceName);
            _requestParametersJSON.put(Constants.RequestKeys.GCM_ID, gcmId);
            _requestParametersJSON.put(Constants.RequestKeys.UID, androidId);
            _requestParametersJSON.put(Constants.RequestKeys.OVER_WRITE, overWrite);
            _requestParametersJSON.put(Constants.RequestKeys.PROPERTIES, SystemInfoUtil.getSystemProperties());

        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create Register Request e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        _RegisterResponse response = null;

        try {
            response = new _RegisterResponse(responseJSON);
        } catch (JSONException e) {
            cast(_callback).onConnectionError();
            return;
        } catch (InternalSystemErrorException internalServerError) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onRegisterSuccess(response);
            return;
        }

        String error = response.error;
        if (error.equals(_RegisterResponse.ErrorValues.INVALID_USER_NAME_OR_PASSWORD)) {
            cast(_callback).onInvalidUserNameOrPasswordError();
            return;
        } else if (error.equals(_RegisterResponse.ErrorValues.DEVICE_NAME_TAKEN)) {
            cast(_callback).onDeviceNameError();
            return;
        } else if (error.equals(_RegisterResponse.ErrorValues.ACCOUNT_EXPIRED)) {
            cast(_callback).onAccountExpired();
            return;
        } else if (error.equals(_RegisterResponse.ErrorValues.MAXIMUM_DEVICE_COUNT_REACHED)) {
            cast(_callback).onMaximumDeviceCountReached();
            return;
        }

        LOG.I("Register Response is unknown:" + responseJSON.toString());
        cast(_callback).onConnectionError();
    }

    private _RegisterResultCallback cast(PresenterResultCallback callback) {
        return ((_RegisterResultCallback) callback);
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
