#pragma once
#ifndef _WIFI_WEB_SERVER_H
#define _WIFI_WEB_SERVER_H

#include "Arduino.h"
#include "GuartinelConfig.h"

class WifiWebServer {

public:
   WifiWebServer();
   CLIConfig WaitForConfig();
   CLIConfig WaitForConfig(int maxWaitSec);
   void StartWebServer();
   void StopWebServer();
   void CheckInComingRequests();
   CLIConfig GetLatestConfig();
   bool HasConfigChanged();
   bool isSendingFreezed();

};


#endif