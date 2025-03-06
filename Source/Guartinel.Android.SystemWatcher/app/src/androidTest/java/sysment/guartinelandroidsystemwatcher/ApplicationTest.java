package sysment.guartinelandroidsystemwatcher;

import android.app.Application;
import android.test.ApplicationTestCase;

import java.util.ArrayList;

import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstance;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.Settings;
import sysment.guartinelandroidsystemwatcher.persistance.DataStore;

/**
 * <a href="http://d.android.com/tools/testing/testing_android.html">Testing Fundamentals</a>
 */
public class ApplicationTest extends ApplicationTestCase<Application> {
    public ApplicationTest() {
        super(Application.class);

        ArrayList<ServerInstance> servers = new ArrayList<ServerInstance>();
        ServerInstance server = new ServerInstance("addressv","tokenv","vname","descriptionv",true);
        servers.add(server);
        Settings settings = new Settings(servers,false,false,10);
        DataStore.saveSettings(this.getContext(),settings);
        Settings reloaded = DataStore.loadSettings(this.getContext());

    }
}