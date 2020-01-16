// DriverMonitor.h

#ifndef _DRIVERMONITOR_h
#define _DRIVERMONITOR_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include "MotorController.h"
#include "VisibilityMonitor.h"
#include "SpeedMonitor.h"

class DriverControllerClass
{
protected:

private:

public:
	void IsEnable(bool isEnable);

	void init();

	void Tick();
};

extern DriverControllerClass DriverController;

#endif

