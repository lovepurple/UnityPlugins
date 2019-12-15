// 
// 
// 

#include "SpeedMonitor.h"


int lastSensorState = 0;
unsigned long lastPeriodBeginTime = 0;

unsigned int tempSignalCount = 0;
unsigned long lastSensorSignalTime;


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
	int roundPerHour = GetMotorRoundPerSecond() * 3600;

	float wheelRoundPerHour = roundPerHour * SYNC_GEAR_RATIO;

	float wheelPassMetersPerHour = wheelRoundPerHour * WHEEL_METER_PER_ROUND;

	float kmPerHour = wheelPassMetersPerHour * 0.001;

	this->m_kilometersPerHour = kmPerHour;

	return kmPerHour;
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

			UtilityClass::DebugLog("Motor Speed :" + String(GetMotorRoundPerSecond() + "RPS"), true);
		}
	}
}



void SpeedMonitorClass::EnableHallSensorMonitor(bool isEnable)
{
	this->isEnableMonitor = isEnable;
	if (isEnable)
	{
		lastSensorSignalTime = millis();
		lastPeriodBeginTime = millis();
	}
}

int SpeedMonitorClass::ConvertKilometerPerHourToRPS(float kilometerPerHour)
{
	float meterPerSecond = kilometerPerHour * 0.2778f;
	float wheelRoundPerSecond = meterPerSecond / WHEEL_METER_PER_ROUND;

	int motorRoundPerSecond = (int)(wheelRoundPerSecond / SYNC_GEAR_RATIO);

	return motorRoundPerSecond;
}


SpeedMonitorClass SpeedMonitor;

