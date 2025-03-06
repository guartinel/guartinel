package sysment.android.guartinel.ui.accountExpired;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.ui.presenterCallbacks.ActivateAccountResultCallback;
import sysment.android.guartinel.ui.presenterCallbacks.LoginDeviceResultCallback;

public class AccountExpiredPresenter implements LoginDeviceResultCallback, ActivateAccountResultCallback {
    private AccountExpiredView _view;
    private boolean _isForced = false;

    public void onDestroy() {
        _view = null;
    }

    public AccountExpiredPresenter(AccountExpiredView view) {
        _view = view;
    }

    public void retryLogin() {
        GuartinelApp.getManagementServer().deviceAndroidLogin(
                GuartinelApp.getDataStore().getEmail(),
                GuartinelApp.getDataStore().getSaltedPasswordHash(),
                SystemInfoUtil.getHashedAndroidID(),
                GuartinelApp.getDataStore().getGcmToken(), this);
    }

    public void resendActivationCode() {
        GuartinelApp.getManagementServer().accountResendActivationCode(
                GuartinelApp.getDataStore().getEmail(), this);
    }

    @Override
    public void onConnectionError() {
        _view.onConnectionError();
    }

    @Override
    public void onDeviceNotRegistered() {
        _view.openRegisterDeviceActivity(_view.getSuperContext());
    }

    @Override
    public void onAccountExpired() {
        _view.onStillNotActivated();
    }

    @Override
    public void onInvalidUserNameOrPassword() {
        _view.openLoginAccountActivity(_view.getSuperContext());
    }

    @Override
    public void onLoginSuccess(String token, String deviceName) {
        GuartinelApp.getDataStore().setToken(token);
        GuartinelApp.getDataStore().setDeviceName(deviceName);
        _view.openMainActivity(_view.getSuperContext());
    }

    @Override
    public void onUpdateNow() {
        _view.onUpdateNow();
    }

    @Override
    public void onOneHourNotElapsedSinceLastSend() {
        _view.onOneHourNotElapsedSinceLastSend();
    }

    @Override
    public void onResendActivationCodeSuccess() {
        _view.onResendActivationCodeSuccess();
    }
}
