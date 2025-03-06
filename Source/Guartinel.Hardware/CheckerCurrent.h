#pragma once
#pragma once
#include "IChecker.h"
#include "Arduino.h"
#include "GuartinelConfig.h"
#include <EmonLib.h>
#include "CheckerConstansts.h"
#ifndef _CURRENT_CHECKER_H
#define _CURRENT_CHECKER_H

class CheckerCurrent : public IChecker {

public:
   CheckerCurrent();
   String GetMeasurement();
   void Init(CLIConfig config);
private:
   double calibrationValue = 27.397;
   double measureNow();
   double maxA = WRONG_READ_VALUE;
   double minA = WRONG_READ_VALUE;
   EnergyMonitor* energyMonitor;
};
#endif
