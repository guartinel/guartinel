package sysment.android.guartinel.core.connection.managementServer.device._login;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.connection.managementServer.device._register._RegisterResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.presenterCallbacks._LoginResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public class _LoginRequest extends HttpRequest {
    public static String URL = "/device/login";

    public static class Keys {
        public static final String PASSWORD = "password";
        public static final String DEVICE_UUID = "device_uuid";
        public static final String GCM_ID = "gcm_id";
    }

    public _LoginRequest(String password, String deviceUUID, String gcmId, _LoginResultCallback callback) {
        super(callback);
        try {
            _requestParametersJSON.put(Keys.PASSWORD, password);
            _requestParametersJSON.put(Keys.DEVICE_UUID, deviceUUID);
            _requestParametersJSON.put(Keys.GCM_ID, gcmId);
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create _deviceLogin request e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        _LoginResponse response = null;
        try {
            response = new _LoginResponse(responseJSON);
        } catch (JSONException | InternalSystemErrorException e) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onLoginSuccess(response);
            return;
        }

        String error = response.error;
        if (error.equals(_LoginResponse.ErrorValues.INVALID_USER_NAME_OR_PASSWORD)) {
            cast(_callback).onInvalidUserNameOrPasswordError();
            return;
        } else if (error.equals(_LoginResponse.ErrorValues.INVALID_DEVICE_UUID)) {
            cast(_callback).onDeviceUUIDError();
            return;
        } else if (error.equals(_RegisterResponse.ErrorValues.ACCOUNT_EXPIRED)) {
            cast(_callback).onAccountExpired();
            return;
        }
        cast(_callback).onConnectionError();
        LOG.I("Login Response is unknown:" + responseJSON.toString());
    }

    private _LoginResultCallback cast(PresenterResultCallback callback) {
        return ((_LoginResultCallback) callback);
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
