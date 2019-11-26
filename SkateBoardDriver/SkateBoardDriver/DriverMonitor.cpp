// 
// 
// 

#include "DriverMonitor.h"

void DriverMonitorClass::init()
{
	SpeedMonitor.init();

}

void DriverMonitorClass::Tick()
{
	SpeedMonitor.Tick();
}


DriverMonitorClass DriverMonitor;

