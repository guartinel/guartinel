package sysment.android.guartinel.core.network;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.NetworkInfo;
import android.net.wifi.WifiManager;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;

public class WifiEventListener extends BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        if (!intent.getAction().equals(WifiManager.NETWORK_STATE_CHANGED_ACTION)) {
            return;
        }
        NetworkInfo networkInfo = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
        NetworkInfo.DetailedState connectionState = networkInfo.getDetailedState();

        GuartinelApp.networkConnectionState.SSID = WifiUtility.getCurrentSSID(context, networkInfo);
        GuartinelApp.networkConnectionState.isWifiConnected = connectionState == NetworkInfo.DetailedState.CONNECTED;
        LOG.I("WifiState updated.SSID="+ GuartinelApp.networkConnectionState.SSID + " isWifiConnected="+ GuartinelApp.networkConnectionState.isWifiConnected);
    }
}
