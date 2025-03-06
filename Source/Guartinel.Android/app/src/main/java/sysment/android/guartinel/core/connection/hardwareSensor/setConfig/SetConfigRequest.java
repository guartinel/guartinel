package sysment.android.guartinel.core.connection.hardwareSensor.setConfig;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;

import sysment.android.guartinel.BuildConfig;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.ui.presenterCallbacks.ConfigureHardwareSensorCallback;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

public class SetConfigRequest extends HttpRequest {
    protected static class Constants {

        public static String URL = "setConfig";

        public static class RequestKeys {
            public static final String ROUTER_SSID = "routerSSID";
            public static final String ROUTER_PASSWORD = "routerPassword";
            public static final String BACKEND_SERVER_HOST = "backendServerHost";
            public static final String BACKEND_SERVER_PORT = "backendServerPort";
            public static final String BACKEND_SERVER_PROTOCOL_PREFIX = "backendServerProtocolPrefix";
            public static final String UPDATE_SERVER_HOST = "updateServerHost";
            public static final String UPDATE_SERVER_PORT = "updateServerPort";
            public static final String UPDATE_SERVER_PROTOCOL_PREFIX = "updateServerProtocolPrefix";
            public static final String HARDWARE_TYPE = "hardwareType";
            public static final String INSTANCE_NAME = "instanceName";
        }
    }

    public SetConfigRequest(String wifiSSID, String wifiPassword, String updateServerHost, String updateServerProtocolPrefix, int updateServerPort, String hardwareType,String instanceName, PresenterResultCallback listener) {
        super(listener);
        _isURLEncoded = true;
        try {
            _requestParametersJSON.put(Constants.RequestKeys.ROUTER_SSID, wifiSSID);
            _requestParametersJSON.put(Constants.RequestKeys.ROUTER_PASSWORD, wifiPassword);
            _requestParametersJSON.put(Constants.RequestKeys.BACKEND_SERVER_HOST, BuildConfig.BACKEND_HOST_VALUE);
            _requestParametersJSON.put(Constants.RequestKeys.UPDATE_SERVER_HOST, updateServerHost);
            _requestParametersJSON.put(Constants.RequestKeys.UPDATE_SERVER_PORT, updateServerPort);
            _requestParametersJSON.put(Constants.RequestKeys.UPDATE_SERVER_PROTOCOL_PREFIX, updateServerProtocolPrefix);
            _requestParametersJSON.put(Constants.RequestKeys.BACKEND_SERVER_PROTOCOL_PREFIX, BuildConfig.BACKEND_SERVER_PROTOCOL_PREFIX);
            _requestParametersJSON.put(Constants.RequestKeys.HARDWARE_TYPE, hardwareType);
            _requestParametersJSON.put(Constants.RequestKeys.INSTANCE_NAME, instanceName);

            if (BuildConfig.BACKEND_SERVER_PROTOCOL_PREFIX.contains(("https"))) {
                _requestParametersJSON.put(Constants.RequestKeys.BACKEND_SERVER_PORT, BuildConfig.BACKEND_HTTPS_PORT_VALUE);
                _params.put(Constants.RequestKeys.BACKEND_SERVER_PORT, BuildConfig.BACKEND_HTTPS_PORT_VALUE);

            } else {
                _requestParametersJSON.put(Constants.RequestKeys.BACKEND_SERVER_PORT, BuildConfig.BACKEND_HTTP_PORT_VALUE);
                _params.put(Constants.RequestKeys.BACKEND_SERVER_PORT, BuildConfig.BACKEND_HTTP_PORT_VALUE);
            }

            _params.put(Constants.RequestKeys.ROUTER_SSID, wifiSSID);
            _params.put(Constants.RequestKeys.ROUTER_PASSWORD, wifiPassword);
            _params.put(Constants.RequestKeys.BACKEND_SERVER_HOST, BuildConfig.BACKEND_HOST_VALUE);
            _params.put(Constants.RequestKeys.UPDATE_SERVER_HOST, updateServerHost);
            _params.put(Constants.RequestKeys.UPDATE_SERVER_PORT, Integer.toString(updateServerPort));
            _params.put(Constants.RequestKeys.UPDATE_SERVER_PROTOCOL_PREFIX, updateServerProtocolPrefix);
            _params.put(Constants.RequestKeys.BACKEND_SERVER_PROTOCOL_PREFIX, BuildConfig.BACKEND_SERVER_PROTOCOL_PREFIX);
            _params.put(Constants.RequestKeys.HARDWARE_TYPE, hardwareType);
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create setConfig request Request e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        SetConfigResponse response = null;
        try {
            response = new SetConfigResponse(responseJSON);
        } catch (JSONException e) {
            cast(_callback).onConnectionError();
            return;
        } catch (InternalSystemErrorException internalServerError) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onConfigured();
            return;
        }
        _callback.onConnectionError();
        LOG.I("SetConfigResponse Response is unknown:" + responseJSON.toString());
    }

    private ConfigureHardwareSensorCallback cast(PresenterResultCallback callback) {
        return ((ConfigureHardwareSensorCallback) callback);
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
