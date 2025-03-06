package sysment.android.guartinel.core.connection;

import android.content.Context;
import android.net.Network;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;

import javax.net.ssl.HostnameVerifier;
import javax.net.ssl.SSLSession;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.FormBody;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;
import okio.Buffer;
import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;


/**
 * Created by sysment_dev on 02/22/2016.
 */
public class HttpConnector {
    public static class Constants {
        private static final MediaType JSON_MEDIA_TYPE
                = MediaType.parse("application/json; charset=utf-8");
        private static final MediaType X_WWW_FORM_URLENCODED_MEDIA_TYPE
                = MediaType.parse("application/x-www-form-urlencoded; charset=utf-8");
    }


    //private  OkHttpClient wifiAndCellularBoundClient;
    // private OkHttpClient wifiBoundClient;

    public HttpConnector(Context context) {
        //    wifiAndCellularBoundClient = getAnyNetworkClient();
        //  WifiUtility.registerNetworkBoundCallbacks(context);
    }

    public OkHttpClient getWifiClient(Network network) {
        try {
            OkHttpClient.Builder builder = new OkHttpClient.Builder();
            builder.hostnameVerifier(new HostnameVerifier() {
                @Override
                public boolean verify(String hostname, SSLSession session) {
                    return true;
                }
            });
            builder.socketFactory(network.getSocketFactory());
            builder.retryOnConnectionFailure(true);
            return builder.build();
        } catch (Exception e) {
            LOG.I("Cannot init okHttpClient E:" + e.getMessage());
            throw new RuntimeException(e);
        }
    }

    private OkHttpClient getAnyNetworkClient() {
        try {
            OkHttpClient.Builder builder = new OkHttpClient.Builder();
            boolean foundSocket = false;
            if (GuartinelApp.networkConnectionState.wifiSocket != null) {
                LOG.I("HttpConnector Using wifisocket.");
                builder.socketFactory(GuartinelApp.networkConnectionState.wifiSocket.getSocketFactory());
                foundSocket = true;
            }
            if (!foundSocket && GuartinelApp.networkConnectionState.cellSocket != null) {
                LOG.I("HttpConnector Using cellsocket.");
                builder.socketFactory(GuartinelApp.networkConnectionState.cellSocket.getSocketFactory());
                foundSocket = true;
            }
            builder.hostnameVerifier(new HostnameVerifier() {
                @Override
                public boolean verify(String hostname, SSLSession session) {
                    return true;
                }
            });
            builder.retryOnConnectionFailure(true);
            return builder.build();
        } catch (Exception e) {
            LOG.I("Cannot init okHttpClient E:" + e.getMessage());
            throw new RuntimeException(e);
        }
    }


    public void makePostOnWifi(Context context, String URL, final HttpRequest httpRequest) {
        OkHttpClient client = getWifiClient(GuartinelApp.networkConnectionState.wifiSocket);
        RequestBody requestBody;
        if (httpRequest.getIsURLEncoded()) {
            LOG.I("Form body:");
            FormBody.Builder builder = new FormBody.Builder();
            for (String parameterKey : httpRequest._params.keySet()) {
                if (httpRequest._params.get(parameterKey) == null) {
                    continue;
                }
                LOG.I(parameterKey + " : " + httpRequest._params.get(parameterKey));
                builder.add(parameterKey, httpRequest._params.get(parameterKey));
            }
            requestBody = builder.build();
            // RequestBody.create(Constants.X_WWW_FORM_URLENCODED_MEDIA_TYPE, httpRequest.getRequestJSONString());
        } else {
            requestBody = RequestBody.create(Constants.JSON_MEDIA_TYPE, httpRequest.getRequestJSONString());
        }
        Request request = new Request.Builder().url(URL).post(requestBody).build();
        LOG.I("Request: " + requestBody.toString() + " to : " + URL);
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                LOG.I("Message failed .E:" + e.getMessage());
                LOG.E("", e);
                httpRequest.onConnectionError();
            }

            @Override
            public void onResponse(Call call, Response response) {
                String responseBody = null;
                try {
                    responseBody = response.body().string();
                } catch (IOException e) {
                    LOG.E("IOException e:", e);
                    httpRequest.onConnectionError();
                    return;
                }
                JSONObject responseJSON = null;
                LOG.I("Response : " + responseBody.toString());
                try {
                    responseJSON = new JSONObject(responseBody);
                } catch (JSONException e) {
                    LOG.I("JSON response parsing error:" + e.getMessage());
                    httpRequest.onConnectionError();
                    return;
                }
                httpRequest.onResponse(responseJSON);
            }
        });
    }

    public void makePostOnWifiOrCellular(Context context, String URL, final HttpRequest httpRequest) {
        RequestBody requestBody;
        if (httpRequest.getIsURLEncoded()) {
            LOG.I("Form body:");
            FormBody.Builder builder = new FormBody.Builder();
            for (String parameterKey : httpRequest._params.keySet()) {
                if (httpRequest._params.get(parameterKey) == null) {
                    continue;
                }
                LOG.I(parameterKey + " : " + httpRequest._params.get(parameterKey));
                builder.add(parameterKey, httpRequest._params.get(parameterKey));
            }
            requestBody = builder.build();
            // RequestBody.create(Constants.X_WWW_FORM_URLENCODED_MEDIA_TYPE, httpRequest.getRequestJSONString());
        } else {
            requestBody = RequestBody.create(Constants.JSON_MEDIA_TYPE, httpRequest.getRequestJSONString());
        }
        Request request = new Request.Builder().url(URL).post(requestBody).build();

        LOG.I("Request URL " + URL + bodyToString(request));

        OkHttpClient client = null;
        try {
            client = getAnyNetworkClient();
        } catch (Exception e) {
            LOG.E("Could not retrieve any network client", e);
            httpRequest.onConnectionError();
            return;
        }

        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                LOG.I("Message failed .E:" + e.getMessage());
                LOG.E("", e);
                httpRequest.onConnectionError();
            }

            @Override
            public void onResponse(Call call, Response response) {
                String responseBody = null;
                try {
                    responseBody = response.body().string();
                } catch (IOException e) {
                    LOG.E("IOException e:", e);
                    httpRequest.onConnectionError();
                    return;
                }
                JSONObject responseJSON = null;
                LOG.I("Response : " + responseBody.toString());
                try {
                    responseJSON = new JSONObject(responseBody);
                } catch (JSONException e) {
                    LOG.I("JSON response parsing error:" + e.getMessage());
                    httpRequest.onConnectionError();
                    return;
                }
                httpRequest.onResponse(responseJSON);
            }
        });
    }

    private static String bodyToString(final Request request) {

        try {
            final Request copy = request.newBuilder().build();
            final Buffer buffer = new Buffer();
            copy.body().writeTo(buffer);
            return buffer.readUtf8();
        } catch (final IOException e) {
            return "did not work";
        }
    }
}
