package sysment.android.guartinel.ui.main.fragments.configureSensor;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.Dialog;
import android.content.DialogInterface;
import android.net.wifi.ScanResult;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.design.widget.TextInputEditText;
import android.support.v4.app.DialogFragment;
import android.support.v7.app.AlertDialog;
import android.support.v7.view.ContextThemeWrapper;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.text.InputType;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.R;
import sysment.android.guartinel.core.connection.hardwareSensor.HardwareSensorImpl;
import sysment.android.guartinel.core.utils.HardwareSensorSniffer;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.network.WifiConnectionManager;
import sysment.android.guartinel.core.network.WifiUtility;
import sysment.android.guartinel.ui.main.MainActivity;
import sysment.android.guartinel.ui.presenterCallbacks.ConfigureHardwareSensorCallback;
import sysment.android.guartinel.ui.presenterCallbacks.HelloGuartinelCallback;
import sysment.android.guartinel.ui.presenterCallbacks.RegisterHardwareSensorCallback;

public class ConfigureDevicesDialogFragment extends DialogFragment {
    public static class Const {
        public final static String MANUAL = "Manual";
        public final static String MANUAL_ENTRY_MARK = "(Manual)";
    }

    private LinearLayout _selectHardwareLayout;
    private RecyclerView _recyclerView;
    private LinearLayoutManager _linearLayoutManager;
    private LinearLayout _scanningHardwareLinearLayout;
    private LinearLayout _wifiSettingsLinearLayout;
    private ImageButton closeConfigButton;
    private Spinner _wifiSSIDs;
    private TextView _scanStatusTextView;
    private Button _rescanButton;
    private Button _startConfigButton;
    private ProgressBar _scanProgressBar;
    private TextInputEditText _wifiPassword;
    private ProgressBar _configurationProgressBar;
    private TextView _configurationTextView;

    List<String> ssidEntries = new ArrayList<String>();

    HardwareRecyclerViewAdapter _recyclerViewAdapter;
    List<HardwareSensorImpl> _sensors = new ArrayList<HardwareSensorImpl>();

    private volatile boolean fragmentRunning = true;

    @Override
    public void onResume() {
        super.onResume();
        ViewGroup.LayoutParams params = getDialog().getWindow().getAttributes();
        params.width = ViewGroup.LayoutParams.MATCH_PARENT;
        params.height = ViewGroup.LayoutParams.WRAP_CONTENT;
        getDialog().getWindow().setAttributes((android.view.WindowManager.LayoutParams) params);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.configure_devices_dialog, container, false);
        setCancelable(false);
        getActivity().getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        _selectHardwareLayout = (LinearLayout) rootView.findViewById(R.id.select_hardware_layout);
        _scanningHardwareLinearLayout = (LinearLayout) rootView.findViewById(R.id.scanning_hardware_linear_layout);
        _wifiSettingsLinearLayout = (LinearLayout) rootView.findViewById(R.id.wifi_settings_layout);
        _wifiSSIDs = (Spinner) rootView.findViewById(R.id.wifiSSIDs);
        _recyclerView = (RecyclerView) rootView.findViewById(R.id.found_hardware_recycler_view);
        _linearLayoutManager = new LinearLayoutManager(getContext());
        _linearLayoutManager.setOrientation(LinearLayoutManager.VERTICAL);
        _recyclerView.setLayoutManager(_linearLayoutManager);
        _recyclerViewAdapter = new HardwareRecyclerViewAdapter(getActivity(), _sensors);
        _recyclerView.setAdapter(_recyclerViewAdapter);
        closeConfigButton = (ImageButton) rootView.findViewById(R.id.closeConfigButton);
        _scanStatusTextView = (TextView) rootView.findViewById(R.id.scan_status_text_view);
        _rescanButton = (Button) rootView.findViewById(R.id.rescanButton);
        _startConfigButton = (Button) rootView.findViewById(R.id.start_configure_button);
        _scanProgressBar = (ProgressBar) rootView.findViewById(R.id.scan_progress_bar);
        _wifiPassword = (TextInputEditText) rootView.findViewById(R.id.wifiPassword);
        _configurationProgressBar = (ProgressBar) rootView.findViewById(R.id.configuration_progress_bar);
        _configurationTextView = (TextView) rootView.findViewById(R.id.configuration_status_text_view);

        _rescanButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                _rescanButton.setVisibility(View.GONE);
                _scanStatusTextView.setText("Scanning devices...");
                _scanProgressBar.setVisibility(View.VISIBLE);
                doScan();
            }
        });

        _startConfigButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startConfigure();
            }
        });

        closeConfigButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // killThreads() ;
                dismiss();
            }
        });
        ssidEntries.add(Const.MANUAL);

        final ArrayAdapter adapter = new ArrayAdapter<String>(getActivity(), android.R.layout.simple_list_item_1, ssidEntries);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        _wifiSSIDs.setAdapter(adapter);

        _wifiSSIDs.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                if (!ssidEntries.get(i).equals(Const.MANUAL)) {
                    return;
                }
                final AlertDialog dialogBuilder = new AlertDialog.Builder(getActivity()).create();
                LayoutInflater inflater = ((Activity) getActivity()).getLayoutInflater();
                View dialogView = inflater.inflate(R.layout.alert_dialog_with_edit_text, null);
                TextView title = (TextView) dialogView.findViewById(R.id.alert_dialog_with_edit_text_title_edit_text);
                title.setText("Enter your SSID manually");

                final EditText inputEditText = (EditText) dialogView.findViewById(R.id.alert_dialog_with_edit_text);
                inputEditText.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_NORMAL);

                Button cancel = (Button) dialogView.findViewById(R.id.rename_device_cancel_button);
                Button ok = (Button) dialogView.findViewById(R.id.rename_device_ok_button);
                ok.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View view) {
                        ssidEntries.add(0, inputEditText.getText().toString() + Const.MANUAL_ENTRY_MARK);
                        _wifiSSIDs.setSelection(0);
                        adapter.notifyDataSetChanged();
                        dialogBuilder.dismiss();
                    }
                });
                cancel.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View view) {
                        dialogBuilder.dismiss();
                    }
                });

                dialogBuilder.setView(dialogView);
                dialogBuilder.show();
            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {
            }
        });
        return rootView;
    }

    public class DoneSemaphore {
        public boolean isFinished = false;
    }

    public void updateConfigureStatus(final String text) {
        if (getActivity() == null) {
            return;
        }
        getActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                _configurationTextView.setText(text);
            }
        });
    }

    private void updateRecyclerView() {
        if (getActivity() == null) {
            return;
        }
        getActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                _recyclerViewAdapter.notifyDataSetChanged();
            }
        });
    }

    Thread startConfigureThread;

    private void startConfigure() {
        LOG.I("!!!! START CONFIGURE !!!!");
        if (_wifiSSIDs.getSelectedItem() == null) {
            _wifiSSIDs.setSelection(0);
            _wifiSSIDs.setSelection(0, false);
        }
        disableUI();
        for (HardwareSensorImpl sensor : _sensors) {
            sensor.setErrorMessage("");
            sensor.setIsError(false);
        }
        updateRecyclerView();

        String selectedWifiSSID = _wifiSSIDs.getSelectedItem().toString();

        _startConfigButton.setVisibility(View.GONE);
        _configurationProgressBar.setVisibility(View.VISIBLE);
        _configurationTextView.setText("Testing the provided wifi access point...");
        _configurationTextView.setVisibility(View.VISIBLE);
        if (selectedWifiSSID.contains(Const.MANUAL_ENTRY_MARK)) {
            startConfigureThread = new Thread(new Runnable() {
                @Override
                public void run() {
                    getSensorIds();
                }
            });
            startConfigureThread.start();
        } else {
            selectedWifiSSID = selectedWifiSSID.replace(Const.MANUAL_ENTRY_MARK, "");
            final String finalSelectedWifiSSID = selectedWifiSSID;

            startConfigureThread = new Thread(new Runnable() {
                @Override
                public void run() {
                    testWifi(finalSelectedWifiSSID);
                }
            });
            startConfigureThread.setName("startConfigureThread");
            startConfigureThread.start();
        }
    }

    private void disableUI() {
        if(getActivity()== null){return;}
        getActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                _recyclerViewAdapter.disableUI();
                _wifiSSIDs.setEnabled(false);
                _wifiPassword.setEnabled(false);
                _recyclerView.setEnabled(false);

            }
        });
    }

    public void enableUI() {
        if(getActivity()== null){return;}
        getActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                _recyclerViewAdapter.enableUI();
                _wifiSSIDs.setEnabled(true);
                _wifiSSIDs.setEnabled(true);
                _wifiPassword.setEnabled(true);
                _recyclerView.setEnabled(true);
            }
        });
    }

    Thread testWifiThread;

    private void testWifi(String selectedWifiSSID) {
        WifiConnectionManager.testWifiAP(getActivity(),
                selectedWifiSSID,
                _wifiPassword.getText().toString(),
                new WifiUtility.WifiConnectionResult() {
                    @Override
                    public void onConnected(boolean hasInternet) {
                        if (!hasInternet) {
                            showWifiWarningDialog("There is no internet connection on the selected access point.");
                        } else {
                            testWifiThread = new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    getSensorIds();
                                }
                            });
                            testWifiThread.setName("startConfigureThread");
                            testWifiThread.start();
                        }
                    }

                    @Override
                    public void onCannotConnect() {
                        showWifiWarningDialog("Cannot connect to " + _wifiSSIDs.getSelectedItem().toString());
                        enableUI();
                    }
                });
    }

    Thread helloGuartinelThread;

    private void getSensorIds() {
        LOG.I("getSensorIds");

        for (final HardwareSensorImpl sensor : _sensors) {
            if (!sensor.isSelected) {
                continue;
            }
            sensor.setIsError(false);
            sensor.setErrorMessage("");
            updateConfigureStatus("Getting id from " + sensor.getName());
            final DoneSemaphore semaphore = new DoneSemaphore();
            sensor.helloGuartinel(getContext(), GuartinelApp.getConnector(), new HelloGuartinelCallback() {
                @Override
                public void onSuccess(String id) {
                    LOG.I("GetSensorIds=> onSuccess");
                    sensor.setId(id);
                    semaphore.isFinished = true;
                    updateConfigureStatus("Sensor ID retrieved " + sensor.getName());
                }

                @Override
                public void onConnectionError() {
                    LOG.I("GetSensorIds=> onConnectionError");
                    sensor.setIsError(true);
                    sensor.setErrorMessage("Cannot connect and get id.");
                    semaphore.isFinished = true;
                    updateRecyclerView();
                    enableUI();
                }
            });
            while (!semaphore.isFinished && fragmentRunning) {
                try {
                    Thread.sleep(100);
                    LOG.I("Waiting for getSensorIds semaphore.");
                } catch (InterruptedException e) {
                    LOG.I("Cannot sleep " + e.getMessage());
                }
            }
        }
        if (!fragmentRunning) {
            return;
        }
        helloGuartinelThread = new Thread(new Runnable() {
            @Override
            public void run() {
                registerSensors();
            }
        });
        helloGuartinelThread.setName("helloGuartinelThread");
        helloGuartinelThread.start();
    }

    Thread registerSensorThread;

    private void registerSensors() {
        if (getActivity() == null) {
            _configurationProgressBar.setVisibility(View.GONE);
            _startConfigButton.setVisibility(View.VISIBLE);
            return;
        }
        LOG.I("registerSensors");
        updateConfigureStatus("Connecting back to original wifi.");
        WifiConnectionManager.connectBackToOriginalWifi(getActivity(), new WifiUtility.WifiConnectionResult() {
            @Override
            public void onConnected(boolean hasInternet) {
                for (final HardwareSensorImpl sensor : _sensors) {
                    if (!sensor.isSelected) {
                        continue;
                    }
                    if (sensor.isError()) {
                        LOG.I("Skipping sensor : " + sensor.getName() + " due to error: " + sensor.getErrorMessage());
                        continue;
                    }

                    final DoneSemaphore semaphore = new DoneSemaphore();
                    updateConfigureStatus("Registering " + sensor.getName());
                    LOG.I("Registering " + sensor.getName());

                    GuartinelApp.getManagementServer().hardwareSupervisorRegisterHardware(GuartinelApp.getDataStore().getToken(), sensor.getId(), sensor.getName(), new RegisterHardwareSensorCallback() {
                        @Override
                        public void onConnectionError() {
                            semaphore.isFinished = true;
                            LOG.I("registerSensors " + sensor.getName() + " => onConnectionError");
                            sensor.setIsError(true);
                            sensor.setErrorMessage("Cannot connect and register sensor on server.");
                            updateRecyclerView();
                            enableUI();
                        }

                        @Override
                        public void onInvalidToken() {
                            semaphore.isFinished = true;
                            LOG.I("registerSensors " + sensor.getName() + " => onInvalidToken");
                            sensor.setIsError(true);
                            sensor.setErrorMessage("Cannot register sensor on the server.");
                            updateRecyclerView();
                            enableUI();
                        }

                        @Override
                        public void onSuccess(String updateServerHost, String updateServerProtocolPrefix, int updateServerPort, String hardwareType) {
                            LOG.I("registerSensors " + sensor.getName() + " => onSuccess");
                            semaphore.isFinished = true;
                            sensor.setUpdateServerHost(updateServerHost);
                            sensor.setUpdateServerPort(updateServerPort);
                            sensor.setUpdateServerProtocolPrefix(updateServerProtocolPrefix);
                            sensor.setHardwareType(hardwareType);
                        }

                        @Override
                        public void onInternalServerError() {
                            LOG.I("registerSensors " + sensor.getName() + " => onInternalServerError");
                            semaphore.isFinished = true;
                            LOG.printNotImplemented();
                            sensor.setIsError(true);
                            sensor.setErrorMessage("Cannot register sensor on the server.");
                            updateRecyclerView();
                            enableUI();
                        }
                    });

                    while (!semaphore.isFinished && fragmentRunning) {
                        try {
                            Thread.sleep(100);
                            LOG.I("Waiting for register semaphore.");
                        } catch (InterruptedException e) {
                            LOG.I("Cannot sleep " + e.getMessage());
                        }
                    }
                }
                if (!fragmentRunning) {
                    return;
                }
                registerSensorThread = new Thread(new Runnable() {
                    @Override
                    public void run() {
                        sendWifiConfigToSensors();
                    }
                });
                registerSensorThread.setName("registerSensorThread");
                registerSensorThread.start();
            }

            @Override
            public void onCannotConnect() {
                updateConfigureStatus("Sorry we could not connect back to the original wifi.");
                getActivity().runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        _configurationProgressBar.setVisibility(View.GONE);
                        _startConfigButton.setVisibility(View.VISIBLE);
                    }
                });
            }
        });
    }

    private void sendWifiConfigToSensors() {
        LOG.I("sendWifiConfigToSensors");
        for (final HardwareSensorImpl sensor : _sensors) {
            if (!sensor.isSelected) {
                continue;
            }
            if (sensor.isError()) {
                LOG.I("Skipping sensor : " + sensor.getName() + " due to error: " + sensor.getErrorMessage());
                continue;
            }
            LOG.I("Sending wifi config to: " + sensor.getName());
            updateConfigureStatus("Sending wifi config to " + sensor.getName());
            final DoneSemaphore semaphore = new DoneSemaphore();
            String selectedSSID = _wifiSSIDs.getSelectedItem().toString();
            if (selectedSSID.contains(Const.MANUAL_ENTRY_MARK)) {
                selectedSSID = selectedSSID.replace(Const.MANUAL_ENTRY_MARK, "");
            }
            sensor.setConfig(getContext(),
                    GuartinelApp.getConnector(),
                    selectedSSID,
                    _wifiPassword.getText().toString(),
                    sensor.getUpdateServerHost(),
                    sensor.getUpdateServerProtocolPrefix(),
                    sensor.getUpdateServerPort(),
                    sensor.getHardwareType(),
                    sensor.getName(),
                    new ConfigureHardwareSensorCallback() {
                        @Override
                        public void onConnectionError() {
                            semaphore.isFinished = true;
                            LOG.I("sendWifiConfigToSensors => onConnectionError");
                            sensor.setIsError(true);
                            sensor.setErrorMessage("Cannot connect and send wifi config to sensor.");
                            updateRecyclerView();
                            enableUI();
                        }

                        @Override
                        public void onConfigured() {
                            sensor.setIsConfigured(true);
                            semaphore.isFinished = true;
                            updateRecyclerView();
                            LOG.I("sendWifiConfigToSensors  => onConfigured");
                            updateConfigureStatus(sensor.getName() + " is configured.");
                        }
                    });
            while (!semaphore.isFinished && fragmentRunning) {
                try {
                    Thread.sleep(100);
                    LOG.I("Waiting for setConfig semaphore.");
                } catch (InterruptedException e) {
                    LOG.I("Cannot sleep " + e.getMessage());
                }
            }
            if (!fragmentRunning) {
                return;
            }
        }
        returnToOriginalWifi();
    }


    private void returnToOriginalWifi() {
        updateConfigureStatus("Connecting back to the original wifi.");

        WifiConnectionManager.connectBackToOriginalWifi(getContext(), new WifiUtility.WifiConnectionResult() {
            @Override
            public void onConnected(boolean hasInternet) {
                getActivity().runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        _configurationProgressBar.setVisibility(View.GONE);
                        _startConfigButton.setVisibility(View.VISIBLE);
                        _configurationTextView.setVisibility(View.GONE);
                        boolean hasAnyError = false;
                        for (HardwareSensorImpl sensor :
                                _sensors) {
                            if (sensor.isError()) {
                                hasAnyError = true;
                                break;
                            }
                        }
                        if (!hasAnyError) {
                            _startConfigButton.setText("Close");
                            _startConfigButton.setOnClickListener(new View.OnClickListener() {
                                @Override
                                public void onClick(View v) {
                                    dismiss();
                                }
                            });
                        }
                        enableUI();
                    }
                });
            }

            @Override
            public void onCannotConnect() {
                updateConfigureStatus("Sorry we could not connect back to the original wifi.");
                enableUI();
                getActivity().runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        _configurationProgressBar.setVisibility(View.GONE);
                        _startConfigButton.setVisibility(View.VISIBLE);
                    }
                });
            }
        });
    }


    private void showWifiWarningDialog(final String error) {
        if (getActivity() == null) {
            return;
        }
        getActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                @SuppressLint("RestrictedApi") AlertDialog.Builder builder = new AlertDialog.Builder(new ContextThemeWrapper(getActivity(), R.style.AlertDialogCustom));

                builder.setMessage("Do you want to bypass this problem and proceed anyway?");
                builder.setTitle(error);

                builder.setPositiveButton("YES", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int which) {
                        new Thread(new Runnable() {
                            @Override
                            public void run() {
                                getSensorIds();
                            }
                        }).start();
                        dialog.dismiss();
                    }
                });

                builder.setNegativeButton("NO", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        _configurationProgressBar.setVisibility(View.GONE);
                        _startConfigButton.setVisibility(View.VISIBLE);
                        _configurationTextView.setVisibility(View.GONE);
                        _recyclerViewAdapter.notifyDataSetChanged();
                        dialog.dismiss();
                    }
                });
                builder.setCancelable(false);
                AlertDialog alert = builder.create();
                alert.show();
            }
        });
    }

    private void doScan() {
        HardwareSensorSniffer.sniff(getActivity(), new HardwareSensorSniffer.onSniffingDone() {
            @Override
            public void done(List<HardwareSensorImpl> foundItems) {
                if (foundItems.size() == 0) {
                    _scanStatusTextView.setText("Could not find any guartinel devices nearby.");
                    _rescanButton.setVisibility(View.VISIBLE);
                    _scanProgressBar.setVisibility(View.GONE);
                    return;
                }
                _sensors.addAll(foundItems);
                _recyclerViewAdapter.notifyDataSetChanged();
                _scanningHardwareLinearLayout.setVisibility(View.GONE);
                _selectHardwareLayout.setVisibility(View.VISIBLE);
                _wifiSettingsLinearLayout.setVisibility(View.VISIBLE);
            }

            @Override
            public void onWIfiDisabled() {
                LOG.printNotImplemented();
            }
        });
    }

    @Override
    public void onDetach() {
        fragmentRunning = false;
        getActivity().getWindow().clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        WifiUtility.cleanAddedGuartinelAPs(getActivity());
        super.onDetach();
    }

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        boolean hasPermission = ((MainActivity) getActivity()).checkLocationPermission();
        if (!hasPermission) {
            dismiss();
            return;
        }
        List<ScanResult> ssids = WifiUtility.getWifiAccessPointsWithoutGuartinel(getActivity());
        for (int i = 0; i < ssids.size(); i++) {
            ScanResult currentScan = ssids.get(i);
            if(ssidEntries.contains(currentScan.SSID)){
                continue;
            }
            ssidEntries.add(currentScan.SSID);
        }
        doScan();
    }

    @NonNull
    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        Dialog dialog = super.onCreateDialog(savedInstanceState);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        return dialog;
    }

    private void killThreads() {
        ArrayList<Thread> threads = new ArrayList<>();
        threads.add(testWifiThread);
        threads.add(helloGuartinelThread);
        threads.add(startConfigureThread);
        threads.add(registerSensorThread);

        for (Thread thread : threads) {
            String name = "Thread";
            try {
                name = thread.getName();
                thread.join();
            } catch (Exception e) {
                LOG.E("Thread cannot be joined", e);
                continue;
            }
            try {
                thread.interrupt();
            } catch (Exception e) {
                LOG.E("Thread cannot be interrupted", e);
                continue;
            }
            LOG.I(name + " is stopped");
        }
    }


}