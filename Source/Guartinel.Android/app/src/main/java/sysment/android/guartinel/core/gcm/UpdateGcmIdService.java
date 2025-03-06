package sysment.android.guartinel.core.gcm;

import android.app.IntentService;
import android.content.Intent;
import android.text.TextUtils;

import org.json.JSONException;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.utils.GeneralUtil;
import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.ui.MyNotificationManager;
import sysment.android.guartinel.ui.presenterCallbacks.LoginDeviceResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks._LoginResultCallback;

/**
 * Created by sysment_dev on 02/29/2016.
 */
public class UpdateGcmIdService extends IntentService implements LoginDeviceResultCallback {
    public UpdateGcmIdService(String name) {
        super(name);
    }

    @Override
    protected void onHandleIntent(Intent intent) {
        String email = GuartinelApp.getDataStore().getEmail();
        if (TextUtils.isEmpty(email)) {
            LOG.I("No credentials but received gcm id refresh..");
            return;
        }

        GuartinelApp.getManagementServer().deviceAndroidLogin(
                GuartinelApp.getDataStore().getEmail(),
                GuartinelApp.getDataStore().getSaltedPasswordHash(),
                SystemInfoUtil.getHashedAndroidID(),
                GuartinelApp.getDataStore().getGcmToken(), this);

    }

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
        GuartinelApp.getDataStore().setDeviceName(deviceName);
        GuartinelApp.getDataStore().setToken(token);
        LOG.I("Updated gcm token and got new token and device name in the response.");
    }

    @Override
    public void onUpdateNow() {
        MyNotificationManager.showUpdateNotificatin(this);
    }
}
