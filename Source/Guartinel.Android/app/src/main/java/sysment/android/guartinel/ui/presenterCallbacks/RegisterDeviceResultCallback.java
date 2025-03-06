package sysment.android.guartinel.ui.presenterCallbacks;

public interface RegisterDeviceResultCallback extends  PresenterResultCallback {
    void onSuccess(String token);
    void onAccountExpired();
    void onInvalidUserNameOrPassword();
    void onDeviceNameAlreadyTaken();
}
