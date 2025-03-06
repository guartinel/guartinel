package sysment.android.guartinel.ui.loginAccount;

import android.content.Intent;
import android.os.Bundle;
import android.support.design.widget.CoordinatorLayout;
import android.support.design.widget.TextInputEditText;
import android.support.design.widget.TextInputLayout;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.mobsandgeeks.saripaar.ValidationError;
import com.mobsandgeeks.saripaar.Validator;
import com.mobsandgeeks.saripaar.annotation.Email;
import com.mobsandgeeks.saripaar.annotation.Length;
import com.mobsandgeeks.saripaar.annotation.Password;

import java.util.List;

import sysment.android.guartinel.R;
import sysment.android.guartinel.ui.DialogManager;
import sysment.android.guartinel.ui.SuperActivity;
import sysment.android.guartinel.ui.accountExpired.AccountExpiredActivity;
import sysment.android.guartinel.ui.createAccount.CreateAccountActivity;
import sysment.android.guartinel.ui.main.MainActivity;
import sysment.android.guartinel.ui.splash.SplashScreenActivity;

public class LoginAccountActivity extends SuperActivity implements LoginAccountView, Validator.ValidationListener {
    LoginAccountPresenter _presenter;

    @Length(min = 6, message = "Minimum 6 characters")
    @Password()
    EditText _passwordEditText;
    TextInputLayout _passwordLayout;

    @Email(message = "Invalid email address.")
    EditText _emailEditText;
    TextInputLayout _emailLayout;

    Button _loginDeviceButton;

    Validator validator;
    Toolbar toolbar;
    TextView createAccountTextView;
    CoordinatorLayout coordinatorLayout;
    ProgressBar _loginDeviceProgressbar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login_account_activity);
        _presenter = new LoginAccountPresenter(this);
        coordinatorLayout = (CoordinatorLayout) findViewById(R.id.coordinator_layout);

        _passwordEditText = (EditText) findViewById(R.id.login_device_password_edit_text);
        _passwordLayout = (TextInputLayout) findViewById(R.id.login_device_password_layout);
        _emailEditText = (EditText) findViewById(R.id.login_device_email_edit_text);
        _emailLayout = (TextInputLayout) findViewById(R.id.login_device_email_layout);
        _loginDeviceButton = (Button) findViewById(R.id.login_device_login_button);
        _loginDeviceButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                validator.validate();
            }
        });
        _loginDeviceProgressbar = (ProgressBar) findViewById(R.id.login_device_progressbar);
        createAccountTextView = (TextView) findViewById(R.id.login_device_create_account_text_view);
        createAccountTextView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent i = new Intent(LoginAccountActivity.this, CreateAccountActivity.class);
                i.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                startActivity(i);
                finish();
            }
        });


        validator = new Validator(this);
        validator.setValidationListener(this);
        toolbar = (Toolbar) findViewById(R.id.register_account_toolbar);
        toolbar.setLogo(R.drawable.account);
        toolbar.setTitle("Guartinel");
        toolbar.setSubtitle("Log in to your Guartinel account");
        setSupportActionBar(toolbar);
    }

    @Override
    public void showSnackBarError(View layoutView, String text, String actionText, View.OnClickListener listener) {
        super.showSnackBarError(coordinatorLayout, text, actionText, listener);
    }

    @Override
    public void onValidationSucceeded() {
        _emailLayout.setError(null);
        _passwordLayout.setError(null);
        _loginDeviceButton.setVisibility(View.INVISIBLE);
        _loginDeviceProgressbar.setVisibility(View.VISIBLE);
        _presenter.loginDevice(
                _emailEditText.getText().toString(),
                _passwordEditText.getText().toString());
    }

    @Override
    public void onValidationFailed(List<ValidationError> errors) {
        _emailLayout.setError(null);
        _passwordLayout.setError(null);
        for (ValidationError error : errors) {
            View view = error.getView();
            String message = error.getCollatedErrorMessage(this);
            // Display error messages ;)
            if (view instanceof TextInputEditText) {
                //  ((EditText) view).setError(message);
                TextInputLayout textInputLayout = (TextInputLayout) findViewById(view.getId()).getParent().getParent();
                textInputLayout.setError(message);
            }
        }
    }

    @Override
    public void invalidUserNameOrPassword() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                _loginDeviceButton.setVisibility(View.VISIBLE);
                _loginDeviceProgressbar.setVisibility(View.INVISIBLE);

                _emailLayout.setError("Invalid email");
                _passwordLayout.setError("Invalid password");

            }
        });
    }

    @Override
    public void onAccountExpired() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                openAccountExpiredActivity(getSuperContext());
            }
        });
    }

    @Override
    public String getPassword() {
        return _passwordEditText.getText().toString();
    }

    @Override
    public String getEmail() {
        return _emailEditText.getText().toString();

    }

    @Override
    public void showUpdateNow() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                DialogManager.showUpdateNeededDialog(LoginAccountActivity.this);
            }
        });
    }

    @Override
    public void onConnectionError(final View.OnClickListener listener) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                showSnackBarError(coordinatorLayout, "Could not connect to server.", "Retry", listener);
                _loginDeviceButton.setVisibility(View.VISIBLE);
                _loginDeviceProgressbar.setVisibility(View.INVISIBLE);

            }
        });
    }
}
