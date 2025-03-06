package sysment.guartinelandroidsystemwatcher.ui.busEvents;

import android.content.Context;

import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstance;

/**
 * Created by moqs_the_one on 2017.08.15..
 */

public class ServerInstanceConfiguredEvent {
    public ServerInstance instance;
    public Context context;

    public ServerInstanceConfiguredEvent(Context context, ServerInstance instance) {
        this.instance = instance;
        this.context = context;
    }
}
