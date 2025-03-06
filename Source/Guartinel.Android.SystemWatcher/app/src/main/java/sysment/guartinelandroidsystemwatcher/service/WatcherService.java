package sysment.guartinelandroidsystemwatcher.service;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;

import com.firebase.jobdispatcher.JobParameters;
import com.firebase.jobdispatcher.JobService;

import org.joda.time.DateTime;

import sysment.guartinelandroidsystemwatcher.SystemWatcherApp;
import sysment.guartinelandroidsystemwatcher.connection.HTTPInterface;
import sysment.guartinelandroidsystemwatcher.connection.actions.CheckIfGoogleIsAvailableAction;
import sysment.guartinelandroidsystemwatcher.connection.actions.GetManagementServerStatusAction;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstance;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstances;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerStatus;
import sysment.guartinelandroidsystemwatcher.ui.busEvents.ServerInstanceConfiguredEvent;
import sysment.guartinelandroidsystemwatcher.ui.notification.NotificationHandler;
import sysment.guartinelandroidsystemwatcher.util.Tools;

public class WatcherService extends JobService {

    /**
     * This asynctask will run a job once conditions are met with the constraints
     * As soon as user device gets connected with the power supply. it will generate
     * a notification showing that condition is met.
     */
    private AsyncTask mBackgroundTask;

    @Override
    public boolean onStartJob(final JobParameters jobParameters) {
        mBackgroundTask = new AsyncTask() {
            @Override
            protected Object doInBackground(Object[] objects) {
                Context context = WatcherService.this;
                doWatching(context);
                return null;
            }

            @Override
            protected void onPostExecute(Object o) {
                /* false means, that job is done. we don't want to reschedule it*/
                jobFinished(jobParameters, true);
                Log.i("TAG", "onStartJob- OnPost");
            }
        };

        mBackgroundTask.execute();
        return true;
    }

    @Override
    public boolean onStopJob(JobParameters jobParameters) {
        if (mBackgroundTask != null) {
            mBackgroundTask.cancel(true);
        }
        Log.i("TAG", "onStopJob");
        /* true means, we're not done, please reschedule */
        return true;
    }


    public void doWatching(Context context) {
        new ServerChecker().check(context);
    }
}