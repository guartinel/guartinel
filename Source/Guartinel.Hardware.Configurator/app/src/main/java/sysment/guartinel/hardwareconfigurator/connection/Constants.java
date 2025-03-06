package sysment.guartinel.hardwareconfigurator.connection;

/**
 * Created by DAVT on 2018.02.07..
 */

public class Constants {

    public static class Hardware {
        public static final String RESULT = "result";

        public static class Routes {
            public static final String LOGIN = "login";
            public static final String SET_CONFIG = "setConfig";
            public static final String GET_CONFIG = "getConfig";
            public static final String FREEZE = "freeze";
            public static final String GET_DIAGNOSTICS = "getDiagnostics";
        }

        public static class RequestParameters {
            public static final String HARDWARE_TOKEN = "hardwareToken";
            public static final String ROUTER_SSID = "routerSSID";
            public static final String ROUTER_PASSWORD = "routerPassword";
            public static final String DEVICE_PASSWORD = "devicePassword";
            public static final String INSTANCE_NAME = "instanceName";
            public static final String BACKEND_SERVER_HOST = "backendServerHost";
            public static final String BACKEND_SERVER_PORT = "backendServerPort";
            public static final String BACKEND_SERVER_PROTOCOL_PREFIX = "backendServerProtocolPrefix";
        }

        public static class ResponseParameters {
            public static final String LOG = "log";

            public static class ResultValue {
                public static final String INVALID_PASSWORD = "invalid_password";
            }
        }
    }

    public static class ManagementServer {
        public static final String HTTP_PREFIX = "http://";
        public static final String HTTPS_PREFIX = "https://";

        public static class Routes {
            public static final String VALIDATE_HARDWARE = "/hardwareSupervisor/validateHardware";
        }

        public static class RequestParameters {
            public static final String HARDWARE_TOKEN = "hardware_token";
        }

        public static class ResponseParameters {
            public static final String SUCCESS = "success";
            public static final String ERROR = "error";
        }
    }
}
