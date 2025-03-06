package sysment.guartinel.hardwareconfigurator.ui;

import android.content.DialogInterface;
import android.content.Intent;
import android.net.Uri;
import android.net.wifi.ScanResult;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v7.app.AlertDialog;
import android.text.InputFilter;
import android.text.InputType;
import android.text.Spanned;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.Switch;
import android.widget.TextView;

import com.google.zxing.integration.android.IntentIntegrator;
import com.mobsandgeeks.saripaar.ValidationError;
import com.mobsandgeeks.saripaar.Validator;
import com.mobsandgeeks.saripaar.annotation.NotEmpty;

import java.util.ArrayList;
import java.util.List;

import sysment.guartinel.hardwareconfigurator.R;
import sysment.guartinel.hardwareconfigurator.connection.HardwareConnector;
import sysment.guartinel.hardwareconfigurator.connection.ManagementServer;
import sysment.guartinel.hardwareconfigurator.connection.WifiHelper;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardware;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardwareAP;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardwareClient;
import sysment.guartinel.hardwareconfigurator.tools.LOG;


/**
 * Created by DAVT on 2017.12.06..
 */

public class ConfigureHardwareFragment extends Fragment implements Validator.ValidationListener {

    public static class QR_READ_REQUEST_CODES {
        public static int APPLICATION_TOKEN = 9573;
        public static int WIFI_PASSWORD = 3356;

    }

    public static class Const {
        public final static String MANUAL = "Manual";
        public final static String MANUAL_ENTRY_MARK = "(Manual)";
    }

    public static String TAG = "CONFIGURE_HARDWARE_FRAGMENT";
    GuartinelHardware hardware;
    protected Validator validator;

    public ConfigureHardwareFragment() {
    }

    public ConfigureHardwareFragment setHardwareInstance(GuartinelHardware hardware) {
        this.hardware = hardware;
        return this;
    }

    public void setHardwareToken(String token) {
        hardwareToken.setText(token);
    }

    public void setWifiPassword(String pass) {
        wifiPassword.setText(pass);
    }


    @NotEmpty
    EditText instanceNameEditText;
    @NotEmpty
    EditText hardwareToken;
    Spinner wifiSSIDs;
    @NotEmpty
    EditText wifiPassword;
    Button configureButton;
    ProgressBar bar;
    Switch httpsSwitch;

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setHasOptionsMenu(true);
    }

    Menu diagnosticsMenu;
    List<String> ssidEntries = new ArrayList<String>();

    public void onCreateOptionsMenu(Menu menu, MenuInflater inflater) {
        inflater.inflate(R.menu.menu, menu);
        diagnosticsMenu = menu;
        super.onCreateOptionsMenu(menu, inflater);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == R.id.getDiagnostics) {
            diagnosticsMenu.setGroupEnabled(R.menu.menu, false);
            HardwareConnector.getDiagnostics(getActivity(), hardware, new HardwareConnector.GetDiagnosticsListener() {
                @Override
                public void onSuccess(final String log) {
                    diagnosticsMenu.setGroupEnabled(R.menu.menu, true);

                    AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(getActivity());
                    alertDialogBuilder.setMessage(log);
                    alertDialogBuilder.setPositiveButton("OK",
                            new DialogInterface.OnClickListener() {
                                @Override
                                public void onClick(DialogInterface arg0, int arg1) {
                                }
                            });
                    alertDialogBuilder.setNegativeButton("Send in Email!",
                            new DialogInterface.OnClickListener() {
                                @Override
                                public void onClick(DialogInterface arg0, int arg1) {

                                    Intent intent = new Intent(Intent.ACTION_SENDTO);
                                    intent.setData(Uri.parse("mailto:")); // only email apps should handle this
                                    intent.putExtra(Intent.EXTRA_EMAIL, new String[]{"info@guartinel.com"});
                                    intent.putExtra(Intent.EXTRA_SUBJECT, "HardwareSupervisor diagnostics");
                                    intent.putExtra(Intent.EXTRA_TEXT, log);
                                    startActivity(intent);
                                }
                            });
                    AlertDialog alertDialog = alertDialogBuilder.create();
                    alertDialog.show();
                }

                @Override
                public void onSuccess() {

                }

                @Override
                public void onInvalidPassword() {

                }

                @Override
                public void onFailed() {
                    diagnosticsMenu.setGroupEnabled(R.menu.menu, true);
                }
            });
        }
        return true;
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View _view = inflater.inflate(R.layout.configure_cl_fragment, container, false);
        if (getActivity() == null) {
            return _view;
        }
        ((MainActivity) getActivity()).getSupportActionBar().setTitle(hardware.getName());
        validator = new Validator(this);
        validator.setValidationListener(this);

        instanceNameEditText = (EditText) _view.findViewById(R.id.instanceNameEditText);
        InputFilter filter = new InputFilter() {
            public CharSequence filter(CharSequence source, int start, int end,
                                       Spanned dest, int dstart, int dend) {
                for (int i = start; i < end; i++) {
                    if (Character.isWhitespace(source.charAt(i))) {
                        return "";
                    }
                }
                return null;
            }
        };

        instanceNameEditText.setFilters(new InputFilter[]{filter});
        hardwareToken = (EditText) _view.findViewById(R.id.hardwareToken);
        hardwareToken.setOnTouchListener(new DrawableClickListener.RightDrawableClickListener(hardwareToken) {
            @Override
            public boolean onDrawableClick() {
                IntentIntegrator integrator = new IntentIntegrator(getActivity());
                integrator.setOrientationLocked(true);
                integrator.setCaptureActivity(QrScanActivity.class);
                integrator.setBeepEnabled(true);
                integrator.setPrompt("Scan the package token on the website. Use volume buttons to switch flash.");
                integrator.initiateScan(IntentIntegrator.QR_CODE_TYPES);

                getActivity().startActivityForResult(integrator.createScanIntent(), QR_READ_REQUEST_CODES.APPLICATION_TOKEN);
                return true;
            }
        });
        wifiSSIDs = (Spinner) _view.findViewById(R.id.wifiSSIDs);
        configureButton = (Button) _view.findViewById(R.id.configureButton);
        bar = (ProgressBar) _view.findViewById(R.id.configureCliProgressBar);
        httpsSwitch = (Switch) _view.findViewById(R.id.useHTTPSSwitch);


        bar.setVisibility(View.GONE);

        List<ScanResult> ssids = WifiHelper.getLastScanResult(getActivity());
        for (int i = 0; i < ssids.size(); i++) {
            ScanResult currentScan = ssids.get(i);
            ssidEntries.add(currentScan.SSID);
        }
        ssidEntries.add(Const.MANUAL);

        final ArrayAdapter adapter = new ArrayAdapter<String>(getActivity(), android.R.layout.simple_list_item_1, ssidEntries);
        wifiSSIDs.setAdapter(adapter);

        wifiSSIDs.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                if (!ssidEntries.get(i).equals(Const.MANUAL)) {
                    return;
                }
                AlertDialog.Builder builder = new AlertDialog.Builder(getContext());
                builder.setTitle("Enter your ssid manually");

                final EditText input = new EditText(getContext());

                input.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_NORMAL);
                builder.setView(input);


                builder.setPositiveButton("OK", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        ssidEntries.add(0, input.getText().toString() + Const.MANUAL_ENTRY_MARK);
                        wifiSSIDs.setSelection(0);
                        adapter.notifyDataSetChanged();
                    }
                });
                builder.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        dialog.cancel();
                    }
                });
                builder.show();
            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {

            }
        });
        wifiPassword = (EditText) _view.findViewById(R.id.wifiPassword);
        wifiPassword.setOnTouchListener(new DrawableClickListener.RightDrawableClickListener(wifiPassword) {
            @Override
            public boolean onDrawableClick() {
                IntentIntegrator integrator = new IntentIntegrator(getActivity());
                integrator.setOrientationLocked(true);
                integrator.setCaptureActivity(QrScanActivity.class);
                integrator.setBeepEnabled(true);
                integrator.setPrompt("Scan your wifi password. Use volume buttons to switch flash.");
                integrator.initiateScan(IntentIntegrator.QR_CODE_TYPES);

                getActivity().startActivityForResult(integrator.createScanIntent(), QR_READ_REQUEST_CODES.WIFI_PASSWORD);
                return true;
            }
        });
        configureButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (wifiSSIDs.getSelectedItem() == null) {
                    wifiSSIDs.setSelection(0);
                    wifiSSIDs.setSelection(0, false);

                }
                validator.validate();
            }
        });
        hardwareToken.setText(hardware.hardwareToken);
        instanceNameEditText.setText(hardware.instanceName);
        return _view;
    }

    private void setLoading() {
        bar.setVisibility(View.VISIBLE);
        configureButton.setVisibility(View.GONE);
    }

    private void setLoaded() {
        bar.setVisibility(View.GONE);
        configureButton.setVisibility(View.VISIBLE);
    }

    @Override
    public void onValidationSucceeded() {
        setLoading();
        hardware.hardwareToken = hardwareToken.getText().toString();
        hardware.wifiPassword = wifiPassword.getText().toString();
        hardware.instanceName = instanceNameEditText.getText().toString();
        hardware.useHTTPS = httpsSwitch.isChecked();
        if (!wifiSSIDs.getSelectedItem().toString().contains(Const.MANUAL_ENTRY_MARK)) {
            hardware.wifiSSID = wifiSSIDs.getSelectedItem().toString();
            LOG.I("Selected SSID :" + hardware.wifiSSID);
            LOG.I("Check hw token and instance ID on MS");
            WifiHelper.connectToWifi(getActivity(), hardware.wifiSSID, hardware.wifiPassword, new WifiHelper.WifiConnectionResult() {
                @Override
                public void onConnected() {
                    validateOnManagementServer();
                }

                @Override
                public void onCannotConnect() {
                    setLoaded();
                    if (getActivity() == null) {
                        return;
                    }
                    ((MainActivity) getActivity()).showSnackbarError("Invalid wifi password.");
                }
            });
        } else {
            LOG.I("Selected SSID :" + hardware.wifiSSID);
            hardware.wifiSSID = wifiSSIDs.getSelectedItem().toString().replace(Const.MANUAL_ENTRY_MARK, "");
            startConfigure();
        }

    }

    private void validateOnManagementServer() {
        LOG.I("Connected back to the original wifi");
        ManagementServer.validate(getActivity(), hardware, new ManagementServer.ManagementServerResponseListener() {
            @Override
            public void onSuccess() {
                LOG.I("Data validated!");
                if (hardware instanceof  GuartinelHardwareClient && !WifiHelper.isConnectedWifiIsTheOriginal(getActivity())) {
                    LOG.I("Connecting back to the original wifi");
                    WifiHelper.connectBackToOriginalWifi(getActivity());
                    while (!WifiHelper.isConnectedWifiIsTheOriginal(getContext())) {
                        LOG.I("Waiting to connect back..");

                        try {
                            Thread.sleep(200);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }
                }
                LOG.I("Starting to configure");
                startConfigure();
            }

            @Override
            public void onInvalidToken() {
                setLoaded();
                if (getActivity() == null) {
                    return;
                }
                ((MainActivity) getActivity()).showSnackbarError("Invalid package token.");
            }

            @Override
            public void onFailed() {
                setLoaded();
                if (getActivity() == null) {
                    return;
                }
                ((MainActivity) getActivity()).showSnackbarError("Cannot validate data on the server.");
            }
        });
    }

    private void startConfigure() {
        HardwareConnector.configureHardware(getActivity(), hardware, new HardwareConnector.HardwareResponseListener() {
            @Override
            public void onSuccess() {
                if (getActivity() == null) {
                    return;
                }
                ((MainActivity) getActivity()).afterDeviceConfigured();
            }

            @Override
            public void onInvalidPassword() {
                setLoaded();
                if (getActivity() == null) {
                    return;
                }
                ((MainActivity) getActivity()).showSnackbarError("Password is invalid.");
                bar.setVisibility(View.INVISIBLE);
                configureButton.setVisibility(View.VISIBLE);
            }

            @Override
            public void onFailed() {
                setLoaded();
                if (getActivity() == null) {
                    return;
                }
                ((MainActivity) getActivity()).showSnackbarError("Cannot connect to device.");
                bar.setVisibility(View.INVISIBLE);
                configureButton.setVisibility(View.VISIBLE);
            }
        });
    }

    @Override
    public void onValidationFailed(List<ValidationError> errors) {
        bar.setVisibility(View.GONE);
        configureButton.setVisibility(View.VISIBLE);
        for (ValidationError error : errors) {
            View view = error.getView();
            String message = error.getCollatedErrorMessage(getActivity());
            // Display error messages
            if (view instanceof Spinner) {
                Spinner sp = (Spinner) view;
                view = ((LinearLayout) sp.getSelectedView()).getChildAt(0);        // we are actually interested in the text view spinner has
            }

            if (view instanceof TextView) {
                TextView et = (TextView) view;
                et.setError(message);
            }
        }

    }
}
