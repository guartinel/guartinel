#include "LOG.h"
#include "Kernel.h"
#include "Arduino.h"
#include "Version.h"

Kernel kernel;

void setup() {
   kernel.setup();


   LOG::Info("Guartine.General.Hardware version:", VERSION);
}

void loop() {
   kernel.loop();
}



