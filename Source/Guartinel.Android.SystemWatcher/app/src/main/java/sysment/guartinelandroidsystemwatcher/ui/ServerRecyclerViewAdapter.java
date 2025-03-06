package sysment.guartinelandroidsystemwatcher.ui;

import android.content.DialogInterface;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AlertDialog;
import android.support.v7.widget.AppCompatImageView;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.SwitchCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.TextView;

import sysment.guartinelandroidsystemwatcher.R;
import sysment.guartinelandroidsystemwatcher.SystemWatcherApp;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstance;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerInstances;
import sysment.guartinelandroidsystemwatcher.persistance.DBO.ServerStatus;

/**
 * Created by moqs_the_one on 2017.08.15..
 */

public class ServerRecyclerViewAdapter extends RecyclerView.Adapter<ServerRecyclerViewAdapter.ServerViewHolder> {

    private ServerInstances serverInstances;

    public class ServerViewHolder extends RecyclerView.ViewHolder {
        public TextView serverName, description, address, lastSeen, statusMessage, lastChecked;
        public SwitchCompat isEnabled;
        public AppCompatImageView statusImage;
        public ImageButton deleteServerButton;

        public ServerViewHolder(View view) {
            super(view);
            serverName = (TextView) view.findViewById(R.id.serverInstanceCardViewServerNameTextView);
            description = (TextView) view.findViewById(R.id.serverInstanceCardViewServerDescriptionTextView);
            address = (TextView) view.findViewById(R.id.serverInstanceCardViewServerAddressTextView);
            lastSeen = (TextView) view.findViewById(R.id.serverInstanceCardViewServerLastSeenTextView);
            lastChecked = (TextView) view.findViewById(R.id.serverInstanceCardViewLastCheckedTextView);
            statusMessage = (TextView) view.findViewById(R.id.serverInstanceCardViewServerStatusMessageTextView);
            statusImage = (AppCompatImageView) view.findViewById(R.id.serverInstanceCardViewStatusImageView);
            deleteServerButton = (ImageButton) view.findViewById(R.id.serverInstanceCardViewDeleteButton);
            isEnabled = (SwitchCompat) view.findViewById(R.id.serverInstanceCardViewEnabledSwitch);

            // isEnabled.setEnabled(false);

        }
    }

    MainActivity activity;

    public ServerRecyclerViewAdapter(ServerInstances servers, MainActivity activity) {
        this.serverInstances = servers;
        this.activity = activity;
    }

    @Override
    public ServerViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View itemView = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.server_instance_card_view, parent, false);
        return new ServerViewHolder(itemView);
    }

    @Override
    public void onBindViewHolder(ServerViewHolder holder, int position) {
        final ServerInstance server = serverInstances.get(position);
        holder.serverName.setText(server.getName());
        holder.description.setText(server.getDescription());
        holder.statusMessage.setText(server.getStatus().getMessage());
        holder.lastSeen.setText(server.getLastSeen());
        holder.lastChecked.setText(server.getLastChecked());
        holder.address.setText(server.getAddress());
        holder.isEnabled.setChecked(server.isEnabled());
        holder.deleteServerButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                new AlertDialog.Builder(activity)
                        .setTitle("Delete server " + server.getName())
                        .setMessage("Are you sure?")
                        .setIcon(android.R.drawable.ic_dialog_alert)
                        .setPositiveButton(android.R.string.yes, new DialogInterface.OnClickListener() {
                            public void onClick(DialogInterface dialog, int whichButton) {
                                SystemWatcherApp.getDataStore().deleteServerInstance(activity, server);
                            }
                        }).setNegativeButton(android.R.string.no, null).show();
            }
        });

        if (server.getStatus().isUnknown()) {
            holder.statusImage.setImageDrawable(ContextCompat.getDrawable(activity, R.drawable.help));

        }
        if (server.getStatus().isError()) {
            holder.statusImage.setImageDrawable(ContextCompat.getDrawable(activity, R.drawable.alert));
        }

        if (server.getStatus().isOK()) {
            holder.statusImage.setImageDrawable(ContextCompat.getDrawable(activity, R.drawable.check_circle));
        }
        if (server.getStatus().isNoInternet()) {
            holder.statusImage.setImageDrawable(ContextCompat.getDrawable(activity, R.drawable.no_internet_icon));
        }
        holder.itemView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                activity.openServerInstanceConfiguration(server);
            }
        });
    }

    @Override
    public int getItemCount() {
        return serverInstances.size();
    }
}
