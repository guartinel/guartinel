//package sysment.android.guartinel.ui._register;
//
//import sysment.android.guartinel.core.gcm.GCMRegistrationResultReceiver;
//import sysment.android.guartinel.core.utils.GeneralUtil;
//import sysment.android.guartinel.core.utils.SystemInfoUtil;
//import sysment.android.guartinel.GuartinelApp;
//import sysment.android.guartinel.core.connection.managementServer.device._register._RegisterResponse;
//import sysment.android.guartinel.core.connection.HttpResponse;
//import sysment.android.guartinel.core.utils.LOG;
//import sysment.android.guartinel.ui.presenterCallbacks.GcmRegistrationCallback;
//import sysment.android.guartinel.ui.presenterCallbacks._RegisterResultCallback;
//
///**
// * Created by sysment_dev on 02/22/2016.
// */
//
//public class _RegisterPresenter implements  GcmRegistrationCallback, _RegisterResultCallback {
//    _RegisterView _registerView;
//    private boolean _isForced = false;
//    public void onDestroy() {
//        _registerView = null;
//    }
//
//    // ----------------------------------------- ACTIVITY --------------------------------------------------
//    public _RegisterPresenter(_RegisterView registerView) {
//        _registerView = registerView;
//    }
//
//    public void initRegister(boolean isForced) {
//        _isForced = isForced;
//        GuartinelApp.getDataStore().clearAlertHolder();
//        _registerView.startGcmRegistrationService(new GCMRegistrationResultReceiver(this));
//    }
//// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! ACTIVITY !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//
//    // ----------------------------------------- LOGIC --------------------------------------------------
//    @Override
//    public void onGCMRegistrationSuccess() {
//        startDeviceRegistration();
//    }
//
//    private void startDeviceRegistration() {
//            String gcmToken = GuartinelApp.getDataStore().getGcmToken();
//            String deviceName = _registerView.getDeviceName();
//            String email = _registerView.getEmail();
//            String passwordHash = GeneralUtil.generateHashFromString(_registerView.getPassword(), email);
//            String androidID = SystemInfoUtil.getAndroidID();
//            GuartinelApp.getManagementServer()._deviceRegister(email, passwordHash, deviceName, gcmToken,androidID,_isForced, this);
//    }
//
//    @Override
//    public void onRegisterSuccess(HttpResponse response) {
//        GuartinelApp.getDataStore().setToken(((_RegisterResponse) response).getToken());
//        GuartinelApp.getDataStore().setDeviceUUID(((_RegisterResponse) response).getDeviceUUID());
//        GuartinelApp.getDataStore().setDeviceName(_registerView.getDeviceName());
//        GuartinelApp.getDataStore().setEmail(_registerView.getEmail());
//        GuartinelApp.getDataStore().setUserPassword(_registerView.getPassword());
//        _registerView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//                _registerView.openMainActivity();
//            }
//        });
//    }
//
//    @Override
//    public void onGCMRegistrationError() {
//        LOG.I("RegisterPresenter.onGCMRegistrationError");
//        LOG.I("Cannot _deviceRegister GCM");
//        _registerView.setCommunicationError();
//    }
//// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! LOGIC !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//
//
//    /// --------------------------------------------- UI related ----------------------------------------
//    @Override
//    public void onInvalidUserNameOrPasswordError() {
//        _registerView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//                _registerView.setInvalidUserNameOrPasswordError();
//            }
//        });
//    }
//
//    @Override
//    public void onDeviceNameError() {
//        _registerView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//                _registerView.setDeviceNameError();
//            }
//        });
//    }
//
//    @Override
//    public void onAccountExpired() {
//        _registerView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//                _registerView.setAccountExpiredError();
//            }
//        });
//    }
//
//    @Override
//    public void onMaximumDeviceCountReached() {
//        _registerView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//                _registerView.setMaximumDeviceCountReachedError();
//            }
//        });
//    }
//
//
//    @Override
//    public void onConnectionError() {
//        _registerView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//                _registerView.setConnectionError();
//            }
//        });
//    }
////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! UI RELATED !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//
//
//}
