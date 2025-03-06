package sysment.android.guartinel.ui.accountExpired;

import android.Manifest;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.design.widget.CoordinatorLayout;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AlertDialog;
import android.support.v7.view.ContextThemeWrapper;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Button;
import android.widget.ProgressBar;
import android.widget.TextView;

import org.w3c.dom.Text;

import sysment.android.guartinel.R;
import sysment.android.guartinel.ui.DialogManager;
import sysment.android.guartinel.ui.SuperActivity;
import sysment.android.guartinel.ui.main.MainActivity;

public class AccountExpiredActivity extends SuperActivity implements AccountExpiredView {
    AccountExpiredPresenter _presenter;
    TextView resendActivationCodeButton;
    Button retryButton;
    ProgressBar activationProgressBar;
    CoordinatorLayout coordinatorLayout;
    Toolbar toolbar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.account_expired_activity);
        resendActivationCodeButton = (TextView) findViewById(R.id.account_expired_resend_activation_button);
        retryButton = (Button) findViewById(R.id.account_expired_retry_button);

        activationProgressBar = (ProgressBar) findViewById(R.id.account_expired_progress_bar);
        coordinatorLayout = (CoordinatorLayout) findViewById(R.id.coordinator_layout);
        retryButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                retryButton.setVisibility(View.GONE);
                activationProgressBar.setVisibility(View.VISIBLE);
                _presenter.retryLogin();
            }
        });

        resendActivationCodeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                resendActivationCodeButton.setVisibility(View.GONE);
                activationProgressBar.setVisibility(View.VISIBLE);
                retryButton.setVisibility(View.GONE);
                _presenter.resendActivationCode();
            }
        });
        toolbar = (Toolbar) findViewById(R.id.register_account_toolbar);
        toolbar.setLogo(R.drawable.account);
        toolbar.setTitle("Guartinel");
        toolbar.setSubtitle("Activate your Guartinel account");
        setSupportActionBar(toolbar);

        _presenter = new AccountExpiredPresenter(this);
    }

    @Override
    public void onStillNotActivated() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                resendActivationCodeButton.setVisibility(View.VISIBLE);
                activationProgressBar.setVisibility(View.GONE);
                retryButton.setVisibility(View.VISIBLE);

                showSnackBarError(coordinatorLayout, "Your account is still not activated.", "Try again", new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        _presenter.retryLogin();
                    }
                });
            }
        });
    }

    @Override
    public void onConnectionError() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                resendActivationCodeButton.setVisibility(View.VISIBLE);
                activationProgressBar.setVisibility(View.GONE);
                retryButton.setVisibility(View.VISIBLE);
                showSnackBarError(coordinatorLayout, "Could not connect to the server", "Try again", new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        _presenter.retryLogin();
                    }
                });
            }
        });
    }

    @Override
    public void onOneHourNotElapsedSinceLastSend() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                resendActivationCodeButton.setVisibility(View.VISIBLE);
                activationProgressBar.setVisibility(View.GONE);
                retryButton.setVisibility(View.VISIBLE);
                showSnackBarError(coordinatorLayout, "You have to wait one hour between activation code resends!", "OK", new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {

                    }
                });
            }
        });
    }

    @Override
    public void onResendActivationCodeSuccess() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                resendActivationCodeButton.setVisibility(View.VISIBLE);
                activationProgressBar.setVisibility(View.GONE);
                retryButton.setVisibility(View.VISIBLE);
                showSnackBarError(coordinatorLayout, "Activation code was resent!", "OK", new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {

                    }
                });
            }
        });
    }

    @Override
    public void onUpdateNow() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                DialogManager.showUpdateNeededDialog(AccountExpiredActivity.this);
            }
        });
    }
}
