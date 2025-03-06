package sysment.android.guartinel.ui;

import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.net.Uri;
import android.support.v7.app.AlertDialog;
import android.support.v7.view.ContextThemeWrapper;

import sysment.android.guartinel.R;
import sysment.android.guartinel.core.utils.SystemInfoUtil;

public class DialogManager {
    public static void showUpdateNeededDialog(final Context context) {
        new AlertDialog.Builder(new ContextThemeWrapper(context, R.style.AlertDialogCustom))
                .setTitle("Update needed")
                .setMessage("You must update your app to continue.")
                .setPositiveButton("ok", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        try {
                            Intent appStoreIntent = new Intent(Intent.ACTION_VIEW, Uri.parse("market://details?id=" + SystemInfoUtil.getPackageName()));
                            appStoreIntent.setPackage("com.android.vending");
                            context.startActivity(appStoreIntent);
                        } catch (android.content.ActivityNotFoundException exception) {
                            context.startActivity(new Intent(Intent.ACTION_VIEW, Uri.parse("https://play.google.com/store/apps/details?id=" + SystemInfoUtil.getPackageName())));
                        }
                    }
                })
                .create()
                .show();
    }
}
