package sysment.android.guartinel.core.connection.managementServer;

import org.json.JSONException;

import sysment.android.guartinel.BuildConfig;
import sysment.android.guartinel.core.connection.HttpConnector;
import sysment.android.guartinel.core.connection.managementServer.account.create.AccountCreateRequest;
import sysment.android.guartinel.core.connection.managementServer.account.resendActivationCode.ResendActivationCodeRequest;
import sysment.android.guartinel.core.connection.managementServer.account.validateToken.ValidateTokenRequest;
import sysment.android.guartinel.core.connection.managementServer.device.alert.confirm.ConfirmDeviceAlertRequest;
import sysment.android.guartinel.core.connection.managementServer.device._login._LoginRequest;
import sysment.android.guartinel.core.connection.managementServer.device._register._RegisterRequest;
import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.connection.managementServer.device.android.login.DeviceAndroidLoginRequest;
import sysment.android.guartinel.core.connection.managementServer.device.android.register.DeviceAndroidRegisterRequest;
import sysment.android.guartinel.core.connection.managementServer.hardwareSupervisor.RegisterHardwareRequest;
import sysment.android.guartinel.ui.presenterCallbacks.AccountCreateResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.ActivateAccountResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.LoginDeviceResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.RegisterDeviceResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks._LoginResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.RegisterHardwareSensorCallback;
import sysment.android.guartinel.ui.presenterCallbacks._RegisterResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.ValidateTokenCallback;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public class ManagementServerImpl implements IManagementServer {

    private HttpConnector _httHttpConnector;

    public ManagementServerImpl(HttpConnector httHttpConnector) {
        _httHttpConnector = httHttpConnector;
    }

    @Override
    public void _deviceLogin(String password, String deviceUUID, String gcmToken, _LoginResultCallback callback) throws JSONException {
        _LoginRequest request = new _LoginRequest(password, deviceUUID, gcmToken, callback);
        _httHttpConnector.makePostOnWifiOrCellular(GuartinelApp._context, BuildConfig.BACKEND_ADDRESS + request.getURL(), request);
    }

    @Override
    public void _deviceRegister(String email, String password, String deviceName, String gcmId, String androidID, boolean overWrite, _RegisterResultCallback callback) {
        _RegisterRequest request = new _RegisterRequest(email, password, deviceName, gcmId, androidID, overWrite, callback);
        _httHttpConnector.makePostOnWifiOrCellular(GuartinelApp._context, BuildConfig.BACKEND_ADDRESS + request.getURL(), request);
    }

    public void deviceAlertConfirm(String alertId, PresenterResultCallback callback) {
        ConfirmDeviceAlertRequest request = new ConfirmDeviceAlertRequest(GuartinelApp.getDataStore().getToken(), alertId, callback);
        _httHttpConnector.makePostOnWifiOrCellular(GuartinelApp._context, BuildConfig.BACKEND_ADDRESS + request.getURL(), request);
    }

    @Override
    public void hardwareSupervisorRegisterHardware(String token, String id, String deviceName, RegisterHardwareSensorCallback callback) {
        RegisterHardwareRequest request = new RegisterHardwareRequest(token, id, deviceName, callback);
        _httHttpConnector.makePostOnWifiOrCellular(GuartinelApp._context, BuildConfig.BACKEND_ADDRESS + request.getURL(), request);
    }

    @Override
    public void accountValidateToken(String token, ValidateTokenCallback callback) {
        ValidateTokenRequest request = new ValidateTokenRequest(token, callback);
        _httHttpConnector.makePostOnWifiOrCellular(GuartinelApp._context, BuildConfig.BACKEND_ADDRESS + request.getURL(), request);

    }

    @Override
    public void accountCreate(String firstName, String lastName, String email, String password, AccountCreateResultCallback callback) {
        AccountCreateRequest request = new AccountCreateRequest(firstName, lastName, email, password, callback);
        _httHttpConnector.makePostOnWifiOrCellular(GuartinelApp._context, BuildConfig.BACKEND_ADDRESS + request.getURL(), request);
    }

    @Override
    public void accountResendActivationCode(String email, ActivateAccountResultCallback callback) {
        ResendActivationCodeRequest request = new ResendActivationCodeRequest(email, callback);
        _httHttpConnector.makePostOnWifiOrCellular(GuartinelApp._context, BuildConfig.BACKEND_ADDRESS + request.getURL(), request);

    }

    @Override
    public void deviceAndroidLogin(String email, String passwordHash, String androidIdHash, String gcmToken, LoginDeviceResultCallback callback) {
        DeviceAndroidLoginRequest request = new DeviceAndroidLoginRequest(email, passwordHash, androidIdHash, gcmToken, callback);
        _httHttpConnector.makePostOnWifiOrCellular(GuartinelApp._context, BuildConfig.BACKEND_ADDRESS + request.getURL(), request);
    }

    @Override
    public void deviceAndroidRegister(String deviceName, String email, String passwordHash, String androidIdHash, String gcmToken, RegisterDeviceResultCallback callback) {
        DeviceAndroidRegisterRequest request = new DeviceAndroidRegisterRequest(deviceName, email, passwordHash, androidIdHash, gcmToken, callback);
        _httHttpConnector.makePostOnWifiOrCellular(GuartinelApp._context, BuildConfig.BACKEND_ADDRESS + request.getURL(), request);

    }
}
