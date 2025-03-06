package sysment.android.guartinel.ui.presenterCallbacks;

import sysment.android.guartinel.core.connection.HttpResponse;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public interface _LoginResultCallback extends PresenterResultCallback {
    void onInvalidUserNameOrPasswordError();
    void onDeviceUUIDError();
    void onConnectionError();
    void onLoginSuccess(HttpResponse response);
    void onAccountExpired();
}
