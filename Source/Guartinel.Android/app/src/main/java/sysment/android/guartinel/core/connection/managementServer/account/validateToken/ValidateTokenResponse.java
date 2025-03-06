package sysment.android.guartinel.core.connection.managementServer.account.validateToken;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;

public class ValidateTokenResponse extends HttpResponse {


    public static class ErrorValues extends HttpResponse.ErrorValues {
        public static final String INVALID_TOKEN = "INVALID_TOKEN";
    }

    public ValidateTokenResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
        super(responseJSON);
        if (!isSuccess()) {
            return;
        }
    }
}
