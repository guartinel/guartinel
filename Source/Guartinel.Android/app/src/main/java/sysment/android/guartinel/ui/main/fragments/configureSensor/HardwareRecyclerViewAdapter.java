package sysment.android.guartinel.ui.main.fragments.configureSensor;

import android.app.Activity;
import android.content.Context;
import android.graphics.Color;
import android.support.v7.app.AlertDialog;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

import sysment.android.guartinel.R;
import sysment.android.guartinel.core.connection.hardwareSensor.HardwareSensorImpl;
import sysment.android.guartinel.core.utils.LOG;

/**
 * Created by DAVT on 2017.12.06..
 */

public class HardwareRecyclerViewAdapter extends RecyclerView.Adapter<HardwareRecyclerViewAdapter.HardwareViewHolder> {
    private final List<HardwareSensorImpl> guartinelHardwares;
    private boolean isUIDisabled = false;

    public void disableUI() {
        isUIDisabled = true;
        for (CheckBox checkBox : hardwareCheckBoxes) {
            checkBox.setEnabled(false);
        }
    }

    public void enableUI() {
        isUIDisabled = false;
        for (CheckBox checkBox : hardwareCheckBoxes) {
            checkBox.setEnabled(true);
        }
    }

    public HardwareRecyclerViewAdapter(Context context, List<HardwareSensorImpl> guartinelHardwares) {
        this.guartinelHardwares = guartinelHardwares;
        _context = context;
    }

    @Override
    public int getItemCount() {
        return guartinelHardwares.size();
    }

    List<CheckBox> hardwareCheckBoxes = new ArrayList<>();

    @Override
    public void onBindViewHolder(HardwareViewHolder viewHolder, final int i) {
        final HardwareSensorImpl hardware = guartinelHardwares.get(i);
        viewHolder.name.setText(hardware.getName());
        viewHolder.isSelectedCheckBox.setChecked(hardware.isSelected);
        if (hardware.isError()) {
            viewHolder.errorTextView.setVisibility(View.VISIBLE);
            viewHolder.errorTextView.setText(hardware.getErrorMessage());
        } else {
            viewHolder.errorTextView.setVisibility(View.GONE);
        }
        if (hardware.isConfigured()) {
            viewHolder.errorTextView.setVisibility(View.VISIBLE);
            viewHolder.errorTextView.setText("Configured!");
            viewHolder.errorTextView.setTextColor(Color.GREEN);
        }

        if (!hardware.getName().equals(hardware.getOriginalName())) {
            viewHolder.originalName.setVisibility(View.VISIBLE);
            viewHolder.originalName.setText(hardware.getOriginalName());
        } else {
            viewHolder.originalName.setVisibility(View.GONE);
        }
        viewHolder.isSelectedCheckBox.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                if (isUIDisabled) {
                    return;
                }
                hardware.isSelected = isChecked;
            }
        });
        hardwareCheckBoxes.add(viewHolder.isSelectedCheckBox);
    }

    Context _context;

    @Override
    public HardwareViewHolder onCreateViewHolder(ViewGroup viewGroup, final int i) {
        View itemView = LayoutInflater.
                from(viewGroup.getContext()).
                inflate(R.layout.hardware_sensor_list_item, viewGroup, false);
        itemView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (isUIDisabled) {
                    return;
                }
                final AlertDialog dialogBuilder = new AlertDialog.Builder(_context).create();
                LayoutInflater inflater = ((Activity) _context).getLayoutInflater();
                View dialogView = inflater.inflate(R.layout.alert_dialog_with_edit_text, null);
                TextView title = (TextView) dialogView.findViewById(R.id.alert_dialog_with_edit_text_title_edit_text);
                title.setText("Enter your device name");
                final EditText nameEditText = (EditText) dialogView.findViewById(R.id.alert_dialog_with_edit_text);
                Button cancel = (Button) dialogView.findViewById(R.id.rename_device_cancel_button);
                Button ok = (Button) dialogView.findViewById(R.id.rename_device_ok_button);
                nameEditText.setText(guartinelHardwares.get(i).getName());
                ok.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View view) {
                        guartinelHardwares.get(i).setName(nameEditText.getText().toString());
                        notifyDataSetChanged();
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
        });
        return new HardwareViewHolder(itemView);
    }

    public static class HardwareViewHolder extends RecyclerView.ViewHolder {
        private TextView name, originalName, errorTextView;
        private CheckBox isSelectedCheckBox;

        public HardwareViewHolder(View itemView) {
            super(itemView);
            name = (TextView) itemView.findViewById(R.id.hardwareName);
            originalName = (TextView) itemView.findViewById(R.id.hardware_sensor_item_original_name_text_view);
            isSelectedCheckBox = (CheckBox) itemView.findViewById(R.id.select_hardware_sensor_check_box);
            errorTextView = (TextView) itemView.findViewById(R.id.hardware_sensor_item_error_text_view);
        }
    }

}
