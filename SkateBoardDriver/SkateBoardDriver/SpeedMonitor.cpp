// 
// 
// 

#include "SpeedMonitor.h"

int SpeedMonitorClass::GetMotorRoundPerSecond()
{
	return SignalCountPerSecond / MAGNET_COUT;
}

void SpeedMonitorClass::Init()
{
	pinMode(HALL_SENSOR_PIN, INPUT);

}

void SpeedMonitorClass::Tick()
{

}

unsigned long LastSignalTime;

void SpeedMonitorClass::EnableHallSensorMonitor(bool isEnable)
{
	this->isEnableMonitor = isEnable;
	if (isEnable)
		LastSignalTime = millis();
}


SpeedMonitorClass SpeedMonitor;

