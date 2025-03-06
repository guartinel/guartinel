package sysment.guartinelandroidsystemwatcher.ui;


import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.DialogFragment;
import android.support.v7.widget.LinearLayoutCompat;
import android.support.v7.widget.SwitchCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.URLUtil;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.Switch;
import android.widget.TextView;

import sysment.guartinelandroidsystemwatcher.R;
import sysment.guartinelandroidsystemwatcher.SystemWatcherApp;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstance;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerStatus;
import sysment.guartinelandroidsystemwatcher.ui.busEvents.ServerInstanceConfiguredEvent;

/**
 * Created by sysment_dev on 11/18/2016.
 */
public class ConfigureServerDialogFragment extends DialogFragment {


    public ConfigureServerDialogFragment() {
    }

    ServerInstance serverInstance;

    public void setServer(ServerInstance serverInstance) {
        this.serverInstance = serverInstance;
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
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.server_configuraton_dialog, container,
                false);
        getDialog().setTitle("Add New Server");

        Button saveButton = (Button) rootView.findViewById(R.id.serverConfigurationDailogSaveButton);
        ImageButton cancelButton = (ImageButton) rootView.findViewById(R.id.serverConfigurationDialogCancelButton);


        final EditText serverAddressEditText = (EditText) rootView.findViewById(R.id.serverConfigurationDailogServerAddressEditText);
        final EditText serverTokenEditText = (EditText) rootView.findViewById(R.id.serverConfigurationDailogServerTokenEditText);
        final EditText serverNameEditText = (EditText) rootView.findViewById(R.id.serverConfigurationDailogServerNameEditText);
        final EditText serverDescriptionEditText = (EditText) rootView.findViewById(R.id.serverConfigurationDailogServerDescriptionEditText);


        final TextView errorTextView = (TextView) rootView.findViewById(R.id.serverConfigurationDialogErrorTextView);
        final SwitchCompat enabledSwitch = (SwitchCompat) rootView.findViewById(R.id.serverConfigurationDialogIsEnabledSwitch);

        serverTokenEditText.setText(serverInstance.getToken());
        serverAddressEditText.setText(serverInstance.getAddress());
        serverNameEditText.setText(serverInstance.getName());
        enabledSwitch.setChecked(serverInstance.isEnabled());
        saveButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (!URLUtil.isValidUrl(serverAddressEditText.getText().toString())) {
                    errorTextView.setText("Server address is not valid");
                    errorTextView.setVisibility(View.VISIBLE);
                    return;
                }

                if (serverTokenEditText.getText().toString().length() < 4) {
                    errorTextView.setText("Server token is too short");
                    errorTextView.setVisibility(View.VISIBLE);
                    return;
                }
                serverInstance.setAddress(serverAddressEditText.getText().toString());
                serverInstance.setToken(serverTokenEditText.getText().toString());
                serverInstance.setName(serverNameEditText.getText().toString());
                serverInstance.setDescription(serverDescriptionEditText.getText().toString());
                serverInstance.setEnabled(enabledSwitch.isChecked());
                serverInstance.setStatus(new ServerStatus().setUnknown());
                SystemWatcherApp.getBus().post(new ServerInstanceConfiguredEvent(getActivity(), serverInstance));
                dismiss();
            }
        });

        cancelButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                dismiss();
            }
        });
        return rootView;
    }
}
