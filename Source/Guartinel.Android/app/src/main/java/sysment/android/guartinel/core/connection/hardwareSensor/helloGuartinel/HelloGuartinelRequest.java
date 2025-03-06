package sysment.android.guartinel.core.connection.hardwareSensor.helloGuartinel;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.BuildConfig;
import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.connection.hardwareSensor.setConfig.SetConfigRequest;
import sysment.android.guartinel.core.connection.hardwareSensor.setConfig.SetConfigResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.presenterCallbacks.ConfigureHardwareSensorCallback;
import sysment.android.guartinel.ui.presenterCallbacks.HelloGuartinelCallback;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

public class HelloGuartinelRequest extends HttpRequest {
    protected static class Constants {

        public static String URL = "helloGuartinel";

        public static class RequestKeys {
             }
    }
    public HelloGuartinelRequest(PresenterResultCallback listener) {
        super(listener);
        _isURLEncoded = true;
    }
    @Override
    public void onResponse(JSONObject responseJSON) {
        HelloGuartinelResponse response = null;
        try {
            response = new HelloGuartinelResponse(responseJSON);
        } catch (JSONException e) {
            cast(_callback).onConnectionError();
            return;
        } catch (InternalSystemErrorException internalServerError) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onSuccess(response.getID());
            return;
        }
        _callback.onConnectionError();
        LOG.I("SetConfigResponse Response is unknown:" + responseJSON.toString());
    }

    private HelloGuartinelCallback cast(PresenterResultCallback callback) {
        return ((HelloGuartinelCallback) callback);
    }

    @Override
    public String getRequestJSONString() {
        return _requestParametersJSON.toString();
    }

    @Override
    public String getURL() {
        return HelloGuartinelRequest.Constants.URL;
    }
}
