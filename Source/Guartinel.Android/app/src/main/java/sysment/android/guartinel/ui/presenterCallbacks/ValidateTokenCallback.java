package sysment.android.guartinel.ui.presenterCallbacks;

public interface ValidateTokenCallback extends PresenterResultCallback{
    void onInvalidToken();
    void onSuccess();
}
