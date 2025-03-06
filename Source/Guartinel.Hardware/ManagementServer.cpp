#include "LOG.h"
#include "ManagementServer.h"
#include <ESP8266HTTPClient.h>
#include <ArduinoJson.hpp>
#include <ArduinoJson.h>
#include "Utils.h"
#include "FileSystemManager.h"
#include "GuartinelConfig.h"
#include "WiFiClientSecure.h"
#include "WifiManager.h"
#include "Version.h"
WifiManager* _wifiManager;

ManagementServer::ManagementServer(WifiManager* wifi) {
   _wifiManager = wifi;
}


String ManagementServer::SendHTTPSPost(CLIConfig* config, String url, String payload) {
   if (!_wifiManager->isConnectedToWifi()) {
      //LOG::Debug("SendPost", " NOT CONNECTED TO WIFI!!!!!!!!!!");
   }
   WiFiClientSecure _httpsClient = WiFiClientSecure();
   //LOG::Info("SendPost", "Using HTTPS");
   LOG::Info("SendPost Address", config->backendServerHost.c_str());
   LOG::Info("SendPost Port", config->backendServerPort.c_str());
   LOG::Info("SendPost payload: ", payload.c_str());
   _httpsClient.setTimeout(20000);


   if (!_httpsClient.connected()) {
      if (!_httpsClient.connect(config->backendServerHost.c_str(), config->backendServerPort.toInt())) {
         LOG::Info("PostSending: ", "Connection Failed");
         return "";
      }
   }
   LOG::Info("PostSending: ", "Connected");

   _httpsClient.println("POST " + url + " HTTP/1.1");
   _httpsClient.println("Host: " + config->backendServerHost);
   _httpsClient.println("User-Agent: GuartinelHardwareCLI");
   _httpsClient.println("Connection: close");
   _httpsClient.println("Content-Type: application/json");
   _httpsClient.print("Content-Length: ");
   _httpsClient.println(payload.length());
   _httpsClient.println();
   _httpsClient.println(payload);
   _httpsClient.flush();
   ////LOG::Info("PostSending: ", "Request sent");

   //LOG::Info("PostSending: ", "Reading response");
   // ESP.wdtFeed();
   String response = _httpsClient.readString();
   int bodypos = response.indexOf("\r\n\r\n");
   response = response.substring(bodypos);
   LOG::Info("PostSending response: ", response.c_str());
   _httpsClient.stop();
   return response;
}

String ManagementServer::SendHTTPPost(CLIConfig* config, String url, String payload) {
   LOG::Info("HTTP Post", url.c_str());
   LOG::Info("HTTP Post", payload.c_str());

   HTTPClient http;
   http.begin("http://" + config->backendServerHost + ":" + config->backendServerPort + url);
   http.addHeader("Content-Type", "application/json");
   http.POST(payload);

   String responseString = http.getString();
   http.end();
   //LOG::Status("LOOP:RESPONSE:\n" + responseString);

   return responseString;
}

void ManagementServer::RegisterMeasurement(CLIConfig *config, String measurementJSON) {
   if (_wifiManager->WebServer()->isSendingFreezed()) {
      return;
   }
   StaticJsonBuffer<512> jsonBuffer;
   JsonObject& root = jsonBuffer.createObject();
   root["measurement"] = RawJson(measurementJSON.c_str());
   root["instance_id"] = Utils::GetChipID();
   root["firmware"] = VERSION;
   root["startup_time"] = config->startupTime;
   String payLoad = "";
   root.printTo(payLoad);
   jsonBuffer.clear();
   String responseString;

   if (config->backendServerProtocolPrefix != NULL && config->backendServerProtocolPrefix.equals("http://")) {
      responseString = SendHTTPPost(config, "/hardwareSupervisor/registerMeasuredData", payLoad);
   }
   else {
      _wifiManager->WebServer()->StopWebServer();
      responseString = SendHTTPSPost(config, "/hardwareSupervisor/registerMeasuredData", payLoad);
      _wifiManager->WebServer()->StartWebServer();
   }
   LOG::Info("Response:", responseString.c_str());
   StaticJsonBuffer<200> responseBuffer;
   JsonObject& resultParsed = responseBuffer.parseObject(responseString);

   if (!resultParsed.success()) {
      Serial.println("Cannot parse response");
      return;
   }

   if (resultParsed.containsKey("instance_name")) {
      String newInstanceName = resultParsed["instance_name"];
      if (newInstanceName != NULL && newInstanceName.length() != 0 && newInstanceName != config->instanceName) {
         //LOG::Status("New instance name arrived. Saving and restarting.");
         FileSystemManager fileSystemManager;
         CLIConfig cliConfig = fileSystemManager.GetConfig();
         cliConfig.instanceName = newInstanceName;
         fileSystemManager.SaveConfig(cliConfig);
         ESP.restart();
      }
   }
   if (resultParsed.containsKey("backend_server")) {
      JsonObject& backendServer = resultParsed["backend_server"];
      bool isModified = false;
      if (config->backendServerHost != backendServer["host"] || config->backendServerPort != backendServer["port"] || config->backendServerProtocolPrefix != backendServer["protocolPrefix"]) {
         isModified = true;
      }
      if (isModified) {
         FileSystemManager fileSystemManager;
         CLIConfig cliConfig = fileSystemManager.GetConfig();
         String backendServerHost = backendServer["host"];
         String backendServerPort = backendServer["port"];
         String backendServerProtocolPrefix = backendServer["protocolPrefix"];

         cliConfig.backendServerHost = backendServerHost;
         cliConfig.backendServerPort = backendServerPort;
         cliConfig.backendServerProtocolPrefix = backendServerProtocolPrefix;

         fileSystemManager.SaveConfig(cliConfig);
         ESP.restart();
      }
   }
   String error = resultParsed["error"];
   if (error.length() == 0) {
      responseBuffer.clear();
      return;
   }
   if (error == "DISCONNECTED") {
	   Serial.println("Forget the wifi settings");
	   FileSystemManager fileSystemManager;
	   CLIConfig config = fileSystemManager.GetConfig();
	   config.routerPassword = "";
	   config.routerPassword = "";
	   fileSystemManager.SaveConfig(config);
	   ESP.restart();
   }

  if (error == "INVALID_ID") {
      Serial.println("Game over we are determined to reset the config. Bye");
      FileSystemManager fileSystemManager;
      fileSystemManager.Reset();
      ESP.restart();
   } 
}
/*
bool ManagementServer::RegisterHardware(CLIConfig *config) {
   LOG::Info("ManagementServer", "Register Hardware");
   StaticJsonBuffer<500> jsonBuffer;
   JsonObject& root = jsonBuffer.createObject();
   root["instance_name"] = config->instanceName;
   root["hardware_token"] = config->hardwareToken;
   root["instance_id"] = Utils::GetChipID();

   String payLoad;
   root.printTo(payLoad);
   jsonBuffer.clear();
   String responseString;

   if (config->backendServerProtocolPrefix != NULL && config->backendServerProtocolPrefix.equals("http://")) {
      responseString = SendHTTPPost(config, "/hardwareSupervisor/registerHardware", payLoad);
   }
   else {
      _wifiManager->WebServer()->StopWebServer();
      responseString = SendHTTPSPost(config, "/hardwareSupervisor/registerHardware", payLoad);
      _wifiManager->WebServer()->StartWebServer();
   }

   StaticJsonBuffer<400> responseBuffer;
   JsonObject& resultParsed = responseBuffer.parseObject(responseString);

   if (!resultParsed.success()) {
      Serial.println("Cannot parse response");
      return false;
   }

   String success = resultParsed["success"];
   if (success.length() == 0) {
      Serial.println("Cannot find success in response");
      return false;
   }

   if (success != "SUCCESS") {
      Serial.println("Success value is not SUCCESS");
      return false;
   }
   String updateServerHost = resultParsed["update_server_host"];
   String updateServerPort = resultParsed["update_server_port"];
   String updateServerProtocolPrefix = resultParsed["update_server_protocol_prefix"];
   String hardwareType = resultParsed["type"];

   LOG::Info("Management Server, Hardware type:", hardwareType.c_str());
   config->updateServerHost = updateServerHost;
   config->updateServerPort = updateServerPort;
   config->updateServerProtocolPrefix = updateServerProtocolPrefix;
   config->hardwareType = hardwareType;
   responseBuffer.clear();
   return true;
}*/

UpdateResult ManagementServer::CheckForUpdate(CLIConfig *config, int firmwareVersion) {
   if (_wifiManager->WebServer()->isSendingFreezed()) {
      return UpdateResult();
   }
   StaticJsonBuffer<200> jsonBuffer;
   JsonObject& root = jsonBuffer.createObject();
   root["instance_name"] = config->instanceName;
   root["instance_id"] = Utils::GetChipID();
   root["version"] = firmwareVersion;

   String payLoad;
   root.printTo(payLoad);
   jsonBuffer.clear();
   String responseString;
   //LOG::Info("MS", "Check for update 7");

   if (config->backendServerProtocolPrefix != NULL && config->backendServerProtocolPrefix.equals("http://")) {
      responseString = SendHTTPPost(config, "/hardwareSupervisor/checkForUpdate", payLoad);
   }
   else {
      _wifiManager->WebServer()->StopWebServer();
      responseString = SendHTTPSPost(config, "/hardwareSupervisor/checkForUpdate", payLoad);
      _wifiManager->WebServer()->StartWebServer();
   }
   StaticJsonBuffer<400> responseBuffer;
   JsonObject& resultParsed = responseBuffer.parseObject(responseString);

   if (!resultParsed.success()) {
      Serial.println("Cannot parse response");
      return UpdateResult();
   }

   String success = resultParsed["success"];
   if (success.length() == 0) {
      Serial.println("Cannot find success in response");
      return UpdateResult();
   }

   if (success != "SUCCESS") {
      Serial.println("Success value is not SUCCESS");
      return UpdateResult();
   }
   String updateUrl = resultParsed["url"];
   String remoteDebugUrl = resultParsed["remote_debug_url"];
   String currentTime = resultParsed["time"];
   responseBuffer.clear();

   UpdateResult result = UpdateResult();
   result.remoteDebugUrl = remoteDebugUrl;
   result.updateUrl = updateUrl;
   result.currentTime = currentTime;
   return result;
}
