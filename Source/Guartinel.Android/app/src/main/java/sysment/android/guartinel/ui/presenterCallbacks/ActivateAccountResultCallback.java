package sysment.android.guartinel.ui.presenterCallbacks;

public interface ActivateAccountResultCallback extends PresenterResultCallback {
    void onOneHourNotElapsedSinceLastSend();
    void onResendActivationCodeSuccess();
}
