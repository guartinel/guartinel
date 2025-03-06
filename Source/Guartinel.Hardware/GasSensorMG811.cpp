#define POINT_400_VOLTAGE (0.324) //define the output of the sensor in volts when the concentration of CO2 is 400PPM
#define POINT_400_LGPPM (2.602) //lg400=2.602, the start point_on X_axis of the curve

#define POINT_10000_VOLTAGE  (0.00001)//(0.265) //define the output of the sensor in volts when the concentration of CO2 is 10000PPM
#define POINT_10000_LGPPM (-5)//(4.0) //lg10000=4

#define SLOPE (( POINT_400_VOLTAGE-POINT_10000_VOLTAGE )/( POINT_400_LGPPM-POINT_10000_LGPPM)) //define slope of the curve
// SLOPE = -1,339
#include "GasSensorMG811.h"
#include "LOG.h"
float MG811::getPPM() {
   int i;
   double analogValue = 0;
   for (i = 0; i < 10; i++) {
      analogValue += analogRead(_pin); //_pin = A0
      delay(10);
   }
   analogValue = (analogValue / 10.0); //avarage

   LOG::Info("getPPM analog read:", analogValue);
   float volts = (analogValue * 3.3 / 1024.0);  // MEASURED_VOLT * 3,3 VOLT / 2^10   => get real voltage from 10bit reading
   LOG::Info("getPPM volts: ", volts);

   if (volts > POINT_400_VOLTAGE || volts < POINT_10000_VOLTAGE) {
      return -1000; // if we are here, the sensor is probably still warming up
   }
   return pow(10, (volts - POINT_10000_VOLTAGE) / SLOPE + POINT_400_LGPPM);
}

float MG811::getCorrectedPPM(float temp, float humidity) {
   return getPPM();
}

/*
#define DC_GAIN 1.0 //the sensor voltage is amplified by 1 volt!
#define ZERO_POINT_X (2.602) //lg400=2.602, the start point_on X_axis of the curve
#define ZERO_POINT_VOLTAGE (0.324) //define the output of the sensor in volts when the concentration of CO2 is 400PPM
#define MAX_POINT_VOLTAGE (0.265) //define the output of the sensor in volts when the concentration of CO2 is 10000PPM
#define REACTION_VOLTAGE (0.059) //define the voltage dropof the sensor when move the sensor from air into 1000ppm CO2

float CO2Curve[3] = { ZERO_POINT_X, ZERO_POINT_VOLTAGE, (REACTION_VOLTAGE / (2.602 - 4)) };
//Two points are taken from the curve.With these two points, a line is formed which is
//"approximately equivalent" to the original curve. You could use other methods to get more accurate slope
//CO2 Curve format:{ x, y, slope};
//point1: (lg400=2.602, 0.324),
//point2: (lg10000 = 4, 0.265)
//slope = (y1-y2)(i.e.reaction voltage)/ x1-x2 = (0.324-0.265)/(log400 - log10000)

#include "MG811.h"
#include "LOG.h"
float MG811::getPPM() {
int i;
float volts = 0;
for (i = 0; i < 10; i++) {
volts += analogRead(_pin);
delay(10);
}
volts = (volts / 10.0); //avarage
volts = volts * 3.3 / 1024.0;  // MEASURED_VOLT * 3,3 VOLT / 2^10   => get real voltage from 10bit reading
volts = volts -DC_GAIN;

if (volts > ZERO_POINT_VOLTAGE || volts < MAX_POINT_VOLTAGE) {
return -1;
}
return pow(10, (volts - CO2Curve[1]) / CO2Curve[2] + CO2Curve[0]);
}
*/