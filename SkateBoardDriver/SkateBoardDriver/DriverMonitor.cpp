// 
// 
// 

#include "DriverMonitor.h"

void DriverControllerClass::IsEnable(bool isEnable)
{
	SpeedMonitor.EnableHallSensorMonitor(isEnable);
	//VisibilityMonitor.EnableVisibilityMonitor(isEnable);
}

void DriverControllerClass::init()
{
	MotorController.init();

	//VisibilityMonitor.init();

	SpeedMonitor.Init();
}

void DriverControllerClass::Tick()
{
	MotorController.Tick();

	//VisibilityMonitor.Tick();

	SpeedMonitor.Tick();
}


DriverControllerClass DriverController;

