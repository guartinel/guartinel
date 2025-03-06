package sysment.android.guartinel.ui.main.fragments;


import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v7.widget.SwitchCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.TextView;

import sysment.android.guartinel.core.utils.SystemInfoUtil;
import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.R;

/**
 * Created by sysment_dev on 02/26/2016.
 */
public class SettingsFragment extends Fragment {
    public static final String TAG = "Settings";
    SwitchCompat notificationSwitch;
    SwitchCompat forcedNotificationSwitch;

    public SettingsFragment() {
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.settings_fragment, container, false);
        notificationSwitch = (SwitchCompat) view.findViewById(R.id.notification_switch);
        notificationSwitch.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                GuartinelApp.getDataStore().setNotificationEnabled(isChecked);
            }
        });
        notificationSwitch.setChecked(GuartinelApp.getDataStore().isNotificationEnabled());

        forcedNotificationSwitch = (SwitchCompat)view.findViewById(R.id.settings_fragment_forced_notification_switch);
        forcedNotificationSwitch.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                GuartinelApp.getDataStore().setForcedNotificationEnabled(isChecked);
            }
        });
            forcedNotificationSwitch.setChecked(GuartinelApp.getDataStore().isForcedNotificationEnabled());
        TextView versionTextView = (TextView) view.findViewById(R.id.programVersionTextView);
        TextView deviceNameTextView = (TextView) view.findViewById(R.id.deviceNameTextView);
        versionTextView.setText(SystemInfoUtil.getVersion());

        TextView emailAddressTextView = (TextView) view.findViewById(R.id.emailAddressValueTextview);
        emailAddressTextView.setText(GuartinelApp.getDataStore().getEmail());
        String deviceName = GuartinelApp.getDataStore().getDeviceName();
        if (deviceName.length() > 30) {
            deviceName = deviceName.substring(0, 27) + "...";
        }
        deviceNameTextView.setText(deviceName);


        return view;
    }
}
