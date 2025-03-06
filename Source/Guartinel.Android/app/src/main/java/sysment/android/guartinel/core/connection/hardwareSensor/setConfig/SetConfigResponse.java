package sysment.android.guartinel.core.connection.hardwareSensor.setConfig;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;

public class SetConfigResponse extends HttpResponse {
    public SetConfigResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
        super(responseJSON);
    }
}
