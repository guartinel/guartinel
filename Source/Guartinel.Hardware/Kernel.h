#pragma once
#include "Arduino.h"
#include "IChecker.h"
#include "WifiManager.h"
#include "FileSystemManager.h"
#include "ManagementServer.h"
#ifndef _KERNEL_H
#define _KERNEL_H

class Kernel {
private:
   CLIConfig _activeConfig;
   IChecker* _checker;
   WifiManager* _wifiManager;
   FileSystemManager* _fileSystemManager;
   ManagementServer* _managementServer;
   void handleMissingConfig();
    void handleIfCannotConnectToWifi();  
   void restart();  
public:
   Kernel();
   void setup();
   void loop();
   };
#endif
