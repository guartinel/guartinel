package sysment.guartinelandroidsystemwatcher.persistance.DBO;

import org.joda.time.DateTime;

import java.util.UUID;

/**
 * Created by moqs_the_one on 2017.07.28..
 */

public class ServerInstance {
    private String address = "";
    private String token = "";
    private String name = "";
    private String description = "";
    private boolean isEnabled = true;
    private String lastSeen = "Never seen.";
    private String lastChecked = "Never checked.";
    private String ID;
    private ServerStatus status;

    public String getAddress() {
        return address;
    }

    public String getToken() {
        return token;
    }

    public String getName() {
        return name;
    }

    public String getDescription() {
        return description;
    }

    public boolean isEnabled() {
        return isEnabled;
    }

    public String getID() {
        return ID;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public void setName(String name) {
        this.name = name;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public void setEnabled(boolean enabled) {
        isEnabled = enabled;
    }

    public void setLastSeen(String lastSeen) {
        this.lastSeen = lastSeen;
    }

    public void setLastChecked(String lastChecked) {
        this.lastChecked = lastChecked;
    }

    public String getLastChecked() {
        return this.lastChecked;
    }

    public void setStatus(ServerStatus status) {
        this.status = status;
    }

    public String getLastSeen() {

        return lastSeen;
    }

    public ServerStatus getStatus() {
        return status;
    }

    public ServerInstance() {
        ID = UUID.randomUUID().toString();
        setStatus(new ServerStatus().setUnknown());
    }

    public ServerInstance(String id, String address, String token, String name, String description, boolean isEnabled) {
        this.ID = id;
        this.address = address;
        this.token = token;
        this.name = name;
        this.description = description;
        this.isEnabled = isEnabled;
    }
}
