package sysment.guartinel.hardwareconfigurator.ui;

import android.app.Application;

import com.splunk.mint.Mint;

public class GuartinelConfiguratorApplication extends Application {
    @Override
    public void onCreate() {
        super.onCreate();
        Mint.initAndStartSession(this, "6ce59c92");
    }
}
