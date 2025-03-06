package sysment.android.guartinel.ui;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.PowerManager;
import android.support.v4.app.NotificationCompat;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.GeneralUtil;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.R;
import sysment.android.guartinel.ui.main.MainActivity;
import sysment.android.guartinel.ui.splash.SplashScreenActivity;

/**
 * Created by sysment_dev on 02/29/2016.
 */
public class MyNotificationManager {


    public static class Constants {
        public static final String IS_FORCED_ALERT = "is_forced_alert";
        public static final String MESSAGE = "message";
        public static final String ALERT_NOTIFICATION_CONTENT_TITLE = "Guartinel Alert";
        public static final String UPDATE_NOTIFICATION_CONTENT_TITLE = "Update available";
        public static final String UPDATE_NOTIFICATION_MESSAGE = "Please download the newest version of the app!";
        public static final String NOTIFICATION_ID = "notification_id";
        public static final String GCM_MESSAGE_BROADCAST = "gcm_message_broadcast";
    }

    public static void showAlertNotification(Context context, String message, boolean isForcedDeviceAlert) {
        LOG.I("showAlertNotification start");
        if (!GuartinelApp.getDataStore().isNotificationEnabled() && !isForcedDeviceAlert) {
            LOG.I("Notifications are disabled");
            return;
        }
        if (!GuartinelApp.getDataStore().isForcedNotificationEnabled() && isForcedDeviceAlert ) {
            LOG.I("Forced notifications are disabled");
            return;
        }

        int id = createID();

        Intent intent;
        if (GeneralUtil.isActivityRunning(context, MainActivity.class) && isForcedDeviceAlert) {
         //   intent = new Intent(context, MainActivity.class);
            LOG.I("Power manager saying screen is interactive. Sending local broadcast to mainactivity.");
            //we must create a new intent for the broadcast
            intent = new Intent(MyNotificationManager.Constants.GCM_MESSAGE_BROADCAST);
            setUpIntent(intent, isForcedDeviceAlert, message, id);
            intent.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
            context.sendBroadcast(intent);
            return;
        }
        LOG.I("Starting to setup notifications.");
        PowerManager powerManager = (PowerManager) context.getSystemService(Context.POWER_SERVICE);

        intent = new Intent(context, SplashScreenActivity.class);
        setUpIntent(intent, isForcedDeviceAlert, message, id);
        PendingIntent pendingIntent = PendingIntent.getActivity(context, 0, intent,
                PendingIntent.FLAG_ONE_SHOT);

        NotificationManager notificationManager =
                (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

        Uri defaultSoundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
        String CHANNEL_ID = "guartinel_01";

        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
            CharSequence name = "Guartinel";
            String Description = "Guartinel Alert Messages";
            int importance = NotificationManager.IMPORTANCE_HIGH;
            NotificationChannel mChannel = new NotificationChannel(CHANNEL_ID, name, importance);
            mChannel.setDescription(Description);
            mChannel.enableLights(true);
            mChannel.setLightColor(Color.RED);
            mChannel.enableVibration(true);
            // mChannel.setVibrationPattern(new long[]{100, 200, 300, 400, 500, 400, 300, 200, 400});
            mChannel.setShowBadge(false);
            notificationManager.createNotificationChannel(mChannel);
        }

        NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(context, CHANNEL_ID)
                .setSmallIcon(R.drawable.alert_notification_icon)
                .setContentTitle(Constants.ALERT_NOTIFICATION_CONTENT_TITLE)
                .setContentText(message)
                .setAutoCancel(true)
                .setChannelId(CHANNEL_ID);

        if (isForcedDeviceAlert) {
            LOG.I("Making alert forced.");
            notificationBuilder.setLights(Color.RED, 300, 100);
            notificationBuilder.setFullScreenIntent(pendingIntent, true);
           // notificationBuilder.setVibrate(new long[]{100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300});
            notificationBuilder.setVisibility(NotificationCompat.VISIBILITY_PUBLIC);
            //  turn it on screen (get wake_lock for 10 seconds)
            PowerManager.WakeLock wl = powerManager.newWakeLock(PowerManager.FULL_WAKE_LOCK | PowerManager.ACQUIRE_CAUSES_WAKEUP | PowerManager.ON_AFTER_RELEASE, "MH24_SCREENLOCK");
            wl.acquire(10000);
            PowerManager.WakeLock wl_cpu = powerManager.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "MH24_SCREENLOCK");
            wl_cpu.acquire(10000);
        } else {
            LOG.I("Setting notification not forced.");
            notificationBuilder.setContentIntent(pendingIntent);
            notificationBuilder.setSound(defaultSoundUri);
        }
        Notification notification = notificationBuilder.build();
        if (isForcedDeviceAlert) {
            notification.flags = Notification.FLAG_INSISTENT;
        }
        LOG.I("Showing notification");
        notificationManager.notify(id, notification);
    }
    public static void showUpdateNotificatin(Context context){
        if (!GuartinelApp.getDataStore().isNotificationEnabled()) {
            LOG.I("Notifications are disabled");
            return;
        }
        Intent intent = new Intent(context, SplashScreenActivity.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        PendingIntent pendingIntent = PendingIntent.getActivity(context, 0, intent,
                PendingIntent.FLAG_ONE_SHOT);

        Uri defaultSoundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
        NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(context)
                .setSmallIcon(R.drawable.warning_icon)
                .setContentTitle(Constants.UPDATE_NOTIFICATION_CONTENT_TITLE)
                .setContentText(Constants.UPDATE_NOTIFICATION_MESSAGE)
                .setAutoCancel(true)
                .setSound(defaultSoundUri)
                .setContentIntent(pendingIntent);

        android.app.NotificationManager notificationManager =
                (android.app.NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

        notificationManager.notify(createID(), notificationBuilder.build());
    }

    private static void setUpIntent(Intent intent, boolean isForcedDeviceAlert, String message, int id) {
        intent.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
        intent.putExtra(Constants.IS_FORCED_ALERT, isForcedDeviceAlert);
        intent.putExtra(Constants.MESSAGE, message);
        intent.putExtra(Constants.NOTIFICATION_ID, id);
        if(isForcedDeviceAlert){
            intent.setAction(MyNotificationManager.Constants.GCM_MESSAGE_BROADCAST);
        }
    }

    public static void showPartialOKNotification(Context context, String message) {
        if (!GuartinelApp.getDataStore().isNotificationEnabled()) {
            LOG.I("Notifications are disabled");
            return;
        }
        Intent intent = new Intent(context, SplashScreenActivity.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        PendingIntent pendingIntent = PendingIntent.getActivity(context, 0, intent,
                PendingIntent.FLAG_ONE_SHOT);

        Uri defaultSoundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
        NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(context)
                .setSmallIcon(R.drawable.warning_icon)
                .setContentTitle(Constants.ALERT_NOTIFICATION_CONTENT_TITLE)
                .setContentText(message)
                .setAutoCancel(true)
                .setSound(defaultSoundUri)
                .setContentIntent(pendingIntent);

        android.app.NotificationManager notificationManager =
                (android.app.NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

        notificationManager.notify(createID(), notificationBuilder.build());
    }

    public static void showOKNotification(Context context, String message) {
        if (!GuartinelApp.getDataStore().isNotificationEnabled()) {
            LOG.I("Notifications are disabled");
            return;
        }
        Intent intent = new Intent(context, SplashScreenActivity.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        PendingIntent pendingIntent = PendingIntent.getActivity(context, 0, intent,
                PendingIntent.FLAG_ONE_SHOT);

        Uri defaultSoundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
        NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(context)
                .setSmallIcon(R.drawable.check_icon)
                .setContentTitle(Constants.ALERT_NOTIFICATION_CONTENT_TITLE)
                .setContentText(message)
                .setAutoCancel(true)
                .setSound(defaultSoundUri)
                .setContentIntent(pendingIntent);

        android.app.NotificationManager notificationManager =
                (android.app.NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

        notificationManager.notify(createID(), notificationBuilder.build());
    }

    public static void cancelNotification(Context context, int notificationId) {
        android.app.NotificationManager notificationManager =
                (android.app.NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        notificationManager.cancel(notificationId);
    }

    private static int createID() {
        Date now = new Date();
        int id = Integer.parseInt(new SimpleDateFormat("ddHHmmss", Locale.US).format(now));
        return id;
    }
}
