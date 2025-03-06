//package sysment.android.guartinel.ui._register;
//
//import android.content.DialogInterface;
//import android.content.Intent;
//import android.os.Bundle;
//import android.support.design.widget.CoordinatorLayout;
//import android.support.design.widget.FloatingActionButton;
//import android.support.v7.app.AlertDialog;
//import android.support.v7.widget.AppCompatButton;
//import android.support.v7.widget.Toolbar;
//import android.view.View;
//import android.widget.EditText;
//import android.widget.ProgressBar;
//
//import sysment.android.guartinel.core.gcm.GCMRegistrationResultReceiver;
//import sysment.android.guartinel.core.gcm.RegistrationIntentService;
//import sysment.android.guartinel.core.utils.LOG;
//import sysment.android.guartinel.R;
//import sysment.android.guartinel.ui.main.MainActivity;
//import sysment.android.guartinel.ui.SuperActivity;
//
//public class _RegisterActivity extends SuperActivity implements _RegisterView {
//    public _RegisterPresenter _registerPresenter;
//
//    private CoordinatorLayout _coordinatorLayout;
//
//    private AppCompatButton registerButton;
//    private EditText _emailEditText;
//    private EditText _passwordEditText;
//    private EditText _deviceNameEditText;
//    private ProgressBar _progressBar;
//
//    @Override
//    protected void onCreate(Bundle savedInstanceState) {
//        super.onCreate(savedInstanceState);
//        setContentView(R.layout._register_activity);
//        _progressBar = (ProgressBar) findViewById(R.id.progressBar);
//        _coordinatorLayout = (CoordinatorLayout) findViewById(R.id.coordinator_layout);
//        registerButton = (AppCompatButton) findViewById(R.id.register_button);
//
//        _registerPresenter = new _RegisterPresenter(this);
//        _emailEditText = (EditText) findViewById(R.id.email_edit_text);
//        _passwordEditText = (EditText) findViewById(R.id.password_edit_text);
//        _deviceNameEditText = (EditText) findViewById(R.id.device_name_edit_text);
//
//        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
//        setSupportActionBar(toolbar);
//
//        FloatingActionButton fab = (FloatingActionButton) findViewById(R.id.fab);
//        fab.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View view) {
//                String text = getResources().getString(R.string.open_qr_snackbar_text);
//                String actionText = getResources().getString(R.string.ok);
//                showSnackBarInfo(_coordinatorLayout, text, actionText, new View.OnClickListener() {
//                    @Override
//                    public void onClick(View v) {
//                        startQrReading();
//                    }
//                });
//
//            }
//        });
//        registerButton.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//                if (!areInputsValid()) {
//                    return;
//                }
//                registerStarted();
//                hideKeyboard();
//                _registerPresenter.initRegister(false);
//            }
//        });
//        showFirstInitSnackBar();
//    }
//
//    private boolean areInputsValid() {
//        if (_emailEditText.getText().toString().length() < 5 || !_emailEditText.getText().toString().contains("@") || !_emailEditText.getText().toString().contains(".")) {
//            showSnackBarError(_coordinatorLayout, "Email is invalid.", "OK",null);
//            return false;
//        }
//        if (_passwordEditText.getText().toString().length() < 6) {
//            showSnackBarError(_coordinatorLayout, "Password is must be minimum 6 character long.", "OK",null);
//            return false;
//        }
//        if (_deviceNameEditText.getText().toString().length() < 4) {
//            showSnackBarError(_coordinatorLayout, "Device name must be minimum 4 character long.", "OK",null);
//            return false;
//        }
//        return true;
//    }
//
//    @Override
//    protected void onDestroy() {
//        _registerPresenter.onDestroy();
//        super.onDestroy();
//    }
//
//    private void registerStarted() {
//        registerButton.setVisibility(View.GONE);
//        _progressBar.setVisibility(View.VISIBLE);
//    }
//
//    private void registerFinished() {
//        registerButton.setVisibility(View.VISIBLE);
//        _progressBar.setVisibility(View.GONE);
//    }
//
//    @Override
//    public void setInvalidUserNameOrPasswordError() {
//        registerFinished();
//        showSnackBarError(_coordinatorLayout, "Email address or password is invalid.", "OK",null);
//        textColorToRed(_emailEditText);
//        textColorToRed(_passwordEditText);
//        textColorToBlack(_deviceNameEditText);
//    }
//
//
//    @Override
//    public void setDeviceNameError() {
//         showSnackBarError(_coordinatorLayout, "Device name is taken.", "OK",null);
//        textColorToRed(_deviceNameEditText);
//        textColorToBlack(_passwordEditText);
//        textColorToBlack(_emailEditText);
//        showConfirmOverWriteDialog();
//    }
//
//    private void showConfirmOverWriteDialog() {
//        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(
//                this, R.style.AppTheme_Dark_Dialog);
//        alertDialogBuilder.setTitle("Confirm");
//        alertDialogBuilder
//                .setMessage("This device name is already used in your account. Would you like to overwrite it?")
//                .setCancelable(false)
//                .setPositiveButton("Yes", new DialogInterface.OnClickListener() {
//                    public void onClick(DialogInterface dialog, int id) {
//                        registerFinished();
//                        _registerPresenter.initRegister(true);
//                        dialog.cancel();
//                    }
//                })
//                .setNegativeButton("No", new DialogInterface.OnClickListener() {
//                    public void onClick(DialogInterface dialog, int id) {
//                        registerFinished();
//                        dialog.cancel();
//                    }
//                });
//        AlertDialog alertDialog = alertDialogBuilder.create();
//        alertDialog.show();
//    }
//
//    @Override
//    public void setConnectionError() {
//        registerFinished();
//        String text = getResources().getString(R.string.connection_error);
//        String actionText = getResources().getString(R.string.ok);
//        showSnackBarError(_coordinatorLayout, text, actionText,null);
//    }
//
//    @Override
//    public void openMainActivity() {
//        Intent i = new Intent(_RegisterActivity.this, MainActivity.class);
//        i.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
//        startActivity(i);
//        finish();
//    }
//
//    @Override
//    public void setCommunicationError() {
//        registerFinished();
//        showSnackBarError(_coordinatorLayout, "Communication error", "OK",null);
//    }
//
//    @Override
//    public String getEmail() {
//        return _emailEditText.getText().toString();
//    }
//
//    @Override
//    public String getPassword() {
//        return _passwordEditText.getText().toString();
//    }
//
//    @Override
//    public String getDeviceName() {
//        return _deviceNameEditText.getText().toString();
//    }
//
//    public void showFirstInitSnackBar() {
//      //  showSnackBarInfo(_coordinatorLayout, "First app start. Please fill the form!", "OK");
//    }
//
//    @Override
//    public void startGcmRegistrationService(GCMRegistrationResultReceiver receiver) {
//        LOG.I("SplashScreenActivity.startGcmRegistrationService");
//        Intent gcmIntent = new Intent(this, RegistrationIntentService.class);
//        gcmIntent.putExtra("receiver", receiver);
//        startService(gcmIntent);
//    }
//
//    @Override
//    public void setAccountExpiredError() {
//        registerFinished();
//        showSnackBarError(_coordinatorLayout, "Your account is expired. Activate it on the website", "OK",null);
//    }
//
//    @Override
//    public void setMaximumDeviceCountReachedError() {
//        registerFinished();
//        showSnackBarError(_coordinatorLayout, "Maximum device count reached. Upgrade your license to _deviceRegister more device.", "OK",null);
//    }
//
//    @Override
//    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
//        if (data == null) {
//            return;
//        }
//        String scanResultRaw = data.getExtras().getString("SCAN_RESULT");
//        final String keyValueSeparator = "!&!";//Warning IF changed must be changed in website
//        final String keyEqualsSeparator = "&!&";
//        final String emailKey = "email";
//        final String passwordKey = "password";
//        final String deviceNameKey = "device_name";
//
//        String[] keyValues = scanResultRaw.split(keyValueSeparator);
//        for (String keyValue : keyValues) {
//            String[] keyValueSeparated = keyValue.split(keyEqualsSeparator);
//            if (keyValueSeparated[0].equals(emailKey)) {
//                _emailEditText.setText(keyValueSeparated[1]);
//                continue;
//            }
//            if (keyValueSeparated[0].equals(passwordKey)) {
//                _passwordEditText.setText(keyValueSeparated[1]);
//                continue;
//            }
//            if (keyValueSeparated[0].equals(deviceNameKey)) {
//                _deviceNameEditText.setText(keyValueSeparated[1]);
//                continue;
//            }
//        }
//    }
//
//}
