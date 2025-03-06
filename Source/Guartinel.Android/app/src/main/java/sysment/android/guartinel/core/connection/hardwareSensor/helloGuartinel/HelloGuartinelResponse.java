package sysment.android.guartinel.core.connection.hardwareSensor.helloGuartinel;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.connection.managementServer.device._login._LoginResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;

public class HelloGuartinelResponse extends HttpResponse {
    private String id;

    public static class Keys extends _LoginResponse.Keys {
        public static final String ID = "id";
    }

    public HelloGuartinelResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
        super(responseJSON);
        if (!isSuccess()) {
            return;
        }
        this.id = responseJSON.getString(HelloGuartinelResponse.Keys.ID);
    }

    public String getID() {
        return id;
    }
}
