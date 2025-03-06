package sysment.android.guartinel.core.connection.managementServer.account.resendActivationCode;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.presenterCallbacks.ActivateAccountResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

public class ResendActivationCodeRequest extends HttpRequest {
    protected static class Constants {

        public static String URL = "/account/resendActivationCode";

        public static class RequestKeys {
            public static final String EMAIL = "email";
        }
    }

    public ResendActivationCodeRequest(String email, PresenterResultCallback listener) {
        super(listener);
        try {
            _requestParametersJSON.put(ResendActivationCodeRequest.Constants.RequestKeys.EMAIL, email);
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create ResendActivatonCodeRequest e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        ResendActivationCodeResponse response = null;

        try {
            response = new ResendActivationCodeResponse(responseJSON);
        } catch (JSONException e) {
            cast(_callback).onConnectionError();
            return;
        } catch (InternalSystemErrorException internalServerError) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onResendActivationCodeSuccess();
            return;
        }

        String error = response.error;
        if (error.equals(ResendActivationCodeResponse.ErrorValues.ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND)) {
            cast(_callback).onOneHourNotElapsedSinceLastSend();
            return;
        }
        if (error.equals(ResendActivationCodeResponse.ErrorValues.INTERNAL_SYSTEM_ERROR)) {
            cast(_callback).onConnectionError();
            return;
        }
        LOG.I("ResendActivationCodeResponse state is unknown:" + responseJSON.toString());
        cast(_callback).onConnectionError();
    }

    private ActivateAccountResultCallback cast(PresenterResultCallback callback) {
        return ((ActivateAccountResultCallback) callback);
    }

    @Override
    public String getRequestJSONString() {
        return _requestParametersJSON.toString();
    }

    @Override
    public String getURL() {
        return ResendActivationCodeRequest.Constants.URL;
    }
}