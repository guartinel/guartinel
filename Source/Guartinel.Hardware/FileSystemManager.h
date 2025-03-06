
#ifndef _FILESYSTEM_MANAGER
#define _FILESYSTEM_MANAGER

#include "Arduino.h"
#include "GuartinelConfig.h"


class FileSystemManager {
protected:

public:
   FileSystemManager();
   bool InitFileSystem();
   CLIConfig GetConfig();
   void SaveConfig(CLIConfig config);
   void Reset();
   bool IsDeviceRegistered();
};
#endif