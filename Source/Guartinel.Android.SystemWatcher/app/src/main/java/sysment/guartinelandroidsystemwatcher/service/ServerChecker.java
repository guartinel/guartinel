package sysment.guartinelandroidsystemwatcher.service;

import android.content.Context;

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

/**
 * Created by moqs_the_one on 2017.09.22..
 */

public class ServerChecker {

    public void check(Context context){
        //NotificationHandler.checkedServer(context);
        ServerInstances serverInstances = SystemWatcherApp.getDataStore().getServerInstances(context);
        HTTPInterface _httpInterFace = new HTTPInterface();


        for (ServerInstance instance : serverInstances) {
            if (!instance.isEnabled()) {
                continue;
            }
            if (!Tools.isNetworkAvailable(context)) {
                CheckIfGoogleIsAvailableAction checkIfGoogleIsAvailableAction = new CheckIfGoogleIsAvailableAction(_httpInterFace);
                checkIfGoogleIsAvailableAction.execute();
                if (!checkIfGoogleIsAvailableAction.isSuccessful()) {
                    instance.setStatus(new ServerStatus().setNoInternet());
                    instance.setLastChecked(new DateTime().toString());
                    SystemWatcherApp.getBus().post(new ServerInstanceConfiguredEvent(context, instance));
                    continue;
                }
            }

            GetManagementServerStatusAction action = new GetManagementServerStatusAction(_httpInterFace, instance.getAddress(), instance.getToken());
            action.execute();
            if(!action.isSuccessful()){ // retry just in case..
                action =  new GetManagementServerStatusAction(_httpInterFace, instance.getAddress(), instance.getToken());
                action.execute();
            }
            if (action.isSuccessful()) {
                instance.setStatus(new ServerStatus().setOK(action.getStatus()));
                instance.setLastSeen(new DateTime().toString());
            } else {
                if (!instance.getStatus().getStatus().equals(ServerStatus.STATUS.ERROR)) {//only show the error when status is changing
                    NotificationHandler.showServerNotAvailableNotification(context, instance);
                }
                instance.setStatus(new ServerStatus().setError(action.getStatus()));
            }
            instance.setLastChecked(new DateTime().toString());
            SystemWatcherApp.getBus().post(new ServerInstanceConfiguredEvent(context, instance));
        }
    }
}
