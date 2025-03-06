
#ifndef MG811_H
#define MQ135_H
#include "GasSensor.h"

class MG811 : public GasSensor {
private:
   uint8_t _pin;

public:
   MG811(uint8_t pin) { _pin = pin; }
   float getPPM();
   float getCorrectedPPM(float temp, float humidity);
};
#endif