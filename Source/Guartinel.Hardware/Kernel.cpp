#include "Arduino.h"
#include "LOG.h"
#include "GuartinelConfig.h"
#include "FileSystemManager.h"
#include "WifiManager.h"
#include "Utils.h"
#include "ManagementServer.h"
#include "FirmwareUpdater.h"
#include "LedDisplay.h"
#include "Kernel.h"
#include "IChecker.h"
#include "Version.h"
#include "CheckerGeneral.h";
#include "CheckerCurrent.h";
#include "CheckerTemperature.h";
#include "CheckerWaterPresence.h";
#include "time.h"
#include "CheckerConstansts.h"

extern "C" {
#include "user_interface.h"
}

Kernel::Kernel() {
}
void Kernel::restart() {
   //LOG::Info("Restart:", "Started");
   ESP.restart();
   delay(10);
   ESP.reset();
}

void Kernel::handleIfCannotConnectToWifi() {
   LedDisplay::Error();
   LedDisplay::BlinkThreeTimes();
   LOG::Info("handleIfCannotConnectToWifi:", "Starting handling");
   _wifiManager->StartAP(_activeConfig.instanceName);
   _wifiManager->WebServer()->WaitForConfig(360); // if during this time any config received then it is saved and will be used after restart
}

void Kernel::handleMissingConfig() {
   LedDisplay::Error();
   LedDisplay::BlinkTwice();
   LOG::Info("Configuration load:", "FAILED. Starting AP.");
   _wifiManager->StartAP(_activeConfig.instanceName);
   _wifiManager->WebServer()->WaitForConfig();
}

void Kernel::setup() {
   LOG::Init(true);

   LedDisplay::Init();
   LedDisplay::Loading();
   LedDisplay::BlinkThreeTimes();

   LOG::Info("", "#######################################");
   LOG::Info("", "Guartinel.Hardware");
   LOG::Info("Firmware version:", VERSION);
   String sysInfo = "";
   sysInfo.concat("Chip id:");
   sysInfo.concat(Utils::GetChipID());
   sysInfo.concat("\nSDK ver: ");
   sysInfo.concat(ESP.getSdkVersion());
   sysInfo.concat("\nBoot ver: ");
   sysInfo.concat(ESP.getBootVersion());
   sysInfo.concat("\nESP8266 Core ver: ");
   sysInfo.concat(ESP.getCoreVersion());
   sysInfo.concat("\nBoot mode: ");
   sysInfo.concat(ESP.getBootMode());
   sysInfo.concat("\nReset info: ");
   sysInfo.concat(ESP.getResetInfo());
   sysInfo.concat("\nReset reason: ");
   sysInfo.concat(ESP.getResetReason());
   sysInfo.concat("\nSketch size: ");
   sysInfo.concat(ESP.getSketchSize());
   sysInfo.concat("\nMD5 of sketch: ");
   sysInfo.concat(ESP.getSketchMD5());

   LOG::Info("System info", sysInfo.c_str());

   system_update_cpu_freq(160);
   ESP.wdtEnable(WDTO_8S);

   //LOG::Status("#GUARTINEL#\n" + sysInfo); TODO handle
   _fileSystemManager = new FileSystemManager();
   //Check if file system accessible? Did we need a format to get a usable space? If returns yes then formatting is done.
   (_fileSystemManager->InitFileSystem()) ? LOG::Info("File system init:", "OK") : LOG::Info("File system init:", "FAILED");

   _wifiManager = new  WifiManager(Utils::GetChipID());
   _managementServer = new ManagementServer(_wifiManager);
   //Do we have a saved config?
   _activeConfig = _fileSystemManager->GetConfig();
   if (_activeConfig.isSuccessfull && _activeConfig.routerSSID.length() != 0 && _activeConfig.routerPassword.length() != 0 ) {
      LOG::Info("Configuration load:", "OK");
      LOG::Status("Configuration:OK.");
   }
   else {
      LOG::Status("Configuration:MISSING");
      handleMissingConfig();
      LOG::Info("Init:", "Restarting");
      restart();
      return;
   }
   //Config is OK connec to router according to that
   bool wifiConnectionResult = _wifiManager->ConnectTo(_activeConfig);
   if (!wifiConnectionResult) {
      LOG::Status("Wifi:CANNOT CONNECT");
      handleIfCannotConnectToWifi();
      LOG::Info("Init:", "Restarting");
      restart();
      return;
   }
   LOG::Status("Wifi:CONNECTED");
   LedDisplay::OK();
   LedDisplay::BlinkOnce();

   LOG::Info("Wifi&config init is done:", "Starting init the checker");
   LOG::Info("Checker type is: ", _activeConfig.hardwareType.c_str());
   if (_activeConfig.hardwareType.indexOf("voltage") != -1) {
      _checker = new  CheckerGeneral();
   }
   if (_activeConfig.hardwareType.indexOf("current") != -1) {
      _checker = new  CheckerCurrent();
   }
   if (_activeConfig.hardwareType.indexOf("temperature") != -1) {
      _checker = new  CheckerTemperature();
   }
   if (_activeConfig.hardwareType.indexOf("water") != -1) {
	   _checker = new  CheckerWaterPresence();
   }
   _checker->Init(_activeConfig);

   //Start webserver to handle configurations during measurent sending mode and start the working loop!
   _wifiManager->WebServer()->StartWebServer();

   if (ESP.getResetReason().indexOf("Software/System restart") == -1) {
      //startup time injected in config from the check update response
      //only save startup time if it was not a software restart    
      _fileSystemManager->SaveConfig(_activeConfig);
   }
   if (LOG::IsRemoteLOGEnabled()) {
      LOG::RemoteLog("Init info", sysInfo.c_str());
      String configSummary = "";
      configSummary.concat(_activeConfig.backendServerProtocolPrefix);
      configSummary.concat(_activeConfig.backendServerHost);
      configSummary.concat(_activeConfig.backendServerPort);
      configSummary.concat("|");
      configSummary.concat(_activeConfig.hardwareType);
      configSummary.concat("|");
      configSummary.concat(_activeConfig.instanceName);
      configSummary.concat("|");
      configSummary.concat(_activeConfig.routerSSID);
      configSummary.concat("|");
      configSummary.concat(_activeConfig.updateServerProtocolPrefix);
      configSummary.concat(_activeConfig.updateServerHost);
      configSummary.concat(_activeConfig.updateServerPort);
      LOG::RemoteLog("Config", configSummary.c_str());
   }
}

int notConnectedCount = 0;
int okBlinkCount = 0;


void Kernel::loop() {
   FirmwareUpdater::UpdateIfNeeded(&_activeConfig, VERSION, _managementServer);
   delay(10);
   if (okBlinkCount == 0) {
      LedDisplay::OK();
      LedDisplay::BlinkOnce();
   }
   okBlinkCount++;
   if (okBlinkCount == 500) {
      okBlinkCount = 0;
   }
   if (!_wifiManager->isConnectedToWifi()) { // if not connected to wifi for 6 loop(30 sec) then restart
      LedDisplay::Error();
      LOG::Status("LOOP: WIFI DROPPED");
      LOG::Info("Loop:", "Not connected to wifi");
      if (notConnectedCount == 6) {
         restart();
         return;
      }
      notConnectedCount++;
   }
   else {
      notConnectedCount = 0;
   }
   _checker->Update();

   if (_checker->ShouldSendMeasurement() && !_wifiManager->WebServer()->isSendingFreezed()) {
      String measurement = _checker->GetMeasurement();
      _wifiManager->WebServer()->CheckInComingRequests();
      _managementServer->RegisterMeasurement(&_activeConfig, measurement);

   }
   _wifiManager->WebServer()->CheckInComingRequests();
}

