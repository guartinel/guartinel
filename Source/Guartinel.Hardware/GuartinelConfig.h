
#ifndef _GUARTINEL_CONFIG_H
#define _GUARTINEL_CONFIG_H

struct CLIConfig {
	String routerSSID;
	String routerPassword;
	String instanceName;
	bool isSuccessfull = false;
	String backendServerHost;
	String backendServerPort;
	String backendServerProtocolPrefix;
	String updateServerHost;
	String updateServerPort;
	String updateServerProtocolPrefix;
	String hardwareType;
	String startupTime;
};
#endif