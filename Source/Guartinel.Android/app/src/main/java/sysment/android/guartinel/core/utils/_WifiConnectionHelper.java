package sysment.android.guartinel.core.utils;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.ConnectivityManager;
import android.net.DhcpInfo;
import android.net.Network;
import android.net.NetworkCapabilities;
import android.net.NetworkInfo;
import android.net.NetworkRequest;
import android.net.wifi.WifiConfiguration;
import android.net.wifi.WifiManager;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.network.WifiUtility;

import static android.content.Context.CONNECTIVITY_SERVICE;

//public class _WifiConnectionHelper {
//
//    private static String originalWifiSSID;
//
//
//    public static void _saveCurrentWifiSSID(final Context context) {
//        originalWifiSSID = WifiUtility.getCurrentSSID(context,null);
//        LOG.I("saveCurrentWifiSSID: " + originalWifiSSID);
//    }
//
//    private static void connectBackToOriginalWifi(final Context context, final WifiUtility.WifiConnectionResult onResult) {
//        final WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
//
//        if (originalWifiSSID == null && !WifiUtility.isConnectingOrConnected(context)) {
//            LOG.I("connectBackToOriginalWifi OriginalWfiSSDI is null and we are not connected to anything");
//            onResult.onConnected(true);
//            return;
//        }
//      /*  if (originalWifiSSID == null && isConnectingOrConnected(context)) {
//            final String ssidToDisconnectFrom = getCurrentSSID(context);
//            LOG.I("connectBackToOriginalWifi OriginalWfiSSDI is null and we are connected to some wifi. First disconnect.");
//            wifiManager.disconnect();
//
//            context.registerReceiver(new BroadcastReceiver() {
//                @Override
//                public void onReceive(Context c, Intent intent) {
//                    final String action = intent.getAction();
//                    if (!action.equals(WifiManager.NETWORK_STATE_CHANGED_ACTION)) {
//                        return;
//                    }
//
//                    final NetworkInfo info = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
//
//                    if (!info.getDetailedState().equals(NetworkInfo.DetailedState.DISCONNECTED)) {
//                        LOG.I("connectBackToOriginalWifi.BroadcastReceiver Not DISCONNECTED so skipping..");
//                        return;
//                    }
//
//                    if (getCurrentSSID(context) != null && !getCurrentSSID(context).contains(ssidToDisconnectFrom)) {
//                        LOG.I("connectBackToOriginalWifi.BroadcastReceiver. This is not the ssid that we wanted to disconnect from.");
//                        return;
//                    }
//                    safeUnregisterReceiver(context, this);
//                    onResult.onConnected(true);
//                }
//            }, new IntentFilter(WifiManager.NETWORK_STATE_CHANGED_ACTION));
//            return;
//        }*/
//;
//      String currentSSID = WifiUtility.getCurrentSSID(context,null);
//        LOG.I("connectBackToOriginalWifi. CurrentSSID:  " +currentSSID + " original: " + originalWifiSSID);
//        if (currentSSID != null && currentSSID.equals(originalWifiSSID)) {
//            WifiUtility.waitUntilGotIPAndConnection(context, 10000);
//            onResult.onConnected(true);
//            return;
//        }
//
//        int netId = -1;
//        LOG.I("connectBackToOriginalWifi Connect back Original: " + originalWifiSSID);
//
//        for (WifiConfiguration tmp : wifiManager.getConfiguredNetworks()) {
//            if (tmp.SSID.contains(originalWifiSSID)) {
//                netId = tmp.networkId;
//                wifiManager.disconnect();
//                wifiManager.enableNetwork(netId, true);
//                break;
//            }
//        }
//
//        context.registerReceiver(new BroadcastReceiver() {
//            @Override
//            public void onReceive(Context c, Intent intent) {
//                final String action = intent.getAction();
//                if (!action.equals(WifiManager.NETWORK_STATE_CHANGED_ACTION)) {
//                    return;
//                }
//                final NetworkInfo info = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
//
//                LOG.I("connectBackToOriginalWifi.BroadcastReceiver2 state: " + info.getDetailedState());
//                if (!info.getDetailedState().equals(NetworkInfo.DetailedState.CONNECTED)) {
//                    LOG.I("connectBackToOriginalWifi.BroadcastReceiver2 State is not connected so returning.");
//                    return;
//                }
//                WifiUtility.safeUnregisterReceiver(context, this);
//                        /*String extraInfo = info.getExtraInfo(); REMOVE extrainfo
//                        if (extraInfo == null) {
//                            extraInfo = "";
//                        }*/
//                String currentSSID = WifiUtility.getCurrentSSID(context,info);
//                if (currentSSID == null) {
//                    LOG.I("connectBackToOriginalWifi.BroadcastReceiver2 ssid is null so skipping event..");
//                    return;
//                }
//
//                if (isUnknownSSID(currentSSID)) {
//                    LOG.I("connectBackToOriginalWifi.BroadcastReceiver2 ssid is unknown so skipping event..");
//                    return;
//                }
//                // if (!extraInfo.contains(originalWifiSSID)) {  REMOVE extrainfo
//                if (!currentSSID.contains(originalWifiSSID)) {
//                    LOG.I("connectBackToOriginalWifi.BroadcastReceiver2 Connected to " + currentSSID + " which is not the " + originalWifiSSID);
//                    onResult.onCannotConnect();
//                    return;
//                }
//                new Thread(new Runnable() {
//                    @Override
//                    public void run() {
//                        LOG.I("connectBackToOriginalWifi.BroadcastReceiver2 Connected to wifi: " + originalWifiSSID);
//                        WifiUtility.waitUntilGotIPAndConnection(context, 10000);
//                        LOG.I("connectBackToOriginalWifi.BroadcastReceiver2 IP address is: " + WifiUtility.getIPAddress(context));
//                        onResult.onConnected(true);
//                    }
//                }).start();
//            }
//        }, new IntentFilter(WifiManager.NETWORK_STATE_CHANGED_ACTION));
//    }
//
//    private static void testWifiAP(final Context context, final String ssid, String pass, final WifiUtility.WifiConnectionResult onResult) {
//        connectToWifi(context, ssid, pass, new WifiUtility.WifiConnectionResult() {
//            @Override
//            public void onConnected(final boolean hasInternet) {
//                LOG.I("testWifiAP.onConnected");
//                connectBackToOriginalWifi(context, new WifiUtility.WifiConnectionResult() {
//                    @Override
//                    public void onConnected(boolean _hasInternet) {
//                        LOG.I("testWifiAP.testWifiAP.onConnected");
//                        onResult.onConnected(hasInternet);
//                    }
//
//                    @Override
//                    public void onCannotConnect() {
//                        LOG.I("testWifiAP.testWifiAP.onCannotConnect");
//                        onResult.onCannotConnect();
//                    }
//                });
//            }
//
//            @Override
//            public void onCannotConnect() {
//                LOG.I("testWifiAP.onCannotConnect");
//                onResult.onCannotConnect();
//            }
//
//        });
//    }
//
//    private static boolean isUnknownSSID(String ssid){
//        if(ssid == null){return true;}
//        if(ssid.contains("<unknown ssid>")){return true;}
//        return false;
//    }
//    private static void connectToWifi(final Context context, final String ssid, String pass, final WifiUtility.WifiConnectionResult onResult) {
//        LOG.I("connectToWifi. Starting to connect: '" + ssid + "' pass: '" + pass + "'");
//        final String ssidBeforeNewConnection = WifiUtility.getCurrentSSID(context,null) == null ? "" : WifiUtility.getCurrentSSID(context,null);
//         /*if (ssidBeforeNewConnection != null && ssidBeforeNewConnection.contains(ssid)) {
//            LOG.I("connectToWifi Already connected");
//            onResult.onConnected(true);
//            return;
//        }*/
//        final WifiManager wifiManager = (WifiManager) context.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
//        //  wifiManager.disconnect();
//
//        WifiConfiguration wifiConfig = new WifiConfiguration();
//        wifiConfig.SSID = String.format("\"%s\"", ssid);
//        wifiConfig.preSharedKey = String.format("\"%s\"", pass);
//
//        int netId = -666;
//        for (WifiConfiguration tmp : wifiManager.getConfiguredNetworks()) {
//            if (tmp.SSID.contains(ssid)) {
//                netId = tmp.networkId;
//                break;
//            }
//        }
//
//        if (netId != -666) {
//            wifiManager.removeNetwork(netId);
//        }
//        netId = wifiManager.addNetwork(wifiConfig);
//        wifiManager.enableNetwork(netId, true);
//
//        context.registerReceiver(new BroadcastReceiver() {
//            @Override
//            public void onReceive(Context c, Intent intent) {
//                final String action = intent.getAction();
//                if (!action.equals(WifiManager.NETWORK_STATE_CHANGED_ACTION)) {
//                    return;
//                }
//                final NetworkInfo info = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
//
//                final String currentSSID = WifiUtility.getCurrentSSID(context,info);
//                if (currentSSID == null) {
//                    LOG.I("connectToWifi currentSSID is null so skipping event..");
//                    return;
//                }
//                if(isUnknownSSID(currentSSID)){
//                    LOG.I("connectToWifi currentSSID is unknown so skipping event..");
//                    return;
//                }
//
//                boolean notUnKnown = !isUnknownSSID(currentSSID);
//                boolean notThePreviousWifi = ssidBeforeNewConnection != null && !currentSSID.contains(ssidBeforeNewConnection);
//                NetworkInfo.DetailedState connectionState = info.getDetailedState();
//                if (notUnKnown && notThePreviousWifi &&
//                        (connectionState.equals(NetworkInfo.DetailedState.FAILED) ||
//                                connectionState.equals(NetworkInfo.DetailedState.BLOCKED)||
//                                connectionState.equals(NetworkInfo.DetailedState.DISCONNECTED))) {
//                    LOG.I("connectToWifi Cannot connect to wifi: " + ssid);
//                    context.unregisterReceiver(this);
//                    onResult.onCannotConnect();
//                    return;
//                }
//                //if (extraInfo.contains(finalSsidBeforeNewConnection) && info.getDetailedState().equals(NetworkInfo.DetailedState.FAILED)) {REMOVE extrainfo
//                if (currentSSID.contains(ssidBeforeNewConnection) && connectionState.equals(NetworkInfo.DetailedState.FAILED)) {
//                    context.unregisterReceiver(this);
//                    onResult.onCannotConnect();
//                    return;
//                }
//                if (!connectionState.equals(NetworkInfo.DetailedState.CONNECTED)) {
//                    LOG.I("connectToWifi connection state is : " + connectionState + " returning..");
//                    return;
//                }
//                context.unregisterReceiver(this);
//                if (!currentSSID.contains(ssid)) {
//                    LOG.I("connectToWifi Failed to connect " + ssid + " Connected to " + currentSSID + " which is not the " + ssid);
//                    onResult.onCannotConnect();
//                    return;
//                }
//                new Thread(new Runnable() {
//                    @Override
//                    public void run() {
//                        final ConnectivityManager connectivityManager = (ConnectivityManager) context.getSystemService(CONNECTIVITY_SERVICE);
//                        LOG.I("connectToWifi Connected to wifi: " + ssid);
//
//                        NetworkRequest.Builder builder = new NetworkRequest.Builder();
//                        builder.addTransportType(NetworkCapabilities.TRANSPORT_WIFI);
//
//                        ConnectivityManager.NetworkCallback networkCallback = new ConnectivityManager.NetworkCallback() {
//                            @Override
//                            public void onAvailable(Network network) {
//                                super.onAvailable(network);
//                                LOG.I("ConnectToWIFI.NetworkCallback Network: " + network.toString());
//                                NetworkInfo networkInfo = connectivityManager.getNetworkInfo(network);
//
//                                if (networkInfo == null || networkInfo.getExtraInfo() == null) {
//                                    LOG.I("ConnectToWIFI.NetworkCallback networkinfo is null or empty.");
//                                    return;
//                                }
//                                if (!networkInfo.getExtraInfo().contains(ssid)) {
//                                    LOG.I("ConnectToWIFI.NetworkCallback ssid is: " + networkInfo.getExtraInfo() + " which is not the desired one: " + ssid);
//                                    onResult.onCannotConnect();
//                                    connectivityManager.unregisterNetworkCallback(this);
//                                    return;
//                                }
//
//                                LOG.I("ConnectToWIFI.Socket is created to wifi: " + networkInfo.getExtraInfo());
//                                connectivityManager.unregisterNetworkCallback(this);
//                                GuartinelApp.getConnector().getWifiClient(network);
//
//                                WifiUtility.waitUntilGotIPAndConnection(context, 10000);
//                                LOG.I("connectToWifi IP address is: " + WifiUtility.getIPAddress(context));
//
//                                DhcpInfo dhcpInfo = wifiManager.getDhcpInfo();
//                                LOG.I("connectToWifi GW: " + WifiUtility.intToIp(dhcpInfo.gateway));
//                                LOG.I("connectToWifi DNS: " + WifiUtility.intToIp(dhcpInfo.dns1));
//                                LOG.I("connectToWifi NetworkID: " + wifiManager.getConnectionInfo().getNetworkId());
//
//                                onResult.onConnected(WifiUtility.isGooglePingable());
//                            }
//                        };
//                        connectivityManager.registerNetworkCallback(builder.build(), networkCallback);
//
//                    }
//                }).start();
//            }
//        }, new IntentFilter(WifiManager.NETWORK_STATE_CHANGED_ACTION));
//    }
//
//
//}
