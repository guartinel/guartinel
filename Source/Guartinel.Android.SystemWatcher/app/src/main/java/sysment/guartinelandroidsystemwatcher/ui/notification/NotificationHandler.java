package sysment.guartinelandroidsystemwatcher.ui.notification;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.media.RingtoneManager;
import android.net.Uri;
import android.support.v4.app.NotificationCompat;
import android.support.v4.app.TaskStackBuilder;

import java.util.UUID;

import sysment.guartinelandroidsystemwatcher.R;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstance;
import sysment.guartinelandroidsystemwatcher.ui.MainActivity;

/**
 * Created by sysment_dev on 11/19/2016.
 */
public class NotificationHandler {
    private static final int NOTIFICATION_ID = 0001;

    public static void showServerNotAvailableNotification(Context context, ServerInstance serverInstance) {
        issueNotification(context, "Error", "Cannot connect to: " + serverInstance.getName());
    }

    public static void showNoInternetTimeout(Context context) {
        issueNotification(context, "Error", "Internet is not available to check guartinel health.");
    }
    public static void checkedInternet(Context context) {
        issueNotification(context, "Checked internet", "check");
    }
    public static void checkedServer(Context context) {
        issueNotification(context, "Checked server", "check");
    }
    private static void issueNotification(Context context, String title, String message) {

        NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(context);
        mBuilder.setSmallIcon(R.drawable.alert);
        mBuilder.setContentTitle(title);
        mBuilder.setContentText(message);

        Intent resultIntent = new Intent(context, MainActivity.class);
        TaskStackBuilder stackBuilder = TaskStackBuilder.create(context);
        stackBuilder.addParentStack(MainActivity.class);
        stackBuilder.addNextIntent(resultIntent);

        PendingIntent resultPendingIntent = stackBuilder.getPendingIntent(0, PendingIntent.FLAG_UPDATE_CURRENT);
        mBuilder.setContentIntent(resultPendingIntent);

        Uri alarmSound = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
        mBuilder.setSound(alarmSound);


        NotificationManager mNotificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        Notification notification = mBuilder.build();
        mNotificationManager.notify((int)Math.random(), notification);
    }
    /*
    static Notification _currentNotification;
    static NotificationManager _notificationManager;
    static Ringtone ringtone;

    private static int SERVER_NOT_AVAILABLE_NOTIF_ID = 001;

    public static void showServerNotAvailableNotification(){




    }


    private static void alarm(int id,String title,String message) {
        disableAlarm();

        try {
            Uri soundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_ALARM);
            ringtone = RingtoneManager.getRingtone(GuartinelSystemWatcherApp.getContext(), soundUri);
            ringtone.play();

        } catch (Exception e) {
            e.printStackTrace();
        }

        NotificationCompat.Builder mBuilder =
                new NotificationCompat.Builder(GuartinelSystemWatcherApp.getContext())
                        .setSmallIcon(R.drawable.alert)
                        .setContentTitle(title)
                        .setContentText(message);



        Intent openAppIntent = new Intent(GuartinelSystemWatcherApp.getContext(),MainActivity.class);
        openAppIntent.setFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP);

        PendingIntent openAppPendingIntent =
                PendingIntent.getActivity(
                        GuartinelSystemWatcherApp.getContext(),
                        0,
                        openAppIntent,
                        PendingIntent.FLAG_UPDATE_CURRENT
                );


        mBuilder.setContentIntent(openAppPendingIntent);
        mBuilder.setSmallIcon(R.drawable.alert);
        _currentNotification = mBuilder.build();

        _notificationManager =
                (NotificationManager) GuartinelSystemWatcherApp.getContext().getSystemService(GuartinelSystemWatcherApp.getContext().NOTIFICATION_SERVICE);
        _notificationManager.notify(id, _currentNotification);
    }

    private static void disableAlarm(int id) {
        if (_notificationManager != null) {
            _notificationManager.cancel(id);
        }

        if (ringtone != null) {
            ringtone.stop();
        }
    }*/
}
