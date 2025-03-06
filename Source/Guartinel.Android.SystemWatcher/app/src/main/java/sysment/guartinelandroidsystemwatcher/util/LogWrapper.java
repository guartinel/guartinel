package sysment.guartinelandroidsystemwatcher.util;

import android.util.Log;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public class LogWrapper {
    private static class Constants {
        protected static final String TAG = "GuartinelSystemWatcher";
    }

    public static void Inf(String message) {
        System.out.println(message);
        Log.i(Constants.TAG, message);
    }

    public static void Err(String message) {
        System.out.println(message);
        Log.e(Constants.TAG, message);
    }

    public static void Err( String message,Exception e) {
        System.out.println(message);
        Log.e(Constants.TAG, message + "E :" + e.getMessage() + e.getStackTrace());
    }

}
