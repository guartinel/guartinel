package sysment.guartinel.hardwareconfigurator.ui;

import android.Manifest;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.support.annotation.Nullable;
import android.os.Bundle;
import android.support.design.widget.CoordinatorLayout;
import android.support.design.widget.Snackbar;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;


import android.view.WindowManager;

import sysment.guartinel.hardwareconfigurator.R;
import sysment.guartinel.hardwareconfigurator.connection.HardwareConnector;
import sysment.guartinel.hardwareconfigurator.connection.WifiHelper;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardware;
import sysment.guartinel.hardwareconfigurator.tools.LOG;

public class MainActivity extends AppCompatActivity implements HardwareSelectionListener {
    public final int LOCATION_PERMISSION_REQUEST_CODE = 666;

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions,
                                           int[] grantResults) {
        if (requestCode == 001 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
        }
    }

    @Override
    public void onBackPressed() {
        if (getSupportFragmentManager().getBackStackEntryCount() == 1) {
            new AlertDialog.Builder(this)
                    .setTitle("Exit")
                    .setMessage("Are you really want to exit?")
                    .setPositiveButton("Yes", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialogInterface, int i) {
                            finish();
                        }
                    }).setNegativeButton("No", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialogInterface, int i) {

                }
            })
                    .create()
                    .show();
        } else {
            super.onBackPressed();
        }
    }

    public void showSnackbarError(String message) {
        Snackbar.make(mainCoordinatorLayout, message, Snackbar.LENGTH_LONG).show();
    }

    public boolean checkLocationPermission() {
        if (ContextCompat.checkSelfPermission(this,
                Manifest.permission.ACCESS_FINE_LOCATION)
                != PackageManager.PERMISSION_GRANTED) {

            // Should we show an explanation?
            if (ActivityCompat.shouldShowRequestPermissionRationale(this,
                    Manifest.permission.ACCESS_FINE_LOCATION)) {

                // Show an explanation to the user *asynchronously* -- don't block
                // this thread waiting for the user's response! After the user
                // sees the explanation, try again to request the permission.
                new AlertDialog.Builder(this)
                        .setTitle("Please provide location permission")
                        .setMessage("This is needed for the wifi scan result.")
                        .setPositiveButton("ok", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialogInterface, int i) {
                                //Prompt the user once explanation has been shown
                                ActivityCompat.requestPermissions(MainActivity.this,
                                        new String[]{Manifest.permission.ACCESS_FINE_LOCATION},
                                        LOCATION_PERMISSION_REQUEST_CODE);
                            }
                        })
                        .create()
                        .show();


            } else {
                // No explanation needed, we can request the permission.
                ActivityCompat.requestPermissions(this,
                        new String[]{Manifest.permission.ACCESS_FINE_LOCATION},
                        LOCATION_PERMISSION_REQUEST_CODE);
            }
            return false;
        } else {
            return true;
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (data == null) {
            return;
        }
        String scanResultRaw = data.getExtras().getString("SCAN_RESULT");

        if (requestCode == ConfigureHardwareFragment.QR_READ_REQUEST_CODES.APPLICATION_TOKEN) {
            Fragment fragment = getSupportFragmentManager().findFragmentByTag(ConfigureHardwareFragment.TAG);
            if (fragment == null) {
                LOG.I("Cannot find fragment by name: " + ConfigureHardwareFragment.TAG);
                debugFragments();
                return;
            }
            ((ConfigureHardwareFragment) fragment).setHardwareToken(scanResultRaw);
        }
        if (requestCode == ConfigureHardwareFragment.QR_READ_REQUEST_CODES.WIFI_PASSWORD) {
            Fragment fragment = getSupportFragmentManager().findFragmentByTag(ConfigureHardwareFragment.TAG);
            if (fragment == null) {
                LOG.I("Cannot find fragment by name: " + ConfigureHardwareFragment.TAG);
                debugFragments();
                return;
            }
            ((ConfigureHardwareFragment) fragment).setWifiPassword(scanResultRaw);
        }
        if (requestCode == LoginHardwareFragment.QR_READ_REQUEST_CODES.DEVICE_PASSWORD) {
            Fragment fragment = getSupportFragmentManager().findFragmentByTag(LoginHardwareFragment.TAG);
            if (fragment == null) {
                LOG.I("Cannot find fragment by name: " + LoginHardwareFragment.TAG);
                debugFragments();
                return;
            }
            ((LoginHardwareFragment) fragment).setDevicePassword(scanResultRaw);
        }
    }

    CoordinatorLayout mainCoordinatorLayout;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        HardwareConnector.Revive();
        mainCoordinatorLayout = (CoordinatorLayout) findViewById(R.id.mainCoordinatorLayout);
        getSupportFragmentManager().beginTransaction().addToBackStack(SelectHardwareFragment.TAG).replace(R.id.fragment_container, new SelectHardwareFragment(), SelectHardwareFragment.TAG).commit();
        getSupportFragmentManager().executePendingTransactions();
        checkLocationPermission();
        getSupportFragmentManager().addOnBackStackChangedListener(new FragmentManager.OnBackStackChangedListener() {
            @Override
            public void onBackStackChanged() {
                debugFragments();
            }
        });
    }

    @Override
    public void onSelected(GuartinelHardware hardware) {
        getSupportFragmentManager().beginTransaction().addToBackStack(LoginHardwareFragment.TAG).replace(R.id.fragment_container, new LoginHardwareFragment().setHardware(hardware), LoginHardwareFragment.TAG).commit();
        getSupportFragmentManager().executePendingTransactions();
    }

    public void afterDeviceConfigured() {
        showSnackbarError("Configuration sent please wait until the changes take effect.");
        getSupportFragmentManager().popBackStack();
        getSupportFragmentManager().popBackStack();
        getSupportFragmentManager().executePendingTransactions();
        WifiHelper.reconnectWifi(this);
    }

    public void afterSuccessFullLogin(GuartinelHardware hardware) {
        getSupportFragmentManager().beginTransaction().addToBackStack(ConfigureHardwareFragment.TAG).replace(R.id.fragment_container, new ConfigureHardwareFragment().setHardwareInstance(hardware), ConfigureHardwareFragment.TAG).commit();
        getSupportFragmentManager().executePendingTransactions();
     //   WifiHelper.connectBackToOriginalWifi(this);
    }

    private void debugFragments() {
        int fragmentCount = getSupportFragmentManager().getBackStackEntryCount();
        LOG.I("Fragments count on backstack: " + fragmentCount);
        for (int i = 0; i < fragmentCount; i++) {
            FragmentManager.BackStackEntry entry = getSupportFragmentManager().getBackStackEntryAt(i);
            LOG.I("Entry ID: " + entry.getId() + " entry name: " + entry.getName());
        }
    }

    @Override
    protected void onDestroy() {
        try {
            WifiHelper.connectBackToOriginalWifi(this);
        } catch (Exception e) {
        }
        super.onDestroy();
    }
}
