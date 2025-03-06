#include "CheckerTemperature.h"
#include "Utils.h"
#include "ManagementServer.h"
#include "LOG.h"
#include "ArduinoJson.h"
#include "Version.h"
#include <DHT.h>
#include "IChecker.h"
#include <DallasTemperature.h>
#include <OneWire.h>
#include "CheckerConstansts.h"
CheckerTemperature::CheckerTemperature() {}

TemperatureMeasurement CheckerTemperature::readDHT() {
   TemperatureMeasurement measurement;
   _dht->begin();

   for (int i = 0; i < 6; i++) {
      measurement.temp = _dht->readTemperature();

      if (config.hardwareType.indexOf("dht22") != -1) {
         LOG::Info("TempChecker", "DHT22 wait 2100ms");
         delay(2100);
      }

      if (config.hardwareType.indexOf("dht11") != -1) {
         LOG::Info("TempChecker", "DHT11 wait 1100ms");
         delay(1100);
      }

      measurement.humidity = _dht->readHumidity();
      if (isnan(measurement.humidity) || isnan(measurement.temp)) {
         continue;
      }
      return measurement;
   }
}

String CheckerTemperature::GetMeasurement() {

   TemperatureMeasurement measurement;
   if (config.hardwareType.indexOf("dht22") != -1 || config.hardwareType.indexOf("dht11") != -1) {
      measurement = readDHT();
   }

   if (config.hardwareType.indexOf("ds18b20") != -1) {
      _dallasTemp->requestTemperatures();
      measurement.temp = _dallasTemp->getTempCByIndex(0);
      measurement.humidity = UNDEFINED_VALUE;
   }

   if (isnan(measurement.temp)) {
      measurement.temp = WRONG_READ_VALUE;
   }

   if (isnan(measurement.humidity)) {
      measurement.humidity = WRONG_READ_VALUE;
   }

   LOG::Info("Temperature", measurement.temp);
   LOG::Info("Humidity", measurement.humidity);

   StaticJsonBuffer<156> jsonBuffer;// if too big wificlientsecure will throw an exception
   JsonObject& root = jsonBuffer.createObject();
   root["temperature_celsius"] = measurement.temp;
   root["relative_humidity_percent"] = measurement.humidity;
 
   String jsonString;
   root.printTo(jsonString);
   jsonBuffer.clear();
   return jsonString;
}

void CheckerTemperature::Init(CLIConfig _config) {
   LOG::Info("CheckerTemperature", "Init");
   config = _config;
   if (_config.hardwareType.indexOf("dht22") != -1) {
      LOG::RemoteLog("Check init", "Using as dht22");
      LOG::Info("Temperature checker", "Initializing as dht22");
      _dht = new DHT(D1, DHT22);
      _dht->begin();
      delay(1000);
   }

   if (_config.hardwareType.indexOf("dht11") != -1) {
      LOG::RemoteLog("Check init", "Using as dht11");
      LOG::Info("Temperature checker", "Initializing as dht11");
      _dht = new DHT(D1, DHT11);
      _dht->begin();
      delay(1000);
   }

   if (_config.hardwareType.indexOf("ds18b20") != -1) {
      LOG::RemoteLog("Check init", "Using as ds18b20");
      LOG::Info("Temperature checker", "Initializing as ds18b20");
      _oneWire = new OneWire(D1);
      _dallasTemp = new DallasTemperature(_oneWire);

      delay(1000);
   }
}


