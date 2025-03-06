#pragma once
#include "Arduino.h"
#ifndef _UTILS_H
#define _UTILS_H

namespace Utils {
   String GetChipID();
   String GenerateRandomString(int length);
   String CharPointerToString(char* source);
   char* StringToCharPointer(String source);
   uint32 GetSystemTime();
   uint32 GetSystemTimeSec();
   String GetSystemTimeAsString();
}

#endif

