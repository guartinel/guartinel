package sysment.guartinel.hardwareconfigurator.models;

import android.net.wifi.ScanResult;

/**
 * Created by DAVT on 2017.12.06..
 */

public class GuartinelHardwareAP extends GuartinelHardware {

    public ScanResult scan;

    public GuartinelHardwareAP(ScanResult scan) {
        this.scan = scan;
    }

    @Override
    public String getName() {
        return scan.SSID;
    }

    public String getAPPassword() {
        String ssid = scan.SSID.toLowerCase();
        ssid = ssid.replace("guartinel_", "");
        return ssid + ssid;
    }

    @Override
    public String getAddress() {
        return "http://192.168.8.1/";// this is default address of the hardware in AP mode
    }

}
