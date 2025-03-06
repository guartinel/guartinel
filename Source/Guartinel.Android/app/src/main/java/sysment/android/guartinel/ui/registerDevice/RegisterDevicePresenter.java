package sysment.android.guartinel.ui.registerDevice;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.ui.presenterCallbacks.RegisterDeviceResultCallback;

public class RegisterDevicePresenter implements RegisterDeviceResultCallback {
    RegisterDeviceView _view;

    public void onDestroy() {
        _view = null;
    }

    public RegisterDevicePresenter(RegisterDeviceView loginDeviceView) {
        _view = loginDeviceView;
    }

    public void registerDevice(String deviceName) {
        GuartinelApp.getManagementServer().deviceAndroidRegister(
                deviceName,
                GuartinelApp.getDataStore().getEmail(),
                GuartinelApp.getDataStore().getSaltedPasswordHash(),
               SystemInfoUtil.getHashedAndroidID(),
                GuartinelApp.getDataStore().getGcmToken(),
                this);
    }

    @Override
    public void onConnectionError() {

    }

    @Override
    public void onAccountExpired() {
        _view.onAccountExpired();
    }

    @Override
    public void onInvalidUserNameOrPassword() {
        _view.invalidUserNameOrPassword();
    }

    @Override
    public void onDeviceNameAlreadyTaken() {
        _view.deviceNameAlreadyTaken();
    }

    @Override
    public void onSuccess(String token) {
        GuartinelApp.getDataStore().setToken(token);
        GuartinelApp.getDataStore().setDeviceName(_view.getDeviceName());
        _view.openDashboard();
    }
}
