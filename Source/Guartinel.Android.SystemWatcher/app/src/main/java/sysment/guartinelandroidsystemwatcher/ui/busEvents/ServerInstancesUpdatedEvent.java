package sysment.guartinelandroidsystemwatcher.ui.busEvents;

import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstance;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstances;

/**
 * Created by moqs_the_one on 2017.08.15..
 */

public class ServerInstancesUpdatedEvent {

    public ServerInstances serverInstances;

    public ServerInstancesUpdatedEvent(ServerInstances serverInstances) {
        this.serverInstances = serverInstances;
    }
}
