package sysment.android.guartinel.core.network;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.Network;
import android.net.NetworkCapabilities;
import android.net.NetworkRequest;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;


public class SocketListener {
    ConnectivityManager.NetworkCallback wifiCallback = new ConnectivityManager.NetworkCallback() {
        @Override
        public void onAvailable(Network network) {
            LOG.I("SocketListener.wifiCallback");
            super.onAvailable(network);
            GuartinelApp.networkConnectionState.wifiSocket = network;
        }
    };

    ConnectivityManager.NetworkCallback cellOrWifiCallback = new ConnectivityManager.NetworkCallback() {
        @Override
        public void onAvailable(Network network) {
            LOG.I("SocketListener.cellOrWifiSocket");
            super.onAvailable(network);
            GuartinelApp.networkConnectionState.cellOrWifiSocket = network;
        }
    };
    ConnectivityManager.NetworkCallback cellCallback = new ConnectivityManager.NetworkCallback() {
        @Override
        public void onAvailable(Network network) {
            LOG.I("SocketListener.cellCallback");
            super.onAvailable(network);
            GuartinelApp.networkConnectionState.cellSocket = network;
        }
    };

    public SocketListener(Context context) {
        final ConnectivityManager connectivityManager = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);

        NetworkRequest.Builder wifiBuilder = new NetworkRequest.Builder();
        wifiBuilder.addTransportType(NetworkCapabilities.TRANSPORT_WIFI);
        connectivityManager.registerNetworkCallback(wifiBuilder.build(), wifiCallback);

        NetworkRequest.Builder cellOrWifiBuilder = new NetworkRequest.Builder();
        cellOrWifiBuilder.addTransportType(NetworkCapabilities.TRANSPORT_WIFI);
        cellOrWifiBuilder.addTransportType(NetworkCapabilities.TRANSPORT_CELLULAR);
        cellOrWifiBuilder.addCapability(NetworkCapabilities.NET_CAPABILITY_INTERNET);

        connectivityManager.registerNetworkCallback(cellOrWifiBuilder.build(), cellOrWifiCallback);

        NetworkRequest.Builder cellBuilder = new NetworkRequest.Builder();
        cellBuilder.addTransportType(NetworkCapabilities.TRANSPORT_CELLULAR);
        cellBuilder.addCapability(NetworkCapabilities.NET_CAPABILITY_INTERNET);

        connectivityManager.registerNetworkCallback(cellBuilder.build(), cellCallback);
    }
}
