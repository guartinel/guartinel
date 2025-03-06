package sysment.android.guartinel.core.connection;

import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

/**
 * Created by sysment_dev on 02/22/2016.
 */

public abstract class HttpRequest {
    protected JSONObject _requestParametersJSON;
    protected PresenterResultCallback _callback;
    protected boolean _isURLEncoded = false;
    protected HashMap<String, String> _params = new HashMap<>();

    public HttpRequest(PresenterResultCallback listener) {
        _requestParametersJSON = new JSONObject();
        _callback = listener;

    }

    public boolean getIsURLEncoded() {
        return _isURLEncoded;
    }

    public void onConnectionError() {
        _callback.onConnectionError();
    }

    public abstract void onResponse(JSONObject responseJSON);

    public abstract String getRequestJSONString();

    public abstract String getURL();

    public HashMap<String, String> getParameters() {
        return _params;
    }
}
