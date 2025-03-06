package sysment.android.guartinel.ui.registerDevice;

import android.content.Intent;
import android.os.Bundle;
import android.support.design.widget.TextInputEditText;
import android.support.design.widget.TextInputLayout;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;

import com.mobsandgeeks.saripaar.ValidationError;
import com.mobsandgeeks.saripaar.Validator;
import com.mobsandgeeks.saripaar.annotation.Email;
import com.mobsandgeeks.saripaar.annotation.Length;
import com.mobsandgeeks.saripaar.annotation.Password;

import java.util.List;

import sysment.android.guartinel.R;
import sysment.android.guartinel.ui.SuperActivity;
import sysment.android.guartinel.ui.main.MainActivity;
import sysment.android.guartinel.ui.splash.SplashScreenActivity;

public class RegisterDeviceActivity extends SuperActivity implements RegisterDeviceView, Validator.ValidationListener {
    RegisterDevicePresenter _presenter;

    @Length(min = 6, message = "Minimum 6 characters")
    EditText _deviceNameEditText;
    TextInputLayout _deviceNameLayout;

    Button _registerDeviceButton;
    ProgressBar _progressBar;
    Validator validator;
    Toolbar toolbar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.register_device_activity);
        _presenter = new RegisterDevicePresenter(this);

        _deviceNameEditText = (EditText) findViewById(R.id.register_device_device_name_edit_text);
        _deviceNameLayout = (TextInputLayout) findViewById(R.id.register_device_device_name_layout);
        _registerDeviceButton = (Button) findViewById(R.id.register_device_register_button);
        _registerDeviceButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                validator.validate();
            }
        });
        _progressBar = (ProgressBar) findViewById(R.id.register_device_progressbar);
        validator = new Validator(this);
        validator.setValidationListener(this);
        toolbar = (Toolbar) findViewById(R.id.register_account_toolbar);
        toolbar.setLogo(R.drawable.account);
        toolbar.setTitle("Guartinel");
        toolbar.setSubtitle("Bind this device to your account");
        setSupportActionBar(toolbar);
    }


    @Override
    public void onValidationSucceeded() {
        _deviceNameEditText.setError(null);
        _registerDeviceButton.setVisibility(View.GONE);
        _progressBar.setVisibility(View.VISIBLE);
        _presenter.registerDevice(_deviceNameEditText.getText().toString());
    }

    @Override
    public void onValidationFailed(List<ValidationError> errors) {
        _deviceNameEditText.setError(null);
        for (ValidationError error : errors) {
            View view = error.getView();
            String message = error.getCollatedErrorMessage(this);
            if (view instanceof TextInputEditText) {
                TextInputLayout textInputLayout = (TextInputLayout) findViewById(view.getId()).getParent().getParent();
                textInputLayout.setError(message);
            }
        }
    }


    @Override
    public void deviceNameAlreadyTaken() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                _deviceNameEditText.setError("Device name already taken");
                _registerDeviceButton.setVisibility(View.VISIBLE);
                _progressBar.setVisibility(View.GONE);
            }
        });
    }

    @Override
    public void invalidUserNameOrPassword() {

    }

    @Override
    public void onAccountExpired() {

    }

    @Override
    public void openDashboard() {
        openMainActivity(getSuperContext());
    }

    @Override
    public String getDeviceName() {
        return _deviceNameEditText.getText().toString();
    }
}
