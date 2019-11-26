// DriverMonitor.h

#ifndef _DRIVERMONITOR_h
#define _DRIVERMONITOR_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include "SpeedMonitor.h"

class DriverMonitorClass
{
protected:

private:

public:
	void init();

	void Tick();
};

extern DriverMonitorClass DriverMonitor;

#endif

