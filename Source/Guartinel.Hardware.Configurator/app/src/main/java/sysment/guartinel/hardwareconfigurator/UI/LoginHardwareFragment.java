package sysment.guartinel.hardwareconfigurator.ui;

import android.content.Context;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;

import com.google.zxing.integration.android.IntentIntegrator;
import com.mobsandgeeks.saripaar.ValidationError;
import com.mobsandgeeks.saripaar.Validator;
import com.mobsandgeeks.saripaar.annotation.Length;
import com.mobsandgeeks.saripaar.annotation.NotEmpty;

import java.util.List;

import sysment.guartinel.hardwareconfigurator.R;
import sysment.guartinel.hardwareconfigurator.connection.HardwareConnector;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardware;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardwareClient;
import sysment.guartinel.hardwareconfigurator.tools.LOG;

/**
 * Created by DAVT on 2017.12.14..
 */

public class LoginHardwareFragment extends Fragment implements Validator.ValidationListener {
    public static class QR_READ_REQUEST_CODES {
        public static int DEVICE_PASSWORD = 4466;
    }

    public static String TAG = "LOGIN_HARDWARE_FRAGMENT";

    public void setDevicePassword(String pass) {
        devicePassword.setText(pass);
        validator.validate();
    }

    public LoginHardwareFragment setHardware(GuartinelHardware hardware) {
        this._hardware = hardware;
        return this;
    }

    GuartinelHardware _hardware;
    Validator validator;
    @NotEmpty
    @Length(min = 8, message = "Password must minimum 8 character long!")
    EditText devicePassword;
    Button loginButton;
    ProgressBar loginProgressBar;
    TextView loadingStatusTextView;

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View _view = inflater.inflate(R.layout.login_cli_fragment, container, false);
        ((MainActivity) getActivity()).getSupportActionBar().setTitle("Authenticate to: " + _hardware.getName());
        validator = new Validator(this);
        validator.setValidationListener(this);
        loginButton = (Button) _view.findViewById(R.id.loginHardwareButton);
        loginProgressBar = (ProgressBar) _view.findViewById(R.id.loginCLIProgressBar);
        loadingStatusTextView = (TextView) _view.findViewById(R.id.loadingStatusTextView);
        devicePassword = (EditText) _view.findViewById(R.id.devicePassword);
        devicePassword.setOnTouchListener(new DrawableClickListener.RightDrawableClickListener(devicePassword) {
            @Override
            public boolean onDrawableClick() {
                IntentIntegrator integrator = new IntentIntegrator(getActivity());
                integrator.setOrientationLocked(true);
                integrator.setCaptureActivity(QrScanActivity.class);
                integrator.setBeepEnabled(true);
                integrator.setPrompt("Scan the device password. Use volume buttons to switch flash.");
                integrator.initiateScan(IntentIntegrator.QR_CODE_TYPES);

                getActivity().startActivityForResult(integrator.createScanIntent(), LoginHardwareFragment.QR_READ_REQUEST_CODES.DEVICE_PASSWORD);
                return true;
            }
        });
        loginButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                validator.validate();
            }
        });
        if (_hardware instanceof GuartinelHardwareClient) {
            setLoading("Synchronizing with the device.");
            HardwareConnector.freeze(_hardware, new HardwareConnector.HardwareResponseListener() {
                @Override
                public void onSuccess() {
                    if (getActivity() == null) {
                        return;
                    }
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            loginButton.setEnabled(true);
                            setLoaded();
                        }
                    });
                }

                @Override
                public void onInvalidPassword() {
                    if (getActivity() == null) {
                        return;
                    }
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            ((MainActivity) getActivity()).showSnackbarError("Cannot connect to device.");
                            setLoaded();
                        }
                    });
                }

                @Override
                public void onFailed() {
                    if (getActivity() == null) {
                        return;
                    }
                    if (getActivity() == null) {
                        return;
                    }
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            ((MainActivity) getActivity()).showSnackbarError("Cannot connect to device.");
                            setLoaded();
                        }
                    });
                }
            });
        }
        return _view;
    }

    @Override
    public void onValidationSucceeded() {
        InputMethodManager imm = (InputMethodManager) getActivity().getSystemService(Context.INPUT_METHOD_SERVICE);
        imm.hideSoftInputFromWindow(getView().getWindowToken(), 0);
        setLoading("Authenticating with the device.");
        _hardware.devicePassword = devicePassword.getText().toString();
        LOG.I("Form validated. Login started");
        HardwareConnector.isPasswordOK(getActivity(), _hardware, new HardwareConnector.HardwareResponseListener() {
            @Override
            public void onSuccess() {
               /* if (_hardware instanceof GuartinelHardwareAP) {
                    ((MainActivity) getActivity()).afterSuccessFullLogin(_hardware);
                    return;
                }*/
                LOG.I("Login onSuccess. Getting config.");

                HardwareConnector.getConfig(getActivity(), _hardware, new HardwareConnector.HardwareResponseListener() {
                    @Override
                    public void onSuccess() {
                        LOG.I("getConfig.onSuccess");
                        if (getActivity() == null) {
                            return;
                        }
                        ((MainActivity) getActivity()).afterSuccessFullLogin(_hardware);
                    }

                    @Override
                    public void onInvalidPassword() {
                        LOG.I("getConfig.onInvalidPassword ");

                        if (getActivity() == null) {
                            return;
                        }
                        ((MainActivity) getActivity()).showSnackbarError("Password is invalid.");
                        setLoaded();
                    }

                    @Override
                    public void onFailed() {
                        LOG.I("getConfig failed ");
                        if (getActivity() == null) {
                            return;
                        }

                        ((MainActivity) getActivity()).showSnackbarError("Cannot connect to device.");
                        setLoaded();
                    }
                });
            }

            @Override
            public void onInvalidPassword() {
                LOG.I("login.onInvalidPassword");
                if (getActivity() == null) {
                    return;
                }
                ((MainActivity) getActivity()).showSnackbarError("Password is invalid.");
                setLoaded();
            }

            @Override
            public void onFailed() {
                LOG.I("login.onFailed");
                if (getActivity() == null) {
                    return;
                }
                ((MainActivity) getActivity()).showSnackbarError("Cannot connect to device.");
                setLoaded();
            }
        });
    }

    private void setLoading(String text) {
        loginProgressBar.setVisibility(View.VISIBLE);
        loginButton.setVisibility(View.GONE);
        loadingStatusTextView.setText(text);
    }

    private void setLoaded() {
        loginProgressBar.setVisibility(View.GONE);
        loginButton.setVisibility(View.VISIBLE);
        loadingStatusTextView.setText("");
    }

    @Override
    public void onValidationFailed(List<ValidationError> errors) {
        setLoaded();

        for (ValidationError error : errors) {
            View view = error.getView();
            String message = error.getCollatedErrorMessage(getActivity());


            // Display error messages
            if (view instanceof Spinner) {
                Spinner sp = (Spinner) view;
                view = ((LinearLayout) sp.getSelectedView()).getChildAt(0);        // we are actually interested in the text view spinner has
            }

            if (view instanceof TextView) {
                TextView et = (TextView) view;
                et.setError(message);
            }
        }

    }
}
