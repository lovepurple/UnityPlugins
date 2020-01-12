// 
// 
// 

#include "SpeedMonitor.h"


int lastSensorState = 0;
unsigned long lastPeriodBeginTime = 0;

volatile unsigned int tempSignalCount = 0;
unsigned long lastSensorSignalTime;

void SpeedMonitorClass::Init()
{
	pinMode(HALL_SENSOR_PIN, INPUT_PULLUP);
}


void OnTrigHallSensor()
{
	tempSignalCount++;
}


bool SpeedMonitorClass::RefreshSensorValidState()
{
	if (millis() - lastPeriodBeginTime >= 1000)
	{
		this->SignalCountPerSecond = tempSignalCount / 2;

		UtilityClass::DebugLog("Motor Speed :", false);
		UtilityClass::DebugLog(String(GetMotorRoundPerSecond()), false);
		UtilityClass::DebugLog("RPS", true);

		lastPeriodBeginTime = millis();

		tempSignalCount = 0;
	}
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


void SpeedMonitorClass::Tick()
{
	if (this->isEnableMonitor)
		RefreshSensorValidState();
}



void SpeedMonitorClass::EnableHallSensorMonitor(bool isEnable)
{
	this->isEnableMonitor = isEnable;
	if (isEnable)
		attachInterrupt(digitalPinToInterrupt(HALL_SENSOR_PIN), OnTrigHallSensor, RISING);
	else
		detachInterrupt(digitalPinToInterrupt(HALL_SENSOR_PIN));
}

char* SpeedMonitorClass::GetCurrentMotorRPSMesssage()
{
	if (this->isEnableMonitor)
	{
		int motorRps = GetMotorRoundPerSecond();
		char* pMessageBuffer = DynamicBuffer.GetBuffer();
		pMessageBuffer[0] = E_D2C_MOTOR_RPS;
		itoa(motorRps, pMessageBuffer + 1, 10);
		pMessageBuffer[3] = '\0';

		return pMessageBuffer;
	}

	return nullptr;
}

int SpeedMonitorClass::ConvertKilometerPerHourToRPS(float kilometerPerHour)
{
	float meterPerSecond = kilometerPerHour * 0.2778f;
	float wheelRoundPerSecond = meterPerSecond / WHEEL_METER_PER_ROUND;

	int motorRoundPerSecond = (int)(wheelRoundPerSecond / SYNC_GEAR_RATIO);

	return motorRoundPerSecond;
}


SpeedMonitorClass SpeedMonitor;

