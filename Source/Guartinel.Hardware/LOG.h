#include "Arduino.h"
#ifndef _LOG_h
#define _LOG_h

namespace LOG {
   void Init(bool isDebug);
   void Debug(char* TAG, char* message);
   void Status(char * status);
   void Info(char* TAG, int message);
   void Info(char* TAG,  char* message);
   void Info(char* TAG, const char* message);
   void EnableRemoteLog(String adress);
   void RemoteLog(char* TAG, const char* msg);
   bool IsRemoteLOGEnabled();
   String GetEntries();
}
#endif

