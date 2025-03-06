package sysment.android.guartinel.ui;

import android.content.Context;
import android.view.View;

/**
 * Created by sysment_dev on 02/24/2016.
 */
public interface SuperView {

    Context getSuperContext();

    void openMainActivity(final Context context);
    void openMainActivity(final Context context,String alertMessage,int notificationId);

    void openLoginAccountActivity(final Context context);

    void openRegisterDeviceActivity(final Context context);

    void openAccountExpiredActivity(final Context context);

    void showSnackBarError(View layoutView, String text, String actionText, View.OnClickListener listener);

    void showSnackBarInfo(View layoutView, String text, String actionText, View.OnClickListener listener);


}
