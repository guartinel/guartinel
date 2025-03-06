#include "Utils.h"
#include <Esp.h>
#include "osapi.h"

#include "Arduino.h"
#include "LOG.h"
#include <ctime>
#include "Arduino.h"


namespace Utils {
   extern "C" {
#include "user_interface.h"
   }
   extern String  GetChipID() {
      uint32 chipID = ESP.getChipId();
      char result[16];
      sprintf(result, "%04x", chipID); //%04x pad the resulting char array to the provided number with 0s
      return String(result);
   }

   extern String CharPointerToString(char* source) {
      return String(source);
   }

   extern char* StringToCharPointer(String source) {
      return (char*)source.c_str();
   }

   extern String GenerateRandomString(int maxLength) {
      char* generated;
      for (int i = 0; i < maxLength; ++i) {
         int randomChar = random(52) % (26 + 26 + 10);
         if (randomChar < 26)
            generated[i] = 'a' + randomChar;
         else if (randomChar < 26 + 26)
            generated[i] = 'A' + randomChar - 26;
         else
            generated[i] = '0' + randomChar - 26 - 26;
      }
      generated[maxLength] = 0;
      return String(generated);
   }
   extern uint32 GetSystemTime() {
      uint32 time = system_get_time();
      return time;
   }
   extern uint32 GetSystemTimeSec() {
     return Utils::GetSystemTime()/1000000;
   }
   extern String GetSystemTimeAsString() {
      //char result[16];
    //  sprintf(result, "%04x", Utils::GetSystemTime()); //%04x pad the resulting char array to the provided number with 0s
      return String(Utils::GetSystemTime());
   }
}
