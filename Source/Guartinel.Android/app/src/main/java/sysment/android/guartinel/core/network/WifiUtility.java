package sysment.android.guartinel.core.network;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.ConnectivityManager;
import android.net.DhcpInfo;
import android.net.NetworkInfo;
import android.net.wifi.ScanResult;
import android.net.wifi.SupplicantState;
import android.net.wifi.WifiConfiguration;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Build;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.connection.hardwareSensor.HardwareSensorImpl;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.utils.StringUtility;

import static android.content.Context.CONNECTIVITY_SERVICE;

public class WifiUtility {

    public static class Constants {
        public static Integer DEFAULT_WAIT_TIME_SEC = 30;
        public static String UNKNOWN_SSID = "<unknown ssid>";
    }

    public interface OnScanFinished {
        void wifiDisabled();

        void finished(List<ScanResult> result);
    }

    public interface WifiConnectionResult {
        void onConnected(boolean hasInternet);

        void onCannotConnect();
    }

    private static String originalWifiSSID;

    public static String getOriginalWifiSSID() {
        return originalWifiSSID;
    }


    public static void startDebug(final Context context) {
        LOG.I("WifiUtility startDebug!");
        context.registerReceiver(_debugBroadcastReceiver, new IntentFilter(WifiManager.NETWORK_STATE_CHANGED_ACTION));
    }

    public static void stopDebug(final Context context) {
        LOG.I("WifiUtility stopDebug!");
        safeUnregisterReceiver(context, _debugBroadcastReceiver);
    }

    public static void saveCurrentWifiSSID(final Context context) {
        originalWifiSSID = WifiUtility.getCurrentSSID(context, null);
        if (isUnknownSSID(originalWifiSSID)) {
            originalWifiSSID = null;
            LOG.I("saveCurrentWifiSSID: null!");
        } else {
            LOG.I("saveCurrentWifiSSID: " + originalWifiSSID);
        }
    }

    public static NetworkConnectionState getCurrentWifiState(Context context) {
        NetworkConnectionState state = new NetworkConnectionState();
        state.isWifiConnected = isWifiConnected(context);
        state.SSID = getCurrentSSID(context);
        return state;
    }

    private static boolean isUnknownSSID(String ssid) {
        if (ssid == null) {
            return true;
        }
        if (ssid.contains(Constants.UNKNOWN_SSID)) {
            return true;
        }
        return false;
    }

    private static BroadcastReceiver _debugBroadcastReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            final String action = intent.getAction();

            if (action.equals(WifiManager.NETWORK_STATE_CHANGED_ACTION)) {
                NetworkInfo info = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
                String extraInfo = info.getExtraInfo();
                if (extraInfo == null) {
                    extraInfo = "";
                }
                LOG.I("WIFI-DEBUG Action: " + action);
                LOG.I("WIFI-DEBUG detailed state: " + info.getDetailedState());
                LOG.I("WIFI-DEBUG extra infO: " + extraInfo);
                final WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
                final WifiInfo connectionInfo = wifiManager.getConnectionInfo();
                LOG.I("WIFI-DEBUG Wifimanager connection info: " + connectionInfo.getSSID());
            }

            if (action.equals(WifiManager.SUPPLICANT_STATE_CHANGED_ACTION)) {
                SupplicantState supplicantState = ((SupplicantState) intent.getParcelableExtra(WifiManager.EXTRA_NEW_STATE));
                LOG.I("SupplicantState.name: " + supplicantState.name());
            }
        }
    };


    private static boolean isWifiConnected(Context context) {
        ConnectivityManager connManager = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo mWifi = connManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);

        return mWifi.isConnected();
    }


    public static void scanAvailableNetworks(final Context context, final OnScanFinished listener) {
        final WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        if (!wifiManager.isWifiEnabled()) {
            boolean canBeSet = wifiManager.setWifiEnabled(true);
            if (!canBeSet) {
                listener.wifiDisabled();
                return;
            }
        }
        context.registerReceiver(new BroadcastReceiver() {
            @Override
            public void onReceive(Context c, Intent intent) {
                List<ScanResult> result = wifiManager.getScanResults();


                listener.finished(result);
                context.unregisterReceiver(this);
            }
        }, new IntentFilter(WifiManager.SCAN_RESULTS_AVAILABLE_ACTION));
        wifiManager.startScan();
    }

    public static List<ScanResult> getWifiAccessPointsWithoutGuartinel(Context context) {
        final WifiManager wifiManager = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        List<ScanResult> result = wifiManager.getScanResults();

        List<ScanResult> finalResult = new ArrayList<>();
        for (ScanResult scanResult : result) {
            if (!scanResult.SSID.contains(HardwareSensorImpl.Constants.SENSOR_SSID_PREFIX)) {
                finalResult.add(scanResult);
            }
        }
        return finalResult;
    }

    public static void startConnectTo(Context context, String ssid, String pass) {
        GuartinelApp.networkConnectionState.reset();
        final WifiManager wifiManager = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);

        WifiConfiguration wifiConfig = new WifiConfiguration();
        wifiConfig.SSID = String.format("\"%s\"", ssid);
        wifiConfig.preSharedKey = String.format("\"%s\"", pass);

        int netId = -666;
        List<WifiConfiguration> wifiConfigurations = wifiManager.getConfiguredNetworks();
        if (wifiConfigurations == null) {
            wifiConfigurations = new ArrayList<>();
        }
        for (WifiConfiguration tmp : wifiConfigurations) {
            if (tmp.SSID.contains(ssid)) {
                netId = tmp.networkId;
                break;
            }
        }
        if (netId == -666) {
            netId = wifiManager.addNetwork(wifiConfig);
        }

        wifiManager.enableNetwork(netId, true);
    }

    public static boolean startConnectToSavedNetwork(Context context, String ssid) {
        final WifiManager wifiManager = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        int netId = -1;
        List<WifiConfiguration> wifiConfigurations = wifiManager.getConfiguredNetworks();
        if (wifiConfigurations == null) {
            wifiConfigurations = new ArrayList<>();
        }
        for (WifiConfiguration tmp : wifiConfigurations) {
            if (tmp.SSID.contains(ssid)) {
                netId = tmp.networkId;
                wifiManager.disconnect();
                wifiManager.enableNetwork(netId, true);
                break;
            }
        }
        return netId == -1;
    }

    public static String getCurrentSSID(Context context) {
        if (context == null) {
            return null;
        }
        String ssid = null;
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = cm.getActiveNetworkInfo();
        if (networkInfo == null) {
            return null;
        }

        WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        final WifiInfo connectionInfo = wifiManager.getConnectionInfo();
        if (connectionInfo != null && !StringUtility.isBlank(connectionInfo.getSSID())) {
            ssid = connectionInfo.getSSID();
        }
        return ssid;
    }

    public static String getCurrentSSID(Context context, NetworkInfo info) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O || info == null) {
            return getCurrentSSID(context);
        }
        return info.getExtraInfo();
    }

    private static String intToIp(int i) {
        return (i & 0xFF) + "." +
                ((i >> 8) & 0xFF) + "." +
                ((i >> 16) & 0xFF) + "." +
                ((i >> 24) & 0xFF);
    }


    private static String getGateWay(Context context) {
        WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        DhcpInfo dhcpInfo = wifiManager.getDhcpInfo();
        return intToIp(dhcpInfo.gateway);
    }

    private static String getIPAddress(Context context) {
        WifiManager wifiMgr = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        WifiInfo wifiInfo = wifiMgr.getConnectionInfo();
        int ip = wifiInfo.getIpAddress();

        String ipString = intToIp(ip);
        if (ipString.equals("0.0.0.0")) {
            return null;
        }
        return ipString;
    }


    public static boolean waitUntilDisconnectedFromCurrent(Context context, Integer maxWaitSec) {

        if (maxWaitSec == null) {
            maxWaitSec = Constants.DEFAULT_WAIT_TIME_SEC * 1000;
        } else {
            maxWaitSec = maxWaitSec * 1000;
        }
        int currentWait = 0;

        final WifiManager wifiManager = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        wifiManager.disconnect();

        while (GuartinelApp.networkConnectionState.isWifiConnected) {
            try {
                LOG.I("Sleeping while still connected to WIFI");
                Thread.sleep(5000);
                currentWait += 5000;
                if (currentWait > maxWaitSec) {
                    return false;
                }
            } catch (InterruptedException e) {
                LOG.I("Cannot sleep");
            }
        }
        return true;
    }

    public static boolean waitUntilGotIPAndConnection(Context context, Integer maxWaitSec) {
        if (maxWaitSec == null) {
            maxWaitSec = Constants.DEFAULT_WAIT_TIME_SEC * 1000;
        } else {
            maxWaitSec = maxWaitSec * 1000;
        }
        int currentWait = 0;
        boolean isNoIP = getIPAddress(context) == null || getIPAddress(context).length() == 0;
        boolean isNoGW = getGateWay(context) == null || getGateWay(context).length() == 0;
        while (isNoIP || isNoGW) {
            isNoIP = getIPAddress(context) == null || getIPAddress(context).length() == 0;
            isNoGW = getGateWay(context) == null || getGateWay(context).length() == 0;

            try {
                LOG.I("Sleeping while everything OK. IP: " + isNoIP + " GW: " + isNoGW);
                Thread.sleep(500);
                currentWait += 500;
                if (currentWait > maxWaitSec) {
                    return false;
                }
            } catch (InterruptedException e) {
                LOG.I("Cannot sleep");
            }
        }
        try {
            Thread.sleep(1000);
        } catch (InterruptedException e) {
            LOG.I("Cannot sleep");
        }
        LOG.I("IP is not null. Continue!");
        return true;
    }

    public static boolean waitUntilConnectedToSSID(Context context, String
            requiredSSID, Integer maxWaitSec) {
        if (maxWaitSec == null) {
            maxWaitSec = Constants.DEFAULT_WAIT_TIME_SEC * 1000;
        } else {
            maxWaitSec = maxWaitSec * 1000;
        }
        int currentWait = 0;
        while (!GuartinelApp.networkConnectionState.isWifiConnected && GuartinelApp.networkConnectionState.SSID != requiredSSID) {
            try {
                LOG.I("Sleeping while not connected to " + requiredSSID);
                Thread.sleep(500);
                currentWait += 500;
                if (currentWait > maxWaitSec) {
                    return false;
                }
            } catch (InterruptedException e) {
                LOG.I("Cannot sleep");
            }
        }
        try {
            Thread.sleep(1000);
        } catch (InterruptedException e) {
            LOG.I("Cannot sleep");
        }
        LOG.I("Connected to " + requiredSSID + " Continue!");
        return true;
    }


    public static void safeUnregisterReceiver(Context context, BroadcastReceiver receiver) {
        try {
            context.unregisterReceiver(receiver);
        } catch (Exception e) {
            LOG.E("Catched failed broadcastreceiver unregister", e);
        }
    }

    public static boolean isGooglePingable() {
        Runtime runtime = Runtime.getRuntime();
        try {
            Process ipProcess = runtime.exec("/system/bin/ping -c 1 8.8.8.8");
            int exitValue = ipProcess.waitFor();
            return (exitValue == 0);
        } catch (IOException e) {
            e.printStackTrace();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        return false;
    }

    public static void cleanAddedGuartinelAPs(Context context) {
        final WifiManager wifiManager = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        List<WifiConfiguration> wifiConfigurations = wifiManager.getConfiguredNetworks();
        if (wifiConfigurations == null) {
            wifiConfigurations = new ArrayList<>();
        }
        for (WifiConfiguration tmp : wifiConfigurations) {
            if (tmp.SSID.contains(HardwareSensorImpl.Constants.SENSOR_SSID_PREFIX)) {
                wifiManager.removeNetwork(tmp.networkId);
            }
        }
        if (getCurrentSSID(context) != null && getCurrentSSID(context).contains(HardwareSensorImpl.Constants.SENSOR_SSID_PREFIX)) {
            wifiManager.disconnect();
        }
    }

    public static boolean isWifiOn(Context context) {
        WifiManager wifiMgr = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        return wifiMgr.isWifiEnabled();
    }

    public static boolean isConnectingOrConnected(Context context) {
        ConnectivityManager connManager = (ConnectivityManager) context.getSystemService(CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);
        return networkInfo.isConnectedOrConnecting();
    }

}
