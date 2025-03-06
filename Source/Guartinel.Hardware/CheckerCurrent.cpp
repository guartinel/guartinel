#include "CheckerCurrent.h"
#include "Utils.h"
#include "ManagementServer.h"
#include "LOG.h"
#include "ArduinoJson.h"
#include "CheckerConstansts.h"
double CheckerCurrent::measureNow() {
   double sum = 0.0;
   for (int i = 0; i < 5; i++) {
      double read = energyMonitor->calcIrms(1480);
      sum = sum + read;
      delay(50);
   }
   double currentA = sum / 5.0;
   return currentA;
   // double roundedAvarage = round(currentA * 100) / 100.0;
}

CheckerCurrent::CheckerCurrent() {}


String CheckerCurrent::GetMeasurement() {
   double currentA = measureNow();
   if (maxA == WRONG_READ_VALUE || currentA > maxA) {
      maxA = currentA;
   }
   if (minA == WRONG_READ_VALUE || currentA < minA) {
      minA = currentA;
   }

   LOG::RemoteLog("CheckerCurrent", "Starting check");

   StaticJsonBuffer<156> jsonBuffer;// if to big wificlientsecure will throw an exception
   JsonObject& root = jsonBuffer.createObject();
   root["current_a"] = currentA;
   root["current_max_a"] = maxA;
   root["current_min_a"] = minA;

   /* LOG::Info("Check results", ":");
    LOG::Info("CurrentA:",currentA);
    LOG::Info("Current Max A:", maxA);
    LOG::Info("Current Min A:", minA);*/
    //::RemoteLog("CheckerGeneral Check:", ("Current:%f Max: %f Min:%f", currentA, maxA, minA));

   LOG::Info("Check results", ("Current:%f Max: %f Min:%f", currentA, maxA, minA));
   minA = currentA;
   maxA = currentA;

   String jsonString;
   root.printTo(jsonString);
   return jsonString;
}

void CheckerCurrent::Init(CLIConfig _config) {
   energyMonitor = new  EnergyMonitor();
   config = _config;
   if (_config.hardwareType == "hardware_type_current_level_max_30a") {
      calibrationValue = 27.397;
      energyMonitor->current(A0, calibrationValue); // Current: input pin, calibration. Cur Const= Ratio/BurdenR. 2000/77 = 29.  // default: 111.1
   }

   for (int i = 0; i < 10; i++) {// to prevent false reading at start
      double Irms = energyMonitor->calcIrms(1480);
      delay(30);
   }
}


