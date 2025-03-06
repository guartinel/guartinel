package sysment.android.guartinel.core.gcm;

import android.content.Context;
import android.content.Intent;
import android.support.v4.content.LocalBroadcastManager;

import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;

import java.util.Map;

import sysment.android.guartinel.core.persistance.Alert;
import sysment.android.guartinel.core.persistance.AlertHolder;
import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.MyNotificationManager;
import sysment.android.guartinel.ui.presenterCallbacks.ConfirmDeviceAlertCallback;
import sysment.android.guartinel.ui.presenterCallbacks.LoginDeviceResultCallback;

/**
 * Created by sysment_dev on 02/24/2016.
 */
public class MyFireBaseMessagingService extends FirebaseMessagingService {

    public class GCMKeys {
        public static final String ALERT_MESSAGE = "alert_message";
        public static final String ALERT_DETAILS = "alert_details";
        public static final String ALERT_ID = "alert_id";
        public static final String IS_RECOVERY = "is_recovery";
        public static final String PACKAGE_NAME = "package_name";
        public static final String IS_PACKAGE_ALERTED = "is_package_alerted";
        public static final String FORCED_DEVICE_ALERT = "forced_device_alert";
    }

    public static class BroadcastConst {
        public static final String BROADCAST_ID = "gcm_event";
    }

    @Override
    public void onNewToken(String token) {
        super.onNewToken(token);
        LOG.I("MyInstanceIDListenerService.onTokenRefresh Token: " + token);
        GuartinelApp.getDataStore().setGCMToken(token);
        GuartinelApp.getDataStore().setLastAppVersion(SystemInfoUtil.getVersionCode());

        Intent intent = new Intent(this, UpdateGcmIdService.class);
        startService(intent);
    }

    @Override
    public void onMessageReceived(RemoteMessage remoteMessage) {
        Map<String, String> data = remoteMessage.getData();
        super.onMessageReceived(remoteMessage);
        String message = data.get(GCMKeys.ALERT_MESSAGE);
        String alertID = data.get(GCMKeys.ALERT_ID);
        String packageName = data.get(GCMKeys.PACKAGE_NAME);
        String isRecoveryString = data.get(GCMKeys.IS_RECOVERY);
        String isPackageAlertedString = data.get(GCMKeys.IS_PACKAGE_ALERTED);
        String alertDetails = data.get(GCMKeys.ALERT_DETAILS);
        String forcedDeviceAlertString = data.get(GCMKeys.FORCED_DEVICE_ALERT);

        boolean isRecovery = false;
        if (isRecoveryString != null) {
            isRecovery = isRecoveryString.toLowerCase().contains("true");
        }

        boolean isPackageAlerted = false;
        if (isPackageAlertedString != null) {
            isPackageAlerted = isPackageAlertedString.toLowerCase().contains("true");
        }
        boolean isForcedDeviceAlert = false;
        if (forcedDeviceAlertString != null) {
            isForcedDeviceAlert = forcedDeviceAlertString.toLowerCase().contains("true");
        }
        LOG.I(
                "GCM Received. From: " + remoteMessage.getFrom() +
                        " message: " + message +
                        " details: " + alertDetails +
                        " alertID: " + alertID +
                        " packageName: " + packageName +
                        " isForcedDeviceAlert: " + isForcedDeviceAlert +
                        " isRecovery: " + isRecovery +
                        " isPackageAlerted: " + isPackageAlerted);

        LOG.I("isPackageAlerted && !isRecovery: " + (isPackageAlerted && !isRecovery));
        if (isPackageAlerted && !isRecovery) {
            MyNotificationManager.showAlertNotification(this, message, isForcedDeviceAlert);
        }

        if (isPackageAlerted && isRecovery) {
            MyNotificationManager.showPartialOKNotification(this, message);
        }
        if (!isPackageAlerted && isRecovery) {
            MyNotificationManager.showOKNotification(this, message);
        }
        if (!isPackageAlerted && !isRecovery) {
            MyNotificationManager.showAlertNotification(this, message, isForcedDeviceAlert);
        }


        final AlertHolder alertHolder = GuartinelApp.getDataStore().getAlertHolder();
        final Alert newAlert = new Alert().BuildFromGCMMessage(message, alertDetails, alertID, packageName, isRecovery, isPackageAlerted);
        alertHolder.addNewAlert(newAlert);
        GuartinelApp.getDataStore().setAlertHolder(alertHolder);
        final Context context  = this;
        sendBroadcastFromNewAlert();
        LOG.I("Received GCM trying to confirm..");
        GuartinelApp.getManagementServer().deviceAlertConfirm(newAlert.alertID, new ConfirmDeviceAlertCallback() {
            @Override
            public void onInvalidDeviceToken() {
                LOG.I("deviceAlertConfirm -> onInvalidDeviceToken callback");
                tryLoginAndSaveTokenIfSuccessful(context, newAlert);
            }

            @Override
            public void onConnectionError() {
                LOG.I("deviceAlertConfirm -> onConnectionError callback");
                LOG.I("Connection error while confirming alert retrieval.");
            }
        });
    }

    private void tryLoginAndSaveTokenIfSuccessful(final Context context, final Alert newAlert) {

        LOG.I("tryLoginAndSaveTokenIfSuccessful to get a new token");
        GuartinelApp.getManagementServer().deviceAndroidLogin(
                GuartinelApp.getDataStore().getEmail(),
                GuartinelApp.getDataStore().getSaltedPasswordHash(),
                SystemInfoUtil.getHashedAndroidID(),
                GuartinelApp.getDataStore().getGcmToken(), new LoginDeviceResultCallback() {
                    @Override
                    public void onConnectionError() {

                    }

                    @Override
                    public void onDeviceNotRegistered() {

                    }

                    @Override
                    public void onAccountExpired() {

                    }

                    @Override
                    public void onInvalidUserNameOrPassword() {

                    }

                    @Override
                    public void onLoginSuccess(String token, String deviceName) {
                        LOG.I("deviceAlertConfirm -> onLoginSuccess New token: " + token);

                        GuartinelApp.getDataStore().setToken(token);

                        GuartinelApp.getManagementServer().deviceAlertConfirm(newAlert.alertID, new ConfirmDeviceAlertCallback() {
                            @Override
                            public void onInvalidDeviceToken() {
                                LOG.I("deviceAlertConfirm -> onInvalidDeviceToken callback");
                                tryLoginAndSaveTokenIfSuccessful(context,newAlert);
                            }

                            @Override
                            public void onConnectionError() {
                                LOG.I("deviceAlertConfirm -> onConnectionError callback");
                                LOG.I("Connection error while confirming alert retrieval.");
                            }
                        });
                    }

                    @Override
                    public void onUpdateNow() {
                        MyNotificationManager.showUpdateNotificatin(context);
                    }
                });

    }

    private void sendBroadcastFromNewAlert() {
        Intent broadcastIntent = new Intent(MyFireBaseMessagingService.BroadcastConst.BROADCAST_ID);
        LocalBroadcastManager.getInstance(this).sendBroadcast(broadcastIntent);
    }
}
