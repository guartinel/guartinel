package sysment.android.guartinel.ui.createAccount;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.support.design.widget.CoordinatorLayout;
import android.support.design.widget.TextInputEditText;
import android.support.design.widget.TextInputLayout;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ProgressBar;

import com.mobsandgeeks.saripaar.ValidationError;
import com.mobsandgeeks.saripaar.Validator;
import com.mobsandgeeks.saripaar.annotation.Checked;
import com.mobsandgeeks.saripaar.annotation.ConfirmPassword;
import com.mobsandgeeks.saripaar.annotation.Email;
import com.mobsandgeeks.saripaar.annotation.Length;
import com.mobsandgeeks.saripaar.annotation.Password;

import java.util.List;

import sysment.android.guartinel.R;
import sysment.android.guartinel.ui.SuperActivity;

public class CreateAccountActivity extends SuperActivity implements CreateAccountView, Validator.ValidationListener {
    CreateAccountPresenter _presenter;
    EditText _firstNameEditText;
    TextInputLayout _firstNameLayout;
    EditText _lastNameEditText;
    TextInputLayout _lastNameLayout;

    @Length(min = 6, message = "Minimum 6 characters")
    @Password()
    EditText _passwordEditText;
    TextInputLayout _passwordLayout;

    @ConfirmPassword(message = "Passwords not matching.")
    EditText _passwordConfirmEditText;
    TextInputLayout _passworcConfirmLayout;

    @Email(message = "Invalid email address.")
    EditText _emailEditText;
    TextInputLayout _emailLayout;

    @Checked(message = "You must agree to the terms.")
    CheckBox _agreePrivacyCheckBox;

    Button _registerAccountButton;
    Validator validator;
    Toolbar toolbar;

    CoordinatorLayout coordinatorLayout;
    ProgressBar _createAccountProgressBar;

    @Override

    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.create_account_activity);
        _presenter = new CreateAccountPresenter(this);
        _createAccountProgressBar = (ProgressBar) findViewById(R.id.register_account_progressbar);
        _firstNameEditText = (EditText) findViewById(R.id.register_account_first_name_edit_text);
        _firstNameLayout = (TextInputLayout) findViewById(R.id.register_account_first_name_layout);
        _lastNameEditText = (EditText) findViewById(R.id.register_account_last_name_edit_text);
        _lastNameLayout = (TextInputLayout) findViewById(R.id.register_account_last_name_layout);
        _passwordEditText = (EditText) findViewById(R.id.register_account_password_edit_text);
        _passwordLayout = (TextInputLayout) findViewById(R.id.register_account_password_layout);
        _passwordConfirmEditText = (EditText) findViewById(R.id.register_account_password_confirm_edit_text);
        _passworcConfirmLayout = (TextInputLayout) findViewById(R.id.register_account_password_confirm_layout);
        _emailEditText = (EditText) findViewById(R.id.register_account_email_edit_text);
        _emailLayout = (TextInputLayout) findViewById(R.id.register_account_email_layout);
        _agreePrivacyCheckBox = (CheckBox) findViewById(R.id.register_account_agree_privacy_check_box);
        _agreePrivacyCheckBox.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent i = new Intent(Intent.ACTION_VIEW);
                i.setData(Uri.parse("https://docs.wixstatic.com/ugd/53f9e0_5c4b108e84844301ad3b0b0796848ccc.pdf"));
                startActivity(i);
            }
        });
        _registerAccountButton = (Button) findViewById(R.id.register_account_register_button);
        _registerAccountButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                validator.validate();
            }
        });
        validator = new Validator(this);
        validator.setValidationListener(this);
        toolbar = (Toolbar) findViewById(R.id.register_account_toolbar);
        toolbar.setLogo(R.drawable.account);
        toolbar.setTitle("Guartinel ");
        toolbar.setSubtitle("Create a new account");
        setSupportActionBar(toolbar);
        coordinatorLayout = (CoordinatorLayout) findViewById(R.id.coordinator_layout);

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
    public void onConnectionError() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                showSnackBarError(coordinatorLayout, "Could not connect to the server", "Retry", new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        validator.validate();
                    }
                });
            }
        });
    }

    @Override
    public void onEmailAlreadyTaken() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                _createAccountProgressBar.setVisibility(View.GONE);
                _registerAccountButton.setVisibility(View.VISIBLE);
                _emailLayout.setError("Email address already taken. Please choose another one.");

            }
        });
    }

    @Override
    public void onValidationSucceeded() {
        _registerAccountButton.setVisibility(View.GONE);
        _createAccountProgressBar.setVisibility(View.VISIBLE);
        _emailLayout.setError(null);
        _passworcConfirmLayout.setError(null);
        _passwordLayout.setError(null);
        _presenter.registerAccount(_firstNameEditText.getText().toString(),
                _lastNameEditText.getText().toString(),
                _emailEditText.getText().toString(),
                _passwordEditText.getText().toString());
    }

    @Override
    public void onValidationFailed(List<ValidationError> errors) {
        _emailLayout.setError(null);
        _passworcConfirmLayout.setError(null);
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

            if (view instanceof CheckBox) {
                ((CheckBox) view).setError(message);
            }
        }
    }
}
