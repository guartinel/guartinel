#pragma once
#include "IChecker.h"
#include "Arduino.h"
#include "GuartinelConfig.h"
#include <DHT.h>
#ifndef _TEMPERATURE_CHECKER_H
#define _TEMPERATURE_CHECKER_H

class CheckerGas : public  IChecker {
private:
   CLIConfig config;
   DHT* _dht;
public:
   CheckerGas();
   String GetMeasurement();
   void Init(CLIConfig config);
};
#endif
