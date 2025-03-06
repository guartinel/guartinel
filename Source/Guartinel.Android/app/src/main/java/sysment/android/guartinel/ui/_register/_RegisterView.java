package sysment.android.guartinel.ui._register;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public interface _RegisterView {

    void setInvalidUserNameOrPasswordError();

    void setConnectionError();

    void setDeviceNameError();

    void doOnUIThread(Runnable task);

    void openMainActivity();

    void setCommunicationError();

    String getEmail();

    String getPassword();

    String getDeviceName();

    void setAccountExpiredError();

    void setMaximumDeviceCountReachedError();
}
