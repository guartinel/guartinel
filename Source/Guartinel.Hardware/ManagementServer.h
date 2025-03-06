#pragma once
#include "Arduino.h"
#include <ArduinoJson.hpp>
#include <ArduinoJson.h>
#include "GuartinelConfig.h"
#include "WiFiClientSecure.h"
#include "WifiManager.h"
#ifndef _MANAGEMENT_SERVER
#define _MANAGEMENT_SERVER

struct UpdateResult {
   String updateUrl;
   String remoteDebugUrl;
   String currentTime;
};



class ManagementServer {
private:
   WiFiClientSecure* _httpsClient;
   String SendHTTPPost(CLIConfig* config, String url, String payload) ;
   String SendHTTPSPost(CLIConfig* config, String url, String payload) ;
public:
   ManagementServer(WifiManager* wifi);
   void RegisterMeasurement(CLIConfig *config, String measurementJSON);
  // bool RegisterHardware(CLIConfig *config);   
   UpdateResult CheckForUpdate(CLIConfig *config, int firmwareVersion);
    //String SendPost(CLIConfig* config, String url,String payload);
};



#endif
