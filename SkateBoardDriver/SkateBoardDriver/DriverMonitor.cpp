// 
// 
// 

#include "DriverMonitor.h"

void DriverControllerClass::init()
{
	MotorController.init();

	VisibilityMonitor.init();

	SpeedMonitor.Init();
}

void DriverControllerClass::Tick()
{
	MotorController.Tick();

	VisibilityMonitor.Tick();

	SpeedMonitor.Tick();
}


DriverControllerClass DriverController;

