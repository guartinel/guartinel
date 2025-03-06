package sysment.android.guartinel.ui.createAccount;

import sysment.android.guartinel.ui.SuperView;

public interface CreateAccountView extends SuperView {
    void onEmailAlreadyTaken();
    String getPassword();

    String getEmail();

    void onConnectionError();
}
