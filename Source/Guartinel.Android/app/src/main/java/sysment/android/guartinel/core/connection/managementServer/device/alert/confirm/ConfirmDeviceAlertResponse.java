package sysment.android.guartinel.core.connection.managementServer.device.alert.confirm;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;

/**
 * Created by sysment_dev on 05/14/2017.
 */
public class ConfirmDeviceAlertResponse extends HttpResponse {

    public static class ErrorValues extends HttpResponse.ErrorValues {
        public static final String INVALID_TOKEN = "INVALID_TOKEN";
       }

    public ConfirmDeviceAlertResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
        super(responseJSON);
        if (!isSuccess()) {
            return;
        }
    }
}
