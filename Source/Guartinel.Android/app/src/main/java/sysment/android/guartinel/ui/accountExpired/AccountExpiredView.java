package sysment.android.guartinel.ui.accountExpired;

import sysment.android.guartinel.ui.SuperView;

public interface AccountExpiredView extends SuperView {
    void onStillNotActivated();
    void onConnectionError();
    void onOneHourNotElapsedSinceLastSend();
    void onResendActivationCodeSuccess();
    void onUpdateNow();
}
