#pragma once
#pragma once
#include "Arduino.h"
#include "GuartinelConfig.h"
#ifndef _CHECKER_BASE_H
#define _CHECKER_BASE_H
#include "ManagementServer.h"

class IChecker  {
protected: 
  CLIConfig config;
  int flipBit(int in);

public:
   bool ShouldSendMeasurement();
   IChecker();
   virtual String GetMeasurement();
   virtual void Init(CLIConfig config);
   virtual void Update();
private:
   uint32 lastCheck = -1;
   int checkCount = 0;   
};
#endif
