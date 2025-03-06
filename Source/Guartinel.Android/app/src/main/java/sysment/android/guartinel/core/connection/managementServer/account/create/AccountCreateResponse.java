package sysment.android.guartinel.core.connection.managementServer.account.create;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.connection.managementServer.device._login._LoginResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;

/**
 * Created by sysment_dev on 02/24/2016.
 */
public class AccountCreateResponse extends HttpResponse {
    public static class Keys extends _LoginResponse.Keys {
    }

    public static class ErrorValues extends HttpResponse.ErrorValues {
        public static final String EMAIL_ALREADY_REGISTERED = "EMAIL_ALREADY_REGISTERED";
    }

    public AccountCreateResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
        super(responseJSON);
        if (!isSuccess()) {
            return;
        }
    }

}
