#pragma once
#include "IChecker.h"
#include "Arduino.h"
#include "GuartinelConfig.h"
#include <DHT.h>
#include <DallasTemperature.h>
#include <OneWire.h>
#ifndef _TEMPERATURE_CHECKER_H
#define _TEMPERATURE_CHECKER_H
struct TemperatureMeasurement {
   float temp;
   float humidity;
};
class CheckerTemperature : public  IChecker {
private:
   CLIConfig config;
   DHT* _dht;
   OneWire* _oneWire;
   DallasTemperature* _dallasTemp;
   TemperatureMeasurement readDHT();
public:
   CheckerTemperature();
   String GetMeasurement();
   void Init(CLIConfig config);
};


#endif
