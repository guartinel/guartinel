package sysment.guartinelandroidsystemwatcher.connection.actions;

import org.json.JSONObject;

import okhttp3.MediaType;
import okhttp3.RequestBody;
import sysment.guartinelandroidsystemwatcher.util.LogWrapper;
import sysment.guartinelandroidsystemwatcher.connection.HTTPInterface;

/**
 * Created by sysment_dev on 11/17/2016.
 */
public class GetManagementServerStatusAction extends BaseAction {
    private String URL, token, status;

    public GetManagementServerStatusAction(HTTPInterface httpInterface, String url, String token) {
        super(httpInterface);
        try {
            this.URL = url;
            this.token = token;
            configureRequestBuilder();
            buildRequest();
        } catch (Exception e) {
            LogWrapper.Err("Cannot config request", e);
            status = "Cannot reach server with the predefined settings";
        }

    }

    public String getStatus() {
        return status;
    }

    @Override
    public void execute() {
        try {
            HTTPInterface.StructuredResponse result = _interface.executeRequest(_request);
            JSONObject responseJSON = new JSONObject(result.body);
            status = responseJSON.getString("message");
            _isSuccess = true;
            LogWrapper.Inf("GetManagementServerStatusAction success" + _isSuccess);

        } catch (Exception e) {
            status = "Server is unreachable";
            LogWrapper.Err("Cannot execute CheckAvailabilityAction. E:", e);
        }
    }

    @Override
    public boolean isSuccessful() {
        return _isSuccess;
    }

    @Override
    protected void configureRequestBuilder() {
        try {
            _requestBuilder.url(URL);
            JSONObject requestJson = new JSONObject();
            requestJson.put("token", token);
            RequestBody requestBody = RequestBody.create(MediaType.parse("application/json; charset=utf-8"), requestJson.toString());
            _requestBuilder.post(requestBody);
        } catch (Exception e) {
            LogWrapper.Err("Cannnot create request JSON", e);
        }

    }
}
