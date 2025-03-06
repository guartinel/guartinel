#include "LedDisplay.h"
#include "Arduino.h"
#include "LOG.h"
namespace LedDisplay {

   int RED = D0;
   int GREEN = D6;
   int BLUE = D7;
   
   int lastRed = 0;
   int lastGreen = 0;
   int lastBlue = 0;
   void fadeToColor(int red, int green, int blue) {   
         analogWrite(RED, red);
         analogWrite(BLUE, blue);
         analogWrite(GREEN, green);      
         lastRed = red;
         lastGreen = green;
         lastBlue = blue;
   }

   void showRed() {
      fadeToColor(255, 0, 0);
   }

   void showGreen() {
      fadeToColor(0, 255, 0);
   }
   void showBlue() {
      fadeToColor(0, 0, 255);
   }

   void showOrange() {
      fadeToColor(250, 40, 0);
   }

   void showPurple() {
      fadeToColor(175, 255, 175);
   }
   
   extern void Init() {
      pinMode(RED, OUTPUT);
      pinMode(GREEN, OUTPUT);
      pinMode(BLUE, OUTPUT);
      delay(500);
   }
   extern void OK() {
      showGreen();
   }

   extern void Loading() {
      showBlue();
   }
   extern void Error() {
      showRed();
   }
   void TurnOff() {
      analogWrite(RED, 0);
      analogWrite(BLUE, 0);
      analogWrite(GREEN,0);
   }
   extern void BlinkOnce() {
      LOG::Info("LED","Blink once");
      delay(75);
      TurnOff();    
      delay(75);
      fadeToColor(lastRed,lastGreen,lastBlue);     
   }
   extern void BlinkTwice() {
      LOG::Info("LED", "Blink twice");
      BlinkOnce();
      delay(200);
      TurnOff();
      delay(200);
      fadeToColor(lastRed, lastGreen, lastBlue);
   }
   extern void BlinkThreeTimes() {
      LOG::Info("LED", "Blink three times");
      BlinkTwice();
      delay(500);
      TurnOff();
      delay(500);
      fadeToColor(lastRed, lastGreen, lastBlue);
    
   }
}

