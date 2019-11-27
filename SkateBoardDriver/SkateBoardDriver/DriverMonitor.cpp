// 
// 
// 

#include "DriverMonitor.h"

void DriverMonitorClass::init()
{
	VisibilityMonitor.init();

	SpeedMonitor.Init();
}

void DriverMonitorClass::Tick()
{
	VisibilityMonitor.Tick();

	SpeedMonitor.Tick();
}


DriverMonitorClass DriverMonitor;

