package sysment.guartinel.hardwareconfigurator.connection;

import android.content.Context;
import android.util.Log;

import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import cz.msebera.android.httpclient.Header;
import sysment.guartinel.hardwareconfigurator.BuildConfig;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardware;
import sysment.guartinel.hardwareconfigurator.tools.LOG;

/**
 * Created by DTAP on 5/25/2018.
 */

public class ManagementServer {
    public interface ManagementServerResponseListener {
        void onSuccess();

        void onInvalidToken();

        void onFailed();
    }

    private static AsyncHttpClient _client = new AsyncHttpClient();

    public static void validate(Context context, final GuartinelHardware hardware, final ManagementServerResponseListener listener) {
        RequestParams params = new RequestParams();
        params.add(Constants.ManagementServer.RequestParameters.HARDWARE_TOKEN, hardware.hardwareToken);
        String backendServer;
        if (hardware.useHTTPS) {
            backendServer = Constants.ManagementServer.HTTPS_PREFIX + BuildConfig.BACKEND_HOST_VALUE + ":" + BuildConfig.BACKEND_HTTPS_PORT_VALUE;
        } else {
            backendServer = Constants.ManagementServer.HTTP_PREFIX + BuildConfig.BACKEND_HOST_VALUE + ":" + BuildConfig.BACKEND_HTTP_PORT_VALUE;
        }
        LOG.I("Backend: " + backendServer);
        LOG.I("Hardware token: " + hardware.hardwareToken);
        _client.post(backendServer + Constants.ManagementServer.Routes.VALIDATE_HARDWARE, params, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                try {
                    Object success = response.get(Constants.ManagementServer.ResponseParameters.SUCCESS);
                    if (success == null) {
                        listener.onInvalidToken();
                        return;
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                    listener.onFailed();
                    return;
                }
                listener.onSuccess();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                LOG.I("Cannot validate token: " + responseString);
                listener.onFailed();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                listener.onFailed();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONArray errorResponse) {
                listener.onFailed();
            }
        });
    }
}
