package sysment.android.guartinel.ui.registerDevice;

public interface RegisterDeviceView {
    void deviceNameAlreadyTaken();
    void invalidUserNameOrPassword();
    void onAccountExpired();
    void openDashboard();
    String getDeviceName();
}
