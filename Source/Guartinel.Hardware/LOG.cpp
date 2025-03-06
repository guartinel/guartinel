
#include "LOG.h"
#include "Arduino.h"
#include <ESP8266HTTPClient.h>
#include "Utils.h"
#include "ArduinoJson.h"
#include "Version.h"

namespace LOG {
   using namespace std;

   std::vector <String> logEntries;
   bool isDebugEnabled = false;
   bool isRemoteLogEnabled = false;
   String  remoteLogAddress;

   extern  String GetEntries() {
      String result = "";
      for (int i = logEntries.size() - 1; i > -1; i--) {
         result += logEntries[i] + "\n";
      }
      return result;
   }
   extern void EnableRemoteLog(String remoteAddress) {
      isRemoteLogEnabled = true;
      remoteLogAddress = remoteAddress;
   }

   void writeLog(char* type, char* caption, const char*  msg, bool store) {
      Serial.print(type);
      Serial.print("|HP:");
      Serial.print(ESP.getFreeHeap());
      Serial.print("|");
      Serial.print(caption);
      Serial.print(" ");
      Serial.print(msg);
      Serial.println();

      if (!store) { return; }

      logEntries.insert(logEntries.begin(), msg);
      if (logEntries.size() > 3) {
         logEntries.resize(3);
      }
   }
   extern void Status(char* status) {
      writeLog("STATUS", "", status, true);
   }
   extern void Info(char* caption, int msg) {
      int i = msg;
      char str[10];
      sprintf(str, "%d", i);
      writeLog("INFO", caption, str, false);
   }

   extern  void Info(char* caption, char* message) {
      writeLog("INFO", caption, message, false);
   }
   extern  void Info(char* caption, const char* message) {
      writeLog("INFO", caption, message, false);
   }
   extern  void Debug(char* caption, char* message) {
      if (!isDebugEnabled) { return; }
      writeLog("DEBUG", caption, message, false);
   }
   extern bool IsRemoteLOGEnabled() {
      return isRemoteLogEnabled;
   }

   extern void RemoteLog(char* caption, const char* msg) {
      if (!isRemoteLogEnabled) {
         LOG::Info("RemoteLOG","Supressed by setting");
         return;
      }
      LOG::Info("RemoteLOG address", remoteLogAddress.c_str());
      StaticJsonBuffer<156> jsonBuffer;
      JsonObject& root = jsonBuffer.createObject();
      root["caption"] = caption;
      root["message"] = msg;
      root["instance_id"] = Utils::GetChipID();
      root["firmware"] = VERSION;

      String payLoad = "";
      root.printTo(payLoad);
      jsonBuffer.clear();
      LOG::Info("RemoteLog payload: ", payLoad.c_str());
      HTTPClient http;
      http.begin(remoteLogAddress);
      http.addHeader("Content-Type", "application/json");
      http.POST(payLoad);

      String responseString = http.getString();
      LOG::Info("RemoteLOG response",responseString.c_str());
      http.end();
      LOG::Info("RemoteLOG", "Sent");
   }

   extern void Init(bool isDebugLog) {
      isDebugEnabled = isDebugLog;

      logEntries.reserve(3);
      Serial.begin(57600);

      if (isDebugEnabled) {
         Serial.setDebugOutput(true);
      }
   }
}
