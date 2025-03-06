package sysment.android.guartinel.core.connection.managementServer.device.alert.confirm;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.presenterCallbacks.ConfirmDeviceAlertCallback;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

/**
 * Created by sysment_dev on 11/07/2016.
 */
public class ConfirmDeviceAlertRequest extends HttpRequest {
    public static String URL = "/alert/confirmDeviceAlert";

    public static class Keys {
        public static final String TOKEN = "token";
        public static final String ALERT_ID = "alert_id";
    }

    public ConfirmDeviceAlertRequest(String token, String alertID, PresenterResultCallback callback) {
        super(callback);
        try {
            _requestParametersJSON.put(Keys.TOKEN, token);
            _requestParametersJSON.put(Keys.ALERT_ID, alertID);
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create ConfirmAlertArrivalRequest e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        LOG.I(URL + " Response: " + responseJSON.toString());
        ConfirmDeviceAlertResponse response = null;
        try {
            response = new ConfirmDeviceAlertResponse(responseJSON);
        } catch (JSONException e) {
            LOG.I("Confirm device alert onResponse: exception");

            cast(_callback).onConnectionError();
            return;
        } catch (InternalSystemErrorException internalServerError) {
            LOG.I("Confirm device alert onResponse: internal server exception");

            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            LOG.I("Confirm device alert onResponse: success");
            return;
        }

        String error = response.error;
        if (error != null && error.equals(ConfirmDeviceAlertResponse.ErrorValues.INVALID_TOKEN)) {
            LOG.I("Confirm device alert onResponse: INVALID_TOKEN error");
            cast(_callback).onInvalidDeviceToken();
            return;
        }
        LOG.I("ConfirmDeviceAlert Response is unknown:" + responseJSON.toString());
        cast(_callback).onConnectionError();
    }

    private ConfirmDeviceAlertCallback cast(PresenterResultCallback callback) {
        return ((ConfirmDeviceAlertCallback) callback);
    }
    @Override
    public String getRequestJSONString() {
        return  _requestParametersJSON.toString();
    }

    @Override
    public String getURL() {
        return URL;
    }
}
