#pragma once
#include "Arduino.h"
#ifndef _LED_DISPLAY_H
#define _LED_DISPLAY_H

namespace LedDisplay {
   void OK(); 
   void Loading();
   void Error();    
   void Init();
   void TurnOff();
   void BlinkThreeTimes();
   void BlinkOnce();
   void BlinkTwice();
}
#endif
