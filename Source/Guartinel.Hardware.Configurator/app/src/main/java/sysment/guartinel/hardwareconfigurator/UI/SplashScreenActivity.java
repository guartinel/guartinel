package sysment.guartinel.hardwareconfigurator.ui;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.design.widget.CoordinatorLayout;
import android.support.v7.app.AppCompatActivity;

import com.splunk.mint.Mint;

import sysment.guartinel.hardwareconfigurator.R;
import sysment.guartinel.hardwareconfigurator.connection.WifiHelper;

/**
 * Created by sysment_dev on 02/22/2016.
 */
public class SplashScreenActivity extends AppCompatActivity {
    private static int SPLASH_TIME_OUT = 1000;
    private CoordinatorLayout _coordinatorLayout;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.splash_screen_activity);
        _coordinatorLayout = (CoordinatorLayout) findViewById(R.id.splash_coordinator_layout);

        new Handler().postDelayed(new Runnable() {
            @Override
            public void run() {
                Intent startMain = new Intent(SplashScreenActivity.this, MainActivity.class);
                startMain.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                startMain.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
                startActivity(startMain);
            }
        }, SPLASH_TIME_OUT);
    }

}
