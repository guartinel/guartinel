package sysment.android.guartinel.ui.loginAccount;

import android.view.View;

import sysment.android.guartinel.ui.SuperView;

public interface LoginAccountView extends SuperView {
    void invalidUserNameOrPassword();

    void onAccountExpired();

    void onConnectionError(View.OnClickListener listener);
    String getPassword();

    String getEmail();

    void showUpdateNow();
}
