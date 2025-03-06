#pragma once
#include "Arduino.h"
#include "GuartinelConfig.h"
#ifndef _UPDATER_H
#define _UPDATER_H

namespace FirmwareUpdater {
   void UpdateIfNeeded(CLIConfig* config,int currentVersion, ManagementServer* ms);
}
#endif

