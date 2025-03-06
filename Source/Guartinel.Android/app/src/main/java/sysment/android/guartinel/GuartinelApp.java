package sysment.android.guartinel;

import android.app.Application;
import android.content.Context;
import android.os.StrictMode;
import android.util.Log;

import com.splunk.mint.Mint;

import sysment.android.guartinel.core.connection.HttpConnector;
import sysment.android.guartinel.core.connection.managementServer.ManagementServerImpl;
import sysment.android.guartinel.core.persistance.DataStore;
import sysment.android.guartinel.core.network.SocketListener;
import sysment.android.guartinel.core.network.NetworkConnectionState;
import sysment.android.guartinel.core.network.WifiUtility;

/**
 * Created by sysment_dev on 2016.02.22..
 */
public class GuartinelApp extends Application {
    private static DataStore _dataStore;
    private static ManagementServerImpl _managementServer;
    private static HttpConnector _httpConnector;
    public static Context _context;
    public static NetworkConnectionState networkConnectionState;
    private static SocketListener _socketListener;
    @Override
    public void onCreate() {
        super.onCreate();
        Mint.initAndStartSession(this, "b1b1e3dc");
        _context = getApplicationContext();
        _httpConnector = new HttpConnector(_context);
        _managementServer = new ManagementServerImpl(_httpConnector);
        networkConnectionState = WifiUtility.getCurrentWifiState(_context);
        _socketListener = new SocketListener(_context);
        Log.d("GUARTINEL", "GuartinelApp.onCreate()");


        StrictMode.setThreadPolicy(new StrictMode.ThreadPolicy.Builder()
                .detectDiskReads()
                .detectDiskWrites()
                .detectNetwork()   // or .detectAll() for all detectable problems
                .penaltyLog()
                .build());
        StrictMode.setVmPolicy(new StrictMode.VmPolicy.Builder()
                .detectLeakedSqlLiteObjects()
                .detectLeakedClosableObjects()
                .penaltyLog()
                .penaltyDeath()
                .build());
    }

    @Override
    public void onLowMemory() {
        super.onLowMemory();
        Log.d("GUARTINEL", "GuartinelApp.onLowMemory()");
    }

    @Override
    public void onTerminate() {
        super.onTerminate();
        Log.d("GUARTINEL", "GuartinelApp.onTerminate()");
    }

    public static HttpConnector getConnector() {
        return _httpConnector;
    }

    public static ManagementServerImpl getManagementServer() {
        if (_managementServer == null) {
            _managementServer = new ManagementServerImpl(_httpConnector);
        }
        return _managementServer;
    }

    public static DataStore getDataStore() {
        if (_dataStore == null) {
            _dataStore = new DataStore(_context);
        }
        return _dataStore;
    }
}