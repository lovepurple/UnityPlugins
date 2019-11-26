// SpeedMonitor.h

#ifndef _SPEEDMONITOR_h
#define _SPEEDMONITOR_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include "GlobalDefine.h"
#include "NewPing.h"

class SpeedMonitorClass
{
 protected:

private:
	
	NewPing* m_pSonar;

 public:
	void init();

	void Tick();
};

extern SpeedMonitorClass SpeedMonitor;

#endif

