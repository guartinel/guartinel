package sysment.guartinelandroidsystemwatcher.service;

import android.content.Context;

import com.firebase.jobdispatcher.Constraint;
import com.firebase.jobdispatcher.FirebaseJobDispatcher;
import com.firebase.jobdispatcher.GooglePlayDriver;
import com.firebase.jobdispatcher.Job;
import com.firebase.jobdispatcher.Lifetime;
import com.firebase.jobdispatcher.RetryStrategy;
import com.firebase.jobdispatcher.Trigger;
import com.squareup.otto.Subscribe;

import sysment.guartinelandroidsystemwatcher.SystemWatcherApp;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.Settings;
import sysment.guartinelandroidsystemwatcher.ui.busEvents.SettingsChangedEvent;

/**
 * Created by moqs_the_one on 2017.07.31..
 */

public class WatcherMaster {
    private static final String SYSTEM_WATCHER_TAG = "SYSTEM_WATCHER_TAG";
    private static final String INTERNET_CHECKING_SERVICE_TAG = "INTERNET_CHECKING_SERVICE_TAG";
    FirebaseJobDispatcher dispatcher;

    public void init(Context context) {

        dispatcher = new FirebaseJobDispatcher(new GooglePlayDriver(context));
        SystemWatcherApp.getBus().register(this);


        Settings settings = SystemWatcherApp.getDataStore().getSettings(context);
        if (settings.isWatchingEnabled()) {
            startServerWatching(context, settings.getCheckIntervalSec());
        } else {
            dispatcher.cancel(SYSTEM_WATCHER_TAG);
        }
        if (settings.isInternetCheckEnabled()) {
            startInternetWatching(context, settings.getNoInternetCheckIntervalInSec());
        } else {
            dispatcher.cancel(INTERNET_CHECKING_SERVICE_TAG);
        }

    }

    @Subscribe
    public void onSettingsChanged(SettingsChangedEvent event) {
        if (!event.settings.isInternetCheckEnabled()) {
            dispatcher.cancel(INTERNET_CHECKING_SERVICE_TAG);
        } else {
            startInternetWatching(event.context, event.settings.getNoInternetCheckIntervalInSec());
        }
        if (!event.settings.isWatchingEnabled()) {
            dispatcher.cancel(SYSTEM_WATCHER_TAG);
        } else {
            startServerWatching(event.context, event.settings.getCheckIntervalSec());
        }

    }

    private int getIntervalWindowStart(int startSec) {
        return (int) ((double) startSec - (startSec * 0.2));
    }

    public void startServerWatching(Context context, int intervalSec) {
        Job myJob = dispatcher.newJobBuilder()
                // the JobService that will be called
                .setService(WatcherService.class)
                // uniquely identifies the job
                .setTag(SYSTEM_WATCHER_TAG)
                // one-off job
                .setRecurring(true)
                // don't persist past a device reboot
                .setLifetime(Lifetime.FOREVER)
                // start between 0 and 60 seconds from now
                .setTrigger(Trigger.executionWindow(getIntervalWindowStart(intervalSec), intervalSec))
                // don't overwrite an existing job with the same tag
                .setReplaceCurrent(true)
                // retry with exponential backoff
                .setRetryStrategy(RetryStrategy.DEFAULT_EXPONENTIAL)
                // constraints that need to be satisfied for the job to run
                .build();

        dispatcher.mustSchedule(myJob);
    }

    public void startInternetWatching(Context context, int intervalSec) {
        Job myJob = dispatcher.newJobBuilder()
                // the JobService that will be called
                .setService(NoInternetService.class)
                // uniquely identifies the job
                .setTag(INTERNET_CHECKING_SERVICE_TAG)
                // one-off job
                .setRecurring(true)
                // don't persist past a device reboot
                .setLifetime(Lifetime.FOREVER)
                // start between 0 and 60 seconds from now
                .setTrigger(Trigger.executionWindow(getIntervalWindowStart(intervalSec), intervalSec))
                // don't overwrite an existing job with the same tag
                .setReplaceCurrent(true)
                // retry with exponential backoff
                .setRetryStrategy(RetryStrategy.DEFAULT_LINEAR)
                // constraints that need to be satisfied for the job to run
                .build();

        dispatcher.mustSchedule(myJob);
    }
}
