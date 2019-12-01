// 
// 
// 

#include "SpeedMonitor.h"


int lastSensorState = 0;
unsigned long lastPeriodBeginTime = 0;

unsigned int tempSignalCount = 0;

bool SpeedMonitorClass::IsSensorValidState()
{
	int currentSensorState = digitalRead(HALL_SENSOR_PIN);

	bool isValidState = false;
	if (currentSensorState == LOW && currentSensorState != lastSensorState && millis() - lastSensorSignalTime > SIGNAL_DELTA_TIME)
	{
		lastSensorSignalTime = millis();
		tempSignalCount++;
		isValidState = true;
	}
	lastSensorState = currentSensorState;

	return isValidState;
}

int SpeedMonitorClass::GetMotorRoundPerSecond()
{
	return SignalCountPerSecond / MAGNET_COUNT;
}

float SpeedMonitorClass::GetCurrentSpeedKilometerPerHour()
{
	return 0.0f;
}

void SpeedMonitorClass::Init()
{
	pinMode(HALL_SENSOR_PIN, INPUT);

}

void SpeedMonitorClass::Tick()
{
	if (this->isEnableMonitor)
	{
		if (millis() - lastPeriodBeginTime >= 1000)
		{
			this->SignalCountPerSecond = tempSignalCount;
			tempSignalCount = 0;
			lastPeriodBeginTime = millis();
#ifdef DEBUG_MODE
			Serial.print("Motor Speed:");
			Serial.print(GetMotorRoundPerSecond());
			Serial.println(" RPM")
#endif 

		}
	}
}


unsigned long lastSensorSignalTime;

void SpeedMonitorClass::EnableHallSensorMonitor(bool isEnable)
{
	this->isEnableMonitor = isEnable;
	if (isEnable)
	{
		lastSensorSignalTime = millis();
		lastPeriodBeginTime = millis();
	}
}


SpeedMonitorClass SpeedMonitor;

