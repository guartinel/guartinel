//package sysment.android.guartinel.ui._login;
//
//import org.json.JSONException;
//import sysment.android.guartinel.core.connection.HttpResponse;
//import sysment.android.guartinel.core.utils.GeneralUtil;
//import sysment.android.guartinel.GuartinelApp;
//import sysment.android.guartinel.core.utils.LOG;
//import sysment.android.guartinel.ui.presenterCallbacks._LoginResultCallback;
//
///**
// * Created by sysment_dev on 02/23/2016.
// */
//public class _LoginPresenter implements _LoginResultCallback {
//    _LoginView _loginView;
//    String deviceUUID;
//    String email;
//    String passwordHash;
//
//    public _LoginPresenter(_LoginView view) {
//        _loginView = view;
//    }
//
//    public void onDestroy() {
//        _loginView = null;
//    }
//
//    public void initLogin() {
//        deviceUUID = GuartinelApp.getDataStore().getDeviceUUID();
//        email = _loginView.getEmail();
//        passwordHash = GeneralUtil.getGuartinelAccountPassword(deviceUUID);
//       String gcmToken = GuartinelApp.getDataStore().getGcmToken();
//        try {
//            GuartinelApp.getManagementServer()._deviceLogin( passwordHash, deviceUUID,gcmToken, this);
//        } catch (JSONException e) {
//            _loginView.doOnUIThread(new Runnable() {
//                @Override
//                public void run() {
//                    _loginView.setConnectionError();
//                }
//            });
//        }
//    }
//
//    @Override
//    public void onConnectionError() {
//        _loginView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//                _loginView.setConnectionError();
//            }
//        });
//    }
//
//    @Override
//    public void onLoginSuccess(final HttpResponse response) {
//        _loginView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//               // GuartinelApp.getDataStore().updateEmailPassword(email, passwordHash); TODO handle this
//                _loginView.openMainActivity();
//            }
//        });
//    }
//
//    @Override
//    public void onAccountExpired() {
//        _loginView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//                _loginView.setAccountExpiredError();
//            }
//        });
//    }
//
//    @Override
//    public void onInvalidUserNameOrPasswordError() {
//        _loginView.doOnUIThread(new Runnable() {
//            @Override
//            public void run() {
//                _loginView.setInvalidUserNameOrPasswordError();
//            }
//        });
//    }
//
//    @Override
//    public void onDeviceUUIDError() {
//        LOG.I("Device UUID is incorrect.");
//    }
//}
