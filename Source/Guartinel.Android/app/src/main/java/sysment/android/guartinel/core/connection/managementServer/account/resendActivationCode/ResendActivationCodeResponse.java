package sysment.android.guartinel.core.connection.managementServer.account.resendActivationCode;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;

public class ResendActivationCodeResponse extends HttpResponse {


    public static class ErrorValues extends HttpResponse.ErrorValues {
        public static final String INVALID_TOKEN = "INVALID_TOKEN";
        public static final String ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND = "ONE_HOUR_NOT_ELAPSED_AFTER_LAST_EMAIL_SEND";

    }

    public ResendActivationCodeResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
        super(responseJSON);
        if (!isSuccess()) {
            return;
        }
    }
}
