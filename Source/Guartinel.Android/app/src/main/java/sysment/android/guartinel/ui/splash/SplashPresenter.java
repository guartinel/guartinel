package sysment.android.guartinel.ui.splash;

import android.text.TextUtils;

import com.google.android.gms.tasks.OnSuccessListener;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.iid.InstanceIdResult;

import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.ui.presenterCallbacks.LoginDeviceResultCallback;

/**
 * Created by sysment_dev on 02/23/2016.
 */
public class SplashPresenter implements LoginDeviceResultCallback {//, GcmRegistrationCallback {
    SplashView _splashView;

    public SplashPresenter(SplashView view) {
        _splashView = view;
    }

    public void onDestroy() {
        _splashView = null;
    }

    public void afterViewInit() {

        FirebaseInstanceId.getInstance().getInstanceId().addOnSuccessListener(_splashView.getActivity(), new OnSuccessListener<InstanceIdResult>() {
            @Override
            public void onSuccess(InstanceIdResult instanceIdResult) {
                String token = instanceIdResult.getToken();
                LOG.I("  FirebaseInstanceId.getInstance().getInstanceId() result: " + token);
                GuartinelApp.getDataStore().setGCMToken(token);
            }
        });

        LOG.I("SplashPresenter.afterViewInit");
        if (TextUtils.isEmpty(GuartinelApp.getDataStore().getEmail()) || TextUtils.isEmpty(GuartinelApp.getDataStore().getSaltedPasswordHash())) {
            _splashView.openLoginAccountActivity(_splashView.getSuperContext());
            return;
        }

        GuartinelApp.getManagementServer().deviceAndroidLogin(
                GuartinelApp.getDataStore().getEmail(),
                GuartinelApp.getDataStore().getSaltedPasswordHash(),
                SystemInfoUtil.getHashedAndroidID(),
                GuartinelApp.getDataStore().getGcmToken(),
                this);
    }

    private boolean isViewNull() {
        return _splashView == null;
    }

    @Override
    public void onConnectionError() {
        if (isViewNull()) {
            return;
        }
        _splashView.showConnectionError();
    }

    @Override
    public void onDeviceNotRegistered() {
        _splashView.openRegisterDeviceActivity(_splashView.getSuperContext());
    }

    @Override
    public void onAccountExpired() {
        _splashView.openLoginAccountActivity(_splashView.getSuperContext());
    }

    @Override
    public void onInvalidUserNameOrPassword() {
        _splashView.openLoginAccountActivity(_splashView.getSuperContext());
    }

    @Override
    public void onLoginSuccess(String token, String deviceName) {
        GuartinelApp.getDataStore().setDeviceName(deviceName);
        GuartinelApp.getDataStore().setToken(token);
        if (_splashView == null) {
            LOG.I("SplashPresenter.onLoginSuccess splashView is null.");
            return;
        }
        if (!_splashView.isFromNotification()) {
            _splashView.openMainActivity(_splashView.getSuperContext());
            return;
        }
        _splashView.openMainActivity(_splashView.getSuperContext(), _splashView.getAlertMessage(), _splashView.getNotificationID());
    }

    @Override
    public void onUpdateNow() {
        _splashView.onUpdateNow();
    }
}
