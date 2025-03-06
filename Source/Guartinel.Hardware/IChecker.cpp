#include "IChecker.h"
#include "GuartinelConfig.h"
#include "ManagementServer.h"
#include "Utils.h"
#include "LOG.h"

IChecker::IChecker() {};
String IChecker::GetMeasurement() {};
void IChecker::Init(CLIConfig config) {
   LOG::Info("IChecker", "Init");
};

int IChecker::flipBit(int originalValue) {
   if (originalValue == 0) {
      return 1;
   }
   return 0;
}
void IChecker::Update() {

}

bool IChecker::ShouldSendMeasurement() {
	const int CHECK_INTERVAL_SEC = 45;
   if (lastCheck != -1) {
      int now = Utils::GetSystemTimeSec();
      int diff = now - lastCheck;
      if (diff < 0) {
         lastCheck = 0;
      }
	    if (lastCheck != -1 && diff < CHECK_INTERVAL_SEC) {
		   return false;//not yet..
      }
   }
   checkCount++;
   lastCheck = Utils::GetSystemTimeSec();
   LOG::Info("Check count", checkCount);
   return true;
}