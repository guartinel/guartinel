package sysment.android.guartinel.ui.main.fragments.dashboard;

import android.annotation.SuppressLint;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.res.Configuration;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.v4.app.Fragment;
import android.support.v4.content.ContextCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.AlertDialog;
import android.support.v7.view.ContextThemeWrapper;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.helper.ItemTouchHelper;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.List;

import sysment.android.guartinel.core.gcm.MyFireBaseMessagingService;
import sysment.android.guartinel.core.persistance.Alert;
import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.R;
import sysment.android.guartinel.ui.main.MainActivity;

/**
 * Created by sysment_dev on 02/26/2016.
 */
public class DashBoardFragment extends Fragment {
    public static final String TAG = "Guartinel Alert Dashboard";
    private static Context _context;
    private View _view;
    private RecyclerViewAdapter _adapter;
    private RecyclerView _recyclerView;
    private List<Alert> _alertsList;
    private LinearLayoutManager linearLayoutManager;
    private MainActivity _activity;

    private TextView _dashBoardPageTitle;
    private ImageView _noAlertImage;
    public BroadcastReceiver _gcmBroadcastReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            LOG.I("_gcmBroadcastReceiver.onReceive");
            if (_alertsList == null) {
                return;
            }
            refreshAlertsListFromDB();
        }
    };

    private void refreshAlertsListFromDB() {
        List<Alert> currentAlerts = GuartinelApp.getDataStore().getAlertHolder().alerts;
        _alertsList.clear();
        _alertsList.addAll(currentAlerts);

        if (_activity == null) {
            LOG.I("_activity is null. Skipping to show new alert on the recyclerview");
            return;
        }
        _activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                _adapter.notifyDataSetChanged();
                setPageTitle();
            }
        });
    }

    private void setPageTitle() {
        if (_adapter.getItemCount() == 0) {
            _dashBoardPageTitle.setVisibility(View.VISIBLE);
            _dashBoardPageTitle.setText("You currently don't have any alert.");
            _noAlertImage.setVisibility(View.VISIBLE);
            return;
        }
        _dashBoardPageTitle.setText("");
        _dashBoardPageTitle.setVisibility(View.GONE);
        _noAlertImage.setVisibility(View.GONE);
    }

    public DashBoardFragment() {}

    private void setupRecyclerView(View view) {
        _recyclerView = (RecyclerView) view.findViewById(R.id.dash_board_recycler_view);
        _recyclerView.setHasFixedSize(true);

        _dashBoardPageTitle = (TextView) view.findViewById(R.id.dashBoardPageTitle);
        _noAlertImage = (ImageView) view.findViewById(R.id.noAlertImage);

        linearLayoutManager = new LinearLayoutManager(getContext());
        linearLayoutManager.setOrientation(LinearLayoutManager.VERTICAL);
        _recyclerView.setLayoutManager(linearLayoutManager);

        _alertsList = GuartinelApp.getDataStore().getAlertHolder().alerts;
        _adapter = new RecyclerViewAdapter(_alertsList);
        _recyclerView.setAdapter(_adapter);
        ItemTouchHelper.SimpleCallback simpleCallback = new ItemTouchHelper.SimpleCallback(0, ItemTouchHelper.LEFT | ItemTouchHelper.RIGHT) {
            @Override
            public boolean onMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target) {
                return false;
            }

            @Override
            public void onSwiped(final RecyclerView.ViewHolder viewHolder, int direction) {
                final int position = viewHolder.getAdapterPosition(); //get position which is swipe

                @SuppressLint("RestrictedApi") AlertDialog.Builder builder =  new AlertDialog.Builder(new ContextThemeWrapper(_context,R.style.AlertDialogCustom)); //alert for confirm to delete
                builder.setMessage("Are you sure to delete?");    //set message

                builder.setPositiveButton("REMOVE", new DialogInterface.OnClickListener() { //when click on DELETE
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        _adapter.notifyItemRemoved(position);    //item removed from recylcerview
                        GuartinelApp.getDataStore().removeAlertById(_alertsList.get(position).UUID);
                        refreshAlertsListFromDB();
                        return;
                    }
                }).setNegativeButton("CANCEL", new DialogInterface.OnClickListener() {  //not removing items if cancel is done
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        _adapter.notifyItemRemoved(position + 1);    //notifies the RecyclerView Adapter that data in adapter has been removed at a particular position.
                        _adapter.notifyItemRangeChanged(position, _adapter.getItemCount());   //notifies the RecyclerView Adapter that positions of element in adapter has been changed from position(removed element index to end of list), please update it.
                        return;
                    }
                }).setCancelable(false).show();  //show alert dialog
            }
        };
        ItemTouchHelper itemTouchHelper = new ItemTouchHelper(simpleCallback);
        itemTouchHelper.attachToRecyclerView(_recyclerView); //set swipe to recylcerview
        refreshAlertsListFromDB();
    }

    public DashBoardFragment setActivity(MainActivity activity) {
        _activity = activity;
        return this;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        _context = getContext();
    }
    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        refreshAlertsListFromDB();
    }

    @Override
    public void onResume() {
        super.onResume();
        _context = getContext();
        LocalBroadcastManager.getInstance(getActivity()).registerReceiver(_gcmBroadcastReceiver, new IntentFilter(MyFireBaseMessagingService.BroadcastConst.BROADCAST_ID));
        refreshAlertsListFromDB();
    }

    @Override
    public void onPause() {
        super.onPause();
        LocalBroadcastManager.getInstance(getActivity()).unregisterReceiver(_gcmBroadcastReceiver);
    }

    @Override
    public void onDestroy() {
        _view = null;
        LocalBroadcastManager.getInstance(getActivity()).unregisterReceiver(_gcmBroadcastReceiver);
        super.onDestroy();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        _view = inflater.inflate(R.layout.dashboard_fragment, container, false);
        setupRecyclerView(_view);

        FloatingActionButton fab = (FloatingActionButton) _view.findViewById(R.id.fab);
        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                showConfirmDialog();
            }
        });
        return _view;
    }

    private void showConfirmDialog() {
        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(
                _context, R.style.AppTheme_Dark_Dialog);
        alertDialogBuilder.setTitle("Confirm");
        alertDialogBuilder
                .setMessage("Are you sure to clear all alerts?")
                .setCancelable(false)
                .setPositiveButton("Yes", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        GuartinelApp.getDataStore().clearAlertHolder();
                        _alertsList.clear();
                        if (_activity == null) {
                            _activity = (MainActivity) getActivity();
                            if (_activity == null) {
                                dialog.cancel();
                                return;
                            }
                        }
                        refreshAlertsListFromDB();
                        dialog.cancel();
                    }
                })
                .setNegativeButton("No", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {

                        dialog.cancel();
                    }
                });
        AlertDialog alertDialog = alertDialogBuilder.create();
        alertDialog.show();
    }


    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        _recyclerView.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrollStateChanged(RecyclerView recyclerView, int newState) { // 1 or 2=scroll 0 = stopped
                super.onScrollStateChanged(recyclerView, newState);
                if (newState == 0) {
                    if (_activity != null) {
                        //  _activity.showToolbar();
                    }
                }
            }

            @Override
            public void onScrolled(RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
               /* if(dy > 10 ||dy < -10){ // Disabled
                    _activity.hideScrollBar();
                }*/
            }
        });
    }

    public static Drawable getAlertDrawAble() {
        return ContextCompat.getDrawable(_context, R.drawable.alert_icon);
    }
    public static Drawable getWarningDrawable() {
        return ContextCompat.getDrawable(_context, R.drawable.warning_icon);
    }
    public static Drawable getRecoveryDrawable() {
       return ContextCompat.getDrawable(_context, R.drawable.check_icon);
    }
}
