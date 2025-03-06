package sysment.android.guartinel.core.connection.managementServer.account.create;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpRequest;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.presenterCallbacks.AccountCreateResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

/**
 * Created by sysment_dev on 02/23/2016.
 */
public class AccountCreateRequest extends HttpRequest {
    protected static class Constants {

        public static String URL = "/account/create";

        public static class RequestKeys {
            public static final String EMAIL = "email";
            public static final String PASSWORD = "password";
            public static final String FIRST_NAME = "first_name";
            public static final String LAST_NAME = "last_name";
        }
    }

    public AccountCreateRequest(String firstName, String lastName,String email, String password, AccountCreateResultCallback listener) {
        super(listener);
        try {
            _requestParametersJSON.put(Constants.RequestKeys.EMAIL, email);
            _requestParametersJSON.put(Constants.RequestKeys.PASSWORD, password);
            _requestParametersJSON.put(Constants.RequestKeys.FIRST_NAME, firstName);
            _requestParametersJSON.put(Constants.RequestKeys.LAST_NAME, lastName);
        } catch (JSONException e) {
            e.printStackTrace();
            LOG.I("Cannot create Register Request e:" + e.getMessage());
        }
    }

    @Override
    public void onResponse(JSONObject responseJSON) {
        AccountCreateResponse response = null;

        try {
            response = new AccountCreateResponse(responseJSON);
        } catch (JSONException | InternalSystemErrorException e) {
            cast(_callback).onConnectionError();
            return;
        }
        if (response.isSuccess()) {
            cast(_callback).onCreateSuccess();
            return;
        }

        String error = response.error;
        if (error.equals(AccountCreateResponse.ErrorValues.INTERNAL_SYSTEM_ERROR)) {
            cast(_callback).onConnectionError();
            return;
        }
        if (error.equals(AccountCreateResponse.ErrorValues.EMAIL_ALREADY_REGISTERED)) {
            cast(_callback).onEmailAlreadyRegistered();
            return;
        }
        LOG.I("Create account Response is unknown:" + responseJSON.toString());
        cast(_callback).onConnectionError();
    }

    private AccountCreateResultCallback cast(PresenterResultCallback callback) {
        return ((AccountCreateResultCallback) callback);
    }

    @Override
    public String getRequestJSONString() {
        return _requestParametersJSON.toString();
    }

    @Override
    public String getURL() {
        return Constants.URL;
    }
}
