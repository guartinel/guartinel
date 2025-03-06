#include "CheckerGas.h"
#include "Utils.h"
#include "ManagementServer.h"
#include "LOG.h"
#include "ArduinoJson.h"
#include "Version.h"
#include "DHT.h"
#include "GasSensorMQ135.h"
#include "GasSensorMG811.h"
#include "CheckerConstansts.h"

GasSensor* gasSensor;


CheckerGas::CheckerGas() {}


String CheckerGas::GetMeasurement() {
 
   LOG::Info("GasChecker", "Checking");
   StaticJsonBuffer<150> jsonBuffer;
   JsonObject& root = jsonBuffer.createObject();

   if (config.hardwareType.indexOf("mq135") != -1) {
      root["D1"] = flipBit(digitalRead(D1));
      root["A0"] = analogRead(A0);
   }

   if (config.hardwareType.indexOf("mg811") != -1) {
      root["A0"] = gasSensor->getPPM();
   }

   String jsonString;
   root.printTo(jsonString);
   jsonBuffer.clear();  
}

void CheckerGas::Init(CLIConfig _config) {
   config = _config;
   if (_config.hardwareType.indexOf("mq135") != -1) {
      LOG::Info("Gas checker", "Initializing as MQ135 ");
      gasSensor = new MQ135(A0);
   }
   if (_config.hardwareType.indexOf("mg811") != -1) {
      LOG::Info("Gas checker", "Initializing as MG811!");
      gasSensor = new  MG811(A0);
   }
}