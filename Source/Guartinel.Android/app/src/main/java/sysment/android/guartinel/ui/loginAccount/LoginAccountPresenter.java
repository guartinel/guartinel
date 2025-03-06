package sysment.android.guartinel.ui.loginAccount;

import android.view.View;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.utils.StringUtility;
import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.ui.presenterCallbacks.LoginDeviceResultCallback;

public class LoginAccountPresenter implements LoginDeviceResultCallback {
    private LoginAccountView _loginAccountView;

    public void onDestroy() {
        _loginAccountView = null;
    }

    public LoginAccountPresenter(LoginAccountView loginAccountView) {
        _loginAccountView = loginAccountView;
    }

    public void loginDevice(String email, String password) {
        GuartinelApp.getManagementServer().deviceAndroidLogin(
                email,
                StringUtility.getHashWithSalt(password, email),
                SystemInfoUtil.getHashedAndroidID(),
                GuartinelApp.getDataStore().getGcmToken(),
                this);
    }


    @Override
    public void onConnectionError() {
        _loginAccountView.onConnectionError(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

            }
        });
    }

    @Override
    public void onDeviceNotRegistered() {
        LOG.I("LoginAccountPresenter.onDeviceNotRegistered");

        GuartinelApp.getDataStore().setPlainPassword(_loginAccountView.getPassword());
        GuartinelApp.getDataStore().setEmail(_loginAccountView.getEmail());
       _loginAccountView.openRegisterDeviceActivity(_loginAccountView.getSuperContext());
    }

    @Override
    public void onAccountExpired() {
        _loginAccountView.onAccountExpired();
    }

    @Override
    public void onInvalidUserNameOrPassword() {
        _loginAccountView.invalidUserNameOrPassword();

    }

    @Override
    public void onLoginSuccess(String token, String deviceName) {
        LOG.I("LoginAccountPresenter.onLoginSuccess");
        GuartinelApp.getDataStore().setPlainPassword(_loginAccountView.getPassword());
        GuartinelApp.getDataStore().setEmail(_loginAccountView.getEmail());
        GuartinelApp.getDataStore().setToken(token);
        GuartinelApp.getDataStore().setDeviceName(deviceName);
        _loginAccountView.openMainActivity(_loginAccountView.getSuperContext());
    }

    @Override
    public void onUpdateNow() {
        _loginAccountView.showUpdateNow();
    }

}
