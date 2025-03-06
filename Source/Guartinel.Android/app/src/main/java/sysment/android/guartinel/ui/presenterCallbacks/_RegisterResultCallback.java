package sysment.android.guartinel.ui.presenterCallbacks;

import sysment.android.guartinel.core.connection.HttpResponse;

/**
 * Created by sysment_dev on 02/23/2016.
 */

public interface _RegisterResultCallback extends PresenterResultCallback {
    void onDeviceNameError();
    void onInvalidUserNameOrPasswordError();
    void onConnectionError();
     void onRegisterSuccess(HttpResponse response);
    void onAccountExpired();
    void onMaximumDeviceCountReached();
}
