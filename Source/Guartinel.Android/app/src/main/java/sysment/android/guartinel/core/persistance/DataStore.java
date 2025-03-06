package sysment.android.guartinel.core.persistance;

import android.content.Context;
import android.content.SharedPreferences;

import java.util.ArrayList;
import java.util.List;

import sysment.android.guartinel.core.persistance.encryption.Encryption;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.utils.StringUtility;
import sysment.android.guartinel.core.utils.SystemInfoUtil;


public class DataStore {


    private static class Constants {
        protected static final String DB_NAME = "GuartinelDB";

        public static class Keys {
            public static class Credential {
                public static final String DEVICE_UUID = "DEVICE_UUID";
                public static final String EMAIL = "EMAIL";
                public static final String DEVICE_NAME = "DEVICE_NAME";
                public static final String TOKEN = "TOKEN";
                public static final String PASSWORD = "PASSWORD";
            }

            public static final String GCM_TOKEN = "GCM_TOKEN";
            public static final String ALERT_HOLDER = "ALERT_HOLDER";
            public static final String LAST_APP_VERSION = "LAST_APP_VERSION";
            public static final String IS_NOTIFICATION_ENABLED = "IS_NOTIFICATION_ENABLED";
            public static final String IS_FORCED_NOTIFICATION_ENABLED = "IS_FORCED_NOTIFICATION_ENABLED";
        }
    }

    String key = "S0Rg7yhxlZtlz3GoZZdE";
    String salt = SystemInfoUtil.getHashedAndroidID();
    byte[] iv = new byte[16];
    Encryption encryption = Encryption.getDefault(key, salt, iv);

    private SharedPreferences sharedPreferences;
    private SharedPreferences.Editor editor;

    public DataStore(Context context) {
        sharedPreferences = context.getSharedPreferences("SharedPreferencesGuartinel", Context.MODE_PRIVATE);
        editor = sharedPreferences.edit();
    }

    private void trySaveEncryptedOrRaw(String key, String payload) {
        String result = encryption.encryptOrNull(payload);
        if (result == null) {
            result = payload;
        }
        editor.putString(key, result);
        editor.commit();
    }

    public String tryLoadDecryptedOrRaw(String key) {
        String resultRaw = sharedPreferences.getString(key, "");
        String resultProcessed = encryption.decryptOrNull(resultRaw);
        if (resultProcessed == null) {
            return resultRaw;
        }
        return resultProcessed;
    }

    public void setEmail(String email) {
        trySaveEncryptedOrRaw(Constants.Keys.Credential.EMAIL, email);
    }

    public String getEmail() {
        return tryLoadDecryptedOrRaw(Constants.Keys.Credential.EMAIL);
    }

    public void setPlainPassword(String password) {
        LOG.I("setUserPassword: " + password);
        trySaveEncryptedOrRaw(Constants.Keys.Credential.PASSWORD, password);
    }

    private String getPlainPassword() {
        String pswd = tryLoadDecryptedOrRaw(Constants.Keys.Credential.PASSWORD);
        return pswd;
    }

    public String getSaltedPasswordHash() {
        return StringUtility.getHashWithSalt(getPlainPassword(), getEmail());
    }


    public void setDeviceName(String deviceName) {
        trySaveEncryptedOrRaw(Constants.Keys.Credential.DEVICE_NAME, deviceName);
    }

    public String getDeviceName() {
        return tryLoadDecryptedOrRaw(Constants.Keys.Credential.DEVICE_NAME);
    }

    public void setToken(String token) {
        trySaveEncryptedOrRaw(Constants.Keys.Credential.TOKEN, token);
    }

    public String getToken() {
        return tryLoadDecryptedOrRaw(Constants.Keys.Credential.TOKEN);
    }

    public void setGCMToken(String token) {
        trySaveEncryptedOrRaw(Constants.Keys.GCM_TOKEN, token);
    }

    public String getGcmToken() {
        return tryLoadDecryptedOrRaw(Constants.Keys.GCM_TOKEN);
    }

    public void setAlertHolder(AlertHolder alertHolder) {
        editor.putString(Constants.Keys.ALERT_HOLDER, alertHolder.getJSONString());
        editor.commit();
    }

    public void removeAlertById(String alertUUID) {
        AlertHolder alertHolder = new AlertHolder(sharedPreferences.getString(Constants.Keys.ALERT_HOLDER, ""));
        List<Alert> filteredAlerts = new ArrayList<Alert>();
        for (Alert alert : alertHolder.alerts) {
            if (!alert.UUID.equals(alertUUID)) {
                filteredAlerts.add(alert);
            }
        }
        alertHolder.alerts = filteredAlerts;
        setAlertHolder(alertHolder);
    }

    public AlertHolder getAlertHolder() {
        return new AlertHolder(sharedPreferences.getString(Constants.Keys.ALERT_HOLDER, ""));
    }

    public int getLastAppVersion() {
        return sharedPreferences.getInt(Constants.Keys.LAST_APP_VERSION, Integer.MIN_VALUE);
    }

    public void setLastAppVersion(int value) {
        editor.putInt(Constants.Keys.LAST_APP_VERSION, value);
        editor.commit();
    }

    public boolean isNotificationEnabled() {
        return sharedPreferences.getBoolean(Constants.Keys.IS_NOTIFICATION_ENABLED, true);
    }

    public void setNotificationEnabled(boolean value) {
        editor.putBoolean(Constants.Keys.IS_NOTIFICATION_ENABLED, value);
        editor.commit();
    }

    public boolean isForcedNotificationEnabled() {
        return sharedPreferences.getBoolean(Constants.Keys.IS_FORCED_NOTIFICATION_ENABLED, true);
    }

    public void setForcedNotificationEnabled(boolean value) {
        editor.putBoolean(Constants.Keys.IS_FORCED_NOTIFICATION_ENABLED, value);
        editor.commit();
    }


    public void clearAlertHolder() {
        editor.remove(Constants.Keys.ALERT_HOLDER);
        editor.commit();
    }
}
