package sysment.android.guartinel.core.connection.managementServer;

import org.json.JSONException;


import sysment.android.guartinel.ui.presenterCallbacks.AccountCreateResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.ActivateAccountResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.LoginDeviceResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.RegisterDeviceResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks._LoginResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.RegisterHardwareSensorCallback;
import sysment.android.guartinel.ui.presenterCallbacks._RegisterResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.ValidateTokenCallback;
import sysment.android.guartinel.ui.registerDevice.RegisterDevicePresenter;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public interface IManagementServer {
    void _deviceLogin(String password, String deviceUUID, String gcmID, _LoginResultCallback callback) throws JSONException;

    void _deviceRegister(String email, String password, String deviceName, String gcmId, String androidId, boolean overWrite, _RegisterResultCallback callback) throws JSONException;

    void deviceAlertConfirm(String alertId, PresenterResultCallback callback);

    void hardwareSupervisorRegisterHardware(String token, String id, String deviceName, RegisterHardwareSensorCallback callback);

    void accountValidateToken(String token, ValidateTokenCallback callback);

    void accountCreate(String firstName, String lastName, String email, String password, AccountCreateResultCallback callback);

    void accountResendActivationCode(String email, ActivateAccountResultCallback callback);

    void deviceAndroidLogin(String email, String passwordHash, String androidIdHash, String gcmID, LoginDeviceResultCallback callback);

    void deviceAndroidRegister(String deviceName, String email, String passwordHash, String androidID, String gcmToken, RegisterDeviceResultCallback registerDeviceResultCallback);
}

