package sysment.guartinelandroidsystemwatcher.ui.busEvents;

import android.content.Context;

import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstances;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.Settings;

/**
 * Created by moqs_the_one on 2017.08.16..
 */

public class SettingsChangedEvent {
    public Settings settings;
    public Context context;

    public SettingsChangedEvent(Context context, Settings settings) {
        this.settings = settings;
        this.context = context;
    }
}
