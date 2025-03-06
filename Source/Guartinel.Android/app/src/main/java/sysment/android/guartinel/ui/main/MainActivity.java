package sysment.android.guartinel.ui.main;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.graphics.drawable.Drawable;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.support.design.widget.CoordinatorLayout;
import android.support.design.widget.TabLayout;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.content.ContextCompat;
import android.support.v4.view.ViewPager;
import android.support.v7.app.AlertDialog;
import android.support.v7.view.ContextThemeWrapper;
import android.support.v7.widget.Toolbar;
import android.text.SpannableString;
import android.text.Spanned;
import android.view.LayoutInflater;
import android.view.View;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.TextView;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.R;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.network.WifiUtility;
import sysment.android.guartinel.ui.MyNotificationManager;
import sysment.android.guartinel.ui.main.fragments.configureSensor.ConfigureDevicesDialogFragment;
import sysment.android.guartinel.ui.main.fragments.dashboard.DashBoardFragment;
import sysment.android.guartinel.ui.main.fragments.ManageAccountFragment;
import sysment.android.guartinel.ui.main.fragments.SettingsFragment;
import sysment.android.guartinel.ui.SuperActivity;
import sysment.android.guartinel.ui.presenterCallbacks.ValidateTokenCallback;

public class MainActivity extends SuperActivity {

    public class ShouldVibrateSemaphore {
        boolean shouldRun = true;
    }

    private TabLayout tabLayout;
    private ViewPager viewPager;
    private CoordinatorLayout _coordinatorLayout;

    ViewPagerAdapter adapter;
    public final int LOCATION_PERMISSION_REQUEST_CODE = 666;
    public static final int REQUEST_WRITE_EXTERNAL_STORAGE_CODE = 777;
    Fragment selectedFragment = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
       // getWindow().addFlags(WindowManager.LayoutParams.FLAG_SHOW_WHEN_LOCKED);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        _coordinatorLayout = (CoordinatorLayout) findViewById(R.id.mainActivityCoordinatorLayout);

        setSupportActionBar(toolbar);
        viewPager = (ViewPager) findViewById(R.id.viewpager);
        setupViewPager(viewPager);
        viewPager.addOnPageChangeListener(new ViewPager.OnPageChangeListener() {
            @Override
            public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {

            }

            @Override
            public void onPageSelected(int position) {
                selectedFragment = adapter.getItem(position);
                getSupportActionBar().setTitle(adapter.getFragmentTitleList().get(position));
            }

            @Override
            public void onPageScrollStateChanged(int state) {

            }
        });
        tabLayout = (TabLayout) findViewById(R.id.tabs);
        tabLayout.setupWithViewPager(viewPager);
        getSupportActionBar().setTitle(adapter.getFragmentTitleList().get(0));
        getSupportActionBar().hide();
        hideKeyboard();
        registerReceiver(new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                intent.setClass(context, MainActivity.class);
                startActivity(intent);
            }
        }, new IntentFilter(MyNotificationManager.Constants.GCM_MESSAGE_BROADCAST));
        onNewIntent(getIntent());
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent);
        try {
            showAlertDialogIfNeeded();
        } catch (IOException e) {
            LOG.E("Cannot show alert dialog", e);
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (data == null) {
            return;
        }
    }

    @Override
    protected void onPause() {
        // WifiUtility.stopDebug(getSuperContext());
        super.onPause();
    }

    @SuppressLint("RestrictedApi")
    public boolean checkLocationPermission() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED) {
            return true;
        }
        // Should we show an explanation?
        if (ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission.ACCESS_FINE_LOCATION)) {

            // Show an explanation to the user *asynchronously* -- don't block
            // this thread waiting for the user's response! After the user
            // sees the explanation, try again to request the permission.
            new AlertDialog.Builder(new ContextThemeWrapper(MainActivity.this, R.style.AlertDialogCustom))
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
    }

    @Override
    public void onBackPressed() {
        if (selectedFragment != null && selectedFragment instanceof ManageAccountFragment) {
            ((ManageAccountFragment) selectedFragment).goBack();
            return;
        }
        super.onBackPressed();
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
    }

    @Override
    protected void onResume() {
        super.onResume();
        // WifiUtility.startDebug(getSuperContext());
        LOG.I("Validating token");
        GuartinelApp.getManagementServer().accountValidateToken(GuartinelApp.getDataStore().getToken(), new ValidateTokenCallback() {
            @Override
            public void onInvalidToken() {
                LOG.I("Token is invalid. Restarting app.");
                Intent i = getBaseContext().getPackageManager()
                        .getLaunchIntentForPackage(getBaseContext().getPackageName());
                i.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
                i.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TASK);
                startActivity(i);
            }

            @Override
            public void onSuccess() {
                LOG.I("Token is OK");
            }

            @Override
            public void onConnectionError() {

            }
        });
        manageAccountFragment.refreshToken();
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions,
                                           int[] grantResults) {
        if (requestCode == LOCATION_PERMISSION_REQUEST_CODE && grantResults.length != 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            FragmentTransaction ft = getSupportFragmentManager().beginTransaction();
            Fragment prev = getSupportFragmentManager().findFragmentByTag("dialog");
            if (prev != null) {
                ft.remove(prev);
            }
            ft.addToBackStack(null);
            WifiUtility.saveCurrentWifiSSID(this);
            ConfigureDevicesDialogFragment dialogFragment = new ConfigureDevicesDialogFragment();
            dialogFragment.show(ft, "dialog");
        }
        if (requestCode == LOCATION_PERMISSION_REQUEST_CODE && grantResults.length != 0 && grantResults[0] == PackageManager.PERMISSION_DENIED) {

            showSnackBarError(_coordinatorLayout, "Without permission we cannot search wifi devices.", "Retry", new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    checkLocationPermission();
                }
            });
        }

        if (requestCode == REQUEST_WRITE_EXTERNAL_STORAGE_CODE && grantResults.length != 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            manageAccountFragment.retryDownload();
        }
        if (requestCode == REQUEST_WRITE_EXTERNAL_STORAGE_CODE && grantResults.length != 0 && grantResults[0] == PackageManager.PERMISSION_DENIED) {

            showSnackBarError(_coordinatorLayout, "Without permission we cannot download the file", "Retry", new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    manageAccountFragment.retryDownload();
                }
            });
        }
    }

    SettingsFragment settingsFragment;
    DashBoardFragment dashBoardFragment;
    ManageAccountFragment manageAccountFragment;

    private void setupViewPager(ViewPager viewPager) {
        adapter = new ViewPagerAdapter(getSupportFragmentManager(), MainActivity.this);
        dashBoardFragment = (DashBoardFragment) getSupportFragmentManager().findFragmentByTag(DashBoardFragment.TAG);
        if (dashBoardFragment == null) {
            dashBoardFragment = new DashBoardFragment();
        }
        settingsFragment = (SettingsFragment) getSupportFragmentManager().findFragmentByTag(SettingsFragment.TAG);
        if (settingsFragment == null) {
            settingsFragment = new SettingsFragment();
        }
        manageAccountFragment = (ManageAccountFragment) getSupportFragmentManager().findFragmentByTag(ManageAccountFragment.TAG);
        if (manageAccountFragment == null) {
            manageAccountFragment = new ManageAccountFragment();
        }
        adapter.addFragment(dashBoardFragment.setActivity(this), DashBoardFragment.TAG);
        adapter.addFragment(manageAccountFragment, ManageAccountFragment.TAG);
        adapter.addFragment(settingsFragment, SettingsFragment.TAG);
        viewPager.setAdapter(adapter);
        viewPager.setOffscreenPageLimit(3);
    }

    private boolean isAlertDialogOpen = false;

    private void showAlertDialogIfNeeded() throws IOException {
        if (getIntent().getExtras() == null) {
            return;
        }
        if (isAlertDialogOpen) {
            return;
        }

        String alertMessage = getIntent().getExtras().getString(MyNotificationManager.Constants.MESSAGE);
        final int notificationId = getIntent().getExtras().getInt(MyNotificationManager.Constants.NOTIFICATION_ID);

        if (alertMessage == null) {
            return;
        }

        isAlertDialogOpen = true;
        getIntent().replaceExtras(new Bundle());

        Uri notification = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);

        final MediaPlayer player = new MediaPlayer();// MediaPlayer.create(this, notification);
        player.setAudioStreamType(AudioManager.STREAM_MUSIC);
        player.setDataSource(this, notification);
        player.setVolume(50, 50);
        player.prepare();
        player.setLooping(true);
        player.start();

        final ShouldVibrateSemaphore shouldVibrateSemaphore = new ShouldVibrateSemaphore();

        final AlertDialog dialogBuilder = new AlertDialog.Builder(this).create();
        LayoutInflater inflater = ((Activity) this).getLayoutInflater();
        View dialogView = inflater.inflate(R.layout.forced_alert_dialog, null);
        TextView messageTextView = (TextView) dialogView.findViewById(R.id.forced_alert_dialog_message_text_view);
        messageTextView.setText(alertMessage);
         Button snoozeButton = (Button) dialogView.findViewById(R.id.forced_alert_dialog_snooze_button);
        snoozeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                player.stop();
                shouldVibrateSemaphore.shouldRun = false;
                isAlertDialogOpen = false;
                MyNotificationManager.cancelNotification(MainActivity.this, notificationId);
                dialogBuilder.dismiss();
            }
        });
        dialogBuilder.setCancelable(false);
        dialogBuilder.setView(dialogView);
        dialogBuilder.show();




       // @SuppressLint("RestrictedApi") AlertDialog.Builder builder = new AlertDialog.Builder(new ContextThemeWrapper(this, R.style.AlertDialogCustom)); //alert for confirm to delete
      //  builder.setMessage(alertMessage);    //set message


       /* builder.setPositiveButton("OK", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {

                return;
            }
        }).setCancelable(false).show();*/

        final Vibrator v = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);

        new Thread(new Runnable() {
            @Override
            public void run() {
                while (shouldVibrateSemaphore.shouldRun) {
                    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                        v.vibrate(VibrationEffect.createOneShot(200, VibrationEffect.DEFAULT_AMPLITUDE));
                    } else {
                        //deprecated in API 26
                        v.vibrate(200);
                    }
                    try {
                        Thread.sleep(200);
                    } catch (InterruptedException e) {
                        LOG.E("Thread is not sleepy. Cannot make it slep", e);
                    }
                }
            }
        }).start();
    }

    public class ViewPagerAdapter extends FragmentPagerAdapter {
        private final List<Fragment> mFragmentList = new ArrayList<>();
        private final List<String> mFragmentTitleList = new ArrayList<>();

        public List<String> getFragmentTitleList() {
            return mFragmentTitleList;
        }

        Context context;
        private int[] imageResId = {
                R.drawable.alerts,
                R.drawable.account,
                R.drawable.settings

        };

        private String[] pageTitles = {
                "Alert",
                "Manage",
                "Settings"

        };

        public ViewPagerAdapter(FragmentManager manager, Context _context) {
            super(manager);
            this.context = _context;
        }

        @Override
        public Fragment getItem(int position) {
            return mFragmentList.get(position);
        }

        @Override
        public int getCount() {
            return mFragmentList.size();
        }

        public void addFragment(Fragment fragment, String title) {
            mFragmentList.add(fragment);
            mFragmentTitleList.add(title);
        }

        @Override
        public CharSequence getPageTitle(int position) {
            Drawable image = ContextCompat.getDrawable(context, imageResId[position]);//          AppCompatDrawableManager.get().getDrawable(context,imageResId[position]);//
            image.setBounds(0, 0, image.getIntrinsicWidth(), image.getIntrinsicHeight());
            SpannableString spannableString = new SpannableString("  " + pageTitles[position]);
            CenteredImageSpan imageSpan = new CenteredImageSpan(context, imageResId[position]);

            spannableString.setSpan(imageSpan, 0, 1, Spanned.SPAN_INCLUSIVE_EXCLUSIVE);
            return spannableString;
        }
    }
}