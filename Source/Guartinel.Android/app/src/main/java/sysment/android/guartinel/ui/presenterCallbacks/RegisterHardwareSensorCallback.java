package sysment.android.guartinel.ui.presenterCallbacks;

public interface RegisterHardwareSensorCallback extends PresenterResultCallback {
    void onInvalidToken();
    void onSuccess(String updateServerHost, String updateServerProtocolPrefix, int updateServerPort, String hardwareType);
    void onInternalServerError();
}
