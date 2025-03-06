#pragma once
#ifndef _WIFI_MANAGER_H
#define _WIFI_MANAGER_H


#include "Arduino.h"
#include "GuartinelConfig.h"
#include "WifiWebServer.h"

class WifiManager {
private :
   WifiWebServer* _wifiWebServer;
public:
   WifiManager(String guid);
   void StartAP(String instanceName);  
   bool ConnectTo(CLIConfig config);
   WifiWebServer* WebServer();
   void Stop();   
   bool isConnectedToWifi();
};

#endif