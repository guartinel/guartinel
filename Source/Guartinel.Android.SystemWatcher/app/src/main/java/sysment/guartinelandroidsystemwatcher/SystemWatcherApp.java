package sysment.guartinelandroidsystemwatcher;

import android.app.Application;

import com.squareup.otto.Bus;
import com.squareup.otto.Subscribe;
import com.squareup.otto.ThreadEnforcer;

import sysment.guartinelandroidsystemwatcher.persistance.DBO.Settings;
import sysment.guartinelandroidsystemwatcher.persistance.DataStore;
import sysment.guartinelandroidsystemwatcher.service.WatcherMaster;

/**
 * Created by moqs_the_one on 2017.07.28..
 */

public class SystemWatcherApp extends Application {
    private static Bus _bus = new Bus(ThreadEnforcer.ANY);
    private static DataStore store;
    private WatcherMaster watcherMaster;

    @Override
    public void onCreate() {
        super.onCreate();
        store = new DataStore();
        watcherMaster = new WatcherMaster();
        watcherMaster.init(this);
    }


    public static Bus getBus() {
        return _bus;
    }

    public static DataStore getDataStore() {
        return store;
    }
}
