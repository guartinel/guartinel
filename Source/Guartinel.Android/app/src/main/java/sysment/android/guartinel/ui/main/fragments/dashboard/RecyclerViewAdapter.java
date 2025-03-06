package sysment.android.guartinel.ui.main.fragments.dashboard;

import android.graphics.Color;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.List;

import sysment.android.guartinel.core.persistance.Alert;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.R;

/**
 * Created by sysment_dev on 02/26/2016.
 */
public class RecyclerViewAdapter extends RecyclerView.Adapter<RecyclerViewAdapter.AlertViewHolder> {

    private List<Alert> alertsList;

    public RecyclerViewAdapter(List<Alert> alertsList) {
        this.alertsList = alertsList;
    }

    @Override
    public int getItemCount() {
        return alertsList.size();
    }

    @Override
    public void onBindViewHolder(AlertViewHolder contactViewHolder, int i) {
        Alert alert = alertsList.get(i);
        contactViewHolder.initFromAlert(alert);
    }

    @Override
    public AlertViewHolder onCreateViewHolder(ViewGroup viewGroup, int i) {
        View itemView = LayoutInflater.
                from(viewGroup.getContext()).
                inflate(R.layout.card_view, viewGroup, false);

        return new AlertViewHolder(itemView);
    }

    public static class AlertViewHolder extends RecyclerView.ViewHolder {
        private ImageView imageView;
        private TextView timeTextView;
        private TextView titleTextView;
        private TextView packageNameTextView;
        private TextView packageStatusTextView;
        private TextView alertDetailsTextView;

        public AlertViewHolder(View itemView) {
            super(itemView);
            imageView = (ImageView) itemView.findViewById(R.id.card_view_image_view);
            timeTextView = (TextView) itemView.findViewById(R.id.card_view_time_text_view);
            titleTextView = (TextView) itemView.findViewById(R.id.card_view_title_text_view);
            packageNameTextView = (TextView) itemView.findViewById(R.id.card_view_package_name_text_view);
            packageStatusTextView = (TextView) itemView.findViewById(R.id.card_view_package_status_text_view);
            alertDetailsTextView = (TextView) itemView.findViewById(R.id.card_view_alert_details_text_view);
        }

        public void initFromAlert(Alert alert) {
            LOG.I("Building alert card.\n AlertJSON: "+ alert.getJSON().toString());
            packageNameTextView.setText("Package: " + alert.packageName);
            alertDetailsTextView.setText(alert.alertDetails);
            packageStatusTextView.setVisibility(View.GONE);
            titleTextView.setText(alert.alertMessage);
            timeTextView.setText(alert.timeStamp);
            if (alert.isRecovery && !alert.isPackageAlerted) {
                imageView.setImageDrawable(DashBoardFragment.getRecoveryDrawable());
                packageStatusTextView.setText("This package is fully recovered!");
                packageStatusTextView.setTextColor(Color.parseColor("#6AC259"));
                packageStatusTextView.setVisibility(View.VISIBLE);
                return;
            }
            if (alert.isRecovery && alert.isPackageAlerted) {
                imageView.setImageDrawable(DashBoardFragment.getWarningDrawable());
                packageStatusTextView.setVisibility(View.VISIBLE);
                packageStatusTextView.setText("Some part of the package is still alerted!");
                return;
            }
            imageView.setImageDrawable(DashBoardFragment.getAlertDrawAble());
        }

    }
}