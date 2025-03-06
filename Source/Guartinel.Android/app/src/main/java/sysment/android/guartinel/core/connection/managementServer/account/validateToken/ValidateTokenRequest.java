package sysment.android.guartinel.core.connection.managementServer.account.validateToken;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.ValidateTokenCallback;

public class ValidateTokenRequest extends HttpRequest {
    protected static class Constants {

        public static String URL = "/account/validateToken";

        public static class RequestKeys {
            public static final String TOKEN = "token";
        }
    }

    public ValidateTokenRequest(String token, PresenterResultCallback listener) {
        super(listener);
        try {
            _requestParametersJSON.put(Constants.RequestKeys.TOKEN, token);
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create ValidateTokenRequest e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        ValidateTokenResponse response = null;

        try {
            response = new ValidateTokenResponse(responseJSON);
        } catch (JSONException e) {
            cast(_callback).onConnectionError();
            return;
        } catch (InternalSystemErrorException internalServerError) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onSuccess();
            return;
        }

        String error = response.error;
        if (error.equals(ValidateTokenResponse.ErrorValues.INTERNAL_SYSTEM_ERROR)) {
            cast(_callback).onConnectionError();
            return;
        }
        if (error.equals(ValidateTokenResponse.ErrorValues.INVALID_TOKEN)) {
            cast(_callback).onInvalidToken();
            return;
        }
        LOG.I("ValidateTokenResponse is unknown:" + responseJSON.toString());
        cast(_callback).onConnectionError();
    }

    private ValidateTokenCallback cast(PresenterResultCallback callback) {
        return ((ValidateTokenCallback) callback);
    }

    @Override
    public String getRequestJSONString() {
        return _requestParametersJSON.toString();
    }

    @Override
    public String getURL() {
        return ValidateTokenRequest.Constants.URL;
    }
}