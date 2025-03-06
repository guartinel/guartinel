package sysment.guartinel.hardwareconfigurator.models;

import android.util.Log;

import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import cz.msebera.android.httpclient.Header;
import sysment.guartinel.hardwareconfigurator.BuildConfig;
import sysment.guartinel.hardwareconfigurator.connection.Constants;
import sysment.guartinel.hardwareconfigurator.connection.HardwareConnector;
import sysment.guartinel.hardwareconfigurator.tools.LOG;

/**
 * Created by DAVT on 2017.12.06..
 */

public abstract class GuartinelHardware {

    public abstract String getName();

    public abstract String getAddress();

    public String devicePassword, hardwareToken, wifiSSID, wifiPassword, instanceName;
    public boolean useHTTPS = false;


    public void isPasswordOK(AsyncHttpClient _client, final HardwareConnector.HardwareResponseListener listener) {
        RequestParams params = new RequestParams();
        params.add(Constants.Hardware.RequestParameters.DEVICE_PASSWORD, devicePassword);
        params.setHttpEntityIsRepeatable(true);

        _client.post(getAddress() + Constants.Hardware.Routes.LOGIN, params, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                try {
                    if (((String) response.get(Constants.Hardware.RESULT)).contains(Constants.Hardware.ResponseParameters.ResultValue.INVALID_PASSWORD)) {
                        listener.onInvalidPassword();
                        return;
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
                listener.onSuccess();
            }

            @Override
            public void onRetry(int retryNo) {
                LOG.I("Retrying login... Count: " + retryNo);
                super.onRetry(retryNo);
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                super.onFailure(statusCode, headers, responseString, throwable);
                listener.onFailed();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                super.onFailure(statusCode, headers, throwable, errorResponse);
                listener.onFailed();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONArray errorResponse) {
                super.onFailure(statusCode, headers, throwable, errorResponse);
                listener.onFailed();
            }
        });
    }

    public void freeze() {

    }

    public void getConfig(AsyncHttpClient _client, final HardwareConnector.HardwareResponseListener listener) {
        RequestParams params = new RequestParams();
        params.add(Constants.Hardware.RequestParameters.DEVICE_PASSWORD, devicePassword);
        params.setHttpEntityIsRepeatable(true);

        _client.post(getAddress() + Constants.Hardware.Routes.GET_CONFIG, params, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                try {
                    if (((String) response.get(Constants.Hardware.RESULT)).contains(Constants.Hardware.ResponseParameters.ResultValue.INVALID_PASSWORD)) {
                        listener.onInvalidPassword();
                        return;
                    }
                    if (response.has(Constants.Hardware.RequestParameters.HARDWARE_TOKEN)) {
                        hardwareToken = response.getString(Constants.Hardware.RequestParameters.HARDWARE_TOKEN);
                    }
                    if (response.has(Constants.Hardware.RequestParameters.ROUTER_SSID)) {

                        wifiSSID = (response.getString(Constants.Hardware.RequestParameters.ROUTER_SSID));
                    }
                    if (response.has(Constants.Hardware.RequestParameters.INSTANCE_NAME)) {
                        instanceName = (response.getString(Constants.Hardware.RequestParameters.INSTANCE_NAME));
                    }
                    listener.onSuccess();
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                super.onFailure(statusCode, headers, responseString, throwable);
                listener.onFailed();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                super.onFailure(statusCode, headers, throwable, errorResponse);
                listener.onFailed();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONArray errorResponse) {
                super.onFailure(statusCode, headers, throwable, errorResponse);
                listener.onFailed();
            }
        });
    }

    public void setConfig(AsyncHttpClient _client, final HardwareConnector.HardwareResponseListener listener) {


        RequestParams params = new RequestParams();
        params.add(Constants.Hardware.RequestParameters.HARDWARE_TOKEN, hardwareToken);
        params.add(Constants.Hardware.RequestParameters.ROUTER_SSID, wifiSSID);
        params.add(Constants.Hardware.RequestParameters.ROUTER_PASSWORD, wifiPassword);
        params.add(Constants.Hardware.RequestParameters.DEVICE_PASSWORD, devicePassword);
        params.add(Constants.Hardware.RequestParameters.INSTANCE_NAME, instanceName);
        if (useHTTPS) {
            params.add(Constants.Hardware.RequestParameters.BACKEND_SERVER_HOST, BuildConfig.BACKEND_HOST_VALUE);
            params.add(Constants.Hardware.RequestParameters.BACKEND_SERVER_PORT, BuildConfig.BACKEND_HTTPS_PORT_VALUE);
            params.add(Constants.Hardware.RequestParameters.BACKEND_SERVER_PROTOCOL_PREFIX, Constants.ManagementServer.HTTPS_PREFIX);

        } else {
            params.add(Constants.Hardware.RequestParameters.BACKEND_SERVER_HOST, BuildConfig.BACKEND_HOST_VALUE);
            params.add(Constants.Hardware.RequestParameters.BACKEND_SERVER_PORT, BuildConfig.BACKEND_HTTP_PORT_VALUE);
            params.add(Constants.Hardware.RequestParameters.BACKEND_SERVER_PROTOCOL_PREFIX, Constants.ManagementServer.HTTP_PREFIX);
        }
        params.setHttpEntityIsRepeatable(true);

        Log.i("Guartinel", "Sending request: " + params.toString());
        _client.post(getAddress() + Constants.Hardware.Routes.SET_CONFIG, params, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                listener.onSuccess();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                listener.onSuccess();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                listener.onSuccess();
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONArray errorResponse) {
                listener.onSuccess();
            }
        });
    }


    public void getDiagnostics(AsyncHttpClient _client, final HardwareConnector.GetDiagnosticsListener listener) {
        RequestParams params = new RequestParams();
        params.add(Constants.Hardware.RequestParameters.DEVICE_PASSWORD, devicePassword);
        params.setHttpEntityIsRepeatable(true);

        _client.post(getAddress() + Constants.Hardware.Routes.GET_DIAGNOSTICS, params, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                try {
                    String log = response.getString(Constants.Hardware.ResponseParameters.LOG);
                    listener.onSuccess(log);
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
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
