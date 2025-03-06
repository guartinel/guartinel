package sysment.guartinel.hardwareconfigurator.connection;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.ScanResult;
import android.net.wifi.WifiConfiguration;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.List;

import sysment.guartinel.hardwareconfigurator.tools.LOG;

/**
 * Created by DAVT on 2017.12.06..
 */

public class WifiHelper {
    public interface OnScanFinished {
        void wifiDisabled();

        void finished(List<ScanResult> result);
    }

    private static String originalWifiSSID;

    public static void saveOriginalWifiSSID(Context context) {
        originalWifiSSID = getCurrentSSID(context);
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

    public static List<ScanResult> getLastScanResult(Context context) {
        final WifiManager wifiManager = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        List<ScanResult> result = wifiManager.getScanResults();
        return result;
    }

    public static InetAddress getCurrentWifiNetworkAddress(Context context) throws UnknownHostException {
        WifiManager wifiManager = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        // wifiManager.getDhcpInfo().;
        return InetAddress.getByName(intToIp(wifiManager.getDhcpInfo().gateway));
    }

    public static String getCurrentSSID(Context context) {
        String ssid = null;
        ConnectivityManager connManager = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);
        if (networkInfo.isConnected()) {
            final WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
            final WifiInfo connectionInfo = wifiManager.getConnectionInfo();
            if (connectionInfo != null) {
                ssid = connectionInfo.getSSID();
            }
        }
        return ssid;
    }

    private static String intToIp(int i) {
        return (i & 0xFF) + "." +
                ((i >> 8) & 0xFF) + "." +
                ((i >> 16) & 0xFF) + "." +
                ((i >> 24) & 0xFF);
    }


    public interface WifiConnectionResult {
        void onConnected();

        void onCannotConnect();
    }

    public static void connectBackToOriginalWifi(final Context context) {
        if (originalWifiSSID == null) {
            return;
        }
        WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        int netId = -1;
        LOG.I("Connectback Original: " + originalWifiSSID);

        for (WifiConfiguration tmp : wifiManager.getConfiguredNetworks()) {
            //if (tmp.SSID.equals("\"" + originalWifiSSID + "\"")) {
            if (tmp.SSID.contains(originalWifiSSID)) {
                netId = tmp.networkId;
                wifiManager.enableNetwork(netId, true);
                break;
            }
        }
    }

    public static boolean isConnectedWifiIsTheOriginal(Context context) {
        if (originalWifiSSID.equals(getCurrentSSID(context))) {
            return true;
        }
        return false;
    }

    public static void connectToWifi(final Context context, final String ssid, String pass, final WifiConnectionResult onResult) {
        LOG.I("WIFI_CONNECT. Starting to connect :" + ssid);
        final String ssidBeforeNewConnection = getCurrentSSID(context);

        if (ssidBeforeNewConnection != null && ssidBeforeNewConnection.contains(ssid)) {
            LOG.I("WIFI_CONNECT Already connected");
            onResult.onConnected();
            return;
        }

        WifiConfiguration wifiConfig = new WifiConfiguration();
        wifiConfig.SSID = String.format("\"%s\"", ssid);
        wifiConfig.preSharedKey = String.format("\"%s\"", pass);

        WifiManager wifiManager = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        int netId = wifiManager.addNetwork(wifiConfig);
        wifiManager.disconnect();
        wifiManager.enableNetwork(netId, true);
        wifiManager.reconnect();

        context.registerReceiver(new BroadcastReceiver() {
            @Override
            public void onReceive(Context c, Intent intent) {
                final String action = intent.getAction();
                if (action.equals(WifiManager.NETWORK_STATE_CHANGED_ACTION)) {
                    NetworkInfo info = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);

                    LOG.I("WIFI_CONNECT action:" + info.getDetailedState() + " SSID: " + info.getExtraInfo());
                    boolean notThePreviousWifi = ssidBeforeNewConnection != null && !info.getExtraInfo().contains(ssidBeforeNewConnection);
                    boolean notUnKnown = !info.getExtraInfo().contains("<unknown ssid>");
                    if (notUnKnown && notThePreviousWifi && !info.getExtraInfo().contains("<unknown ssid>") &&
                            (info.getDetailedState().equals(NetworkInfo.DetailedState.FAILED) ||
                                    info.getDetailedState().equals(NetworkInfo.DetailedState.BLOCKED) ||
                                    info.getDetailedState().equals(NetworkInfo.DetailedState.DISCONNECTED))) {
                        LOG.I("WIFI_CONNECT Cannot connect to wifi: " + ssid);
                        context.unregisterReceiver(this);
                        onResult.onCannotConnect();
                    }

                    if (info.getDetailedState().equals(NetworkInfo.DetailedState.CONNECTED)) {
                        /*if (info.getExtraInfo().equals(ssidBeforeNewConnection)) {
                            LOG.I("Cannot connect to wifi: "+ssid + " .Connected back to the original wifi");
                            context.unregisterReceiver(this);
                            onResult.onCannotConnect();
                        }*/
                        if (info.getExtraInfo().contains(ssid)) {
                            LOG.I("WIFI_CONNECT Connected to wifi: " + ssid);
                            context.unregisterReceiver(this);
                            onResult.onConnected();
                        } else {
                            LOG.I("WIFI_CONNECT Connected to " + info.getExtraInfo() + " which is not the " + ssid);
                            context.unregisterReceiver(this);
                            onResult.onCannotConnect();
                        }

                    }
                }
            }
        }, new IntentFilter(WifiManager.NETWORK_STATE_CHANGED_ACTION));
    }


    public static void reconnectWifi(Context context) {
        WifiManager wifi = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        wifi.disconnect();
        wifi.reconnect();
    }


    public static boolean isConnectedToThisSSID(Context context, String ssid) {
        WifiManager mainWifi = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        WifiInfo currentWifi = mainWifi.getConnectionInfo();
        if (currentWifi != null) {
            if (currentWifi.getSSID() != null) {
                if (currentWifi.getSSID().contains(ssid))
                    return true;
            }
        }
        return false;
    }

    public static boolean isWifiOn(Context context) {
        WifiManager wifiMgr = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        return wifiMgr.isWifiEnabled();
    }

    public static boolean isConnectedToAnyWifi(Context context) {
        WifiManager wifiMgr = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);

        if (wifiMgr.isWifiEnabled()) { // Wi-Fi adapter is ON
            WifiInfo wifiInfo = wifiMgr.getConnectionInfo();
            if (wifiInfo.getNetworkId() == -1) {
                return false; // Not connected to an access point
            }
            return true; // Connected to an access point
        }
        return false; // Wi-Fi adapter is OFF
    }
}
