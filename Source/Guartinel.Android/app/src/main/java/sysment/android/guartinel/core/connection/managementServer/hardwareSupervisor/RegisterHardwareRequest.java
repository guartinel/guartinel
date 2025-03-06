package sysment.android.guartinel.core.connection.managementServer.hardwareSupervisor;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.connection.managementServer.account.validateToken.ValidateTokenResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.RegisterHardwareSensorCallback;

public class RegisterHardwareRequest extends HttpRequest {
    protected static class Constants {

        static String URL = "/hardwareSupervisor/register";

        static class RequestKeys {
            static final String TOKEN = "token";
            static final String INSTANCE_NAME = "instance_name";
            static final String INSTANCE_ID = "instance_id";
        }
    }

    public RegisterHardwareRequest(String token, String id, String deviceName, PresenterResultCallback listener) {
        super(listener);
        try {
            _requestParametersJSON.put(Constants.RequestKeys.TOKEN, token);
            _requestParametersJSON.put(Constants.RequestKeys.INSTANCE_ID, id);
            _requestParametersJSON.put(Constants.RequestKeys.INSTANCE_NAME, deviceName);
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create Register Request e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        RegisterHardwareResponse response = null;

        try {
            response = new RegisterHardwareResponse(responseJSON);
        } catch (JSONException e) {
            cast(_callback).onConnectionError();
            return;
        } catch (InternalSystemErrorException internalServerError) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onSuccess(response.getUpdateServerHost(),
                    response.getUpdateServerProtocolPrefix(),
                    response.getUpdateServerPort(),
                    response.getHardwareType());
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

        LOG.I("Register Response is unknown:" + responseJSON.toString());
        cast(_callback).onConnectionError();
    }

    private RegisterHardwareSensorCallback cast(PresenterResultCallback callback) {
        return ((RegisterHardwareSensorCallback) callback);
    }

    @Override
    public String getRequestJSONString() {
        return _requestParametersJSON.toString();
    }

    @Override
    public String getURL() {
        return RegisterHardwareRequest.Constants.URL;
    }
}
