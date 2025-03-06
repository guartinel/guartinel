package sysment.guartinel.hardwareconfigurator.ui;

import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import java.util.List;

import sysment.guartinel.hardwareconfigurator.R;
import sysment.guartinel.hardwareconfigurator.models.GuartinelHardware;

/**
 * Created by DAVT on 2017.12.06..
 */

public class HardwareRecyclerViewAdapter extends RecyclerView.Adapter<HardwareRecyclerViewAdapter.HardwareViewHolder> {
    private final List<GuartinelHardware> guartinelHardwares;
    HardwareSelectionListener listener;

    public HardwareRecyclerViewAdapter(List<GuartinelHardware> guartinelHardwares, HardwareSelectionListener listener) {
        this.guartinelHardwares = guartinelHardwares;
        this.listener = listener;
    }

    @Override
    public int getItemCount() {
        return guartinelHardwares.size();
    }

    @Override
    public void onBindViewHolder(HardwareViewHolder viewHolder, final int i) {
        GuartinelHardware hardware = guartinelHardwares.get(i);
        viewHolder.name.setText(hardware.getName());
        viewHolder.name.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                listener.onSelected(guartinelHardwares.get(i));
            }
        });
    }

    @Override
    public HardwareViewHolder onCreateViewHolder(ViewGroup viewGroup, int i) {
        View itemView = LayoutInflater.
                from(viewGroup.getContext()).
                inflate(R.layout.hardware_list_item, viewGroup, false);

        return new HardwareViewHolder(itemView);
    }

    public static class HardwareViewHolder extends RecyclerView.ViewHolder {
        private TextView name;

        public HardwareViewHolder(View itemView) {
            super(itemView);
            name = (TextView) itemView.findViewById(R.id.hardwareName);
        }

    }

}
