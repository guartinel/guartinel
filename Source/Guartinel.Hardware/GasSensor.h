#pragma once
#pragma once
#include "Arduino.h"
#include "GuartinelConfig.h"
#ifndef _IGAS_SENSOR
#define _IGAS_SENSOR

class GasSensor {
public:
   virtual float getPPM() = 0;
   virtual float getCorrectedPPM(float temp, float humidity) = 0;
};
#endif
