package sysment.android.guartinel.core.connection;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;
import sysment.android.guartinel.core.utils.LOG;

/**
 * Created by sysment_dev on 02/24/2016.
 */
public class HttpResponse {
    public String success;
    public String error;

    public static class Keys {
        public static final String SUCCESS = "success";
        public static final String ERROR = "error";
    }

    public static class ErrorValues {
        public static final String INTERNAL_SYSTEM_ERROR = "INTERNAL_SYSTEM_ERROR";
        public static final String ACCOUNT_EXPIRED = "ACCOUNT_EXPIRED";
    }

    public static class SuccessValues {
        public static final String SUCCESS = "SUCCESS";
    }

    public HttpResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
       if(responseJSON.has(Keys.SUCCESS)){
           try {
               this.success = responseJSON.getString(Keys.SUCCESS);
           } catch (Exception e) {
               LOG.I("Cannot get success from response:" + responseJSON.toString());
           }
       }

        if(responseJSON.has(Keys.ERROR)){
            try {
                this.error = responseJSON.getString(Keys.ERROR);
               } catch (Exception e) {
                LOG.I("Cannot get error from response:" + responseJSON.toString());
            }
        }


        if (isErrorInternalSystemError()) {
            throw new InternalSystemErrorException();
        }
    }

    public boolean isSuccess() {
        if (success != null && success.equals(SuccessValues.SUCCESS)) {
            return true;
        }
        return false;
    }

    public boolean isErrorInternalSystemError() {
        if (error != null && error.equals(ErrorValues.INTERNAL_SYSTEM_ERROR)) {
            return true;
        }
        return false;
    }
}

