package sysment.guartinel.hardwareconfigurator.connection;

import android.content.Context;
import android.util.Log;

import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.UnknownHostException;
import java.util.concurrent.TimeUnit;

import cz.msebera.android.httpclient.Header;
import cz.msebera.android.httpclient.conn.ConnectTimeoutException;
import cz.msebera.android.httpclient.conn.ConnectionPoolTimeoutException;
import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.FormBody;
import okhttp3.Interceptor;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;
import sysment.guartinel.hardwareconfigurator.BuildConfig;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardwareAP;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardware;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardwareClient;
import sysment.guartinel.hardwareconfigurator.tools.LOG;

/**
 * Created by DAVT on 2017.12.07..
 */

public   class HardwareConnector {
    public interface HardwareResponseListener {
        void onSuccess();

        void onInvalidPassword();

        void onFailed();
    }

    public interface GetDiagnosticsListener extends HardwareResponseListener {
        void onSuccess(String log);
    }

        public static void Revive(){

        }

    private static AsyncHttpClient _client = new AsyncHttpClient();
    //private static OkHttpClient _okHTTPClient;

    static  {
        LOG.I("HARDWARE CONNECTOR CONFIGURATED!");
        _client.allowRetryExceptionClass(IOException.class);
        _client.allowRetryExceptionClass(IllegalArgumentException.class);
        _client.allowRetryExceptionClass(ConnectTimeoutException.class);
        _client.blockRetryExceptionClass(UnknownHostException.class);
        _client.blockRetryExceptionClass(ConnectionPoolTimeoutException.class);
        _client.blockRetryExceptionClass(Exception.class);
        _client.setMaxRetriesAndTimeout(10, 6000);

        _okHTTPClient = new OkHttpClient.Builder()
                .connectTimeout(40, TimeUnit.SECONDS)
                .writeTimeout(40, TimeUnit.SECONDS)
                .readTimeout(40, TimeUnit.SECONDS)
                .retryOnConnectionFailure(true)
                .addInterceptor(new Interceptor() {
                    @Override
                    public Response intercept(Chain chain) throws IOException {
                        LOG.I("Intercepted request");

                        Request request = chain.request();

                        Response response = null;
                        boolean responseOK = false;
                        int tryCount = 0;

                        while (!responseOK && tryCount < 5) {
                            try {
                                LOG.I("Retrying request: "+ tryCount);
                                response = chain.proceed(request);
                                responseOK = response.isSuccessful();
                                Thread.sleep(1000);
                            } catch (Exception e) {
                                Log.d("intercept", "Request is not successful - " + tryCount);
                            } finally {
                                tryCount++;
                            }
                        }
                        if(response == null){
                            throw new IOException("Cannot finish request.");
                        }

                        // otherwise just pass the original response on
                        return response;
                    }
                })
                .build();
    }

    public static void freeze(final GuartinelHardware hardware, final HardwareResponseListener listener) {

        RequestBody formBody = new FormBody.Builder()
                .add("token", "c0ac94e3-88d8-4304")
                .build();

        final Request request = new Request.Builder()
                .url(hardware.getAddress() + Constants.Hardware.Routes.FREEZE)
                .post(formBody)
                .build();

        LOG.I("Sending freeze request");
        _okHTTPClient.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                LOG.I("Freeze request failed");
                listener.onFailed();
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                LOG.I("Freeze request succeeded");

                listener.onSuccess();
            }
        });
    }

    public static void getDiagnostics(Context context, final GuartinelHardware hardware, final GetDiagnosticsListener listener) {
        if (hardware instanceof GuartinelHardwareAP) {
            WifiHelper.connectToWifi(context, hardware.getName(), hardware.devicePassword, new WifiHelper.WifiConnectionResult() {
                @Override
                public void onConnected() {
                    hardware.getDiagnostics(_client, listener);
                }

                @Override
                public void onCannotConnect() {
                    listener.onFailed();
                }
            });
        }
        if (hardware instanceof GuartinelHardwareClient) {
            hardware.getDiagnostics(_client, listener);
        }
    }

    public static void configureHardware(Context context, final GuartinelHardware hardware, final HardwareResponseListener listener) {
        if (hardware instanceof GuartinelHardwareAP) {
            WifiHelper.connectToWifi(context, hardware.getName(), hardware.devicePassword, new WifiHelper.WifiConnectionResult() {
                @Override
                public void onConnected() {
                    hardware.setConfig(_client, listener);
                }

                @Override
                public void onCannotConnect() {
                    listener.onFailed();
                }
            });
        }
        if (hardware instanceof GuartinelHardwareClient) {
            hardware.setConfig(_client, listener);
        }
    }

    public static void isPasswordOK(Context context, final GuartinelHardware hardware, final HardwareResponseListener listener) {
        if (hardware instanceof GuartinelHardwareAP) {
            WifiHelper.connectToWifi(context, hardware.getName(), hardware.devicePassword, new WifiHelper.WifiConnectionResult() {
                @Override
                public void onConnected() {
                    hardware.isPasswordOK(_client, listener);
                }

                @Override
                public void onCannotConnect() {
                    listener.onFailed();
                }
            });
        }
        if (hardware instanceof GuartinelHardwareClient) {
            hardware.isPasswordOK(_client, listener);
        }
    }

    public static void getConfig(Context context, final GuartinelHardware hardware, final HardwareResponseListener listener) {
        if (hardware instanceof GuartinelHardwareAP) {
            WifiHelper.connectToWifi(context, hardware.getName(), hardware.devicePassword, new WifiHelper.WifiConnectionResult() {
                @Override
                public void onConnected() {
                    hardware.getConfig(_client, listener);
                }

                @Override
                public void onCannotConnect() {
                    listener.onFailed();
                }
            });
        }
        if (hardware instanceof GuartinelHardwareClient) {
            hardware.getConfig(_client, listener);
        }
    }
}
