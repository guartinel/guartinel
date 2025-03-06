
#pragma once
#include "IChecker.h"
#include "Arduino.h"
#include "GuartinelConfig.h"
#include "CheckerConstansts.h"
#ifndef _CHECKER_WATER_PRESENCE_H
#define _CHECKER_WATER_PRESENCE_H

class CheckerWaterPresence : public  IChecker {
public:
	CheckerWaterPresence();
	String GetMeasurement();
	void Init(CLIConfig config);
	void Update();
private:	
	double maxA0 = WRONG_READ_VALUE;
	double measureAnalog();
};
#endif
