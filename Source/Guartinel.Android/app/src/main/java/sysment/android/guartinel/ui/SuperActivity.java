package sysment.android.guartinel.ui;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.TextView;

import com.google.zxing.integration.android.IntentIntegrator;

import sysment.android.guartinel.R;
import sysment.android.guartinel.ui.accountExpired.AccountExpiredActivity;
import sysment.android.guartinel.ui.createAccount.CreateAccountActivity;
import sysment.android.guartinel.ui.loginAccount.LoginAccountActivity;
import sysment.android.guartinel.ui.main.MainActivity;
import sysment.android.guartinel.ui.registerDevice.RegisterDeviceActivity;

/**
 * Created by sysment_dev on 02/24/2016.
 */
public class SuperActivity extends AppCompatActivity implements SuperView {
    @Override
    public void showSnackBarError(View layoutView, String text, String actionText, View.OnClickListener listener) {
        Snackbar snackbar = Snackbar.make(layoutView, text, Snackbar.LENGTH_INDEFINITE)
                .setAction(actionText, listener);

        snackbar.setActionTextColor(Color.RED);
        View sbView = snackbar.getView();
        TextView textView = (TextView) sbView.findViewById(android.support.design.R.id.snackbar_text);
        textView.setTextColor(Color.YELLOW);
        snackbar.show();
    }

 /*   @Override
    public void showSnackBarError(View layoutView, String text, String actionText) {
        showSnackBarError(layoutView, text, actionText, new View.OnClickListener() {
            @Override
            public void onClick(View v) {
            }
        });
    }*/

    @Override
    public void showSnackBarInfo(View layoutView, String text, String actionText, View.OnClickListener listener) {
        Snackbar snackbar = Snackbar.make(layoutView, text, Snackbar.LENGTH_INDEFINITE)
                .setAction(actionText, listener);
        snackbar.setActionTextColor(Color.GREEN);
        View sbView = snackbar.getView();
        TextView textView = (TextView) sbView.findViewById(android.support.design.R.id.snackbar_text);
        textView.setTextColor(Color.YELLOW);
        snackbar.show();
    }

    /*@Override
    public void showSnackBarInfo(View layoutView, String text, String actionText) {
        showSnackBarInfo(layoutView, text, actionText, new View.OnClickListener() {
            @Override
            public void onClick(View v) {
            }
        });
    }*/


    @Override
    public Context getSuperContext() {
        return getApplicationContext();
    }

    public void textColorToRed(EditText editText) {
        editText.setTextColor(getResources().getColor(R.color.Red));
    }

    public void textColorToBlack(EditText editText) {
        editText.setTextColor(getResources().getColor(R.color.Black));
    }

    public void startQrReading() {
        new IntentIntegrator(this).initiateScan();
    }

    public void hideKeyboard() {
        InputMethodManager imm = (InputMethodManager) this.getSystemService(Activity.INPUT_METHOD_SERVICE);

        View view = this.getCurrentFocus();
        if (view == null) {
            view = new View(this);
        }
        imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
    }

    public void openMainActivity(final Context context) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Intent i = new Intent(context, MainActivity.class);
                i.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                startActivity(i);
                finish();
            }
        });
    }
    public void openMainActivity(final Context context, final String alertMessage, final int notificationID) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Intent i = new Intent(context, MainActivity.class);
                i.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                //i.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                //i.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK);

                i.putExtra(MyNotificationManager.Constants.MESSAGE,alertMessage);
                i.putExtra(MyNotificationManager.Constants.NOTIFICATION_ID,notificationID);
                startActivity(i);
                finish();
            }
        });
    }
    public void openLoginAccountActivity(final Context context) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Intent i = new Intent(context, LoginAccountActivity.class);
                i.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                startActivity(i);
                finish();
            }
        });
    }

    public void openRegisterDeviceActivity(final Context context) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Intent i = new Intent(context, RegisterDeviceActivity.class);
                i.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                startActivity(i);
                finish();
            }
        });
    }

    @Override
    public void openAccountExpiredActivity(final Context context) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Intent i = new Intent(context, AccountExpiredActivity.class);
                i.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                startActivity(i);
                finish();
            }
        });
    }

    protected void createAccountActivity(final Context context) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Intent i = new Intent(context, CreateAccountActivity.class);
                i.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                startActivity(i);
                finish();
            }
        });
    }

}
