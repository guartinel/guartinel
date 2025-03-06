package sysment.android.guartinel.core.utils;

import android.app.ActivityManager;
import android.content.Context;

import java.io.UnsupportedEncodingException;
import java.math.BigInteger;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;

import sysment.android.guartinel.core.utils.SystemInfoUtil;

/**
 * Created by sysment_dev on 02/29/2016.
 */
public class GeneralUtil {

    public static String getCurrentTimeString() {
      /*  DateFormat df = DateFormat.getTimeInstance();
        df.setTimeZone(TimeZone.getTimeZone("gmt"));
        String gmtTime = df.format(new Date());
        return gmtTime;*/
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy.MM.dd. HH:mm:ss");
        String dateTimeString = simpleDateFormat.format(new Date());
        return dateTimeString;
    }

    public static boolean isActivityRunning(Context context,Class activityClass)
    {
        ActivityManager activityManager = (ActivityManager) context.getSystemService(Context.ACTIVITY_SERVICE);
        List<ActivityManager.RunningTaskInfo> tasks = activityManager.getRunningTasks(Integer.MAX_VALUE);

        for (ActivityManager.RunningTaskInfo task : tasks) {
            if (activityClass.getCanonicalName().equalsIgnoreCase(task.baseActivity.getClassName()))
                return true;
        }

        return false;
    }


  /*  public static String getGuartinelAccountPassword(String deviceId) {
        String androidId = SystemInfoUtil.getAndroidID();
        String password = generateHashFromString(androidId, deviceId);
        return password;
    }*/



}
