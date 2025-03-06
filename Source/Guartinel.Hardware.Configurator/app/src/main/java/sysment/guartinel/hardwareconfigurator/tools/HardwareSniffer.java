package sysment.guartinel.hardwareconfigurator.tools;

import android.content.Context;
import android.net.wifi.ScanResult;

import org.json.JSONObject;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import okhttp3.Headers;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import sysment.guartinel.hardwareconfigurator.ui.IDiscoveryProgress;
import sysment.guartinel.hardwareconfigurator.connection.WifiHelper;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardwareAP;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardwareClient;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardware;

/**
 * Created by DAVT on 2017.12.06..
 */

public class HardwareSniffer {

    public interface onSniffingDone {
        void done(List<GuartinelHardware> foundItems);
    }


    public static void sniff(final Context context, final IDiscoveryProgress progress, final onSniffingDone onDone) {
        WifiHelper.scanAvailableNetworks(context, new WifiHelper.OnScanFinished() {
            @Override
            public void wifiDisabled() {
            }

            @Override
            public void finished(final List<ScanResult> wifiScanResult) {
                new Thread(new Runnable() {
                    @Override
                    public void run() {
                        try {
                            List<GuartinelHardware> foundArduinos = new ArrayList<GuartinelHardware>();

                            ConcurrentHashMap<String, InetAddress> foundGuartinelHosts = HardwareSniffer.getDevicesFromNetwork(context, progress, WifiHelper.getCurrentWifiNetworkAddress(context));
                            for (String key : foundGuartinelHosts.keySet()) {
                                foundArduinos.add(new GuartinelHardwareClient(key, foundGuartinelHosts.get(key)));
                            }

                            for (ScanResult scan : wifiScanResult) {
                                if (scan.SSID.toLowerCase().startsWith("guartinel")) {
                                    foundArduinos.add(new GuartinelHardwareAP(scan));
                                }
                            }
                            onDone.done(foundArduinos);
                        } catch (UnsupportedEncodingException e) {
                            e.printStackTrace();
                        } catch (UnknownHostException e) {
                            e.printStackTrace();
                        }
                    }
                }).start();
            }
        });
    }


    private static final int THREADS_COUNT = 100;


    private static ConcurrentHashMap<String, InetAddress> getDevicesFromNetwork(Context context, IDiscoveryProgress progress, InetAddress networkAddress) throws UnsupportedEncodingException {
        LOG.I("Start scanning");
        String addressString = networkAddress.getHostAddress();
        final ConcurrentHashMap<String, InetAddress> resultList = new ConcurrentHashMap<String, InetAddress>();
        String addressBase = addressString.substring(0, addressString.lastIndexOf('.'));
        addressBase += ".";
        ExecutorService executor = Executors.newFixedThreadPool(THREADS_COUNT);
        for (int dest = 0; dest < 255; dest++) {
            String host = addressBase + dest;
            executor.execute(pingRunnable(host, progress, resultList));
        }

        LOG.I("Waiting for executor to terminate...");

        executor.shutdown();
        try {
            executor.awaitTermination(60 , TimeUnit.SECONDS);
        } catch (InterruptedException ignored) {
        }

        LOG.I("Scan finished");

        return resultList;
    }

    private static Runnable pingRunnable(final String host, final IDiscoveryProgress progress, final ConcurrentHashMap<String, InetAddress> result) {
        return new Runnable() {
            public void run() {
                try {
                    LOG.I("Thread " + Thread.currentThread().getId() + " is doing the ping");
                    //  Log.d(LOG_TAG, "Pinging " + host + "...");
                    InetAddress inet = InetAddress.getByName(host);
                    boolean reachable = inet.isReachable(2000);
                    if (!reachable) {
                        LOG.I("=> Host: " + host + " is NOT reachable");
                        progress.increase();
                        return;
                    }
                    LOG.I("=> Host: " + host + " is reachable");
                    String hostName = null;
                    int tryCount = 0;
                    while (tryCount < 2) {
                        tryCount++;
                        LOG.I("=> Host: " + host + " Retry count: " + tryCount);
                        hostName = sendHelloGuartinelRequest(host);
                        if (hostName != null) {
                            break;
                        }
                        LOG.I("=> Host: " + host + " Host name: " + inet.getHostName() + " canonical hostname: " + inet.getCanonicalHostName());
                        if (inet.getHostName().toLowerCase().contains("guartinel")) {
                            LOG.I("=> Host: " + host + " Must be a sleepy guartinel wemos. Wait a little bit");
                            Thread.sleep(1500);
                            tryCount--;
                        }
                        Thread.sleep(1500);
                        LOG.I("=> Host: " + host + " Not successful. Retry!");
                    }


                    if (hostName == null) {
                        LOG.I("=> Host: " + host + " Cannot identify device after retries so skipping it.");
                        progress.increase();
                        return;
                    }

                    synchronized (result) {
                        result.put(hostName, inet);
                    }
                    progress.increase();
                } catch (Exception e) {
                    LOG.I("=> Host: " + host + " is not guartinel device! E: " + e.getMessage());
                    progress.increase();
                }
            }
        };
    }

    private static String sendHelloGuartinelRequest(String host) {
        try{


        OkHttpClient  client = new OkHttpClient.Builder()
                .connectTimeout(60, TimeUnit.SECONDS)
                .writeTimeout(60, TimeUnit.SECONDS)
                .readTimeout(60, TimeUnit.SECONDS)
                .retryOnConnectionFailure(true)
                .build();

        Request request = new Request.Builder()
                .url("http://" + host + "/helloGuartinel")
                .build();

        try (Response response = client.newCall(request).execute()) {
            if (!response.isSuccessful()) throw new IOException("Unexpected code " + response);

            Headers responseHeaders = response.headers();
            for (int i = 0; i < responseHeaders.size(); i++) {
                System.out.println(responseHeaders.name(i) + ": " + responseHeaders.value(i));
            }
            String responseRaw = response.body().string();

            JSONObject responseParsed = new JSONObject(responseRaw);
            String hello = responseParsed.getString("hello");
            if (!hello.equals("guartinel")) {
                LOG.I("=> Host: " + host + " is not guartinel device! Response: " + responseRaw);
                return null;
            }
            String hostName = responseParsed.getString("instanceName");
            LOG.I("=> Host: " + host + " Identified by helloGuartinel route response!");
            return hostName;
        }
        }catch (Exception e){
            LOG.I("=> Host: " + host + " Exception while getting instance name E: " + e.getMessage());
            return null;
        }

/*
        try {
            URL obj = new URL("http://" + host + "/helloGuartinel");
            HttpURLConnection con = (HttpURLConnection) obj.openConnection();
            con.setConnectTimeout(60000);
            con.setRequestMethod("GET");
            int responseCode = con.getResponseCode();

            if (responseCode != 200) {
                con.disconnect();
                LOG.I("=> Host: " + host + " is not guartinel device! Http code: " + responseCode);
                return null;
            }
            ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
            byte[] buffer = new byte[1024];
            int length = 0;
            while ((length = con.getInputStream().read(buffer)) != -1) {
                byteArrayOutputStream.write(buffer, 0, length);
            }
            String response = byteArrayOutputStream.toString();
            con.disconnect();
            JSONObject responseParsed = new JSONObject(response);
            String hello = responseParsed.getString("hello");
            if (!hello.equals("guartinel")) {
                LOG.I("=> Host: " + host + " is not guartinel device! Response: " + response);
                return null;
            }
            String hostName = responseParsed.getString("instanceName");
            LOG.I("=> Host: " + host + " Identified by helloGuartinel route response!");
            return hostName;
        } catch (Exception e) {
            LOG.I("=> Host: " + host + " Exception while getting instance name E: " + e.getMessage());
            return null;
        }*/
    }
}
