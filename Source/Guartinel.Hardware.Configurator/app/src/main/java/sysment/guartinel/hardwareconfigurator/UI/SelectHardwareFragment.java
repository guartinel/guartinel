package sysment.guartinel.hardwareconfigurator.ui;

import android.content.Context;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.TextView;

import java.lang.ref.WeakReference;
import java.util.ArrayList;
import java.util.List;

import sysment.guartinel.hardwareconfigurator.R;
import sysment.guartinel.hardwareconfigurator.connection.WifiHelper;
import sysment.guartinel.hardwareconfigurator.tools.HardwareSniffer;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardware;

/**
 * Created by DAVT on 2017.12.06..
 */

public class SelectHardwareFragment extends Fragment implements IDiscoveryProgress {

    @Override
    public void increase() {
        bar.setProgress(bar.getProgress() + 1);
    }

    private HardwareRecyclerViewAdapter _adapter;
    private RecyclerView _recyclerView;
    private List<GuartinelHardware> _guartinelHardwares = new ArrayList<>();
    private LinearLayoutManager linearLayoutManager;
    private ProgressBar bar;
    public static String TAG = "SELECT_HARDWARE_FRAGMENT";

        IDiscoveryProgress progressInterface;
    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        View _view = inflater.inflate(R.layout.start_fragment, container, false);
        progressInterface = this;
        ((MainActivity) getActivity()).getSupportActionBar().setTitle("Guartinel Configurator");

        final ImageButton startButton = (ImageButton) _view.findViewById(R.id.startScanButton);
        bar = (ProgressBar) _view.findViewById(R.id.progressBar);
        final TextView statusText = (TextView) _view.findViewById(R.id.statusText);
        final TextView discoveryResultTextView = (TextView) _view.findViewById(R.id.discoveryResultTextView);
        bar.setVisibility(View.GONE);
        final Context context = getActivity();
        startButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                 if (!WifiHelper.isWifiOn(context)) {
                    ((MainActivity) getActivity()).showSnackbarError("First enable wifi!");
                    return;
                }

                WifiHelper.saveOriginalWifiSSID(getActivity());
                getActivity().getWindow().addFlags(android.view.WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
                startButton.setVisibility(View.GONE);
                bar.setVisibility(View.VISIBLE);
                bar.setProgress(0);
                statusText.setText("Discovery running...");
                HardwareSniffer.sniff(getActivity(), progressInterface, new HardwareSniffer.onSniffingDone() {
                    @Override
                    public void done(final List<GuartinelHardware> foundItems) {
                        if (getActivity() == null) {
                            return;
                        }

                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                getActivity().getWindow().clearFlags(android.view.WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
                                bar.setVisibility(View.GONE);
                                startButton.setVisibility(View.VISIBLE);
                                statusText.setText("Refresh discovery!");

                                if (foundItems.size() == 0) {
                                    discoveryResultTextView.setVisibility(View.GONE);
                                    _guartinelHardwares.clear();
                                    _adapter.notifyDataSetChanged();
                                    statusText.setText("Unfortunately we did not find any Guartinel device. Please make sure that the device is on and connected to the same network, and try again.");

                                } else {
                                    discoveryResultTextView.setVisibility(View.VISIBLE);
                                    _guartinelHardwares.clear();
                                    _guartinelHardwares.addAll(foundItems);
                                    _adapter.notifyDataSetChanged();
                                }
                            }
                        });

                    }
                });
            }
        });
        _recyclerView = (RecyclerView) _view.findViewById(R.id.found_hardware_recycler_view);
        linearLayoutManager = new LinearLayoutManager(getContext());
        linearLayoutManager.setOrientation(LinearLayoutManager.VERTICAL);
        _recyclerView.setLayoutManager(linearLayoutManager);
        _adapter = new HardwareRecyclerViewAdapter(_guartinelHardwares, (MainActivity) getActivity());
        _recyclerView.setAdapter(_adapter);
        return _view;
    }
}
