package sysment.android.guartinel.core.network;

import android.net.Network;

public class NetworkConnectionState {
    public boolean isWifiConnected = false;
    public String SSID = null;
    public Network wifiSocket;
    public Network cellOrWifiSocket;
    public Network cellSocket;

    public void reset() {
        isWifiConnected = false;
        SSID = null;
        wifiSocket = null;
        cellOrWifiSocket = null;
        cellSocket = null;
    }

}
