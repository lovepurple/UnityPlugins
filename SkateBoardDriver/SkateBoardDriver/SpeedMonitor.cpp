// 
// 
// 

#include "SpeedMonitor.h"

void SpeedMonitorClass::init()
{
	m_pSonar = &NewPing(11, 13, 500);
}

void SpeedMonitorClass::Tick()
{

}


SpeedMonitorClass SpeedMonitor;

