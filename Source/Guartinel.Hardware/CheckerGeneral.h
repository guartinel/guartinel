
#pragma once
#include "IChecker.h"
#include "Arduino.h"
#include "GuartinelConfig.h"
#include "CheckerConstansts.h"
#ifndef _GENERAL_CHECKER_H
#define _GENERAL_CHECKER_H

class CheckerGeneral : public  IChecker {
public:
	CheckerGeneral();
	String GetMeasurement();
	void Init(CLIConfig config);
	void Update();
private:
	double minA0 = WRONG_READ_VALUE;
	double maxA0 = WRONG_READ_VALUE;
	int minD1 = WRONG_READ_VALUE;
	int maxD1 = WRONG_READ_VALUE;
	int minD2 = WRONG_READ_VALUE;
	int maxD2 = WRONG_READ_VALUE;
	int minD3 = WRONG_READ_VALUE;
	int maxD3 = WRONG_READ_VALUE;
	double measureAnalog();
};
#endif
