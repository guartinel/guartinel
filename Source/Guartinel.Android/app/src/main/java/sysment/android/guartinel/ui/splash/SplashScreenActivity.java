package sysment.android.guartinel.ui.splash;

import android.os.Bundle;
import android.os.Handler;
import android.support.design.widget.CoordinatorLayout;
import android.view.View;
import android.view.WindowManager;


import sysment.android.guartinel.R;

import sysment.android.guartinel.ui.DialogManager;
import sysment.android.guartinel.ui.MyNotificationManager;
import sysment.android.guartinel.ui.SuperActivity;
import sysment.android.guartinel.ui.accountExpired.AccountExpiredActivity;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public class SplashScreenActivity extends SuperActivity implements SplashView {
    private static int SPLASH_TIME_OUT = 1000;
    private SplashPresenter _splashPresenter;
    private CoordinatorLayout _coordinatorLayout;

    public boolean isFromNotification() {
        return isFromNotification;
    }

    public void setFromNotification(boolean fromNotification) {
        isFromNotification = fromNotification;
    }

    public String getAlertMessage() {
        return alertMessage;
    }

    public void setAlertMessage(String alertMessage) {
        this.alertMessage = alertMessage;
    }

    public int getNotificationID() {
        return notificationID;
    }

    @Override
    public SplashScreenActivity getActivity() {
        return this;
    }

    @Override
    public void onUpdateNow() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                DialogManager.showUpdateNeededDialog(SplashScreenActivity.this);
            }
        });
    }

    public void setNotificationID(int notificationID) {
        this.notificationID = notificationID;
    }

    protected boolean isFromNotification = false;
    protected String alertMessage ;
    protected  int notificationID;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.splash_screen_activity);
        _splashPresenter = new SplashPresenter(this);
        _coordinatorLayout = (CoordinatorLayout) findViewById(R.id.splash_coordinator_layout);

          if(getIntent().getExtras()!= null && getIntent().getExtras().getBoolean( MyNotificationManager.Constants.IS_FORCED_ALERT)){
            setFromNotification(true);
            setAlertMessage(getIntent().getExtras().getString(MyNotificationManager.Constants.MESSAGE));
           setNotificationID(getIntent().getExtras().getInt(MyNotificationManager.Constants.NOTIFICATION_ID));
        }
       new Handler().postDelayed(new Runnable() {
            @Override
            public void run() {
                _splashPresenter.afterViewInit();
            }
        }, SPLASH_TIME_OUT);
    }

    @Override
    protected void onDestroy() {
        _splashPresenter.onDestroy();
        super.onDestroy();
    }

    @Override
    public void showConnectionError() {
        String text = getResources().getString(R.string.connection_error);
        String actionText = getResources().getString(R.string.try_again);
        showSnackBarError(_coordinatorLayout, text, actionText, new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                _splashPresenter.afterViewInit();
            }
        });
    }


}
