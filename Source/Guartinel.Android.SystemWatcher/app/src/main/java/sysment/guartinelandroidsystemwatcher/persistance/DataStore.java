package sysment.guartinelandroidsystemwatcher.persistance;

import android.content.Context;
import android.content.SharedPreferences;
import android.text.TextUtils;

import com.squareup.otto.Subscribe;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

import sysment.guartinelandroidsystemwatcher.SystemWatcherApp;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstance;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstances;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.Settings;
import sysment.guartinelandroidsystemwatcher.ui.busEvents.ServerInstanceConfiguredEvent;
import sysment.guartinelandroidsystemwatcher.ui.busEvents.ServerInstancesUpdatedEvent;
import sysment.guartinelandroidsystemwatcher.ui.busEvents.SettingsChangedEvent;

/**
 * Created by moqs_the_one on 2017.07.28..
 */

public class DataStore {

    public DataStore() {
        SystemWatcherApp.getBus().register(this);
    }

    @Subscribe
    public void onServerInstanceConfigured(ServerInstanceConfiguredEvent event) {
        boolean foundAndOverWritten = false;
        for (ServerInstance instance :
                serverInstances) {
            if (instance.getID().equals(event.instance.getID())) {
                instance = event.instance;
                foundAndOverWritten = true;
                break;
            }
        }
        if (!foundAndOverWritten) {
            serverInstances.add(event.instance);
        }
        saveServerInstances(event.context, serverInstances);
        SystemWatcherApp.getBus().post(new ServerInstancesUpdatedEvent(serverInstances));
    }

    private final static String PREFERENCES_NAME = "SystemWatcher";
    private final static String SETTINGS_KEY = "SETTINGS";
    private final static String SERVER_INSTANCES = "SERVER_INSTANCES";

    private ServerInstances serverInstances;
    private Settings settings;

    public Settings getSettings(Context context) {
        if (settings == null) {
            settings = loadSettings(context);
        }
        return settings;
    }

    public ServerInstances getServerInstances(Context context) {
        if (serverInstances == null) {
            serverInstances = loadServerInstances(context);
        }
        return serverInstances;
    }

    public void deleteServerInstance(Context context, ServerInstance instanceToDelete) {
        ServerInstance instanceInDeathRow = null;
        for (ServerInstance instance :
                serverInstances) {
            if (instance.getID().equals(instanceToDelete.getID())) {
                instanceInDeathRow = instance;
                break;
            }
        }
        if (instanceInDeathRow != null) {
            serverInstances.remove(instanceInDeathRow);
        }
        saveServerInstances(context, serverInstances);
        SystemWatcherApp.getBus().post(new ServerInstancesUpdatedEvent(serverInstances));

    }

    public void saveSettings(Context context, Settings settings, boolean shouldPostEvent) {
        SharedPreferences.Editor editor = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE).edit();
        editor.putString(SETTINGS_KEY, new Gson().toJson(settings));
        editor.commit();
        if (shouldPostEvent) {
            SystemWatcherApp.getBus().post(new SettingsChangedEvent(context, settings));
        }
    }

    private Settings loadSettings(Context context) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE);
        String resultJSON = sharedPreferences.getString(SETTINGS_KEY, "");
        if (TextUtils.isEmpty(resultJSON)) {
            return new Settings();
        }
        return new Gson().fromJson(resultJSON, Settings.class);
    }

    private ServerInstances loadServerInstances(Context context) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE);
        String resultJSON = sharedPreferences.getString(SERVER_INSTANCES, "");
        if (TextUtils.isEmpty(resultJSON)) {
            return new ServerInstances();
        }
        return new ServerInstances().fromJSON(resultJSON);
    }

    private void saveServerInstances(Context context, ServerInstances servers) {
        SharedPreferences.Editor editor = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE).edit();
        editor.putString(SERVER_INSTANCES, servers.toJSON());
        editor.commit();
    }


}
