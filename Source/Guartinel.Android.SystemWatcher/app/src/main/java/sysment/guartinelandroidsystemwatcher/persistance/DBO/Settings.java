package sysment.guartinelandroidsystemwatcher.persistance.DBO;

import org.joda.time.DateTime;

/**
 * Created by moqs_the_one on 2017.07.28..
 */

public class Settings {

    public Settings( boolean isWatchingEnabled, boolean isInternetCheckEnabled, int checkIntervalSec) {
        this.isWatchingEnabled = isWatchingEnabled;
        this.isInternetCheckEnabled = isInternetCheckEnabled;
        this.checkIntervalSec = checkIntervalSec;
    }

    public Settings(){}

    public boolean isWatchingEnabled() {
        return isWatchingEnabled;
    }

    public boolean isInternetCheckEnabled() {
        return isInternetCheckEnabled;
    }

    public int getCheckIntervalSec() {
        return checkIntervalSec;
    }

   private boolean isWatchingEnabled = false;
    private boolean isInternetCheckEnabled = false;
    private int checkIntervalSec = 5;
    private int noInternetCheckIntervalInSec = 5;
    private String lastSeenInternet = new DateTime().toString();

    public int getNoInternetCheckIntervalInSec() {
        return noInternetCheckIntervalInSec;
    }

    public void setNoInternetCheckIntervalInSec(int noInternetCheckIntervalInSec) {
        this.noInternetCheckIntervalInSec = noInternetCheckIntervalInSec;
    }


    public void setWatchingEnabled(boolean watchingEnabled) {
        isWatchingEnabled = watchingEnabled;
    }

    public void setInternetCheckEnabled(boolean internetCheckEnabled) {
        isInternetCheckEnabled = internetCheckEnabled;
    }

    public void setCheckIntervalSec(int checkIntervalSec) {
        this.checkIntervalSec = checkIntervalSec;
    }

    public void setLastSeenInternet(String lastSeenInternet) {
        this.lastSeenInternet = lastSeenInternet;
    }

    public String getLastSeenInternet() {
        return lastSeenInternet;
    }
}
