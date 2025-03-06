//package sysment.android.guartinel.ui._login;
//
//import android.content.Intent;
//import android.os.Bundle;
//import android.support.design.widget.CoordinatorLayout;
//import android.support.v7.widget.Toolbar;
//import android.view.View;
//import android.widget.Button;
//import android.widget.EditText;
//
//import sysment.android.guartinel.R;
//import sysment.android.guartinel.ui.main.MainActivity;
//import sysment.android.guartinel.ui.SuperActivity;
//
//public class _LoginActivity extends SuperActivity implements _LoginView {
//    private _LoginPresenter _loginPresenter;
//    private CoordinatorLayout _coordinatorLayout;
//    private Button _loginButton;
//    private EditText _emailEditText;
//    private EditText _passwordEditText;
//
//    @Override
//    protected void onCreate(Bundle savedInstanceState) {
//        super.onCreate(savedInstanceState);
//        setContentView(R.layout._login_activity);
//        _loginPresenter = new _LoginPresenter(this);
//        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
//        setSupportActionBar(toolbar);
//        _coordinatorLayout = (CoordinatorLayout) findViewById(R.id.login_coordinator_layout);
//        _loginButton = (Button) findViewById(R.id.login_button);
//        _emailEditText = (EditText)findViewById(R.id.login_email_edit_text);
//        _passwordEditText = (EditText)findViewById(R.id.login_password_edit_text);
//
//        _loginButton.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//                hideKeyboard();
//                _loginPresenter.initLogin();
//            }
//        });
//
//        showWhyThisActivityStarted();
//    }
//
//    @Override
//    protected void onDestroy() {
//        _loginPresenter.onDestroy();
//        super.onDestroy();
//    }
//
//    public void showWhyThisActivityStarted(){
//        String text = getResources().getString(R.string.login_activity_cause);
//        showSnackBarInfo(_coordinatorLayout, text, "OK", new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//
//            }
//        });
//    }
//
//    @Override
//    public void doOnUIThread(Runnable task) {
//        runOnUiThread(task);
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
//    public void setConnectionError() {
//
//    }
//
//    @Override
//    public void openMainActivity() {
//        Intent i = new Intent(_LoginActivity.this, MainActivity.class);
//        i.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
//        startActivity(i);
//        finish();
//    }
//
//    @Override
//    public void setInvalidUserNameOrPasswordError() {
//        textColorToRed(_passwordEditText);
//        textColorToRed(_emailEditText);
//        showSnackBarError(_coordinatorLayout, "Invalid email or password", "OK", new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//
//            }
//        });
//    }
//
//    @Override
//    public void setAccountExpiredError() {
//        showSnackBarError(_coordinatorLayout, "Your account is expired. Activate it on the website", "OK", new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//
//            }
//        });
//    }
//
//
//}
