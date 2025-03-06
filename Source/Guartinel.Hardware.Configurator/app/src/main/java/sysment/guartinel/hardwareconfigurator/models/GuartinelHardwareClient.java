package sysment.guartinel.hardwareconfigurator.models;

import java.net.InetAddress;

/**
 * Created by DAVT on 2017.12.06..
 */

public class GuartinelHardwareClient extends GuartinelHardware {
    String _host;
    InetAddress _inet;

    public GuartinelHardwareClient(String host, InetAddress inet) {
        this._host = host;
        this._inet = inet;
    }

    @Override
    public String getName() {
        return _host;
    }

    @Override
    public String getAddress() {
        return "http://"+ _inet.getHostAddress()+ "/";
    }
}
