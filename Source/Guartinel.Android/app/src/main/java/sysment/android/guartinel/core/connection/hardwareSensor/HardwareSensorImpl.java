package sysment.android.guartinel.core.connection.hardwareSensor;

import android.content.Context;
import android.net.wifi.ScanResult;

import sysment.android.guartinel.core.connection.hardwareSensor.helloGuartinel.HelloGuartinelRequest;
import sysment.android.guartinel.core.utils.GeneralUtil;
import sysment.android.guartinel.core.connection.HttpConnector;
import sysment.android.guartinel.core.connection.hardwareSensor.setConfig.SetConfigRequest;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.network.WifiConnectionManager;
import sysment.android.guartinel.core.network.WifiUtility;
import sysment.android.guartinel.core.utils.StringUtility;
import sysment.android.guartinel.ui.presenterCallbacks.PresenterResultCallback;

public class HardwareSensorImpl implements IHardwareSensor {
    public static class Constants{
        public static String SENSOR_SSID_PREFIX = "Guartinel-";
    }
    private ScanResult _scanResult;
    public boolean isSelected = true;
    private String _name;
    private String _hardwareType;
    private String _updateServerProtocolPrefix;
    private int _updateServerPort;
    private String _updateServerHost;
    private boolean _isError = false;
    private String _errorMessage;
    private boolean _isConfigured = false;

    public void setIsConfigured(boolean val){
        _isConfigured = val;
    }

    public boolean isConfigured(){
        return  _isConfigured;
    }
    public boolean isError() {
        return _isError;
    }

    public void setIsError(boolean _isError) {
        this._isError = _isError;
    }

    public String getErrorMessage() {
        return _errorMessage;
    }

    public void setErrorMessage(String _errorMessage) {
        this._errorMessage = _errorMessage;
    }


    public String getHardwareType() {
        return _hardwareType;
    }

    public void setHardwareType(String _hardwareType) {
        this._hardwareType = _hardwareType;
    }

    public String getUpdateServerProtocolPrefix() {
        return _updateServerProtocolPrefix;
    }

    public void setUpdateServerProtocolPrefix(String _updateServerProtocolPrefix) {
        this._updateServerProtocolPrefix = _updateServerProtocolPrefix;
    }

    public int getUpdateServerPort() {
        return _updateServerPort;
    }

    public void setUpdateServerPort(int _updateServerPort) {
        this._updateServerPort = _updateServerPort;
    }

    public String getUpdateServerHost() {
        return _updateServerHost;
    }

    public void setUpdateServerHost(String _updateServerHost) {
        this._updateServerHost = _updateServerHost;
    }

    private String _originalName;
    private String _id;

    public HardwareSensorImpl(ScanResult scanResult) {
        this._scanResult = scanResult;
        this._name = _scanResult.SSID;
        this._originalName = this._name;
    }

    @Override
    public void helloGuartinel(final Context context, final HttpConnector connector, final PresenterResultCallback callback) {
        WifiConnectionManager.connect(context, getAccessPointSSID(), getAccessPointPassword(), new WifiUtility.WifiConnectionResult() {
            @Override
            public void onConnected(boolean hasInternet) {
                LOG.I("HardwareSensorImpl.helloGuartinel.onConnected");
                HelloGuartinelRequest request = new HelloGuartinelRequest(callback);
                connector.makePostOnWifi(context,getAddress() + request.getURL(), request);
            }

            @Override
            public void onCannotConnect() {
                LOG.I("HardwareSensorImpl.helloGuartinel.onConnected");
                callback.onConnectionError();
            }
        });
    }

    @Override
    public void setConfig(final Context context, final HttpConnector connector, final String wifiSSID, final String wifiPassword, final String updateServerHost, final String updateServerProtocolPrefix, final int updateServerPort, final String hardwareType, final String instanceName, final PresenterResultCallback callback) {
        WifiConnectionManager.connect(context, getAccessPointSSID(), getAccessPointPassword(), new WifiUtility.WifiConnectionResult() {
            @Override
            public void onConnected(boolean hasInternet) {
                LOG.I("HardwareSensorImpl.setConfig.onConnected");
                SetConfigRequest request = new SetConfigRequest(wifiSSID, wifiPassword, updateServerHost, updateServerProtocolPrefix, updateServerPort, hardwareType, instanceName, callback);
                connector.makePostOnWifi(context,getAddress() + request.getURL(), request);
            }

            @Override
            public void onCannotConnect() {
                LOG.I("HardwareSensorImpl.setConfig.onCannotConnect");
                callback.onConnectionError();
            }
        });
    }

    @Override
    public void getDiagnostics(Context context, HttpConnector connector, String alertId, PresenterResultCallback callback) {
    }

    public String getAccessPointSSID() {
        return _scanResult.SSID;
    }

    public String getAccessPointPassword() {
        String ssid = _scanResult.SSID;//.toLowerCase();
        ssid = ssid.replace("Guartinel-", "");
        return StringUtility.getHash(ssid);
    }

    public void setName(String name) {
        this._name = name;
    }

    public String getName() {
        return _name;
    }

    public String getOriginalName() {
        return _originalName;
    }

    private String getAddress() {
        return "http://192.168.8.1/";// this is default address of the hardware in AP mode
    }

    public String getId() {
        return _id;
    }

    public void setId(String id) {
        this._id = id;
    }
}
