package sysment.guartinelandroidsystemwatcher.util;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;

import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * Created by sysment_dev on 11/18/2016.
 */
public class Tools {

    public static String getTimeStamp() {
        SimpleDateFormat sdf = new SimpleDateFormat("MM.dd. - HH:mm:ss");
        String currentTime = sdf.format(new Date());
        return currentTime;
    }

    public static boolean isNetworkAvailable(Context context) {
        ConnectivityManager connectivityManager
                = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo activeNetworkInfo = connectivityManager.getActiveNetworkInfo();
        return activeNetworkInfo != null && activeNetworkInfo.isConnected();
    }
}
