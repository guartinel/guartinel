package sysment.android.guartinel.core.utils;

import android.content.Context;
import android.net.wifi.ScanResult;
import java.util.ArrayList;
import java.util.List;
import sysment.android.guartinel.core.connection.hardwareSensor.HardwareSensorImpl;
import sysment.android.guartinel.core.network.WifiUtility;

public class HardwareSensorSniffer {
    public interface onSniffingDone {
        void done(List<HardwareSensorImpl> foundItems);
        void onWIfiDisabled();
    }


    public static void sniff(final Context context, final onSniffingDone onDone) {
        WifiUtility.scanAvailableNetworks(context, new WifiUtility.OnScanFinished() {
            @Override
            public void wifiDisabled() {
                onDone.onWIfiDisabled();
            }

            @Override
            public void finished(final List<ScanResult> wifiScanResult) {
                List<HardwareSensorImpl> foundArduinos = new ArrayList<HardwareSensorImpl>();
                for (ScanResult scan : wifiScanResult) {
                    if (scan.SSID.contains(HardwareSensorImpl.Constants.SENSOR_SSID_PREFIX)) {
                        foundArduinos.add(new HardwareSensorImpl(scan));
                    }
                }
                onDone.done(foundArduinos);
            }
        });
    }
}
