#include "Arduino.h"
#include <ESP8266HTTPClient.h>
#include <ESP8266httpUpdate.h>
#include "Utils.h"
#include "GuartinelConfig.h"
#include "ManagementServer.h"
#include "LOG.h"

namespace FirmwareUpdater {
   int lastUpdateCheck = -1;
   const int ONE_HOUR = 60 * 60 * 1;

   extern void UpdateIfNeeded(CLIConfig* config, int currentVersion, ManagementServer* managementServer) {
      int now = Utils::GetSystemTimeSec();
      int diff = now - lastUpdateCheck;
      if (diff < 0) {
         lastUpdateCheck = 0;
      }

      if (lastUpdateCheck != -1 && diff < ONE_HOUR) {
         return ;//not yet..
      }
      lastUpdateCheck = now;
      LOG::Info("Updater:", "Checking for new update");

      UpdateResult result = managementServer->CheckForUpdate(config, currentVersion);
      String updateURL = result.updateUrl;
      if (result.remoteDebugUrl != NULL && result.remoteDebugUrl.length() != 0) {
         LOG::EnableRemoteLog(result.remoteDebugUrl);
      }
      config->startupTime = result.currentTime;
      if (updateURL == NULL || updateURL.length() == 0) {
         LOG::Status("Updater: Update not needed.");
         return ;
      }
      LOG::Info("Updater. URL:", updateURL.c_str());

      LOG::RemoteLog("FirmwareUpdater is updating firmware. URL:", updateURL.c_str());
      LOG::Status("Updater: Update started.");
      ESPhttpUpdate.rebootOnUpdate(true);
      t_httpUpdate_return ret = ESPhttpUpdate.update(updateURL);
      //  t_httpUpdate_return ret = ESPhttpUpdate.update("https://" + updateServerPath + LATEST_FIRMWARE_PATH,"","‎c6 f6 2a cf ab 50 92 57 65 91 ae 78 3b fa 9b 32 cd b7 46 37");
        // ESPhttpUpdate.update("https://"+ config.updateServerHost, config.updateServerPort, "/HardwareSupervisor/" + config.hardwareType + LATEST_FIRMWARE_PART);

      switch (ret) {
      case HTTP_UPDATE_FAILED:
         LOG::Status("Update:UPDATE FAILED");
         LOG::Info("Updater:", ("HTTP_UPDATE_FAILD Error (%d): %s", ESPhttpUpdate.getLastError(), ESPhttpUpdate.getLastErrorString().c_str()));
         break;
      case HTTP_UPDATE_NO_UPDATES:
         //LOG::Info("Updater:", "HTTP_UPDATE_NO_UPDATES");
         break;
      }
   }
}


/*t_httpUpdate_return  ret = ESPhttpUpdate.update("http://backend2.guartinel.com:8889/Guartinel_cli.bin");

switch (ret) {
case HTTP_UPDATE_FAILED:
Serial.printf("HTTP_UPDATE_FAILD Error (%d): %s", ESPhttpUpdate.getLastError(), ESPhttpUpdate.getLastErrorString().c_str());
break;

case HTTP_UPDATE_NO_UPDATES:
Serial.println("HTTP_UPDATE_NO_UPDATES");
break;

case HTTP_UPDATE_OK:
Serial.println("HTTP_UPDATE_OK");
break;
}*/


//"‎f9:aa:75:d3:68:a4:71:ca:b4:58:02:f9:fa:8d:01:df:b0:26:19:75"

// t_httpUpdate_return ret = ESPhttpUpdate.update("https://backend2.guartinel.com:8889/Guartinel_cli.bin","","‎f9 aa 75 d3 68 a4 71 ca b4 58 02 f9 fa 8d 01 df b0 26 19 75");

/* const char* host = "backend2.guartinel.com";
int httpsPort = 8889;
const char* fingerprint = "f9 aa 75 d3 68 a4 71 ca b4 58 02 f9 fa 8d 01 df b0 26 19 75";
const char* url = "/Guartinel_cli.bin";
WiFiClientSecure client;
Serial.print("connecting to ");
Serial.println(host);
if (!client.connect(host, httpsPort)) {
Serial.println("connection failed");

}

if (client.verify(fingerprint, host)) {
Serial.println("certificate matches");
}
else {
Serial.println("certificate doesn't match");

}

Serial.print("Starting OTA from: ");
Serial.println(url);

auto ret = ESPhttpUpdate.update(client, host, url);
// if successful, ESP will restart
Serial.println("update failed");
Serial.println((int)ret);*/

