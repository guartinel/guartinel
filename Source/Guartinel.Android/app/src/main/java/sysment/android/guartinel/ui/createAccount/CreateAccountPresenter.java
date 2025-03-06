package sysment.android.guartinel.ui.createAccount;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.utils.StringUtility;
import sysment.android.guartinel.ui.presenterCallbacks.AccountCreateResultCallback;

public class CreateAccountPresenter implements AccountCreateResultCallback {
    private CreateAccountView _registerView;

    public void onDestroy() {
        _registerView = null;
    }

    public CreateAccountPresenter(CreateAccountView registerView) {
        _registerView = registerView;
    }

    public void registerAccount(String firstName, String lastName, String email, String password) {
        GuartinelApp.getManagementServer().accountCreate(
                firstName,
                lastName,
                email,
                StringUtility.getHashWithSalt(password,email), this);
    }

    @Override
    public void onEmailAlreadyRegistered() {
        _registerView.onEmailAlreadyTaken();
    }

    @Override
    public void onCreateSuccess() {
        LOG.I("CreateAccountPresenter.onCreateSuccess");
        GuartinelApp.getDataStore().setEmail(_registerView.getEmail());
        GuartinelApp.getDataStore().setPlainPassword(_registerView.getPassword());
        _registerView.openAccountExpiredActivity(_registerView.getSuperContext());
    }

    @Override
    public void onConnectionError() {
        _registerView.onConnectionError();
    }
}
