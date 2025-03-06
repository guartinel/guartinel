package sysment.android.guartinel.core.network;

import android.content.Context;
import sysment.android.guartinel.core.utils.LOG;

public class WifiConnectionManager {

    public static void connect(Context context, String ssid, String password, WifiUtility.WifiConnectionResult onResult) {
        String currentSSID = WifiUtility.getCurrentSSID(context);
        if(currentSSID!= null && currentSSID.contains(ssid)){
            LOG.I("Already connected..");
            onResult.onConnected(true);
            return;
        }
        WifiUtility.startConnectTo(context, ssid, password);
        boolean isSuccessful = WifiUtility.waitUntilConnectedToSSID(context, ssid, null);
        if (!isSuccessful) {
            onResult.onCannotConnect();
            return;
        }
        boolean hasIP = WifiUtility.waitUntilGotIPAndConnection(context, null);
        if (!hasIP) {
            onResult.onCannotConnect();
            return;
        }
        onResult.onConnected(WifiUtility.isGooglePingable());
    }

    private static void connect(Context context, String ssid, WifiUtility.WifiConnectionResult onResult) {
        WifiUtility.startConnectToSavedNetwork(context, ssid);
        boolean isSuccessful = WifiUtility.waitUntilConnectedToSSID(context, ssid, null);
        if (!isSuccessful) {
            onResult.onCannotConnect();
            return;
        }
        boolean hasIP = WifiUtility.waitUntilGotIPAndConnection(context, null);
        if (!hasIP) {
            onResult.onCannotConnect();
            return;
        }
        onResult.onConnected(WifiUtility.isGooglePingable());
    }

    public static void connectBackToOriginalWifi(final Context context, final WifiUtility.WifiConnectionResult onConnectBackFinished) {
        if (WifiUtility.getOriginalWifiSSID() == null ) {
            WifiUtility.waitUntilDisconnectedFromCurrent(context, null);
            LOG.I("WifiConnectionManager.connectBackToOriginalWifi No saved ap. Returning onConnected");
            onConnectBackFinished.onConnected(true);
            return;
        }
        LOG.I("WifiConnectionManager.connectBackToOriginalWifi Starting to connect");
        connect(context, WifiUtility.getOriginalWifiSSID(), onConnectBackFinished);
    }

    public static void testWifiAP(final Context context, final String ssid, String password, final WifiUtility.WifiConnectionResult onTestAPFinished) {

        final WifiUtility.WifiConnectionResult connectBackResult = new WifiUtility.WifiConnectionResult() {
            @Override
            public void onConnected(boolean hasInternet) {
                LOG.I("WifiConnectionManager.test connectBackResult:onConnected");
                onTestAPFinished.onConnected(hasInternet);
            }

            @Override
            public void onCannotConnect() {
                LOG.I("WifiConnectionManager.test connectBackResult:onCannotConnect");
                onTestAPFinished.onCannotConnect();
            }
        };
        final WifiUtility.WifiConnectionResult testResult = new WifiUtility.WifiConnectionResult() {
            @Override
            public void onConnected(final boolean hasInternet) {
                LOG.I("WifiConnectionManager.testWifiAP testResult:onConnected");
                connectBackToOriginalWifi(context, connectBackResult);
            }

            @Override
            public void onCannotConnect() {
                LOG.I("WifiConnectionManager.testWifiAP testResult:onCannotConnect");
                onTestAPFinished.onCannotConnect();
            }
        };
        LOG.I("WifiConnectionManager.testWifiAP starting");
        connect(context, ssid, password, testResult);
    }
}
