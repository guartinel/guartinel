package sysment.android.guartinel.core.connection.hardwareSensor;

import android.content.Context;

import sysment.android.guartinel.core.connection.HttpConnector;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

public interface IHardwareSensor {
    void helloGuartinel(Context context, HttpConnector connector, PresenterResultCallback callback);

    void setConfig(Context context, HttpConnector connector, String wifiSSID, String wifiPassword, String updateServerHost, String updateServerProtocolPrefix, int updateServerPort, String hardwareType, String instanceName, PresenterResultCallback callback);

    void getDiagnostics(Context context, HttpConnector connector, String alertId, PresenterResultCallback callback);
}
