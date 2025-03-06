package sysment.guartinelandroidsystemwatcher.util;

import android.content.Context;
import android.content.SharedPreferences;

import org.json.JSONException;
import org.json.JSONObject;


/**
 * Created by sysment_dev on 11/17/2016.
 */
public class Store {
/*
    private class Keys {
        public static final String SERVER = "SERVER";
        public static final String CHECK_INTERVAL = "CHECK_INTERVAL";
        public static final String NO_INTERNET_TIMEOUT = "NO_INTERNET_TIMEOUT";
        public static final String IS_ENABLED = "IS_ENABLED";

    }

    private static final String PREFERENCES_NAME = "ServerPrefs";

    private static void save(String key, String value) {
        SharedPreferences.Editor editor = GuartinelSystemWatcherApp.getContext().getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE).edit();
        editor.putString(key, value);
        editor.commit();
    }

    private static void save(String key, boolean value) {
        SharedPreferences.Editor editor = GuartinelSystemWatcherApp.getContext().getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE).edit();
        editor.putBoolean(key, value);
        editor.commit();
    }
    private static void save(String key, int value) {
        SharedPreferences.Editor editor = GuartinelSystemWatcherApp.getContext().getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE).edit();
        editor.putInt(key, value);
        editor.commit();
    }
    private static String load(String key) {
        SharedPreferences sharedPreferences = GuartinelSystemWatcherApp.getContext().getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE);
        return sharedPreferences.getString(key, "");
    }

    private static boolean loadBoolean(String key) {
        SharedPreferences sharedPreferences = GuartinelSystemWatcherApp.getContext().getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE);
        return sharedPreferences.getBoolean(key, false);
    }

    private static int loadInt(String key) {
        SharedPreferences sharedPreferences = GuartinelSystemWatcherApp.getContext().getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE);
        return sharedPreferences.getInt(key, 3);
    }
    public static Server loadServer() {
        try {
            return new Server().fromJSON(new JSONObject(load(Keys.SERVER)));
        } catch (JSONException e) {
            LogWrapper.Err("Cannot load server");
            return null;
        }
    }

    public static int loadCheckInterval() {
        return loadInt(Keys.CHECK_INTERVAL);
    }
    public static void saveCheckInterval(int value) {
        save(Keys.CHECK_INTERVAL, value);
    }

    public static int loadNoInternetTimeout() {
        return loadInt(Keys.NO_INTERNET_TIMEOUT);
    }
    public static void saveNoInternetTimeout(int value) {
        save(Keys.NO_INTERNET_TIMEOUT, value);
    }
    public static boolean loadIsEnabled() {
        return loadBoolean(Keys.IS_ENABLED);
    }
    public static void saveIsEnabled(boolean value) {
        save(Keys.IS_ENABLED, value);
    }



    public static void saveServer(Server server) {
        save(Keys.SERVER, server.toJSON().toString());
    }*/
}
