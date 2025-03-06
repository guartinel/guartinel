package sysment.guartinelandroidsystemwatcher.ui;

import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.DialogFragment;
import android.support.v7.widget.LinearLayoutCompat;
import android.support.v7.widget.SwitchCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.SeekBar;
import android.widget.TextView;

import sysment.guartinelandroidsystemwatcher.R;
import sysment.guartinelandroidsystemwatcher.SystemWatcherApp;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.Settings;
import sysment.guartinelandroidsystemwatcher.ui.busEvents.ServerInstanceConfiguredEvent;
import sysment.guartinelandroidsystemwatcher.ui.busEvents.SettingsChangedEvent;

/**
 * Created by moqs_the_one on 2017.08.15..
 */

public class SettingsFragment extends DialogFragment {
    private int[] seekBarValuesSec = {60, 120, 180, 240, 300, 360, 600, 1800, 3600, 10800};
    private String[] seekBarValuesText = {"1m", "2m", "3m", "4m", "5m", "6m", "10m", "30m", "1h", "3h"};

    Settings settings;

    public void setSettings(Settings settings) {
        this.settings = settings;
    }

    @Override
    public void onResume() {
        super.onResume();
        ViewGroup.LayoutParams params = getDialog().getWindow().getAttributes();
        params.width = LinearLayoutCompat.LayoutParams.MATCH_PARENT;
        params.height = LinearLayoutCompat.LayoutParams.WRAP_CONTENT;
        getDialog().getWindow().setAttributes((android.view.WindowManager.LayoutParams) params);

    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.settings_dialog, container,
                false);

        getDialog().setTitle("Settings");

        final SeekBar checkIntervalSeekBar = (SeekBar) rootView.findViewById(R.id.settingsDialogCheckIntervalSeekBar);
        final TextView checkIntervalValueTextView = (TextView) rootView.findViewById(R.id.settingsDialogcheckIntervalValueTextView);
        final SeekBar noInternetCheckIntervalSeekBar = (SeekBar) rootView.findViewById(R.id.settingsDialogNoInternetCheckIntervalSeekBar);
        final TextView noInternetCheckIntervalValueTextView = (TextView) rootView.findViewById(R.id.settingsDialogNointernetCheckIntervalValueTextView);

        final SwitchCompat isWatchingEnabledSwitch = (SwitchCompat) rootView.findViewById(R.id.settingsDialogWatchingEnabledSwitch);
        isWatchingEnabledSwitch.setChecked(settings.isWatchingEnabled());

        final SwitchCompat isInternetCheckEnabledSwitch = (SwitchCompat) rootView.findViewById(R.id.settingsDialogInternetCheckEnabledSwitch);
        isInternetCheckEnabledSwitch.setChecked(settings.isInternetCheckEnabled());

        checkIntervalSeekBar.setProgress(getSeekBarProgressFromSecond(settings.getCheckIntervalSec()));
        checkIntervalValueTextView.setText(seekBarValuesText[checkIntervalSeekBar.getProgress()]);

        noInternetCheckIntervalSeekBar.setProgress(getSeekBarProgressFromSecond(settings.getNoInternetCheckIntervalInSec()));
        noInternetCheckIntervalValueTextView.setText(seekBarValuesText[noInternetCheckIntervalSeekBar.getProgress()]);

        checkIntervalSeekBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                checkIntervalValueTextView.setText(seekBarValuesText[progress]);
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {

            }
        });
        noInternetCheckIntervalSeekBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                noInternetCheckIntervalValueTextView.setText(seekBarValuesText[progress]);
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {

            }
        });

        final Button saveButton = (Button) rootView.findViewById(R.id.settingsDialogSaveButton);
        saveButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                settings.setCheckIntervalSec(seekBarValuesSec[checkIntervalSeekBar.getProgress()]);
                settings.setNoInternetCheckIntervalInSec(seekBarValuesSec[noInternetCheckIntervalSeekBar.getProgress()]);
                settings.setWatchingEnabled(isWatchingEnabledSwitch.isChecked());
                settings.setInternetCheckEnabled(isInternetCheckEnabledSwitch.isChecked());
                SystemWatcherApp.getDataStore().saveSettings(getActivity(), settings,true);

                dismiss();
            }
        });

        final ImageButton cancelButton = (ImageButton) rootView.findViewById(R.id.settingsDialogCancelButton);
        cancelButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                dismiss();
            }
        });
        return rootView;
    }

    private int getSeekBarProgressFromSecond(int second) {
        for (int i = 0; i < seekBarValuesSec.length; i++) {
            if (seekBarValuesSec[i] == second) {
                return i;
            }
        }
        return 0;
    }
}
