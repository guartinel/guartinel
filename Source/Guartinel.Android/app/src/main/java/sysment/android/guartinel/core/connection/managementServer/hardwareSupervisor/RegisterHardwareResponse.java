package sysment.android.guartinel.core.connection.managementServer.hardwareSupervisor;

import org.json.JSONException;
import org.json.JSONObject;

import sysment.android.guartinel.core.connection.HttpResponse;
import sysment.android.guartinel.core.exceptions.InternalSystemErrorException;

public class RegisterHardwareResponse extends HttpResponse {
    public static class Keys {
        public static final String UPDATE_SERVER_HOST = "update_server_host";
        public static final String UPDATE_SERVER_PORT = "update_server_port";
        public static final String UPDATE_SERVER_PROTOCOL_PREFIX = "update_server_protocol_prefix";
        public static final String TYPE = "type";
    }

    public static class ErrorValues extends HttpResponse.ErrorValues {
        public static final String INVALID_TOKEN = "INVALID_TOKEN";
    }

    private String updateServerHost;
    private int updateServerPort;
    private String updateServerProtocolPrefix;



    private String hardwareType;

    public RegisterHardwareResponse(JSONObject responseJSON) throws JSONException, InternalSystemErrorException {
        super(responseJSON);
        if (!isSuccess()) {
            return;
        }
        this.updateServerHost = responseJSON.getString(Keys.UPDATE_SERVER_HOST);
        this.updateServerProtocolPrefix = responseJSON.getString(Keys.UPDATE_SERVER_PROTOCOL_PREFIX);
        this.updateServerPort = responseJSON.getInt(Keys.UPDATE_SERVER_PORT);
        this.hardwareType = responseJSON.getString(Keys.TYPE);
    }

    public String getUpdateServerHost() {
        return updateServerHost;
    }

    public int getUpdateServerPort() {
        return updateServerPort;
    }

    public String getUpdateServerProtocolPrefix() {
        return updateServerProtocolPrefix;
    }
    public String getHardwareType() {
        return hardwareType;
    }
}

