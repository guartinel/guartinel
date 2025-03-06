package sysment.android.guartinel.ui.presenterCallbacks;

public interface AccountCreateResultCallback extends PresenterResultCallback {
    void onEmailAlreadyRegistered();
    void onCreateSuccess();
}
