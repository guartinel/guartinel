package sysment.guartinelandroidsystemwatcher.ui;

import android.os.Bundle;
import android.support.v4.app.FragmentManager;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.DefaultItemAnimator;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.RelativeLayout;
import android.widget.Toast;

import com.squareup.otto.Subscribe;

import sysment.guartinelandroidsystemwatcher.R;
import sysment.guartinelandroidsystemwatcher.SystemWatcherApp;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstance;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstances;
import sysment.guartinelandroidsystemwatcher.service.ServerChecker;
import sysment.guartinelandroidsystemwatcher.ui.busEvents.ServerInstancesUpdatedEvent;

public class MainActivity extends AppCompatActivity implements IActivity {//implements OnDataRefreshedListener {

   private MainActivity activity;
    private ServerInstances serverInstanceList = new ServerInstances();
    private RecyclerView recyclerView;
    private ServerRecyclerViewAdapter mAdapter;
    private RelativeLayout addServerLayout;
    private ImageButton addServerButton;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
activity  = this;
        serverInstanceList.addAll(SystemWatcherApp.getDataStore().getServerInstances(this));
        if (serverInstanceList.size() == 0) {
            Toast.makeText(this, "You should add a server at the right top corner menu", Toast.LENGTH_LONG).show();
        }
        recyclerView = (RecyclerView) findViewById(R.id.serverInstancesRecyclerView);
        mAdapter = new ServerRecyclerViewAdapter(serverInstanceList, this);
        RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(getApplicationContext());
        recyclerView.setLayoutManager(mLayoutManager);
        recyclerView.setItemAnimator(new DefaultItemAnimator());
        recyclerView.setAdapter(mAdapter);

        addServerLayout = (RelativeLayout) findViewById(R.id.mainActivityNoServerAddedLayout);
        addServerButton = (ImageButton) findViewById(R.id.mainActivityAddServerButton);
        addServerButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                openServerInstanceConfiguration(new ServerInstance());
            }
        });
        getSupportActionBar().setTitle("Guartinel System Watcher");
        showAddServerLayoutIfNeeded();
    }

    @Override
    protected void onStart() {
        super.onStart();
        SystemWatcherApp.getBus().register(this);
        ServerInstances serverInstances = SystemWatcherApp.getDataStore().getServerInstances(this);
        serverInstanceList.clear();
        serverInstanceList.addAll(serverInstances);
        mAdapter.notifyDataSetChanged();

    }

    @Override
    protected void onPause() {
        SystemWatcherApp.getBus().unregister(this);
        super.onPause();
    }
    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_BACK) {
            //preventing default implementation previous to android.os.Build.VERSION_CODES.ECLAIR
            return true;
        }
        return super.onKeyDown(keyCode, event);
    }
    @Subscribe
    public void onServerInstancesUpdated(final ServerInstancesUpdatedEvent event) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                serverInstanceList.clear();
                serverInstanceList.addAll(event.serverInstances);
                mAdapter.notifyDataSetChanged();
                showAddServerLayoutIfNeeded();
            }
        });
    }

    private void showAddServerLayoutIfNeeded() {
        if (serverInstanceList.size() == 0) {
            addServerLayout.setVisibility(View.VISIBLE);
        } else {
            addServerLayout.setVisibility(View.GONE);
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        FragmentManager fragmentManager = getSupportFragmentManager();

        switch (item.getItemId()) {
            case R.id.addServer:
                openServerInstanceConfiguration(new ServerInstance());
                break;
            case R.id.settings:
                SettingsFragment settingsFragment = new SettingsFragment();
                settingsFragment.setSettings(SystemWatcherApp.getDataStore().getSettings(this));
                settingsFragment.show(fragmentManager, "Settings Fragment");
                break;
            case R.id.checkNow:
                new Thread(new Runnable() {
                    @Override
                    public void run() {
                        new ServerChecker().check(activity);
                    }
                }).start();
                break;
        }
        return true;
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.menu, menu);
        return true;
    }

    @Override
    public void openServerInstanceConfiguration(ServerInstance serverInstance) {
        ConfigureServerDialogFragment configureServerDialogFragment = new ConfigureServerDialogFragment();
        configureServerDialogFragment.setServer(serverInstance);
        configureServerDialogFragment.show(getSupportFragmentManager(), "Server Configuration Fragment");
    }

   /*@Override
    public void onDataRefreshed() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                configureServerView();
            }
        });
    }*/

}
