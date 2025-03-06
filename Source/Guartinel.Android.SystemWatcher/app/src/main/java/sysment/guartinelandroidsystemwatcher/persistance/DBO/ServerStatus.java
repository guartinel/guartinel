package sysment.guartinelandroidsystemwatcher.persistance.DBO;

/**
 * Created by moqs_the_one on 2017.07.31..
 */

public class ServerStatus {

    public enum STATUS {
        OK,
        ERROR,
        UNKNOWN,
        NO_INTERNET
    }

    private STATUS serverStatus;
    private String okMessage = "";
    private String errorMessage = "";

    public STATUS getStatus() {
        return serverStatus;
    }


    public ServerStatus setUnknown() {
        this.serverStatus = STATUS.UNKNOWN;
        return this;
    }

    public boolean isUnknown() {
        return serverStatus.equals(STATUS.UNKNOWN);
    }

    public boolean isOK() {
        return serverStatus.equals(STATUS.OK);

    }

    public boolean isError() {
        return serverStatus.equals(STATUS.ERROR);

    }

    public boolean isNoInternet() {
        return serverStatus.equals(STATUS.NO_INTERNET);

    }

    public ServerStatus setNoInternet() {
        this.serverStatus = STATUS.NO_INTERNET;
        this.errorMessage = "No internet. Cannot check server.";
        return this;
    }

    public ServerStatus setError(String errorMessage) {
        this.serverStatus = STATUS.ERROR;
        this.errorMessage = errorMessage;
        return this;
    }

    public ServerStatus setOK(String okMessage) {
        this.serverStatus = STATUS.OK;
        this.okMessage = okMessage;
        return this;
    }

    public String getMessage() {
        if (serverStatus.equals(STATUS.OK)) {
            return okMessage;
        }
        if (serverStatus.equals(STATUS.ERROR)) {
            return errorMessage;
        }

        if (serverStatus.equals(STATUS.NO_INTERNET)) {
            return errorMessage;
        }

        return "Unknown";
    }
}
