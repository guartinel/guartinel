package sysment.android.guartinel.core.utils;

import android.util.Log;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public class LOG {
    private static class Constants {
        protected static final String TAG = "GUARTINEL";
    }

    public static void I(String message) {
        Log.d(Constants.TAG, message);
    }

    public static void printNotImplemented(){
        I("NOT IMPLEMENTED: "+ Log.getStackTraceString(new Exception()));
    }

    public static void E(String msg,Exception e){
        Log.e(Constants.TAG, msg, e);
    }
}
