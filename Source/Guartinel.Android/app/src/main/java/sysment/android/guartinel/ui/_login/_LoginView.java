package sysment.android.guartinel.ui._login;

/**
 * Created by sysment_dev on 02/23/2016.
 */
public interface _LoginView {
    void doOnUIThread(Runnable task);

    String getEmail();

    String getPassword();

    void setConnectionError();

    void openMainActivity();
    void setInvalidUserNameOrPasswordError();

    void setAccountExpiredError();
}
