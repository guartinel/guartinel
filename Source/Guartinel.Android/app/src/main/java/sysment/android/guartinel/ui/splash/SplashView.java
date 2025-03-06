package sysment.android.guartinel.ui.splash;

import sysment.android.guartinel.ui.SuperView;

/**
 * Created by sysment_dev on 02/23/2016.
 */
public interface SplashView extends SuperView {
    void showConnectionError();

    boolean isFromNotification();

    String getAlertMessage();

    int getNotificationID();
    SplashScreenActivity getActivity();

    void onUpdateNow();
}
