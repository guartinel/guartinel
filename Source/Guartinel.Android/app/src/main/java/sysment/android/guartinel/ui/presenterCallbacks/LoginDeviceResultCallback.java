package sysment.android.guartinel.ui.presenterCallbacks;

public interface LoginDeviceResultCallback extends PresenterResultCallback {
    void onDeviceNotRegistered();
    void onAccountExpired();
    void onInvalidUserNameOrPassword();
    void onLoginSuccess(String token, String deviceName);
    void onUpdateNow();
}
